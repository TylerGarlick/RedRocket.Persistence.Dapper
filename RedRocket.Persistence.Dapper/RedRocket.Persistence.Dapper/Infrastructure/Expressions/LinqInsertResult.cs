//Copyright (c) Chris Pietschmann 2013 (http://pietschsoft.com)
//Licensed under the GNU Library General Public License (LGPL)
//License can be found here: http://sqlinq.codeplex.com/license

using System;
using System.Collections.Generic;
using System.Text;

namespace RedRocket.Persistence.Dapper.Infrastructure.Expressions
{
    public class LinqInsertResult : ILinqResult
    {
        public string Table { get; set; }

        public Dictionary<string, string> Fields { get; set; }

        public IDictionary<string, object> Parameters { get; set; }

        public string ToSql()
        {
            if (string.IsNullOrWhiteSpace((Table ?? string.Empty).Trim()))
            {
                throw new ArgumentException("SQLinqInsertResult.Table is required to have a value.", "Table");
            }
            if (Fields == null || Fields.Count == 0)
            {
                throw new ArgumentNullException("Fields");
            }
            if (Parameters == null || Parameters.Count == 0)
            {
                throw new ArgumentNullException("Parameters");
            }

            var fieldList = new StringBuilder();
            var parameterList = new StringBuilder();

            bool isFirst = true;
            foreach (var f in Fields)
            {
                if (!isFirst)
                {
                    fieldList.Append(", ");
                    parameterList.Append(", ");
                }
                else
                {
                    isFirst = false;
                }

                if (f.Key.StartsWith("["))
                {
                    fieldList.Append(f.Key);
                }
                else
                {
                    fieldList.Append("[");
                    fieldList.Append(f.Key);
                    fieldList.Append("]");
                }

                parameterList.Append(f.Value);
            }

            return string.Format("INSERT {0} ({1}) VALUES ({2})", Table, fieldList, parameterList);
        }
    }
}