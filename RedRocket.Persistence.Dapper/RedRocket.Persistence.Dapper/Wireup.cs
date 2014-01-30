using System.Data;
using System.Data.SqlClient;
using FlitBit.IoC;
using FlitBit.Wireup;
using FlitBit.Wireup.Meta;
using RedRocket.Persistence.Dapper.Infrastructure;

namespace RedRocket.Persistence.Dapper
{
    [assembly: Wireup(typeof(RedRocket.Persistence.Dapper.AssemblyWireup))]
    public class AssemblyWireup : IWireupCommand
    {
        public void Execute(IWireupCoordinator coordinator)
        {
            Container.Root
                     .ForType<IDbConnection>()
                     .Register<SqlConnection>()
                     .ResolveAnInstancePerRequest()
                     .End();

            Container.Root.ForGenericType(typeof (IRepository<>))
                     .Register(typeof (Repository<>))
                     .ResolveAnInstancePerScope()
                     .End();
        }
    }
}