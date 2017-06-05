using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace mangasurvlib.Anime
{
    class HorribleSubs : IAnimeLoader, Helper.ICache
    {
        private static ILogger logger = Logging.ApplicationLogging.CreateLogger<HorribleSubs>();
        private const string AnimeListurl = "http://horriblesubs.info/shows/";
        private const string AnimeUrl = "http://horriblesubs.info";
        private HtmlDocument AnimeListCache { get; set; }

        private const string _JavaScriptAnimeId = "<script type=\"text/javascript\">var hs_showid =";
        private const string _ShowsUrl = "http://horriblesubs.info/lib/getshows.php?type=show&showid=";

        public HorribleSubs()
        {
            this.PrepareCache();
        }

        public Uri GetAnime(string Name)
        {
            foreach (HtmlNode link in AnimeListCache.GetElementbyId("main").Descendants())
            {
                if (link.Name == "a" && link.Attributes["href"] != null && link.InnerText == Name)
                {
                    string sLink = link.Attributes["href"].Value.Replace("&amp;", "&");

                    return new Uri(AnimeUrl + sLink);
                }
            }

            return null;
        }

        public List<AnimeEpisode> GetEpisodes(Anime Anime, Uri Url)
        {
            logger.LogInformation("Getting all Episodes for Anime '{0}'", Anime.Name);

            List<AnimeEpisode> lAnimeEpisodes = new List<AnimeEpisode>();

            if (Url == null)
            {
                logger.LogInformation("Anime '{0}' not found in Anime list!", Anime.Name);
                return new List<AnimeEpisode>();
            }

            // The Episodes are not all on one page, sometimes if there are many Episodes > 200
            // It will get separated in more sites to display all Episodes
            List<Uri> episodePages = new List<Uri> { Url };

            string sHtml = Helper.WebHelper.DownloadString(Url.AbsoluteUri);
            
            int s = sHtml.IndexOf(_JavaScriptAnimeId) + _JavaScriptAnimeId.Length;
            int e = sHtml.IndexOf(";", s);
            string sId = sHtml.Substring(s, e - s).Trim();

            // Das Html enthält nicht die ganzen Shows, diese werden erst mit JAvascript nachgeladen
            // D.h. wir suchen das javascript raus und laden es dann selber
            HtmlDocument doc = new HtmlDocument();
            string sShowUrls = _ShowsUrl + sId;
            doc.LoadHtml("<html>" + Helper.WebHelper.DownloadString(sShowUrls) + "</html>");

            doc.Save(new System.IO.FileStream("C:\\temp\\hr.html", System.IO.FileMode.Create));

            foreach (HtmlNode el in doc.DocumentNode.SelectNodes("//div"))
            {
                if(el.Attributes["class"] != null && el.Attributes["class"].Value.Contains("release-links"))
                {
                    foreach(HtmlNode lbl in el.SelectNodes("//i"))
                    {
                        // Wenn Text dann Animenamen und die [1080p] enthält (nur dann wollen wir es laden, keine low-quality
                        // z.B. "Berserk - 21.5 [1080p]"
                        if(lbl.InnerText.StartsWith(Anime.Name) && lbl.InnerText.EndsWith("[1080p]"))
                        {
                            foreach(HtmlNode link in lbl.ParentNode.ParentNode.Descendants("a"))
                            {
                                // Wenn es auch einen Torrent dazu gibt
                                if (link.Name == "a" && link.Attributes["href"] != null && link.InnerText == "Torrent")
                                {
                                    string sLink = link.Attributes["href"].Value.Replace("&amp;", "&");

                                    try
                                    {
                                        string sEp = lbl.InnerText.Replace(" [1080p]", "");
                                        int start = -1;
                                        for(int i = sEp.Length; i > 0; i--)
                                        {
                                            if(lbl.InnerText[i] == '-')
                                            {
                                                start = i;
                                                break;
                                            }
                                        }

                                        // Nur wenn wir die Episode rauslesen können, wird es aufgenommen
                                        // Sonst haben wir ja keine Episodennummer -> logisch oder?
                                        if (start > -1)
                                        {
                                            double ep = double.Parse(sEp.Substring(start + 1).Trim(), System.Globalization.CultureInfo.InvariantCulture);
                                            AnimeEpisode episode = new AnimeEpisode(Anime, new Uri(sShowUrls), ep);
                                            if(!lAnimeEpisodes.Contains(episode))
                                                lAnimeEpisodes.Add(episode);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        logger.LogError(ex.Message);
                                    }
                                }
                            }
                        }
                    }
                
                }
            }

            logger.LogInformation("'{0}' Episodes found on WebSite '{1}'", lAnimeEpisodes.Count, Url.AbsoluteUri);

            return lAnimeEpisodes;
        }

        public List<KeyValuePair<int, Uri>> GetFiles(string anime, double episodeNo, Uri EpisodeUrl)
        {
            if (EpisodeUrl == null)
                return new List<KeyValuePair<int, Uri>>();
            
            // Load Files
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(Helper.WebHelper.DownloadString(EpisodeUrl.AbsoluteUri));

            List<KeyValuePair<int, Uri>> iuFiles = new List<KeyValuePair<int, Uri>>();

            string epNo = episodeNo.ToString().Replace(",", ".");

            foreach (HtmlNode lbl in doc.DocumentNode.SelectNodes("//i"))
            {
                // Wenn Text dann Animenamen und die [1080p] enthält (nur dann wollen wir es laden, keine low-quality
                // z.B. "Berserk - 21.5 [1080p]"
                if (lbl.InnerText.StartsWith(anime) && lbl.InnerText.EndsWith(epNo + " [1080p]"))
                {
                    foreach (HtmlNode link in lbl.ParentNode.ParentNode.Descendants("a"))
                    {
                        if (link.Name == "a" && link.Attributes["href"] != null && link.InnerText == "Torrent")
                        {
                            string sLink = link.Attributes["href"].Value.Replace("&amp;", "&").Replace("&#038;", "&");

                            iuFiles.Add(new KeyValuePair<int, Uri>(1, new Uri(sLink)));
                            logger.LogInformation("New Torrent '{0}'", sLink);
                            return iuFiles;
                        }
                    }
                }
            }

            return iuFiles;
        }

        public Uri GetAnimePictureUrl(Uri animeUrl)
        {
            try
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(Helper.WebHelper.DownloadString(animeUrl));
                foreach (HtmlNode img in doc.DocumentNode.SelectNodes("//img"))
                {
                    //cat-thumbnail
                    if (img.ParentNode.Attributes["class"].Value == "series-image")
                    {
                        return new Uri(AnimeUrl + img.Attributes["src"].Value);
                    }
                }
            }
            catch
            { }

            return null;
        }

        public void PrepareCache()
        {
            AnimeListCache = new HtmlDocument();
            AnimeListCache.LoadHtml(Helper.WebHelper.DownloadString(AnimeListurl));
        }
    }
}
