namespace RedRocket.Persistence.Dapper.Infrastructure.Diagnostic.Providers
{
    public interface ILogProvider
    {
        void Write(ILogEvent logEvent);
    }
}