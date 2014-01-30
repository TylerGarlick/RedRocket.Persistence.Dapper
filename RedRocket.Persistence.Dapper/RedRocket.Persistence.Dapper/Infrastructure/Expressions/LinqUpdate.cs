//Copyright (c) Chris Pietschmann 2013 (http://pietschsoft.com)
//Licensed under the GNU Library General Public License (LGPL)
//License can be found here: http://sqlinq.codeplex.com/license

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using RedRocket.Persistence.Dapper.Infrastructure.Attributes;
using RedRocket.Persistence.Dapper.Infrastructure.Expressions.Compiler;

namespace RedRocket.Persistence.Dapper.Infrastructure.Expressions
{
    public abstract class AbstractLinqExpression<T>
    {
        public List<Expression> Expressions { get; private set; }
        public T Data { get; set; }
        public string TableNameOverride { get; set; }

        protected AbstractLinqExpression(T data)
        {
            Data = data;
            Expressions = new List<Expression>();
        }

        protected AbstractLinqExpression(T data, string tableNameOverride)
            : this(data)
        {
            TableNameOverride = tableNameOverride;
        }

        protected SqlExpressionCompilerResult ToSQL_Where(int parameterNumber, string parameterNamePrefix, IDictionary<string, object> parameters)
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

        protected string GetTableName()
        {
            string tableName = string.Empty;
            if (!string.IsNullOrEmpty(TableNameOverride))
                tableName = TableNameOverride;
            else
            {
                // Get Table / View Name
                Type type = Data.GetType();
                tableName = type.Name;
                var tableAttribute = type.GetCustomAttributes(typeof (TableAttribute), false).FirstOrDefault() as TableAttribute;
                if (tableAttribute != null)
                {
                    // Table / View name is explicitly set, use that instead
                    tableName = tableAttribute.Table;
                }
            }

            return string.Format("[{0}]", tableName);
        }

        protected ILinqResult GenerateSql(int existingParameterCount = 0, string parameterNamePrefix = SqlExpressionCompiler.DefaultParameterNamePrefix)
        {
            int _parameterNumber = existingParameterCount;
            _parameterNumber++;

            Type type = Data.GetType();
            var parameters = new Dictionary<string, object>();
            var fields = new Dictionary<string, string>();

            // Get Table / View Name
            string tableName = GetTableName();

            foreach (PropertyInfo p in type.GetProperties())
            {
                bool includeInUpdate = true;
                string fieldName = p.Name;
                var attr = p.GetCustomAttributes(typeof(ColumnAttribute), true).FirstOrDefault() as ColumnAttribute;
                if (attr != null)
                {
                    includeInUpdate = attr.Update;
                    if (!string.IsNullOrEmpty(attr.Column))
                    {
                        fieldName = attr.Column;
                    }
                }

                if (includeInUpdate)
                {
                    string parameterName = "@" + parameterNamePrefix + _parameterNumber;

                    fields.Add(fieldName, parameterName);
                    parameters.Add(parameterName, p.GetValue(Data, null));

                    _parameterNumber++;
                }
            }
            _parameterNumber = existingParameterCount + parameters.Count;

            // ****************************************************
            // **** WHERE *****************************************
            SqlExpressionCompilerResult whereResult = ToSQL_Where(_parameterNumber, parameterNamePrefix, parameters);


            return new LinqUpdateResult
            {
                Table = tableName,
                Where = whereResult == null ? null : whereResult.SQL,
                Fields = fields,
                Parameters = parameters
            };
        }
    }

    public class LinqUpdate<T> : AbstractLinqExpression<T>, ILinqUpdate
    {
        public LinqUpdate(T data)
            : base(data)
        {
        }

        public LinqUpdate(T data, string tableNameOverride)
            : base(data, tableNameOverride)
        {
        }


        /// <summary>
        /// </summary>
        /// <param name="expression"></param>
        /// <returns>The SQLinq instance to allow for method chaining.</returns>
        public LinqUpdate<T> Where(Expression<Func<T, bool>> expression)
        {
            Expressions.Add(expression);
            return this;
        }

        /// <summary>
        /// </summary>
        /// <param name="expression"></param>
        /// <returns>The SQLinq instance to allow for method chaining.</returns>
        public LinqUpdate<T> Where(Expression expression)
        {
            Expressions.Add(expression);
            return this;
        }

        public ILinqResult ToSql(int existingParameterCount = 0, string parameterNamePrefix = SqlExpressionCompiler.DefaultParameterNamePrefix)
        {
            return GenerateSql(existingParameterCount, parameterNamePrefix);
        }
    }
}