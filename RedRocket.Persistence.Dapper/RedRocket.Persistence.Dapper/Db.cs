using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using FlitBit.IoC;
using RedRocket.Persistence.Dapper.Infrastructure.Configuration;

namespace RedRocket.Persistence.Dapper
{
    public static class Db
    {
        static Db()
        {
            Connections = Create.New<Dictionary<string, IConnection>>();
            foreach (ConnectionStringSettings connectionString in ConfigurationManager.ConnectionStrings)
                Connections.Add(connectionString.Name, Create.NewWithParams<IConnection>(LifespanTracking.Automatic, Param.FromValue(connectionString.ConnectionString)));

            var key = Connections.Keys.First();
            Current = Connections[key];
        }

        public static IConnection Current { get; set; }
        public static Dictionary<string, IConnection> Connections { get; set; }

        public static void ChangeCurrentConnection(string connectionName)
        {
            if (!Connections.ContainsKey(connectionName))
                throw new KeyNotFoundException("Connection with name {0} was not found".P(connectionName));

            Current = Connections[connectionName];
        }

        public static IDbConnection NewConnection()
        {
            return Current.Open();
        }

        public static IDbTransaction NewConnectionWithTransaction()
        {
            return Current.OpenAndCreateTransaction();
        }

        public static IDbTransaction NewConnectionWithTransaction(IsolationLevel isolationLevel)
        {
            return Current.OpenAndCreateTransaction(isolationLevel);
        }
    }
}