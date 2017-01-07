using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mangasurvlib.Helper
{
    public class StringHelper
    {
        /// <summary>
        /// Replace all special characters from given string with "_"
        /// </summary>
        /// <param name="sString">String which has to be replaced.</param>
        /// <returns></returns>
        public static string ReplaceSpecialCharacters(string sString)
        {
            return sString.Replace(":", "_").Replace("-", "_").Replace("?", "_");
        }

        /// <summary>
        /// Replace all special characters from given string
        /// </summary>
        /// <param name="sString">String which has to be replaced.</param>
        /// <param name="sReplace">String which is used instead of replaced string.</param>
        /// <returns></returns>
        public static string ReplaceSpecialCharacters(string sString, string sReplace)
        {
            return sString.Replace(":", sReplace).Replace("-", sReplace).Replace("?", sReplace);
        }
    }
}
