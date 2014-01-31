using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using RedRocket.Persistence.Dapper.Infrastructure.Expressions;
using RedRocket.Persistence.Dapper.Infrastructure.Repositories;

namespace RedRocket.Persistence.Dapper.Repositories
{
    public interface IReadOnlyDapperRepository<T> : IReadOnlyRepository<T>
    {
        IEnumerable<T> All(IEnumerable<Linq<T>.OrderByExpression> sortingExpression);
        IEnumerable<T> EntitiesBy(Expression<Func<T, bool>> predicate);
        IEnumerable<T> EntitiesBy(Expression<Func<T, bool>> predicate, IEnumerable<Linq<T>.OrderByExpression> sortingExpression);
    }
}