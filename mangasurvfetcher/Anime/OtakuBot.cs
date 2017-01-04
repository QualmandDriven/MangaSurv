using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace mangasurvlib.Anime
{
    internal class OtakuBot : IAnimeLoader, Helper.ICache
    {
        private static ILogger logger = Logging.ApplicationLogging.CreateLogger<OtakuBot>();
        private const string AnimeUrl = "http://www.otakubot.org/";
        private static HtmlDocument AnimeListCache { get; set; }

        public OtakuBot()
        {
            
        }

        public Uri GetAnime(string Name)
        {
            throw new NotImplementedException();
        }

        public List<AnimeEpisode> GetEpisodes(Anime Anime, Uri Url)
        {
            throw new NotImplementedException();
        }

        public List<KeyValuePair<int, Uri>> GetFiles(Uri EpisodeUrl)
        {
            List<KeyValuePair<int, Uri>> iuFiles = new List<KeyValuePair<int, Uri>>();

            // Auf Otakubot wechseln und torrent finden
            if (!String.IsNullOrEmpty(EpisodeUrl.AbsoluteUri))
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(Helper.WebHelper.DownloadString(EpisodeUrl.AbsoluteUri));

                foreach (HtmlNode divTitle in doc.DocumentNode.SelectNodes("//div"))
                {
                    if (divTitle.Name == "div" && divTitle.Attributes["class"] != null)
                    {
                        if (divTitle.Attributes["class"].Value != "dl_box")
                            continue;

                        foreach (HtmlNode link in divTitle.ParentNode.Descendants())
                        {
                            if (link.Name == "a" && link.InnerText.Contains("nyaa.se"))
                            {
                                string sLink = link.Attributes["href"].Value.Replace("&amp;", "&")
                                    .Replace("&#038;", "&");

                                iuFiles.Add(new KeyValuePair<int, Uri>(1, new Uri(sLink)));
                                logger.LogInformation("New Torrent '{0}'", sLink);
                                return iuFiles;
                            }
                        }
                    }
                }
            }

            return iuFiles;
        }

        public Uri GetAnimePictureUrl(Uri animeUrl)
        {
            throw new NotImplementedException();
        }

        public void PrepareCache()
        {
            throw new NotImplementedException();
        }
    }
}
