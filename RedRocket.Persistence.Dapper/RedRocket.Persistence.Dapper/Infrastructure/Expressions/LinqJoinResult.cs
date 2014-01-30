//Copyright (c) Chris Pietschmann 2012 (http://pietschsoft.com)
//Licensed under the GNU Library General Public License (LGPL)
//License can be found here: http://sqlinq.codeplex.com/license

using System.Collections.Generic;

namespace RedRocket.Persistence.Dapper.Infrastructure.Expressions
{
    public class LinqJoinResult
    {
        public LinqJoinResult()
            : this(new string[0]) {}

        public LinqJoinResult(IEnumerable<string> join)
            : this(join, new Dictionary<string, object>()) {}

        public LinqJoinResult(IEnumerable<string> join, IDictionary<string, object> parameters)
        {
            Join = join;
            Parameters = parameters;
        }

        public IEnumerable<string> Join { get; set; }
        public IDictionary<string, object> Parameters { get; set; }
    }
}