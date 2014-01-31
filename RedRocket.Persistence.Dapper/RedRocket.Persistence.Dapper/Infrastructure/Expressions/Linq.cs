//Copyright (c) Chris Pietschmann 2012 (http://pietschsoft.com)
//Licensed under the GNU Library General Public License (LGPL)
//License can be found here: http://sqlinq.codeplex.com/license

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using RedRocket.Persistence.Dapper.Infrastructure.Attributes;
using RedRocket.Persistence.Dapper.Infrastructure.Expressions.Compiler;
using RedRocket.Persistence.Dapper.Infrastructure.Expressions.Dynamic;

namespace RedRocket.Persistence.Dapper.Infrastructure.Expressions
{
    public static class Linq
    {
        /// <summary>
        ///     Creates a new SQLinq object for the Type of the object specified.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object that defines the Type to use for creating the SQLinq object instance for.</param>
        /// <returns></returns>
        public static Linq<T> Create<T>(T obj, string tableName)
        {
            return new Linq<T>(tableName);
        }

        /// <summary>
        ///     Creates a new DynamicSQLinq object for the specified table name.
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static DynamicLinq Create(string tableName)
        {
            return new DynamicLinq(tableName);
        }

        /// <summary>
        ///     Creates a new SQLinqInsert object for the specified Object.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static LinqInsert<T> Insert<T>(T data)
        {
            return new LinqInsert<T>(data);
        }

        /// <summary>
        ///     Creates a new SQLinqInsert object for the specified Object and table name.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static LinqInsert<T> Insert<T>(T data, string tableName)
        {
            return new LinqInsert<T>(data, tableName);
        }

        /// <summary>
        ///     Creates a new SQLinqInsert object for the specified Object.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static LinqUpdate<T> Update<T>(T data)
        {
            return new LinqUpdate<T>(data);
        }

        /// <summary>
        ///     Creates a new SQLinqInsert object for the specified Object and table name.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static LinqUpdate<T> Update<T>(T data, string tableName)
        {
            return new LinqUpdate<T>(data, tableName);
        }
    }

    
}