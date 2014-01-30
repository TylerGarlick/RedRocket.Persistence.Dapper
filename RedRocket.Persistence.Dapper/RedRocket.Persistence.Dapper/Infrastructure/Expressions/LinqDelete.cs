using System;
using System.Linq.Expressions;
using RedRocket.Persistence.Dapper.Infrastructure.Expressions.Compiler;

namespace RedRocket.Persistence.Dapper.Infrastructure.Expressions
{
    public class LinqDelete<T> : AbstractLinqExpression<T>, ILinqDelete
    {
        public LinqDelete(T data)
            : base(data)
        {
        }

        public LinqDelete(T data, string tableNameOverride)
            : base(data, tableNameOverride)
        {
        }


        public ILinqResult ToSql(int existingParameterCount = 0, string parameterNamePrefix = SqlExpressionCompiler.DefaultParameterNamePrefix)
        {
            throw new NotImplementedException();
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