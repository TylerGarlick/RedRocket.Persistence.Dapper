using FlitBit.IoC;
using RedRocket.Persistence.Dapper.Infrastructure.Diagnostic;

namespace RedRocket.Persistence.Dapper
{
    public static class Logger
    {
        public static void Write(string message)
        {
            Create.New<ILogger>().Write(Create.NewInit<ILogEvent>().Init(new
                                                                         {
                                                                             Message = message
                                                                         }));
        }
    }
}