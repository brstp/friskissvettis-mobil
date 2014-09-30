using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FriskisSvettisLib.Helpers
{
    public class DictionaryHelper
    {

        public static T GetValue<T>(string key, Dictionary<string, object> dict, T defaultValue)
        {
            if (dict.ContainsKey(key))
            {
                return Convert<T>(dict[key], defaultValue);
            }

            return defaultValue;
        }
            

        /// <summary>
        /// Helper to convert a object to a type. Defaults to 
        /// a default value if the conversion was unsuccessfull.
        /// </summary>
        /// <typeparam name="T">Type to convert to</typeparam>
        /// <param name="value">Original value</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns></returns>
        private static T Convert<T>(object value, T defaultValue)
        {

            T convertedValue = defaultValue;
            try
            {
                convertedValue = (T)System.Convert.ChangeType(value, typeof(T));
            }
            catch { }
            return convertedValue;
        }
    }
}
