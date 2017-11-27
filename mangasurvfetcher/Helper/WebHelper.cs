using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace mangasurvlib.Helper
{
    public class WebHelper
    {
        private static ILogger logger = mangasurvlib.Logging.ApplicationLogging.CreateLogger<WebHelper>();

        public static bool UriExists(Uri uriToCheck)
        {
            try
            {
                var result = client.GetAsync(uriToCheck).Result;

                //Returns TRUE if the Status code == 200
                return result.StatusCode == HttpStatusCode.OK;
            }
            catch
            {
                return false;
            }
        }

        private static HttpClient client = new HttpClient();
        public async static void DownloadFile(Uri uri, string sFilename)
        {
            var result = client.GetAsync(uri).Result;
            using (Stream contentStream = await result.Content.ReadAsStreamAsync(), fileStream = new FileStream(sFilename, FileMode.Create, FileAccess.ReadWrite))
            {
                var totalRead = 0L;
                var totalReads = 0L;
                var buffer = new byte[8192];
                var isMoreToRead = true;

                do
                {
                    var read = await contentStream.ReadAsync(buffer, 0, buffer.Length);
                    if (read == 0)
                    {
                        isMoreToRead = false;
                    }
                    else
                    {
                        await fileStream.WriteAsync(buffer, 0, read);

                        totalRead += read;
                        totalReads += 1;
                    }
                }
                while (isMoreToRead);
            }
        }

        public static string DownloadString(string uri)
        {
            return DownloadString(new Uri(uri));
        }
        public static string DownloadString(Uri uri)
        {
            return client.GetStringAsync(uri).Result;
        }
    }
}
