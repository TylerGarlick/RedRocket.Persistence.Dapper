using System.Data;
using FlitBit.IoC;
using FlitBit.IoC.Meta;

namespace RedRocket.Persistence.Dapper.Infrastructure.Configuration
{
    public interface IConnection
    {
        IDbConnection Open(bool openConnection = true);
        IDbTransaction OpenAndCreateTransaction();
        IDbTransaction OpenAndCreateTransaction(IsolationLevel isolationLevel);
    }

    [ContainerRegister(typeof(IConnection), RegistrationBehaviors.Default)]
    public class DefaultConnection : IConnection
    {
        readonly string _connectionString;

        public DefaultConnection(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection Open(bool openConnection = true)
        {
            var connection = Create.NewWithParams<IDbConnection>(LifespanTracking.Automatic, Param.FromValue(_connectionString));
            if(openConnection)
                connection.Open();
            return connection;
        }

        public IDbTransaction OpenAndCreateTransaction()
        {
            var connection = Open();
            return connection.BeginTransaction();
        }

        public IDbTransaction OpenAndCreateTransaction(IsolationLevel isolationLevel)
        {
            var connection = Open();
            return connection.BeginTransaction();
        }
    }
}