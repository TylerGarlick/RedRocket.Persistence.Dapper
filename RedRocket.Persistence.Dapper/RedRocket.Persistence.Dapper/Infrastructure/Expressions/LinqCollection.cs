//Copyright (c) Chris Pietschmann 2013 (http://pietschsoft.com)
//Licensed under the GNU Library General Public License (LGPL)
//License can be found here: http://sqlinq.codeplex.com/license

using System.Collections.Generic;
using RedRocket.Persistence.Dapper.Infrastructure.Expressions.Compiler;

namespace RedRocket.Persistence.Dapper.Infrastructure.Expressions
{
    public class LinqCollection : List<ILinq>, ILinq
    {
        public LinqCollection() {}

        public LinqCollection(params ILinq[] queries)
        {
            AddRange(queries);
        }

        public LinqCollection(IEnumerable<ILinq> queries)
        {
            AddRange(queries);
        }

        public ILinqResult ToSql(int existingParameterCount = 0, string parameterNamePrefix = SqlExpressionCompiler.DefaultParameterNamePrefix)
        {
            int paramCount = existingParameterCount;

            var result = new LinqCollectionResult();

            foreach (ILinq query in this)
            {
                ILinqResult r = query.ToSql(paramCount, parameterNamePrefix);

                foreach (var p in r.Parameters)
                {
                    result.Parameters.Add(p);
                }

                result.Queries.Add(r.ToQuery());

                paramCount = existingParameterCount + result.Parameters.Count;
            }

            return result;
        }
    }
}