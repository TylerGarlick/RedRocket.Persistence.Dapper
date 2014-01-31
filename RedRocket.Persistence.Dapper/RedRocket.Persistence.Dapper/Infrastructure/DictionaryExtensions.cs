using System.Collections.Generic;
using System.Dynamic;

namespace RedRocket.Persistence.Dapper.Infrastructure
{
    public static class DictionaryExtensions
    {
        public static object ToDynamicThenObject(this IDictionary<string, object> dictionary)
        {
            var parameters = (ICollection<KeyValuePair<string, object>>)new ExpandoObject();
            foreach (var keyValuePair in dictionary)
                parameters.Add(keyValuePair);
            return parameters;
        }
    }
}
