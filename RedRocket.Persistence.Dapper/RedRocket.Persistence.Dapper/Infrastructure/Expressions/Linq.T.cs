using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using RedRocket.Persistence.Dapper.Infrastructure.Attributes;
using RedRocket.Persistence.Dapper.Infrastructure.Expressions.Compiler;

namespace RedRocket.Persistence.Dapper.Infrastructure.Expressions
{
    /// <summary>
    ///     Allows for Ad-Hoc SQL queries to be generated using LINQ in a stongly type manner, while also taking advantage of
    ///     compile time validation.
    /// </summary>
    /// <typeparam name="T">
    ///     The Type that contains a strongly typed reference of the scheme for the database table/view to be
    ///     queried.
    /// </typeparam>
    public class Linq<T> : ILinq
    {
        /// <summary>
        ///     Creates a new SQLinq object
        /// </summary>
        public Linq()
        {
            Expressions = new List<Expression>(); //= new List<Expression<Func<T, bool>>>();
            //this.JoinExpressions = new List<ISQLinqJoinExpression>();
            OrderByExpressions = new List<OrderByExpression>();
        }

        /// <summary>
        ///     Creates a new SQLinq object
        /// </summary>
        /// <param name="tableNameOverride">
        ///     The database table name to use. This explicitly overrides any use of the SQLinqTable
        ///     attribute.
        /// </param>
        public Linq(string tableNameOverride)
            : this()
        {
            TableNameOverride = tableNameOverride;
        }

        public string TableNameOverride { get; private set; }

        public List<Expression> Expressions { get; private set; }

        public Expression<Func<T, object>> Selector { get; private set; }
        public int? TakeRecords { get; private set; }
        public int? SkipRecords { get; private set; }
        public List<OrderByExpression> OrderByExpressions { get; private set; }

        private bool? DistinctValue { get; set; }

        private ILinq Parent { get; set; }

        /// <summary>
        ///     Returns a SQLinqResult that contains the information for the query.
        /// </summary>
        /// <param name="existingParameterCount">
        ///     Used to set the unique id's of the query parameters. The first query parameter
        ///     will be 'existingParameterCount' plus one.
        /// </param>
        /// <returns></returns>
        public ILinqResult Render(int existingParameterCount = 0, string parameterNamePrefix = SqlExpressionCompiler.DefaultParameterNamePrefix)
        {
            int _parameterNumber = existingParameterCount;

            Type type = typeof(T);
            var parameters = new Dictionary<string, object>();

            // Get Table / View Name
            string tableName = GetTableName(true);
            var subqueryAttr = type.GetCustomAttributes(typeof(SubQueryAttribute), false).FirstOrDefault() as SubQueryAttribute;
            if (subqueryAttr != null)
            {
                string tableQuery = subqueryAttr.GetQuery(parameters);
                _parameterNumber = parameters.Count;
                tableName = string.Format("({0}) AS {1}", tableQuery, tableName);
            }

            //// JOIN
            //var join = new List<string>();
            //foreach (var j in this.JoinExpressions)
            //{
            //    join.Add(j.Process(parameters));
            //}

            //// SELECT
            SqlExpressionCompilerSelectorResult selectResult = ToSQL_Select(_parameterNumber, parameterNamePrefix, parameters);
            _parameterNumber = existingParameterCount + parameters.Count;

            // WHERE
            SqlExpressionCompilerResult whereResult = ToSQL_Where(_parameterNumber, parameterNamePrefix, parameters);
            _parameterNumber = existingParameterCount + parameters.Count;

            // ORDER BY
            SqlExpressionCompilerSelectorResult orderbyResult = ToSQL_OrderBy(_parameterNumber, parameterNamePrefix, parameters);
            _parameterNumber = existingParameterCount + parameters.Count;

            return new LinqSelectResult
            {
                Select = selectResult.Select.ToArray(),
                Distinct = DistinctValue,
                Take = TakeRecords,
                Skip = SkipRecords,
                Table = tableName,
                //Join = join.ToArray(),
                Where = whereResult == null ? null : whereResult.SQL,
                OrderBy = orderbyResult.Select.ToArray(),
                Parameters = parameters
            };
        }

        /// <summary>
        /// </summary>
        /// <param name="distinct">
        ///     Boolean value indicating whether 'DISTINCT' rows should be returned from the generated SQL.
        ///     Default is True
        /// </param>
        /// <returns>The SQLinq instance to allow for method chaining.</returns>
        public Linq<T> Distinct(bool distinct = true)
        {
            DistinctValue = distinct;
            return this;
        }

        /// <summary>
        /// </summary>
        /// <param name="take"></param>
        /// <returns>The SQLinq instance to allow for method chaining.</returns>
        public Linq<T> Take(int take)
        {
            TakeRecords = take;
            return this;
        }

        /// <summary>
        /// </summary>
        /// <param name="skip"></param>
        /// <returns>The SQLinq instance to allow for method chaining.</returns>
        public Linq<T> Skip(int skip)
        {
            if (OrderByExpressions.Count == 0)
            {
                throw new NotSupportedException("SQLinq: Skip can only be performed if OrderBy is specified.");
            }
            SkipRecords = skip;
            return this;
        }

        /// <summary>
        /// </summary>
        /// <param name="expression"></param>
        /// <returns>The SQLinq instance to allow for method chaining.</returns>
        public Linq<T> Where(Expression<Func<T, bool>> expression)
        {
            Expressions.Add(expression);
            return this;
        }

        /// <summary>
        /// </summary>
        /// <param name="expression"></param>
        /// <returns>The SQLinq instance to allow for method chaining.</returns>
        public Linq<T> Where(Expression expression)
        {
            Expressions.Add(expression);
            return this;
        }

        /// <summary>
        /// </summary>
        /// <param name="selector"></param>
        /// <returns>The SQLinq instance to allow for method chaining.</returns>
        public Linq<T> Select(Expression<Func<T, object>> selector)
        {
            Selector = selector;
            return this;
        }

        /// <summary>
        /// </summary>
        /// <param name="keySelector"></param>
        /// <returns>The SQLinq instance to allow for method chaining.</returns>
        public Linq<T> OrderBy(Expression<Func<T, object>> keySelector)
        {
            if (OrderByExpressions.Count > 0)
            {
                OrderByExpressions.Clear();
            }
            OrderByExpressions.Add(new OrderByExpression
            {
                Expression = keySelector,
                Ascending = true
            });
            return this;
        }

        /// <summary>
        /// </summary>
        /// <param name="keySelector"></param>
        /// <returns>The SQLinq instance to allow for method chaining.</returns>
        public Linq<T> OrderByDescending(Expression<Func<T, object>> keySelector)
        {
            if (OrderByExpressions.Count > 0)
            {
                OrderByExpressions.Clear();
            }
            OrderByExpressions.Add(new OrderByExpression
            {
                Expression = keySelector,
                Ascending = false
            });
            return this;
        }

        /// <summary>
        /// </summary>
        /// <param name="keySelector"></param>
        /// <returns>The SQLinq instance to allow for method chaining.</returns>
        public Linq<T> ThenBy(Expression<Func<T, object>> keySelector)
        {
            OrderByExpressions.Add(new OrderByExpression
            {
                Expression = keySelector,
                Ascending = true
            });
            return this;
        }

        /// <summary>
        /// </summary>
        /// <param name="keySelector"></param>
        /// <returns>The SQLinq instance to allow for method chaining.</returns>
        public Linq<T> ThenByDescending(Expression<Func<T, object>> keySelector)
        {
            OrderByExpressions.Add(new OrderByExpression
            {
                Expression = keySelector,
                Ascending = false
            });
            return this;
        }

        //public SQLinq<T> Join<TInner, TKey, TResult>(
        //    //SQLinq<TOuter> outer,
        //    SQLinq<TInner> inner,
        //    Expression<Func<T, TKey>> outerKeySelector,
        //    Expression<Func<TInner, TKey>> innerKeySelector,
        //    Expression<Func<T, TInner, TResult>> resultSelector)
        //{
        //    // http://msdn.microsoft.com/en-us/library/bb738634(v=vs.90).aspx
        //    // http://byatool.com/c/linq-join-method-and-how-to-use-it/

        //    inner.Parent = this;

        //    this.JoinExpressions.Add(new SQLinqJoinExpression<T, TInner, TKey, TResult> {
        //        Outer = this,
        //        Inner = inner,
        //        OuterKeySelector = outerKeySelector,
        //        InnerKeySelector = innerKeySelector,
        //        ResultSelector = resultSelector
        //    });
        //    return this;
        //}

        private string GetTableName(bool withAs = false)
        {
            string tableName = string.Empty;
            string tableAsName = string.Empty;

            if (!string.IsNullOrEmpty(TableNameOverride))
            {
                tableName = tableAsName = TableNameOverride;
            }
            else
            {
                // Get Table / View Name
                Type type = typeof(T);
                tableName = type.Name;
                var tableAttribute = type.GetCustomAttributes(typeof(TableAttribute), false).FirstOrDefault() as TableAttribute;
                if (tableAttribute != null)
                {
                    // Table / View name is explicitly set, use that instead
                    tableName = tableAttribute.Table;
                }

                tableAsName = tableName;
                //if (withAs)
                //{
                //    var joins = this.JoinExpressions;
                //    if (joins.Count == 0)
                //    {
                //        if (this.Parent != null)
                //        {
                //            joins = this.Parent.JoinExpressions;
                //        }
                //    }
                //    if (joins.Count > 0)
                //    {
                //        var je = joins[0];
                //        ParameterExpression p = ((dynamic)je.OuterKeySelector).Parameters[0] as ParameterExpression;
                //        if (p.Type != typeof(T))
                //        {
                //            p = ((dynamic)je.InnerKeySelector).Parameters[0] as ParameterExpression;
                //        }
                //        tableAsName = p.Name;
                //    }
                //}
            }

            if (!tableName.StartsWith("["))
            {
                tableName = string.Format("[{0}]", tableName);
            }

            //if (tableAsName != null)
            //{
            //    if (!tableAsName.StartsWith("["))
            //    {
            //        tableAsName = string.Format("[{0}]", tableAsName);
            //    }
            //}

            //if (tableName == tableAsName)
            //{
            return tableName;
            //}
            //else
            //{
            //    return string.Format("{0} AS {1}", tableName, tableAsName);
            //}
        }

        private SqlExpressionCompilerResult ToSQL_Where(int parameterNumber, string parameterNamePrefix, IDictionary<string, object> parameters)
        {
            SqlExpressionCompilerResult whereResult = null;
            if (Expressions.Count > 0)
            {
                whereResult = SqlExpressionCompiler.Compile(parameterNumber, parameterNamePrefix, Expressions);
                foreach (var item in whereResult.Parameters)
                {
                    parameters.Add(item.Key, item.Value);
                }
            }
            return whereResult;
        }

        private SqlExpressionCompilerSelectorResult ToSQL_Select(int parameterNumber, string parameterNamePrefix, IDictionary<string, object> parameters)
        {
            SqlExpressionCompilerSelectorResult selectResult = SqlExpressionCompiler.CompileSelector(parameterNumber, parameterNamePrefix, Selector);
            foreach (var item in selectResult.Parameters)
            {
                parameters.Add(item.Key, item.Value);
            }
            if (selectResult.Select.Count == 0)
            {
                PropertyInfo[] props = typeof(T).GetProperties();
                bool usesSQLinqColumn = props.Where(d => d.GetCustomAttributes(typeof(ColumnAttribute), false).Length > 0).Count() > 0;
                if (usesSQLinqColumn)
                {
                    foreach (PropertyInfo p in props)
                    {
                        string selectName = SqlExpressionCompiler.GetMemberColumnName(p);
                        string asName = "[" + p.Name + "]";
                        if (selectName == asName)
                        {
                            selectResult.Select.Add(selectName);
                        }
                        else
                        {
                            selectResult.Select.Add(string.Format("{0} AS {1}", selectName, asName));
                        }
                    }
                }
                else
                {
                    selectResult.Select.Add("*");
                }
            }
            return selectResult;
        }

        private SqlExpressionCompilerSelectorResult ToSQL_OrderBy(int parameterNumber, string parameterNamePrefix, IDictionary<string, object> parameters)
        {
            var orderbyResult = new SqlExpressionCompilerSelectorResult();

            for (int i = 0; i < OrderByExpressions.Count; i++)
            {
                SqlExpressionCompilerSelectorResult r = SqlExpressionCompiler.CompileSelector(parameterNumber, parameterNamePrefix, OrderByExpressions[i].Expression);
                foreach (string s in r.Select)
                {
                    orderbyResult.Select.Add(s);
                }
                foreach (var p in r.Parameters)
                {
                    orderbyResult.Parameters.Add(p.Key, p.Value);
                }
            }
            foreach (var item in orderbyResult.Parameters)
            {
                parameters.Add(item.Key, item.Value);
            }
            for (int i = 0; i < OrderByExpressions.Count; i++)
            {
                if (!OrderByExpressions[i].Ascending)
                {
                    orderbyResult.Select[i] = orderbyResult.Select[i] + " DESC";
                }
            }

            return orderbyResult;
        }

        public class OrderByExpression
        {
            public Expression<Func<T, object>> Expression { get; set; }
            public bool Ascending { get; set; }
        }
    }

    public class OrderByExpression<T>
    {
        public Expression<Func<T, object>> Expression { get; set; }
        public bool Ascending { get; set; }
    }
}
