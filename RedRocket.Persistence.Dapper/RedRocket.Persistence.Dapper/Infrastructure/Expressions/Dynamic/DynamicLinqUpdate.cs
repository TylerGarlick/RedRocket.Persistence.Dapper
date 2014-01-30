//Copyright (c) Chris Pietschmann 2013 (http://pietschsoft.com)
//Licensed under the GNU Library General Public License (LGPL)
//License can be found here: http://sqlinq.codeplex.com/license

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using RedRocket.Persistence.Dapper.Infrastructure.Expressions.Compiler;

namespace RedRocket.Persistence.Dapper.Infrastructure.Expressions.Dynamic
{
    public class DynamicLinqUpdate : ILinqUpdate
    {
        public DynamicLinqUpdate(IDictionary<string, object> data, string tableName)
        {
            Table = tableName;
            Data = data;

            WhereClauses = new DynamicLinqExpressionCollection();
        }

        public string Table { get; set; }
        public IDictionary<string, object> Data { get; set; }

        private DynamicLinqExpressionCollection WhereClauses { get; set; }

        public ILinqResult ToSql(int existingParameterCount = 1, string parameterNamePrefix = SqlExpressionCompiler.DefaultParameterNamePrefix)
        {
            int _parameterNumber = existingParameterCount;

            var fields = new Dictionary<string, string>();
            var parameters = new Dictionary<string, object>();

            foreach (var item in Data)
            {
                string fieldName = item.Key;
                object parameterValue = item.Value;
                string parameterName = "@" + parameterNamePrefix + _parameterNumber;

                fields.Add(fieldName, parameterName);
                parameters.Add(parameterName, parameterValue);

                _parameterNumber++;
            }
            int parameterCount = existingParameterCount + parameters.Count - 1;

            // ****************************************************
            // **** FROM ******************************************
            string tableName = Table;
            if (!tableName.StartsWith("["))
            {
                tableName = "[" + tableName + "]";
            }

            // ****************************************************
            // **** WHERE *****************************************
            SqlExpressionCompilerResult compiledWhere = WhereClauses.Compile(parameterCount, parameterNamePrefix);
            foreach (var p in compiledWhere.Parameters)
            {
                parameters.Add(p.Key, p.Value);
            }


            return new LinqUpdateResult
                   {
                       Table = tableName,
                       Where = compiledWhere.SQL,
                       Fields = fields,
                       Parameters = parameters
                   };
        }

        /// <summary>
        ///     Specifies a SQL 'WHERE' clause to use with the generated query using a collection of LINQ expressions.
        /// </summary>
        /// <param name="expressionCollection"></param>
        /// <returns>The DynamicSQLinq instance to allow for method chaining.</returns>
        public DynamicLinqUpdate Where(DynamicLinqExpressionCollection expressionCollection)
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
        public DynamicLinqUpdate Where(string clause, params object[] parameters)
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
        public DynamicLinqUpdate Where<T>(string fieldName, Expression<Func<T, bool>> expression)
        {
            WhereClauses.Add(new DynamicLinqLambdaExpression<T>(fieldName, expression));
            return this;
        }
    }
}