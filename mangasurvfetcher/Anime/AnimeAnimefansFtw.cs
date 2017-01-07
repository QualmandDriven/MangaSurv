using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace mangasurvlib.Anime
{
    internal class AnimeAnimefansFtw : IAnimeLoader, Helper.ICache
    {
        private static ILogger logger = Logging.ApplicationLogging.CreateLogger<AnimeAnimefansFtw>();
        private const string AnimeListurl = "http://www.animefansftw.org/anime-series/";
        private const string AnimeUrl = "http://www.animefansftw.org/";
        private HtmlDocument AnimeListCache { get; set; }

        public AnimeAnimefansFtw()
        {
            this.PrepareCache();
        }

        public Uri GetAnime(string Name)
        {
            foreach (HtmlNode link in AnimeListCache.GetElementbyId("ddmcc_container").Descendants())
            {
                if (link.Name == "a" && link.Attributes["href"] != null && link.InnerText == Name)
                {
                    string sLink = link.Attributes["href"].Value.Replace("&amp;", "&");

                    return new Uri(sLink);
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

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(Helper.WebHelper.DownloadString(Url.AbsoluteUri));

            // Load the other chaptes pages
            //EpisodePages.AddRange(LoadEpisodesPages(doc));

            foreach (Uri episodePage in episodePages)
            {
                try
                {
                    doc = new HtmlDocument();
                    doc.LoadHtml(Helper.WebHelper.DownloadString(episodePage.AbsoluteUri));

                    foreach (HtmlNode link in doc.GetElementbyId("main-content").Descendants())
                    {
                        if (link.Name == "a" && link.Attributes["href"] != null && link.InnerText.Contains(Anime.Name))
                        {
                            string sLink = link.Attributes["href"].Value.Replace("&amp;", "&");

                            try
                            {
                                AnimeEpisode episode = new AnimeEpisode(Anime, new Uri(sLink));
                                lAnimeEpisodes.Add(episode);
                            }
                            catch (Exception ex)
                            {
                                logger.LogError(ex.Message);
                            }
                        }
                    }
                }
                catch
                {
                }
            }

            logger.LogInformation("'{0}' Episodes found on WebSite '{1}'", lAnimeEpisodes.Count, Url.AbsoluteUri);

            return lAnimeEpisodes;
        }

        private List<Uri> LoadEpisodesPages(HtmlDocument Doc)
        {
            List<Uri> episodePages = new List<Uri>();

            foreach (HtmlNode button in Doc.DocumentNode.SelectNodes("//button"))
            {
                if (button.Name == "button" && button.Attributes["href"] != null)
                {
                    if (button.Name == "button" && button.Attributes["href"] != null && button.Attributes["type"] != null)
                    {
                        if (button.Attributes["type"].Value != "button" || button.ParentNode.ParentNode.Name != "ul")
                            continue;

                        string sLink = button.Attributes["href"].Value.Replace("&amp;", "&");

                        episodePages.Add(new Uri(sLink));

                    }
                }
            }

            return episodePages;
        }

        public List<KeyValuePair<int, Uri>> GetFiles(Uri EpisodeUrl)
        {
            if (EpisodeUrl == null)
                return new List<KeyValuePair<int, Uri>>();



            // Load Files
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(Helper.WebHelper.DownloadString(EpisodeUrl.AbsoluteUri));

            List<KeyValuePair<int, Uri>> iuFiles = new List<KeyValuePair<int, Uri>>();

            foreach (HtmlNode divTitle in doc.DocumentNode.SelectNodes("//div"))
            {
                if (divTitle.Name == "div" && divTitle.Attributes["class"] != null)
                {
                    if (divTitle.Attributes["class"].Value != "dl-title" || !divTitle.InnerText.Contains("1080"))
                        continue;

                    foreach (HtmlNode link in divTitle.ParentNode.Descendants())
                    {
                        if (link.Name == "a" && link.InnerText == "Torrent")
                        {
                            string sLink = link.Attributes["href"].Value.Replace("&amp;", "&").Replace("&#038;", "&");

                            iuFiles.Add(new KeyValuePair<int, Uri>(1, new Uri(sLink)));
                            logger.LogInformation("New Torrent '{0}'", sLink);
                            return iuFiles;
                        }
                    }
                }
            }

            if (iuFiles.Count == 0)
            {
                foreach (HtmlNode divTitle in doc.DocumentNode.SelectNodes("//div"))
                {
                    if (divTitle.Name == "div" && divTitle.Attributes["class"] != null)
                    {
                        if (divTitle.Attributes["class"].Value != "dl-title")
                            continue;

                        if (divTitle.InnerText.Contains("720") && (divTitle.InnerText.Contains("HorribleSubs") || divTitle.InnerText.Contains("CCS-Speed") || divTitle.InnerText.Contains("NowWeTheChefs") || divTitle.InnerText.Contains("Underwater") || divTitle.InnerText.Contains("DeadFish") || divTitle.InnerText.Contains("sage") || divTitle.InnerText.Contains("Chyuu")))
                        {
                            foreach (HtmlNode link in divTitle.ParentNode.Descendants())
                            {
                                if (link.Name == "a" && link.InnerText == "Torrent")
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
            }


            // Wenn es eine neue Episode ist, dann hat AnimeFanFTW alles auf Otakubot umgestellt
            // also Otakubot link raussuchen und dann von dort laden
            string sOtakubotlink = String.Empty;
            foreach (HtmlNode divTitle in doc.DocumentNode.SelectNodes("//div"))
            {
                if (divTitle.Name == "div" && divTitle.Attributes["class"] != null)
                {
                    if (divTitle.Attributes["class"].Value != "dl-title" || !divTitle.InnerText.Contains("1080"))
                        continue;

                    foreach (HtmlNode link in divTitle.ParentNode.Descendants())
                    {
                        if (link.Name == "a" && link.InnerText == "Otakubot")
                        {
                            sOtakubotlink = link.Attributes["href"].Value.Replace("&amp;", "&").Replace("&#038;", "&");
                            break;
                        }

                        if (!String.IsNullOrEmpty(sOtakubotlink))
                            break;
                    }
                }
            }

            // Wenn nichts in 1080p gefunden wurde, suchen wir 720p
            if (!String.IsNullOrEmpty(sOtakubotlink))
            {
                foreach (HtmlNode divTitle in doc.DocumentNode.SelectNodes("//div"))
                {
                    if (divTitle.Name == "div" && divTitle.Attributes["class"] != null)
                    {
                        if (divTitle.Attributes["class"].Value != "dl-title")
                            continue;

                        if (divTitle.InnerText.Contains("720") && (divTitle.InnerText.Contains("HorribleSubs") || divTitle.InnerText.Contains("CCS-Speed") || divTitle.InnerText.Contains("NowWeTheChefs") || divTitle.InnerText.Contains("Underwater") || divTitle.InnerText.Contains("DeadFish") || divTitle.InnerText.Contains("sage") || divTitle.InnerText.Contains("Chyuu")))
                        {
                            foreach (HtmlNode link in divTitle.ParentNode.Descendants())
                            {
                                if (link.Name == "a" && link.InnerText == "Torrent")
                                {
                                    sOtakubotlink = link.Attributes["href"].Value.Replace("&amp;", "&")
                                        .Replace("&#038;", "&");
                                }
                            }

                            if (!String.IsNullOrEmpty(sOtakubotlink))
                                break;
                        }
                    }
                }
            }

            // Auf Otakubot wechseln und Torrent finden
            if (!String.IsNullOrEmpty(sOtakubotlink))
            {
                iuFiles = new OtakuBot().GetFiles(new Uri(sOtakubotlink));
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
                    if (img.ParentNode.Attributes["class"].Value == "cat-thumbnail")
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