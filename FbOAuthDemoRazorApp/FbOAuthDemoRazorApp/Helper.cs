using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace FbOAuthDemoRazorApp
{
    public class Helper
    {
        /// <summary>
        /// Converts an object to query string for use with API request handling
        /// </summary>
        /// <param name="obj">The onject to convert</param>
        /// <returns>Query string</returns>
        public static string ObjectToQueryString(object obj)
        {
            var s1 = JsonConvert.SerializeObject(obj);

            var s2 = JsonConvert.DeserializeObject<IDictionary<string, string>>(s1);

            var s3 = s2.Select(x => HttpUtility.UrlEncode(x.Key) + "=" + HttpUtility.UrlEncode(x.Value));

            return string.Join("&", s3);
        }

        /// <summary>
        /// Returns comma seperated string with the property names of a type
        /// </summary>
        /// <param name="type">The class type</param>
        /// <returns></returns>
        public static string JoinPropertiesForDataQuery(Type type)
        {
            return string.Join(",",
                type.GetProperties()
                .Select(p => p.GetCustomAttribute<JsonPropertyAttribute>())
                .Select(jp => jp.PropertyName));
        }
    }
}
