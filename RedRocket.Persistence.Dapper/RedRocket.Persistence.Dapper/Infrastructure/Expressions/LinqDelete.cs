using System;
using System.Linq.Expressions;
using RedRocket.Persistence.Dapper.Infrastructure.Expressions.Compiler;

namespace RedRocket.Persistence.Dapper.Infrastructure.Expressions
{
    public class LinqDelete<T> : AbstractLinqExpression<T>, ILinqDelete
    {
        public LinqDelete()
        {
        }

        public LinqDelete(T data)
            : base(data)
        {
        }

        public LinqDelete(string tableNameOverride)
            : base(tableNameOverride)
        {
        }

        public ILinqResult Render(int existingParameterCount = 0, string parameterNamePrefix = SqlExpressionCompiler.DefaultParameterNamePrefix)
        {
            return GenerateSql(existingParameterCount, parameterNamePrefix);
        }

        public LinqDelete<T> Where(Expression<Func<T, bool>> expression)
        {
            Expressions.Add(expression);
            return this;
        }

        public LinqDelete<T> Where(Expression expression)
        {
            Expressions.Add(expression);
            return this;
        }
    }
}