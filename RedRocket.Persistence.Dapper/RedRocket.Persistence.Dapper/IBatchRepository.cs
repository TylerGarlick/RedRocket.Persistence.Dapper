using RedRocket.Persistence.Dapper.Infrastructure.Repositories;

namespace RedRocket.Persistence.Dapper
{
    public interface IBatchRepository<T> : IRepository<T>, IBulkRepository<T>
    {

    }
}