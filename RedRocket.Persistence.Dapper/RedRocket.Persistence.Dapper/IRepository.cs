namespace RedRocket.Persistence.Dapper
{
    public interface IRepository<T> : IReadOnlyDapperRepository<T>, IBasicUpsertEnabledRepository<T>
    {

    }
}
