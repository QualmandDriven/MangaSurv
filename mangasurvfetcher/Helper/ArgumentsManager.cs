using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mangasurvfetcher.Helper
{
    /// <summary>
    /// Returns values of keys either from command line arguments or environment variable.
    /// </summary>
    public class ArgumentsManager : IArguments
    {
        private CommandLineArguments cmdArguments;
        private EnvironmentArguments envArguments;

        /// <summary>
        /// Create new instance.
        /// </summary>
        public ArgumentsManager()
        {
            this.cmdArguments = new CommandLineArguments();
            this.envArguments = new EnvironmentArguments();
        }

        /// <summary>
        /// Check if key exists as an argument in a source.
        /// </summary>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public bool ContainsKey(string sKey)
        {
            return cmdArguments.ContainsKey(sKey) || envArguments.ContainsKey(sKey);
        }

        /// <summary>
        /// Returns value of key from an argument source.
        /// </summary>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public string GetValue(string sKey)
        {
            if (cmdArguments.ContainsKey(sKey))
                return cmdArguments.GetValue(sKey);

            return envArguments.GetValue(sKey);
        }
    }
}
