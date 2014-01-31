//Copyright (c) Chris Pietschmann 2013 (http://pietschsoft.com)
//Licensed under the GNU Library General Public License (LGPL)
//License can be found here: http://sqlinq.codeplex.com/license

using System;
using System.Collections.Generic;
using System.Text;

namespace RedRocket.Persistence.Dapper.Infrastructure.Expressions
{
    public class LinqUpdateResult : ILinqResult
    {
        public string Table { get; set; }

        public IDictionary<string, string> Fields { get; set; }

        public string Where { get; set; }
        public IDictionary<string, object> Parameters { get; set; }

        public string ToSql()
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

            return string.IsNullOrWhiteSpace(Where) ?
                string.Format("UPDATE {0} SET {1}", Table, fieldParameterList) :
                string.Format("UPDATE {0} SET {1} WHERE {2}", Table, fieldParameterList, Where);
        }
    }
}