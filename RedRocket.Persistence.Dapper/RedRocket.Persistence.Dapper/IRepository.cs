using RedRocket.Persistence.Dapper.Infrastructure.Repositories;
using RedRocket.Persistence.Dapper.Repositories;

namespace RedRocket.Persistence.Dapper
{
    public interface IRepository<T> : IReadOnlyDapperRepository<T>, IUpsertRepository<T>
    {
    }
}
