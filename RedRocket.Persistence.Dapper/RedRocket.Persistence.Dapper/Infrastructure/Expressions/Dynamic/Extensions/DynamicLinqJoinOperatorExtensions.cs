//Copyright (c) Chris Pietschmann 2012 (http://pietschsoft.com)
//Licensed under the GNU Library General Public License (LGPL)
//License can be found here: http://sqlinq.codeplex.com/license

using System;

namespace RedRocket.Persistence.Dapper.Infrastructure.Expressions.Dynamic.Extensions
{
    public static class DynamicLinqJoinOperatorExtensions
    {
        public static string ToSql(this DynamicLinqJoinOperator op)
        {
            switch (op)
            {
                case DynamicLinqJoinOperator.Inner:
                    return "JOIN";
                case DynamicLinqJoinOperator.Left:
                    return "LEFT JOIN";
                case DynamicLinqJoinOperator.Right:
                    return "RIGHT JOIN";
                case DynamicLinqJoinOperator.Full:
                    return "FULL JOIN";
                default:
                    throw new Exception(string.Format("Unsupported DynamicSQLinqJoinOperator value: {0}", op));
            }
        }
    }
}