//Copyright (c) Chris Pietschmann 2013 (http://pietschsoft.com)
//Licensed under the GNU Library General Public License (LGPL)
//License can be found here: http://sqlinq.codeplex.com/license

using System.Collections.Generic;
using System.Text;

namespace RedRocket.Persistence.Dapper.Infrastructure.Expressions
{
    public class LinqSelectResult : ILinqResult
    {
        public string[] Select { get; set; }
        public bool? Distinct { get; set; }
        public int? Take { get; set; }
        public int? Skip { get; set; }
        public string Table { get; set; }
        public string[] Join { get; set; }
        public string Where { get; set; }
        public string Having { get; set; }
        public string[] GroupBy { get; set; }
        public string[] OrderBy { get; set; }
        public IDictionary<string, object> Parameters { get; set; }

        /// <summary>
        ///     Returns the Full SQL statement for the specified query
        /// </summary>
        /// <returns></returns>
        public string ToQuery()
        {
            string orderby = ConcatFieldArray(OrderBy);

            string groupby = ConcatFieldArray(GroupBy);


            var sb = new StringBuilder();

            if (Distinct == true)
            {
                sb.Append("DISTINCT ");
            }

            // SELECT
            sb.Append(ConcatFieldArray(Select));


            if (Skip != null)
            {
                if (sb.Length > 0)
                {
                    sb.Append(",");
                }
                sb.Append(string.Format(" ROW_NUMBER() OVER (ORDER BY {0}) AS [SQLinq_row_number]", orderby));
            }

            sb.Append(" FROM ");

            if (Distinct == true && Skip != null && Take != null)
            {
                sb.Append("(SELECT DISTINCT ");
                sb.Append(ConcatFieldArray(Select));
                sb.Append(" FROM ");
                sb.Append(Table);
                sb.Append(") AS d");
            }
            else
            {
                sb.Append(Table);
            }

            if (Join != null)
            {
                foreach (string j in Join)
                {
                    sb.Append(" ");
                    sb.Append(j);
                }
            }

            if (!string.IsNullOrEmpty(Where))
            {
                sb.Append(" WHERE ");
                sb.Append(Where);
            }

            if (!string.IsNullOrEmpty(groupby))
            {
                sb.Append(" GROUP BY ");
                sb.Append(groupby);
            }

            if (!string.IsNullOrEmpty(Having))
            {
                sb.Append(" HAVING ");
                sb.Append(Having);
            }


            string sqlOrderBy = string.Empty;
            if (orderby.Length > 0)
            {
                sqlOrderBy = " ORDER BY " + orderby;
            }

            if (Skip != null)
            {
                // paging support
                string start = (Skip + 1).ToString();
                string end = (Skip + (Take ?? 0)).ToString();
                if (Take == null)
                {
                    if (Distinct == true)
                    {
                        return string.Format(@"WITH SQLinq_data_set AS (SELECT {0}) SELECT * FROM SQLinq_data_set WHERE [SQLinq_row_number] >= {1} ORDER BY [SQLinq_row_number]", sb.ToString(), start);
                    }
                    else
                    {
                        return string.Format(@"WITH SQLinq_data_set AS (SELECT {0}) SELECT * FROM SQLinq_data_set WHERE [SQLinq_row_number] >= {1}", sb.ToString(), start);
                    }
                }

                return string.Format(@"WITH SQLinq_data_set AS (SELECT {0}) SELECT * FROM SQLinq_data_set WHERE [SQLinq_row_number] BETWEEN {1} AND {2}", sb.ToString(), start, end);
            }
            else if (Take != null)
            {
                string sbQuery = sb.ToString();
                if (sbQuery.ToLower().StartsWith("distinct "))
                {
                    return "SELECT DISTINCT TOP " + Take.ToString() + " " + sbQuery.Substring(9) + sqlOrderBy;
                }
                else
                {
                    return "SELECT TOP " + Take.ToString() + " " + sbQuery + sqlOrderBy;
                }
            }

            return "SELECT " + sb.ToString() + sqlOrderBy;
        }

        private static string ConcatFieldArray(string[] fields)
        {
            if (fields == null)
                return string.Empty;
            if (fields.Length == 0)
                return string.Empty;

            var sb = new StringBuilder();
            for (int s = 0; s < fields.Length; s++)
            {
                if (s > 0)
                {
                    sb.Append(", ");
                }
                sb.Append(fields[s]);
            }
            return sb.ToString();
        }
    }
}