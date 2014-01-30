﻿//Copyright (c) Chris Pietschmann 2013 (http://pietschsoft.com)
//Licensed under the GNU Library General Public License (LGPL)
//License can be found here: http://sqlinq.codeplex.com/license

namespace RedRocket.Persistence.Dapper.Infrastructure.Expressions
{
    public interface ILinqIf : ILinq
    {
        /// <summary>
        ///     The "IF" clause to evaluate
        /// </summary>
        object If { get; }

        /// <summary>
        ///     Specifies the query to execute when the "IF" clause evaluates to TRUE
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        ILinq Then { get; set; }

        /// <summary>
        ///     Specifies the query to execute when the "IF" clause evaluates to FALSE
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        ILinq Else { get; set; }
    }
}