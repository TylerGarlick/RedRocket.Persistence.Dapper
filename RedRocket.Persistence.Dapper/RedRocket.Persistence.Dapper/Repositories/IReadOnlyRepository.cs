using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace RedRocket.Persistence.Dapper.Infrastructure.Repositories
{
    public interface IReadOnlyRepository<T>
    {
        IEnumerable<T> All();

        T FindByKey(Expression<Func<T, bool>> predicate);
    }
}