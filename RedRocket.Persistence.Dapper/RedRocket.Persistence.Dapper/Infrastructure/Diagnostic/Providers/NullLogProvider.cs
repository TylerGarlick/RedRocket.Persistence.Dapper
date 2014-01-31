using FlitBit.IoC.Meta;

namespace RedRocket.Persistence.Dapper.Infrastructure.Diagnostic.Providers
{
    [ContainerRegister(typeof(ILogProvider), RegistrationBehaviors.Default)]
    public class NullLogProvider : ILogProvider
    {
        public void Write(ILogEvent logEvent)
        {

        }
    }
}