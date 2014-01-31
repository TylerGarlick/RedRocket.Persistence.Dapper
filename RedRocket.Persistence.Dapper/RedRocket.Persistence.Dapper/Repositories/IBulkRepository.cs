using System.Collections.Generic;

namespace RedRocket.Persistence.Dapper.Infrastructure.Repositories
{
    public interface IBulkRepository<T> : IRepository<T>
    {
        void BulkAdd(IEnumerable<T> entities);
        void BulkInsert(IEnumerable<T> entities);
        void BulkDelete(IEnumerable<T> entities);
    }
}