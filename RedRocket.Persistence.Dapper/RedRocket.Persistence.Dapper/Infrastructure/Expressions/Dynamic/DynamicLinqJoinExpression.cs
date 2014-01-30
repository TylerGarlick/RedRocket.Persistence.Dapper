//Copyright (c) Chris Pietschmann 2012 (http://pietschsoft.com)
//Licensed under the GNU Library General Public License (LGPL)
//License can be found here: http://sqlinq.codeplex.com/license

using System;
using System.Collections.Generic;
using RedRocket.Persistence.Dapper.Infrastructure.Expressions.Compiler;
using RedRocket.Persistence.Dapper.Infrastructure.Expressions.Dynamic.Extensions;

namespace RedRocket.Persistence.Dapper.Infrastructure.Expressions.Dynamic
{
    public class DynamicLinqJoinExpression : ILinqJoinExpression
    {
        /// <summary>
        ///     Creates a new DynamicSQLinqJoinExpression instance with the JoinOperator set to DynamicSQLinqJoinOperator.Inner
        ///     ("INNER JOIN")
        /// </summary>
        /// <param name="tableName">The database Table / View to Join</param>
        /// <param name="clause">The Join clause</param>
        /// <param name="parameters">Any parameters necessary for the Join clause</param>
        public DynamicLinqJoinExpression(string tableName, string clause, object[] parameters)
            : this(tableName, DynamicLinqJoinOperator.Inner, clause, parameters) {}

        /// <summary>
        ///     Creates a new DynamicSQLinqJoinExpression instance
        /// </summary>
        /// <param name="tableName">The database Table / View to Join</param>
        /// <param name="joinOperator">The JOIN operator</param>
        /// <param name="clause">The Join clause</param>
        /// <param name="parameters">Any parameter values necessary for the Join clause</param>
        public DynamicLinqJoinExpression(string tableName, DynamicLinqJoinOperator joinOperator, string clause, object[] parameters)
        {
            Table = tableName;
            JoinOperator = joinOperator;
            Clause = clause;
            Parameters = parameters;
        }

        /// <summary>
        ///     The database Table / View to Join
        /// </summary>
        public string Table { get; set; }

        /// <summary>
        ///     The JOIN operator
        /// </summary>
        public DynamicLinqJoinOperator JoinOperator { get; set; }

        /// <summary>
        ///     The Join clause
        /// </summary>
        public string Clause { get; set; }

        /// <summary>
        ///     All necessary Join parameter values
        /// </summary>
        public object[] Parameters { get; set; }

        public LinqJoinResult ToSql(int existingParameterCount = 0, string parameterNamePrefix = SqlExpressionCompiler.DefaultParameterNamePrefix)
        {
            if (string.IsNullOrEmpty((parameterNamePrefix ?? string.Empty).Trim()))
            {
                throw new ArgumentException("parameterNamePrefix must be specified.", "parameterNamePrefix");
            }

            var parameters = new Dictionary<string, object>();

            string clause = Clause;
            for (int i = 0; i < Parameters.Length; i++)
            {
                existingParameterCount++;
                string key = "@" + parameterNamePrefix + existingParameterCount;
                clause = clause.Replace("@" + i, key);
                parameters.Add(key, Parameters[i]);
            }

            string join = string.Format("{0} {1} ON {2}", JoinOperator.ToSql(), Table, clause);

            return new LinqJoinResult(
                new[]
                {
                    join
                },
                parameters
                );
        }
    }
}