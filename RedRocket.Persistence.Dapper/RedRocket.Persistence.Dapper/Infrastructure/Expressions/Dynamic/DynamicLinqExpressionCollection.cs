//Copyright (c) Chris Pietschmann 2012 (http://pietschsoft.com)
//Licensed under the GNU Library General Public License (LGPL)
//License can be found here: http://sqlinq.codeplex.com/license

using System;
using System.Collections.Generic;
using System.Text;
using RedRocket.Persistence.Dapper.Infrastructure.Expressions.Compiler;

namespace RedRocket.Persistence.Dapper.Infrastructure.Expressions.Dynamic
{
    public class DynamicLinqExpressionCollection : List<IDynamicLinqExpression>, IDynamicLinqExpression
    {
        public DynamicLinqExpressionCollection()
            : this(DynamicLinqWhereOperator.And) {}

        public DynamicLinqExpressionCollection(DynamicLinqWhereOperator whereOperator)
        {
            WhereOperator = whereOperator;
        }

        public DynamicLinqWhereOperator WhereOperator { get; set; }

        public SqlExpressionCompilerResult Compile(int existingParameterCount = 0, string parameterNamePrefix = SqlExpressionCompiler.DefaultParameterNamePrefix)
        {
            if (string.IsNullOrEmpty(parameterNamePrefix))
            {
                throw new ArgumentException("parameterNamePrefix must be specified.", "parameterNamePrefix");
            }

            var sb = new StringBuilder();
            var parameters = new Dictionary<string, object>();
            int newParameterCount = existingParameterCount;

            int count = Count;
            for (int i = 0; i < count; i++)
            {
                SqlExpressionCompilerResult compiled = this[i].Compile(newParameterCount, parameterNamePrefix);

                if (i > 0)
                {
                    if (WhereOperator == DynamicLinqWhereOperator.Or)
                    {
                        sb.Append(" OR ");
                    }
                    else
                    {
                        sb.Append(" AND ");
                    }
                }
                if (count > 1)
                {
                    sb.Append("(");
                }

                sb.Append(compiled.SQL);

                if (count > 1)
                {
                    sb.Append(")");
                }

                foreach (var p in compiled.Parameters)
                {
                    parameters.Add(p.Key, p.Value);
                }

                newParameterCount = existingParameterCount + parameters.Count;
            }

            return new SqlExpressionCompilerResult(sb.ToString(), parameters);
        }

        public DynamicLinqExpressionCollection Add(string clause, params object[] parameters)
        {
            Add(new DynamicLinqExpression(clause, parameters));
            return this;
        }
    }
}