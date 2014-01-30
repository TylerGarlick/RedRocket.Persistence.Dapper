namespace RedRocket.Persistence.Dapper
{
    public interface IBatchRepository<T> : IRepository<T>, IBatchTransactionRepository<T>
    {

    }
}