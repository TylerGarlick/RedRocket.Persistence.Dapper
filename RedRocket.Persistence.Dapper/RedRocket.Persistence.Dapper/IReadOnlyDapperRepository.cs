using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using RedRocket.Persistence.Dapper.Infrastructure.Expressions;

namespace RedRocket.Persistence.Dapper
{
    public interface IReadOnlyDapperRepository<T> : IReadOnlyRepository<T>
    {
        IQueryable<T> All(string sql);
        IQueryable<T> All(IEnumerable<Linq<T>.OrderByExpression> sortingExpression);
        IQueryable<T> EntitiesBy(string sql);
        IQueryable<T> EntitiesBy(Expression<Func<T, bool>> predicate);
        IQueryable<T> EntitiesBy(Expression<Func<T, bool>> predicate, IEnumerable<Linq<T>.OrderByExpression> sortingExpression);
    }
}