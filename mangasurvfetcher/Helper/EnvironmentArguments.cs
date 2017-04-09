using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mangasurvfetcher.Helper
{
    /// <summary>
    /// Access environment variables.
    /// </summary>
    public class EnvironmentArguments : IArguments
    {
        /// <summary>
        /// Checks if the key exists as environment variable.
        /// </summary>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public bool ContainsKey(string sKey)
        {
            return !String.IsNullOrEmpty(Environment.GetEnvironmentVariable(sKey));
        }

        /// <summary>
        /// Return value of environment variable.
        /// </summary>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public string GetValue(string sKey)
        {
            return Environment.GetEnvironmentVariable(sKey);
        }
    }
}
