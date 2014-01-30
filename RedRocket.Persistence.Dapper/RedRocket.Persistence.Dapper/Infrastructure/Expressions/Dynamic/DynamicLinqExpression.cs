//Copyright (c) Chris Pietschmann 2012 (http://pietschsoft.com)
//Licensed under the GNU Library General Public License (LGPL)
//License can be found here: http://sqlinq.codeplex.com/license

using System;
using System.Collections.Generic;
using RedRocket.Persistence.Dapper.Infrastructure.Expressions.Compiler;

namespace RedRocket.Persistence.Dapper.Infrastructure.Expressions.Dynamic
{
    public class DynamicLinqExpression : IDynamicLinqExpression
    {
        public DynamicLinqExpression(string clause, params object[] args)
        {
            Clause = clause;
            Parameters = args;
        }

        public string Clause { get; set; }
        public object[] Parameters { get; set; }

        public SqlExpressionCompilerResult Compile(int existingParameterCount = 0, string parameterNamePrefix = SqlExpressionCompiler.DefaultParameterNamePrefix)
        {
            if (string.IsNullOrEmpty((parameterNamePrefix ?? string.Empty).Trim()))
            {
                throw new ArgumentException("parameterNamePrefix must be specified.", "parameterNamePrefix");
            }

            string sql = Clause;
            var parameters = new Dictionary<string, object>();

            for (int i = 0; i < Parameters.Length; i++)
            {
                existingParameterCount++;
                string paramName = string.Format("@{0}{1}", parameterNamePrefix, existingParameterCount);

                sql = sql.Replace("@" + i, paramName);
                parameters.Add(paramName, Parameters[i]);
            }

            return new SqlExpressionCompilerResult(sql, parameters);
        }
    }
}