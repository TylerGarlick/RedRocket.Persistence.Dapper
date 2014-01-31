namespace RedRocket.Persistence.Dapper.Infrastructure.Repositories
{
    public interface IBasicRepository<T> : IReadOnlyRepository<T>, IUpsertRepository<T>
    {
        
    }
}