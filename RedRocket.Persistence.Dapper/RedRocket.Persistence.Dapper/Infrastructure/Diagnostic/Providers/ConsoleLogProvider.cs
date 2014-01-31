using System;
using FlitBit.IoC.Meta;

namespace RedRocket.Persistence.Dapper.Infrastructure.Diagnostic.Providers
{
    [ContainerRegister(typeof(ILogProvider), RegistrationBehaviors.Named, "Console")]
    public class ConsoleLogProvider : ILogProvider
    {
        public void Write(ILogEvent logEvent)
        {
            Console.WriteLine(logEvent.Message);
        }
    }
}