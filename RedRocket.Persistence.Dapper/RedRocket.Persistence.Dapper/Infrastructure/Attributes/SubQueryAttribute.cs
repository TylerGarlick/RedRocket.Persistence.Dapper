using System;
using System.Collections.Generic;
using RedRocket.Persistence.Dapper.Infrastructure.Expressions;

namespace RedRocket.Persistence.Dapper.Infrastructure.Attributes
{
    public interface ISubQuery
    {
        string Sql { get; set; }
        Type Type { get; set; }
        string Method { get; set; }
        string GetQuery(Dictionary<string, object> parameters);
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class SubQueryAttribute : Attribute, ISubQuery
    {
        public string Sql { get; set; }
        public Type Type { get; set; }
        public string Method { get; set; }
        public string GetQuery(Dictionary<string, object> parameters)
        {
            if (!string.IsNullOrEmpty(Sql))
                return Sql;

            // Invoke static method that returns the sub-query to use
            var methods = Type.GetMethods();
            var method = Type.GetMethod(Method);
            var query = (ILinq)method.Invoke(null, null);

            var result = query.Render();

            // Add parameters
            foreach (var p in result.Parameters)
                parameters.Add(p.Key, p.Value);

            // return SQL
            return result.ToSql();
        }
    }
}