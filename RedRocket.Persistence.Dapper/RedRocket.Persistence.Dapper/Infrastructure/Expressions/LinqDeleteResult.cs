using System;
using System.Collections.Generic;
using System.Text;

namespace RedRocket.Persistence.Dapper.Infrastructure.Expressions
{
    public class LinqDeleteResult : ILinqResult
    {
        public string Table { get; set; }

        public IDictionary<string, string> Fields { get; set; }

        public string Where { get; set; }
        public IDictionary<string, object> Parameters { get; set; }

        public string ToQuery()
        {
            if (string.IsNullOrWhiteSpace((Table ?? string.Empty).Trim()))
                throw new ArgumentException("SQLinqInsertResult.Table is required to have a value.", "Table");

            if (Fields == null || Fields.Count == 0)
                throw new ArgumentNullException("Fields");

            if (Parameters == null || Parameters.Count == 0)
                throw new ArgumentNullException("Parameters");

            var fieldParameterList = new StringBuilder();

            var isFirst = true;
            foreach (var f in Fields)
            {
                if (!isFirst)
                    fieldParameterList.Append(", ");
                else
                    isFirst = false;

                if (f.Key.StartsWith("["))
                    fieldParameterList.Append(f.Key);
                else
                {
                    fieldParameterList.Append("[");
                    fieldParameterList.Append(f.Key);
                    fieldParameterList.Append("]");
                }

                fieldParameterList.Append(" = ");
                fieldParameterList.Append(f.Value);
            }

            if (string.IsNullOrEmpty(Where))
                throw new ArgumentNullException("Where","Where Parameters are required");

            return "UPDATE {0} SET {1} WHERE {2}".P(Table, fieldParameterList, Where);
        }
    }
}