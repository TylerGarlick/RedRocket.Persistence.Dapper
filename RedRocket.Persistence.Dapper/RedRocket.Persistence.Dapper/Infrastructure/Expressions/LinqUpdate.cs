//Copyright (c) Chris Pietschmann 2013 (http://pietschsoft.com)
//Licensed under the GNU Library General Public License (LGPL)
//License can be found here: http://sqlinq.codeplex.com/license

using System;
using System.Linq.Expressions;
using RedRocket.Persistence.Dapper.Infrastructure.Expressions.Compiler;

namespace RedRocket.Persistence.Dapper.Infrastructure.Expressions
{
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

        public ILinqResult Render(int existingParameterCount = 0, string parameterNamePrefix = SqlExpressionCompiler.DefaultParameterNamePrefix)
        {
            return GenerateSql(existingParameterCount, parameterNamePrefix);
        }
    }
}