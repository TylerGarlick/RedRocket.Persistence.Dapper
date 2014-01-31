//Copyright (c) Chris Pietschmann 2013 (http://pietschsoft.com)
//Licensed under the GNU Library General Public License (LGPL)
//License can be found here: http://sqlinq.codeplex.com/license

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RedRocket.Persistence.Dapper.Infrastructure.Attributes;
using RedRocket.Persistence.Dapper.Infrastructure.Expressions.Compiler;

namespace RedRocket.Persistence.Dapper.Infrastructure.Expressions
{
    public class LinqInsert<T> : ILinqInsert
    {
        public LinqInsert(T data)
        {
            Data = data;
        }

        public LinqInsert(T data, string tableNameOverride)
            : this(data)
        {
            TableNameOverride = tableNameOverride;
        }

        public T Data { get; set; }
        public string TableNameOverride { get; set; }

        public ILinqResult Render(int existingParameterCount = 0, string parameterNamePrefix = SqlExpressionCompiler.DefaultParameterNamePrefix)
        {
            int _parameterNumber = existingParameterCount;
            _parameterNumber++;

            Type type = Data.GetType();
            var parameters = new Dictionary<string, object>();
            var fields = new Dictionary<string, string>();

            // Get Table / View Name
            string tableName = GetTableName();

            foreach (PropertyInfo p in type.GetProperties())
            {
                bool includeInInsert = true;
                string fieldName = p.Name;
                var attr = p.GetCustomAttributes(typeof (ColumnAttribute), true).FirstOrDefault() as ColumnAttribute;
                if (attr != null)
                {
                    includeInInsert = attr.Insert;
                    if (!string.IsNullOrEmpty(attr.Column))
                    {
                        fieldName = attr.Column;
                    }
                }

                if (includeInInsert)
                {
                    string parameterName = "@" + parameterNamePrefix + _parameterNumber;

                    fields.Add(fieldName, parameterName);
                    parameters.Add(parameterName, p.GetValue(Data, null));

                    _parameterNumber++;
                }
            }

            return new LinqInsertResult
                   {
                       Table = tableName,
                       Fields = fields,
                       Parameters = parameters
                   };
        }

        private string GetTableName()
        {
            string tableName = string.Empty;
            if (!string.IsNullOrEmpty(TableNameOverride))
            {
                tableName = TableNameOverride;
            }
            else
            {
                // Get Table / View Name
                Type type = Data.GetType();
                tableName = type.Name;
                var tableAttribute = type.GetCustomAttributes(typeof (TableAttribute), false).FirstOrDefault() as TableAttribute;
                if (tableAttribute != null)
                {
                    // Table / View name is explicitly set, use that instead
                    tableName = tableAttribute.Table;
                }
            }

            return string.Format("[{0}]", tableName);
        }
    }
}