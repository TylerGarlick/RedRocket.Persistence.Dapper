using System;

namespace RedRocket.Persistence.Dapper.Infrastructure.Attributes
{

    public interface IPrimaryKey : IBasicColumn
    {

    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class PrimaryKeyAttribute : ColumnAttribute, IPrimaryKey
    {
        public PrimaryKeyAttribute(string columnName = null, string schema = null) : base(columnName, false, true)
        {
            Schema = schema;
        }
    }
}