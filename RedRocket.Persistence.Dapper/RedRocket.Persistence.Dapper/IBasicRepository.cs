namespace RedRocket.Persistence.Dapper
{
    public interface IBasicRepository<T> : IReadOnlyRepository<T>, IBasicUpsertEnabledRepository<T>
    {
        
    }
}