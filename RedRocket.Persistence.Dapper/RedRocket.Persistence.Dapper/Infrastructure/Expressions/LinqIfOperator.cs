//Copyright (c) Chris Pietschmann 2013 (http://pietschsoft.com)
//Licensed under the GNU Library General Public License (LGPL)
//License can be found here: http://sqlinq.codeplex.com/license

namespace RedRocket.Persistence.Dapper.Infrastructure.Expressions
{
    public enum LinqIfOperator
    {
        /// <summary>
        ///     Does not alter the evaluation of the IF clause
        /// </summary>
        None,

        /// <summary>
        ///     Specifies that the IF clause tests for the existence of rows
        /// </summary>
        Exists,

        /// <summary>
        ///     Reverses the boolean evaluation of the IF clause
        /// </summary>
        Not
    }
}