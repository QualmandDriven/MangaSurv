using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Net;

namespace mangasurvlib.Extensions
{
    public static class ExtensionsClass
    {
        public static string ToFolderString(this double d)
        {
            string sChapter = d.ToString("0.0", CultureInfo.InvariantCulture);
            if (sChapter.EndsWith(".0"))
                sChapter = sChapter.Replace(".0", "");

            return sChapter;
        }

        public static void SetChromeRequest(this HttpWebRequest request)
        {
            //request.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2");
        }
    }
}
