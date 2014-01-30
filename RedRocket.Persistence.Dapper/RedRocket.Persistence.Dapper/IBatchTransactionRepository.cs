using System.Collections.Generic;

namespace RedRocket.Persistence.Dapper
{
    public interface IBatchTransactionRepository<in T>
    {
        void BatchAdd(IEnumerable<T> entities);
        void BatchUpdate(IEnumerable<T> entities);
        void BatchDelete(IEnumerable<T> entities);
    }
}