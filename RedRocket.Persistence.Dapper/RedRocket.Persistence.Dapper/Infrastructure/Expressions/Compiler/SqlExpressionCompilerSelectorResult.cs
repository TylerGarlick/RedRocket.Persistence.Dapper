﻿//Copyright (c) Chris Pietschmann 2012 (http://pietschsoft.com)
//Licensed under the GNU Library General Public License (LGPL)
//License can be found here: http://sqlinq.codeplex.com/license

using System.Collections.Generic;

namespace RedRocket.Persistence.Dapper.Infrastructure.Expressions.Compiler
{
    public class SqlExpressionCompilerSelectorResult : ISqlExpressionResult
    {
        public SqlExpressionCompilerSelectorResult()
        {
            Select = new List<string>();
            Parameters = new Dictionary<string, object>();
        }

        public IList<string> Select { get; set; }
        public IDictionary<string, object> Parameters { get; set; }
    }
}