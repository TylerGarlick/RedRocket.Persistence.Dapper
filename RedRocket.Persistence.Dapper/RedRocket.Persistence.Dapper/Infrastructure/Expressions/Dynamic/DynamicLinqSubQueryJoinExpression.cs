//Copyright (c) Chris Pietschmann 2012 (http://pietschsoft.com)
//Licensed under the GNU Library General Public License (LGPL)
//License can be found here: http://sqlinq.codeplex.com/license

using System;
using System.Collections.Generic;
using RedRocket.Persistence.Dapper.Infrastructure.Expressions.Compiler;
using RedRocket.Persistence.Dapper.Infrastructure.Expressions.Dynamic.Extensions;

namespace RedRocket.Persistence.Dapper.Infrastructure.Expressions.Dynamic
{
    public class DynamicLinqSubQueryJoinExpression : ILinqJoinExpression
    {
        /// <summary>
        ///     Creates a new DynamicSQLinqSubQueryJoinExpression instance with the JoinOperator set to
        ///     DynamicSQLinqJoinOperator.Inner ("INNER JOIN")
        /// </summary>
        /// <param name="query">The sub-query that will be joined</param>
        /// <param name="alias">The Alias to give the sub-query within the main query</param>
        /// <param name="clause">The Join clause</param>
        /// <param name="parameters">Any parameters necessary for the Join clause</param>
        public DynamicLinqSubQueryJoinExpression(ILinq query, string alias, string clause, params object[] parameters)
            : this(query, alias, DynamicLinqJoinOperator.Inner, clause, parameters) {}

        /// <summary>
        ///     Creates a new DynamicSQLinqSubQueryJoinExpression instance
        /// </summary>
        /// <param name="query">The sub-query that will be joined</param>
        /// <param name="alias">The Alias to give the sub-query within the main query</param>
        /// <param name="joinOperator">The JOIN operator</param>
        /// <param name="clause">The Join clause</param>
        /// <param name="parameters">Any parameters necessary for the Join clause</param>
        public DynamicLinqSubQueryJoinExpression(ILinq query, string alias, DynamicLinqJoinOperator joinOperator, string clause, params object[] parameters)
        {
            Query = query;
            Alias = alias;
            JoinOperator = joinOperator;
            Clause = clause;
            Parameters = parameters;
        }

        public ILinq Query { get; set; }
        public string Alias { get; set; }
        public DynamicLinqJoinOperator JoinOperator { get; set; }
        public string Clause { get; set; }
        public object[] Parameters { get; set; }

        public LinqJoinResult ToSql(int existingParameterCount = 0, string parameterNamePrefix = SqlExpressionCompiler.DefaultParameterNamePrefix)
        {
            if (string.IsNullOrEmpty((parameterNamePrefix ?? string.Empty).Trim()))
            {
                throw new ArgumentException("parameterNamePrefix must be specified.", "parameterNamePrefix");
            }

            ILinqResult subqueryResult = Query.ToSql(existingParameterCount, parameterNamePrefix);
            string subquerySql = subqueryResult.ToQuery();

            var parameters = new Dictionary<string, object>();
            foreach (var p in subqueryResult.Parameters)
            {
                parameters.Add(p.Key, p.Value);
            }
            existingParameterCount += subqueryResult.Parameters.Count;

            string clause = Clause;
            for (int i = 0; i < Parameters.Length; i++)
            {
                existingParameterCount++;
                string key = "@" + parameterNamePrefix + existingParameterCount;
                clause = clause.Replace("@" + i, key);
                parameters.Add(key, Parameters[i]);
            }

            string join = string.Format("{0} ({1}) AS {2} ON {3}", JoinOperator.ToSql(), subquerySql, Alias, clause);

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