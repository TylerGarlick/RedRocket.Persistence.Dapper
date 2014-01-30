using System;

namespace RedRocket.Persistence.Dapper.Infrastructure.Attributes
{
    public interface IBasicColumn
    {
        string Column { get; }
        string Schema { get;  }
    }


    public interface IColumn : IBasicColumn
    {
        bool Insert { get; }
        bool Update { get;  }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ColumnAttribute : Attribute, IColumn
    {
        public ColumnAttribute(string columnName = null, bool insert = true, bool update = true, string schema = null)
        {
            Column = columnName;
            Insert = insert;
            Update = update;
        }

        public string Column { get; private set; }
        public string Schema { get; private set; }
        public bool Insert { get; private set; }
        public bool Update { get; private set; }
    }
}