//Copyright (c) Chris Pietschmann 2013 (http://pietschsoft.com)
//Licensed under the GNU Library General Public License (LGPL)
//License can be found here: http://sqlinq.codeplex.com/license

using System;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using RedRocket.Persistence.Dapper.Infrastructure.Expressions.Compiler;

namespace RedRocket.Persistence.Dapper.Infrastructure.Expressions.Dynamic
{
    public class DynamicLinqLambdaExpression<T> : IDynamicLinqExpression
    {
        public DynamicLinqLambdaExpression(string fieldName, LambdaExpression expression) // Expression<Func<T, bool>> expression)
        {
            FieldName = fieldName;
            Expression = expression;
        }

        public string FieldName { get; set; }
        public LambdaExpression Expression { get; set; } // Expression<Func<T, bool>> Expression { get; set; }

        public SqlExpressionCompilerResult Compile(int existingParameterCount = 0, string parameterNamePrefix = SqlExpressionCompiler.DefaultParameterNamePrefix)
        {
            if (string.IsNullOrEmpty((parameterNamePrefix ?? string.Empty).Trim()))
            {
                throw new ArgumentException("parameterNamePrefix must be specified.", "parameterNamePrefix");
            }

            ReadOnlyCollection<ParameterExpression> ps = Expression.Parameters;

            var compiler = new SqlExpressionCompiler(existingParameterCount)
                           {
                               ParameterNamePrefix = parameterNamePrefix
                           };
            SqlExpressionCompilerResult result = compiler.Compile(Expression);


            string fieldName = FieldName;
            if (!fieldName.StartsWith("[") && !fieldName.Contains("."))
            {
                fieldName = "[" + fieldName + "]";
            }


            result.SQL = result.SQL.Replace("{FieldName}", fieldName);

            return result;
        }
    }
}