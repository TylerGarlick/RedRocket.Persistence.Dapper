//Copyright (c) Chris Pietschmann 2013 (http://pietschsoft.com)
//Licensed under the GNU Library General Public License (LGPL)
//License can be found here: http://sqlinq.codeplex.com/license

namespace RedRocket.Persistence.Dapper.Infrastructure.Expressions
{
    public static class ObjectExtensions
    {
        /// <summary>
        ///     Creates an instance of SQLinq that is based off the specified Object Type.
        /// </summary>
        /// <typeparam name="T">The Type to use for creating the SQLinq instance.</typeparam>
        /// <param name="obj">The Object Type to base the SQLinq instance off of.</param>
        /// <param name="tableName">
        ///     Optional. The database table name to use for generated SQL code. If specified, this will
        ///     override the Objects name and/or SQLinqTable attribute usage.
        /// </param>
        /// <returns>A SQLinq instance.</returns>
        public static Linq<T> ToLinq<T>(this T obj, string tableName = null)
        {
            return Linq.Create(obj, tableName);
        }

        public static LinqInsert<T> ToLinqInsert<T>(this T obj, string tableName = null)
        {
            return Linq.Insert(obj, tableName);
        }

        public static LinqUpdate<T> ToLinqUpdate<T>(this T obj, string tableName = null)
        {
            return Linq.Update(obj, tableName);
        }
    }
}