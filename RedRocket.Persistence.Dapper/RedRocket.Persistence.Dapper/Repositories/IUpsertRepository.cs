namespace RedRocket.Persistence.Dapper.Infrastructure.Repositories
{
    public interface IUpsertRepository<T>
    {
        T Add(T entity);
        T Update(T entity);
        void Delete(T entity);
    }
}