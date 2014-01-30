namespace RedRocket.Persistence.Dapper
{
    public interface IBasicUpsertEnabledRepository<T>
    {
        T Add(T entity);
        T Update(T entity);
        void Delete(T entity);
    }
}