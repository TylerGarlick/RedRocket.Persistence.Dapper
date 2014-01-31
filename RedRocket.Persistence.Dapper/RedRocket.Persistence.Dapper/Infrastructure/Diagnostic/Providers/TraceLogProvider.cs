using System.Diagnostics;
using FlitBit.IoC.Meta;

namespace RedRocket.Persistence.Dapper.Infrastructure.Diagnostic.Providers
{
    [ContainerRegister(typeof(ILogProvider), RegistrationBehaviors.Named, "Trace")]

    public class TraceLogProvider : ILogProvider
    {
        public void Write(ILogEvent logEvent)
        {
            Trace.WriteLine(logEvent);
        }
    }
}
