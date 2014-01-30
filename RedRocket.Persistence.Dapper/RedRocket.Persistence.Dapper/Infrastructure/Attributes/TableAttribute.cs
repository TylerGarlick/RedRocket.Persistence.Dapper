using System;
using FlitBit.Dto;

namespace RedRocket.Persistence.Dapper.Infrastructure.Attributes
{
    public interface ITable
    {
        string Table { get; }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class TableAttribute : DTOAttribute, ITable
    {
        public TableAttribute(string tableName)
        {
            Table = tableName;
        }

        public string Table { get; private set; }
    }
}
