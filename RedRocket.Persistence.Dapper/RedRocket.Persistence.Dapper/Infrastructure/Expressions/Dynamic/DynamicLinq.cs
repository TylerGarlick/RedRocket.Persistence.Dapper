//Copyright (c) Chris Pietschmann 2013 (http://pietschsoft.com)
//Licensed under the GNU Library General Public License (LGPL)
//License can be found here: http://sqlinq.codeplex.com/license

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using RedRocket.Persistence.Dapper.Infrastructure.Expressions.Compiler;

namespace RedRocket.Persistence.Dapper.Infrastructure.Expressions.Dynamic
{
    /// <summary>
    ///     Allows for dynamic ad-hoc SQL code generation with a similar API to LINQ while also allowing for LINQ expressions
    ///     to be used via Lambda expressions.
    /// </summary>
    public class DynamicLinq : ILinq
    {
        public DynamicLinq(string tableName)
        {
            TableName = tableName;
            SelectFields = new List<string>();
            JoinClauses = new List<ILinqJoinExpression>();
            WhereClauses = new DynamicLinqExpressionCollection();
            HavingClauses = new DynamicLinqExpressionCollection();
            GroupByFields = new List<string>();
            OrderByFields = new List<string>();
        }

        /// <summary>
        ///     The database table/view name to use for the generated query.
        /// </summary>
        public string TableName { get; private set; }

        private int? TakeCount { get; set; }
        private int? SkipCount { get; set; }

        private bool? DistinctValue { get; set; }

        private List<string> SelectFields { get; set; }
        private List<string> GroupByFields { get; set; }
        private List<string> OrderByFields { get; set; }
        private DynamicLinqExpressionCollection WhereClauses { get; set; }
        private DynamicLinqExpressionCollection HavingClauses { get; set; }
        private List<ILinqJoinExpression> JoinClauses { get; set; }

        /// <summary>
        ///     Generated SQL code and query parameters from the specified query details.
        /// </summary>
        /// <param name="existingParameterCount"></param>
        /// <returns></returns>
        public ILinqResult ToSql(int existingParameterCount = 0, string parameterNamePrefix = SqlExpressionCompiler.DefaultParameterNamePrefix)
        {
            int parameterCount = existingParameterCount;

            var result = new LinqSelectResult();
            result.Parameters = new Dictionary<string, object>();

            result.Take = TakeCount;
            result.Skip = SkipCount;

            result.Distinct = DistinctValue;

            // ****************************************************
            // **** FROM ******************************************
            result.Table = TableName;


            // ****************************************************
            // **** SELECT ****************************************
            if (SelectFields.Count == 0)
            {
                result.Select = new[]
                                {
                                    "*"
                                };
            }
            else
            {
                result.Select = SelectFields.ToArray();
            }


            // ****************************************************
            // **** JOIN ******************************************
            var joins = new List<string>();
            foreach (ILinqJoinExpression jc in JoinClauses)
            {
                LinqJoinResult r = jc.ToSql(parameterCount, parameterNamePrefix);
                foreach (string j in r.Join)
                {
                    joins.Add(j);
                }
                foreach (var i in r.Parameters)
                {
                    result.Parameters.Add(i.Key, i.Value);
                }
            }
            result.Join = joins.ToArray();

            parameterCount = existingParameterCount + result.Parameters.Count;

            // ****************************************************
            // **** WHERE *****************************************
            SqlExpressionCompilerResult compiledWhere = WhereClauses.Compile(parameterCount, parameterNamePrefix);
            result.Where = compiledWhere.SQL;
            foreach (var p in compiledWhere.Parameters)
            {
                result.Parameters.Add(p.Key, p.Value);
            }

            parameterCount = existingParameterCount + result.Parameters.Count;

            // ****************************************************
            // **** GROUP BY **************************************
            if (GroupByFields.Count > 0)
            {
                result.GroupBy = GroupByFields.ToArray();
            }

            // ****************************************************
            // **** HAVING ****************************************
            SqlExpressionCompilerResult compiledHaving = HavingClauses.Compile(parameterCount, parameterNamePrefix);
            result.Having = compiledHaving.SQL;
            foreach (var p in compiledHaving.Parameters)
            {
                result.Parameters.Add(p.Key, p.Value);
            }

            // ****************************************************
            // **** ORDER BY **************************************
            if (OrderByFields.Count > 0)
            {
                result.OrderBy = OrderByFields.ToArray();
            }


            return result;
        }

        /// <summary>
        ///     Allows for SQL 'SELECT DISTINCT' to be performed.
        /// </summary>
        /// <param name="distinct">
        ///     Boolean value indicating whether 'DISTINCT' rows should be returned from the generated SQL.
        ///     Default is True
        /// </param>
        /// <returns>The DynamicSQLinq instance to allow for method chaining.</returns>
        public DynamicLinq Distinct(bool distinct = true)
        {
            DistinctValue = distinct;
            return this;
        }

        /// <summary>
        ///     Allows for SQL 'SELECT' fields to be specified.
        /// </summary>
        /// <param name="fields">The table/view fields to select.</param>
        /// <returns>The DynamicSQLinq instance to allow for method chaining.</returns>
        public DynamicLinq Select(params string[] fields)
        {
            SelectFields.AddRange(fields);
            return this;
        }

        /// <summary>
        ///     Allows for SQL 'SELECT' fields to be specified from the specified Table/View
        /// </summary>
        /// <param name="tableName">The table/view name to select fields from.</param>
        /// <param name="fields">The fields to select.</param>
        /// <returns>The DynamicSQLinq instance to allow for method chaining.</returns>
        public DynamicLinq SelectTable(string tableName, params string[] fields)
        {
            foreach (string f in fields)
            {
                SelectFields.Add(string.Format("{0}.{1}", tableName, f));
            }
            return this;
        }

        /// <summary>
        ///     Allows for SQL 'GROUP BY' fields to be specified.
        /// </summary>
        /// <param name="fields">The table/view fields to 'GROUP BY'</param>
        /// <returns>The DynamicSQLinq instance to allow for method chaining.</returns>
        public DynamicLinq GroupBy(params string[] fields)
        {
            GroupByFields.AddRange(fields);
            return this;
        }

        /// <summary>
        ///     Specifies the number of results to return from the generated query.
        /// </summary>
        /// <param name="takeCount"></param>
        /// <returns>The DynamicSQLinq instance to allow for method chaining.</returns>
        public DynamicLinq Take(int takeCount)
        {
            TakeCount = takeCount;
            return this;
        }

        /// <summary>
        ///     Specifies the number of matching result from the the generated query to skip before return results.
        /// </summary>
        /// <param name="skipCount"></param>
        /// <returns>The DynamicSQLinq instance to allow for method chaining.</returns>
        public DynamicLinq Skip(int skipCount)
        {
            SkipCount = skipCount;
            return this;
        }

        /// <summary>
        ///     Specifies a SQL 'WHERE' clause to use with the generated query using a collection of LINQ expressions.
        /// </summary>
        /// <param name="expressionCollection"></param>
        /// <returns>The DynamicSQLinq instance to allow for method chaining.</returns>
        public DynamicLinq Where(DynamicLinqExpressionCollection expressionCollection)
        {
            WhereClauses.Add(expressionCollection);
            return this;
        }

        /// <summary>
        ///     Specifies a SQL 'WHERE' clause to use with the generated query.
        /// </summary>
        /// <param name="clause">The SQL code to use.</param>
        /// <param name="parameters">The parameters to use for the specified 'WHERE' clause.</param>
        /// <returns>The DynamicSQLinq instance to allow for method chaining.</returns>
        public DynamicLinq Where(string clause, params object[] parameters)
        {
            WhereClauses.Add(new DynamicLinqExpression(clause, parameters));
            return this;
        }

        /// <summary>
        ///     Specifies a SQL 'WHERE' clause to use with the generated query using a LINQ expression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldName">The Table/View Column name</param>
        /// <param name="expression">The LINQ / Lambda expression to use</param>
        /// <returns>The DynamicSQLinq instance to allow for method chaining.</returns>
        public DynamicLinq Where<T>(string fieldName, Expression<Func<T, bool>> expression)
        {
            WhereClauses.Add(new DynamicLinqLambdaExpression<T>(fieldName, expression));
            return this;
        }

        /// <summary>
        ///     Specifies a SQL 'HAVING' clause to use with the generated query.
        /// </summary>
        /// <param name="clause">The SQL code to use.</param>
        /// <param name="parameters">The parameters to use for the specified 'HAVING' clause.</param>
        /// <returns>The DynamicSQLinq instance to allow for method chaining.</returns>
        public DynamicLinq Having(string clause, params object[] parameters)
        {
            HavingClauses.Add(new DynamicLinqExpression(clause, parameters));
            return this;
        }

        /// <summary>
        ///     Specifies a SQL 'HAVING' clause to use with the generated query using a LINQ expression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldName">The Table/View Column name</param>
        /// <param name="expression">The LINQ / Lambda expression to use</param>
        /// <returns>The DynamicSQLinq instance to allow for method chaining.</returns>
        public DynamicLinq Having<T>(string fieldName, Expression<Func<T, bool>> expression)
        {
            HavingClauses.Add(new DynamicLinqLambdaExpression<T>(fieldName, expression));
            return this;
        }

        /// <summary>
        ///     Specifies a SQL 'JOIN' to use with the generated query.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="clause"></param>
        /// <returns></returns>
        public DynamicLinq Join(string tableName, string clause, params object[] parameters)
        {
            JoinClauses.Add(new DynamicLinqJoinExpression(tableName, clause, parameters));
            return this;
        }

        /// <summary>
        ///     Specifies a SQL join to use with the generated query.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="clause"></param>
        /// <returns></returns>
        public DynamicLinq Join(string tableName, DynamicLinqJoinOperator joinOperator, string clause, params object[] parameters)
        {
            JoinClauses.Add(new DynamicLinqJoinExpression(tableName, joinOperator, clause, parameters));
            return this;
        }

        public DynamicLinq Join(ILinq subquery, string subqueryAlias, DynamicLinqJoinOperator joinOperator, string clause, params object[] parameters)
        {
            JoinClauses.Add(new DynamicLinqSubQueryJoinExpression(subquery, subqueryAlias, joinOperator, clause, parameters));
            return this;
        }

        /// <summary>
        ///     Specifies a SQL 'LEFT JOIN' to use with the generated query.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="clause"></param>
        /// <returns></returns>
        public DynamicLinq LeftJoin(string tableName, string clause, params object[] parameters)
        {
            return Join(tableName, DynamicLinqJoinOperator.Left, clause, parameters);
        }

        /// <summary>
        ///     Specifies a SQL 'RIGHT JOIN' to use with the generated query.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="clause"></param>
        /// <returns></returns>
        public DynamicLinq RightJoin(string tableName, string clause, params object[] parameters)
        {
            return Join(tableName, DynamicLinqJoinOperator.Right, clause, parameters);
        }

        /// <summary>
        ///     Specifies a SQL 'FULL JOIN' to use with the generated query.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="clause"></param>
        /// <returns></returns>
        public DynamicLinq FullJoin(string tableName, string clause, params object[] parameters)
        {
            return Join(tableName, DynamicLinqJoinOperator.Full, clause, parameters);
        }

        /// <summary>
        ///     Specifies columns to sort the query result.
        /// </summary>
        /// <param name="fieldNames"></param>
        /// <returns>The DynamicSQLinq instance to allow for method chaining.</returns>
        public DynamicLinq OrderBy(params string[] fieldNames)
        {
            OrderByFields.AddRange(fieldNames);
            return this;
        }

        /// <summary>
        ///     Specifies columns to sort the query result in Descending order.
        /// </summary>
        /// <param name="fieldNames"></param>
        /// <returns></returns>
        public DynamicLinq OrderByDescending(params string[] fieldNames)
        {
            foreach (string f in fieldNames)
            {
                string orderByField = f;
                string lf = f.ToLowerInvariant();
                if (!(lf.EndsWith(" desc") || lf.EndsWith(" descending")))
                {
                    orderByField = f + " DESC";
                }

                OrderByFields.Add(orderByField);
            }
            return this;
        }
    }
}