using System;
using System.Linq;
using System.Linq.Expressions;

namespace RedRocket.Persistence.Dapper
{
    public interface IReadOnlyRepository<T>
    {
        IQueryable<T> All();

        T FindByKey(Expression<Func<T, bool>> predicate);
    }
}