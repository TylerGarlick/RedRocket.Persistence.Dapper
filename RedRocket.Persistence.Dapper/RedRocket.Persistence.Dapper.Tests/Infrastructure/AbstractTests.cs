using FlitBit.Wireup;

namespace RedRocket.Persistence.Dapper.Tests.Infrastructure
{
    public abstract class AbstractTests
    {
        protected AbstractTests()
        {
            WireupCoordinator.SelfConfigure();
        }
    }
}
