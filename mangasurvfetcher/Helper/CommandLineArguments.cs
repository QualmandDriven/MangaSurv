using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mangasurvfetcher.Helper
{
    /// <summary>
    /// Provides easier access for command line arguments.
    /// Possibility to check if key exists and to get the corresponding value(s).
    /// </summary>
    public class CommandLineArguments : IArguments
    {
        private static ILogger logger = mangasurvlib.Logging.ApplicationLogging.CreateLogger<Program>();

        private static readonly string[] _keyidentifiers = new string[] { "--", "-" };
        private readonly Dictionary<string, string> _dicArgs = new Dictionary<string, string>();

        /// <summary>
        /// Create new instance.
        /// </summary>
        /// <param name="args">Command line arguments which were provided by main method.</param>
        public CommandLineArguments(string[] args)
        {
            logger.LogInformation("Parsing following command line arguments: {0}", String.Join(" ", args));

            string sLastKey = String.Empty;
            foreach (string s in args)
            {
                string sTempKey;
                if (this.IsKey(s, out sTempKey))
                {
                    sLastKey = sTempKey;
                    this._dicArgs.Add(sLastKey, null);
                }
                else
                {
                    this._dicArgs[sLastKey] = s;
                }
            }

            logger.LogInformation("Parsed command line arguments:");
            foreach (KeyValuePair<string, string> pair in this._dicArgs)
            {
                logger.LogInformation("Key: '{0}' Value: '{0}'", pair.Key, pair.Value);
            }
        }

        /// <summary>
        /// Create new instance and automatically load command line arguments.
        /// </summary>
        public CommandLineArguments() : this(Environment.GetCommandLineArgs()) { }

        /// <summary>
        /// Checks if given key can be identified as such or as a value.
        /// </summary>
        /// <param name="sKey">Key which should be checked.</param>
        /// <param name="sFormattedKey">Formatted Key.</param>
        /// <returns></returns>
        private bool IsKey(string sKey, out string sFormattedKey)
        {
            sFormattedKey = String.Empty;

            foreach (string sIdentifier in _keyidentifiers)
            {
                if (sKey.Length > sIdentifier.Length && sKey.Substring(0, sIdentifier.Length) == sIdentifier)
                {
                    sFormattedKey = sKey.Substring(sIdentifier.Length);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Check if Key exists in Arguments
        /// </summary>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public bool ContainsKey(string sKey)
        {
            return this._dicArgs.ContainsKey(sKey);
        }

        /// <summary>
        /// Return value of given key.
        /// If key does not exists it returns null.
        /// </summary>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public string GetValue(string sKey)
        {
            if (this.ContainsKey(sKey))
                return this._dicArgs[sKey];

            return null;
        }
    }
}
