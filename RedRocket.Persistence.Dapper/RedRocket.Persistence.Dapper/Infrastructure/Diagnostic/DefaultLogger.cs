using System;
using System.Collections.Generic;
using FlitBit.IoC;
using FlitBit.IoC.Meta;
using RedRocket.Persistence.Dapper.Infrastructure.Diagnostic.Providers;

namespace RedRocket.Persistence.Dapper.Infrastructure.Diagnostic
{
    public interface ILogger
    {
        bool Enabled { get; set; }
        void Write(ILogEvent logEvent);
    }


    [ContainerRegister(typeof(ILogger), RegistrationBehaviors.Default, ScopeBehavior = ScopeBehavior.Singleton)]
    public class DefaultLogger : ILogger
    {
        public IEnumerable<ILogProvider> Providers { get; private set; }

        public bool Enabled { get; set; }

        public DefaultLogger(bool enabled = true)
        {
            Enabled = enabled;
            Providers = new List<ILogProvider>()
                        {
                            Create.New<ILogProvider>(),
                            Create.NewNamed<ILogProvider>("Console"),
                            Create.NewNamed<ILogProvider>("Trace")
                        };
        }

        public DefaultLogger(IEnumerable<ILogProvider> providers)
        {
            Providers = providers;
        }

        public void Write(ILogEvent logEvent)
        {
            if (Enabled)
                foreach (var provider in Providers)
                    provider.Write(logEvent);
        }
    }
}