using FlitBit.Dto;

namespace RedRocket.Persistence.Dapper.Infrastructure.Diagnostic
{
    [DTO]
    public interface ILogEvent
    {
        string Message { get; set; }
    }
}