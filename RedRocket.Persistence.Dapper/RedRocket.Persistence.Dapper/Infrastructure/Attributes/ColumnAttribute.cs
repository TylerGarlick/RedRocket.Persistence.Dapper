using System;

namespace RedRocket.Persistence.Dapper.Infrastructure.Attributes
{
    public interface IBasicColumn
    {
        string Column { get; }
        string Schema { get; set; }
    }


    public interface IColumn : IBasicColumn
    {
        bool Insert { get; set; }
        bool Update { get; set; }
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
        public string Schema { get; set; }
        public bool Insert { get; set; }
        public bool Update { get; set; }
    }
}