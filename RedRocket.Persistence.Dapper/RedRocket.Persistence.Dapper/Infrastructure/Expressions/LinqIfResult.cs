//Copyright (c) Chris Pietschmann 2013 (http://pietschsoft.com)
//Licensed under the GNU Library General Public License (LGPL)
//License can be found here: http://sqlinq.codeplex.com/license

using System.Collections.Generic;
using System.Text;

namespace RedRocket.Persistence.Dapper.Infrastructure.Expressions
{
    public class LinqIfResult : ILinqResult
    {
        public LinqIfResult()
        {
            Parameters = new Dictionary<string, object>();
        }

        public LinqIfOperator Operator { get; set; }
        public string If { get; set; }
        public string Then { get; set; }
        public string Else { get; set; }
        public IDictionary<string, object> Parameters { get; set; }

        public string ToSql()
        {
            bool isThenDefined = !string.IsNullOrWhiteSpace(Then);
            bool isElseDefined = !string.IsNullOrWhiteSpace(Else);

            var sql = new StringBuilder("IF (");

            if (!isThenDefined && isElseDefined)
            {
                sql.Append("NOT(");
            }

            bool isOperatorDefined = true;
            switch (Operator)
            {
                case LinqIfOperator.Exists:
                    sql.Append("EXISTS(");
                    break;

                case LinqIfOperator.Not:
                    sql.Append("NOT(");
                    break;

                case LinqIfOperator.None:
                default:
                    isOperatorDefined = false;
                    break;
            }

            sql.Append(If);

            if (isOperatorDefined)
            {
                sql.Append(")");
            }

            if (!isThenDefined && isElseDefined)
            {
                sql.Append(")");
            }

            sql.AppendLine(")");

            if (isThenDefined)
            {
                sql.AppendLine("BEGIN");
                sql.AppendLine(Then);
                sql.AppendLine("END");

                if (isElseDefined)
                {
                    sql.AppendLine("ELSE");
                }
            }

            if (isElseDefined)
            {
                sql.AppendLine("BEGIN");
                sql.AppendLine(Else);
                sql.AppendLine("END");
            }

            return sql.ToString();
        }
    }
}