//Copyright (c) Chris Pietschmann 2013 (http://pietschsoft.com)
//Licensed under the GNU Library General Public License (LGPL)
//License can be found here: http://sqlinq.codeplex.com/license

using System.Collections.Generic;
using RedRocket.Persistence.Dapper.Infrastructure.Expressions.Compiler;

namespace RedRocket.Persistence.Dapper.Infrastructure.Expressions.Dynamic
{
    public class DynamicLinqInsert : ILinqInsert
    {
        public DynamicLinqInsert(IDictionary<string, object> data, string tableName)
        {
            Table = tableName;
            Data = data;
        }

        public string Table { get; set; }
        public IDictionary<string, object> Data { get; set; }

        public ILinqResult Render(int existingParameterCount = 1, string parameterNamePrefix = SqlExpressionCompiler.DefaultParameterNamePrefix)
        {
            int _parameterNumber = existingParameterCount;

            var fields = new Dictionary<string, string>();
            var parameters = new Dictionary<string, object>();

            foreach (var item in Data)
            {
                string fieldName = item.Key;
                object parameterValue = item.Value;
                string parameterName = "@" + parameterNamePrefix + _parameterNumber;

                fields.Add(fieldName, parameterName);
                parameters.Add(parameterName, parameterValue);

                _parameterNumber++;
            }

            return new LinqInsertResult
                   {
                       Table = Table,
                       Fields = fields,
                       Parameters = parameters
                   };
        }
    }
}