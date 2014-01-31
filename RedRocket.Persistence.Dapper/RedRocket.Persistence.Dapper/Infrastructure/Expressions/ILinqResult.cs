//Copyright (c) Chris Pietschmann 2012 (http://pietschsoft.com)
//Licensed under the GNU Library General Public License (LGPL)
//License can be found here: http://sqlinq.codeplex.com/license

using System.Collections.Generic;

namespace RedRocket.Persistence.Dapper.Infrastructure.Expressions
{
    public interface ILinqResult
    {
        IDictionary<string, object> Parameters { get; set; }
        string ToSql();
    }
}