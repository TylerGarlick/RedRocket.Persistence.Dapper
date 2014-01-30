//Copyright (c) Chris Pietschmann 2013 (http://pietschsoft.com)
//Licensed under the GNU Library General Public License (LGPL)
//License can be found here: http://sqlinq.codeplex.com/license

using System.Collections.Generic;
using System.Text;

namespace RedRocket.Persistence.Dapper.Infrastructure.Expressions
{
    public class LinqCollectionResult : ILinqResult
    {
        public LinqCollectionResult()
        {
            Parameters = new Dictionary<string, object>();
            Queries = new List<string>();
        }

        public IList<string> Queries { get; set; }
        public IDictionary<string, object> Parameters { get; set; }

        public string ToQuery()
        {
            var sql = new StringBuilder();

            foreach (string query in Queries)
            {
                sql.AppendLine(query);
            }

            return sql.ToString();
        }
    }
}