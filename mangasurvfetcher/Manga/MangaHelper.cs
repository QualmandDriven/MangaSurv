using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using HtmlAgilityPack;
using System.IO;
using mangasurvlib.Extensions;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace mangasurvlib.Manga
{
    internal class MangaHelper
    {
        private static MangaConstants.MangaPage Page = MangaConstants.MangaPage.MangaBB;
        private static Dictionary<MangaConstants.MangaPage, IMangaLoader> InitializedMangaLoaders = new Dictionary<MangaConstants.MangaPage, IMangaLoader>();

        public static IMangaLoader GetMangaClass()
        {
            if (InitializedMangaLoaders.ContainsKey(Page))
                return InitializedMangaLoaders[Page];

            IMangaLoader loader = null;

            switch (Page)
            {
                case MangaConstants.MangaPage.MangaBB:
                    loader = MangaFactory.CreateLoaderMangaBB();
                    break;
                case MangaConstants.MangaPage.MangaPanda:
                    loader = MangaFactory.CreateLoaderMangaPanda();
                    break;
                case MangaConstants.MangaPage.MangaReader:
                    loader = MangaFactory.CreateLoaderMangaReader();
                    break;
                case MangaConstants.MangaPage.Batoto:
                    loader = MangaFactory.CreateLoaderBatoto();
                    break;
            }

            if (loader != null)
                InitializedMangaLoaders.Add(Page, loader);

            return loader;
        }

        public static IMangaLoader GetMangaClass(MangaConstants.MangaPage mangaPage)
        {
            Page = mangaPage;

            return GetMangaClass();
        }

        public static Uri ExtractMangaLink(string sName, string sHtml)
        {
            string sRegexName = sName.Replace("(", ".*").Replace(")", ".*").Replace("-", ".*").Replace(" ", ".*").Trim();
            Regex rx = new Regex("<a.*>.*" + sRegexName + ".*</a>", RegexOptions.IgnoreCase);
            MatchCollection ms = rx.Matches(sHtml);
            foreach (Match m in ms)
            {
                if (m.Success)
                {
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(m.Value);

                    foreach (HtmlNode link in doc.DocumentNode.Descendants("a"))
                    {
                        rx = new Regex("^" + sRegexName + "$", RegexOptions.IgnoreCase);
                        if (link.Name == "a" && link.Attributes["href"] != null && rx.IsMatch(link.InnerText.Trim()))
                        {
                            string sLink = link.Attributes["href"].Value.Replace("&amp;", "&");

                            return new System.Uri(sLink);
                        }
                    }
                }
            }

            return null;
        }
    }

    internal class MangaBB : IMangaLoader
    {
        private static ILogger logger = Logging.ApplicationLogging.CreateLogger<MangaBB>();

        private const string _MANGALISTURL = "http://www.mangabb.me/manga-list";
        private const string _MANGAURL = "http://www.mangabb.me/";

        private static string MangaListCache = String.Empty;

        public MangaBB()
        {
            this.PrepareCache();
        }

        public Uri GetManga(string sName)
        {
            return MangaHelper.ExtractMangaLink(sName, MangaListCache);
        }

        public List<MangaChapter> GetChapters(Manga manga, Uri uMangaUrl)
        {
            logger.LogInformation("Getting all Chapters for Manga '{0}'", manga.Name);

            List<MangaChapter> lMangaChapters = new List<MangaChapter>();

            if (uMangaUrl == null)
            {
                logger.LogInformation("Manga '{0}' not found in Manga list of 'MangaBB'!", manga.Name);
                return new List<MangaChapter>();
            }

            // The Chapters are not all on one page, sometimes if there are many chapters > 200
            // It will get separated in more sites to display all chapters
            List<Uri> ChapterPages = new List<Uri>();
            ChapterPages.Add(uMangaUrl);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(Helper.WebHelper.DownloadString(uMangaUrl.AbsoluteUri));

            // Load the other chaptes pages
            ChapterPages.AddRange(LoadChaptersPages(doc));

            foreach (Uri chapterPage in ChapterPages)
            {
                try
                {
                    doc = new HtmlDocument();
                    doc.LoadHtml(Helper.WebHelper.DownloadString(chapterPage.AbsoluteUri));

                    foreach (HtmlNode link in doc.GetElementbyId("chapters").Descendants())
                    {
                        if (link.Name == "a" && link.Attributes["href"] != null && link.InnerText.Trim().ToUpper().Contains(manga.Name.Trim().ToUpper()))
                        {
                            string sLink = link.Attributes["href"].Value.Replace("&amp;", "&");

                            MangaChapter chapter = MangaFactory.CreateMangaChapter(manga, new System.Uri(sLink), MangaConstants.MangaPage.MangaBB);
                            chapter.MangaPage = MangaConstants.MangaPage.MangaBB;
                            lMangaChapters.Add(chapter);
                        }
                    }
                }
                catch
                {
                }
            }

            logger.LogInformation("'{0}' Chapters found on WebSite '{1}'", lMangaChapters.Count, uMangaUrl.AbsoluteUri);

            return lMangaChapters;
        }

        public List<Uri> LoadChaptersPages(HtmlDocument doc)
        {
            List<Uri> ChapterPages = new List<Uri>();

            foreach (HtmlNode button in doc.DocumentNode.SelectNodes("//button"))
            {
                if (button.Name == "button" && button.Attributes["href"] != null)
                {
                    if (button.Name == "button" && button.Attributes["href"] != null && button.Attributes["type"] != null)
                    {
                        if (button.Attributes["type"].Value != "button" || button.ParentNode.ParentNode.Name != "ul")
                            continue;

                        string sLink = button.Attributes["href"].Value.Replace("&amp;", "&");

                        ChapterPages.Add(new System.Uri(sLink));

                    }
                }
            }

            return ChapterPages;
        }

        public List<KeyValuePair<int, Uri>> GetFiles(Uri uChapterUrl)
        {
            if (uChapterUrl == null)
                return new List<KeyValuePair<int, Uri>>();

            // Load Files
            List<Uri> lFileUrl = new List<Uri>();

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(Helper.WebHelper.DownloadString(uChapterUrl.AbsoluteUri));

            foreach (HtmlNode option in doc.DocumentNode.SelectNodes("//option"))
            {
                if (option.Name == "option" && option.Attributes["value"] != null)
                {
                    if (option.ParentNode.Name == "select" && option.ParentNode.Attributes["name"] != null && option.ParentNode.Attributes["name"].Value == "page_select")
                    {

                        string sLink = option.Attributes["value"].Value.Replace("&amp;", "&");

                        lFileUrl.Add(new System.Uri(sLink));

                    }
                }
            }

            List<KeyValuePair<int, Uri>> iuFiles = new List<KeyValuePair<int, Uri>>();

            // Get Files
            for (int i = 0; i < lFileUrl.Count; i++)
            {
                doc.LoadHtml(Helper.WebHelper.DownloadString(lFileUrl[i].AbsoluteUri));
                foreach (HtmlNode image in doc.GetElementbyId("manga_viewer").Descendants())
                {
                    if (image.Name != "img")
                        continue;

                    if (image.Attributes["src"] != null)
                    {
                        iuFiles.Add(new KeyValuePair<int, Uri>(i + 1, new Uri(image.Attributes["src"].Value)));
                    }
                }
            }

            return iuFiles;
        }


        public Uri GetMangaPictureUrl(Uri mangaUrl)
        {
            try
            {
                //series_image
                List<Uri> lFileUrl = new List<Uri>();

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(Helper.WebHelper.DownloadString(mangaUrl));

                HtmlNode image = doc.GetElementbyId("series_image");
                return new Uri(image.Attributes["src"].Value);
            }
            catch
            {
                return null; 
            }
        }


        public void PrepareCache()
        {
            //MangaListCache.LoadHtml(Helper.WebHelper.DownloadString(_MANGALISTURL));
            MangaListCache = WebUtility.HtmlDecode(Helper.WebHelper.DownloadString(_MANGALISTURL));
        }
    }

    internal class MangaPanda : IMangaLoader
    {
        private static ILogger logger = Logging.ApplicationLogging.CreateLogger<MangaPanda>();
        private const string _MANGALISTURL = "http://www.mangapanda.com/alphabetical";
        private const string _MANGAURL = "http://www.mangapanda.com";

        private static string MangaListCache = String.Empty;

        public MangaPanda()
        {
            this.PrepareCache();
        }

        public Uri GetManga(string sName)
        {
            return MangaHelper.ExtractMangaLink(sName, MangaListCache);
        }

        public List<MangaChapter> GetChapters(Manga manga, Uri uMangaUrl)
        {
            logger.LogInformation("Getting all Chapters for Manga '{0}'", manga.Name);

            List<MangaChapter> lMangaChapters = new List<MangaChapter>();

            if (uMangaUrl == null)
            {
                logger.LogInformation("Manga '{0}' not found in Manga list of 'MangaPanda'!", manga.Name);
                return new List<MangaChapter>();
            }

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(Helper.WebHelper.DownloadString(uMangaUrl.AbsoluteUri));

            foreach (HtmlNode link in doc.GetElementbyId("chapterlist").Descendants())
            {
                if (link.Name == "a" && link.Attributes["href"] != null && link.InnerText.Trim().ToUpper().Contains(manga.Name.Trim().ToUpper()))
                {
                    string sLink = link.Attributes["href"].Value.Replace("&amp;", "&");

                    MangaChapter chapter = MangaFactory.CreateMangaChapter(manga, new System.Uri(_MANGAURL + sLink), MangaConstants.MangaPage.MangaPanda);
                    lMangaChapters.Add(chapter);
                }
            }

            logger.LogInformation("'{0}' Chapters found on WebSite '{1}'", lMangaChapters.Count, uMangaUrl.AbsoluteUri);

            return lMangaChapters;
        }

        public List<KeyValuePair<int, Uri>> GetFiles(Uri uChapterUrl)
        {
            if (uChapterUrl == null)
                return new List<KeyValuePair<int, Uri>>();

            // Load Files
            List<Uri> lFileUrl = new List<Uri>();

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(Helper.WebHelper.DownloadString(uChapterUrl.AbsoluteUri));

            foreach (HtmlNode option in doc.GetElementbyId("pageMenu").Descendants())
            {
                if (option.Name == "option" && option.Attributes["value"] != null)
                {
                    string sLink = option.Attributes["value"].Value.Replace("&amp;", "&");

                    lFileUrl.Add(new System.Uri(_MANGAURL + sLink));

                }
            }

            List<KeyValuePair<int, Uri>> iuFiles = new List<KeyValuePair<int, Uri>>();

            // Get Files
            for (int i = 0; i < lFileUrl.Count; i++)
            {
                doc.LoadHtml(Helper.WebHelper.DownloadString(lFileUrl[i].AbsoluteUri));
                HtmlNode image = doc.GetElementbyId("img");

                if (image.Name != "img")
                    continue;

                if (image.Attributes["src"] != null)
                {
                    iuFiles.Add(new KeyValuePair<int, Uri>(i + 1, new Uri(image.Attributes["src"].Value)));
                }

            }

            return iuFiles;
        }


        public Uri GetMangaPictureUrl(Uri mangaUrl)
        {
            try
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(Helper.WebHelper.DownloadString(mangaUrl));
                return new Uri(doc.GetElementbyId("mangaimg").FirstChild.Attributes["src"].Value);
            }
            catch
            { }

            return null;
        }


        public void PrepareCache()
        {
            //MangaListCache.LoadHtml(Helper.WebHelper.DownloadString(_MANGALISTURL));
            MangaListCache = WebUtility.HtmlDecode(Helper.WebHelper.DownloadString(_MANGALISTURL));
        }
    }

    internal class MangaReader : IMangaLoader
    {
        private static ILogger logger = Logging.ApplicationLogging.CreateLogger<MangaReader>();
        private const string _MANGALISTURL = "http://www.mangareader.net/alphabetical";
        private const string _MANGAURL = "http://www.mangareader.net";

        private static string MangaListCache = String.Empty;

        public MangaReader()
        {
            this.PrepareCache();
        }

        public Uri GetManga(string sName)
        {
            return MangaHelper.ExtractMangaLink(sName, MangaListCache);
        }

        public List<MangaChapter> GetChapters(Manga manga, Uri uMangaUrl)
        {
            List<MangaChapter> lMangaChapters = new List<MangaChapter>();

            if (uMangaUrl == null)
            {
                logger.LogInformation("Manga '{0}' not found in Manga list of 'MangaReader'!", manga.Name);
                return new List<MangaChapter>();
            }
            
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(Helper.WebHelper.DownloadString(uMangaUrl.AbsoluteUri));

            foreach (HtmlNode link in doc.GetElementbyId("chapterlist").Descendants())
            {
                if (link.Name == "a" && link.Attributes["href"] != null && link.InnerText.Trim().ToUpper().Contains(manga.Name.Trim().ToUpper()))
                {
                    string sLink = link.Attributes["href"].Value.Replace("&amp;", "&");

                    MangaChapter chapter = MangaFactory.CreateMangaChapter(manga, new System.Uri(_MANGAURL + sLink), MangaConstants.MangaPage.MangaReader);
                    chapter.MangaPage = MangaConstants.MangaPage.MangaReader;
                    lMangaChapters.Add(chapter);
                }
            }

            logger.LogInformation("'{0}' Chapters found on WebSite '{1}'", lMangaChapters.Count, uMangaUrl.AbsoluteUri);

            return lMangaChapters;
        }

        public List<KeyValuePair<int, Uri>> GetFiles(Uri uChapterUrl)
        {
            if (uChapterUrl == null)
                return new List<KeyValuePair<int, Uri>>();

            // Load Files
            List<Uri> lFileUrl = new List<Uri>();

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(Helper.WebHelper.DownloadString(uChapterUrl.AbsoluteUri));

            foreach (HtmlNode option in doc.GetElementbyId("pageMenu").Descendants())
            {
                if (option.Name == "option" && option.Attributes["value"] != null)
                {
                    string sLink = option.Attributes["value"].Value.Replace("&amp;", "&");

                    lFileUrl.Add(new System.Uri(_MANGAURL + sLink));

                }
            }

            List<KeyValuePair<int, Uri>> iuFiles = new List<KeyValuePair<int, Uri>>();

            // Get Files
            for (int i = 0; i < lFileUrl.Count; i++)
            {
                doc.LoadHtml(Helper.WebHelper.DownloadString(lFileUrl[i].AbsoluteUri));
                HtmlNode image = doc.GetElementbyId("img");

                if (image.Name != "img")
                    continue;

                if (image.Attributes["src"] != null)
                {
                    iuFiles.Add(new KeyValuePair<int, Uri>(i + 1, new Uri(image.Attributes["src"].Value)));
                }

            }

            return iuFiles;
        }


        public Uri GetMangaPictureUrl(Uri mangaUrl)
        {
            try
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(Helper.WebHelper.DownloadString(mangaUrl));
                return new Uri(doc.GetElementbyId("mangaimg").FirstChild.Attributes["src"].Value);
            }
            catch
            { }

            return null;
        }


        public void PrepareCache()
        {
            //MangaListCache.LoadHtml(Helper.WebHelper.DownloadString(_MANGALISTURL));
            MangaListCache = WebUtility.HtmlDecode(Helper.WebHelper.DownloadString(_MANGALISTURL));
        }
    }

    internal class Batoto : IMangaLoader
    {
        private static ILogger logger = Logging.ApplicationLogging.CreateLogger<Batoto>();
        private const string _MANGALISTURL = "";
        private const string _MANGAURL = "http://bato.to/";
        private const string _FILENAME = "batotoMangaList.html";

        private static string MangaListCache = String.Empty;

        public Batoto()
        {
            this.PrepareCache();
        }

        public Uri GetManga(string sName)
        {
            return MangaHelper.ExtractMangaLink(sName, MangaListCache);
        }

        public List<MangaChapter> GetChapters(Manga manga, Uri uMangaUrl)
        {
            logger.LogInformation("Getting all Chapters for Manga '{0}'", manga.Name);

            List<MangaChapter> lMangaChapters = new List<MangaChapter>();

            if (uMangaUrl == null)
            {
                logger.LogInformation("Manga '{0}' not found in Manga list of 'Batoto'!", manga.Name);
                return new List<MangaChapter>();
            }
            
            string sTest = Helper.WebHelper.DownloadString(uMangaUrl.AbsoluteUri);

            HtmlDocument doc = new HtmlDocument();

            doc.LoadHtml(sTest);

            try
            {
                foreach (HtmlNode element in doc.GetElementbyId("content").Descendants())
                {
                    // Wenn es keine Tabellenzeile ist oder die Tabellenzeile nicht die richtige klasse hat überspringen wir
                    if (element.Name != "tr" || element.GetAttributeValue("class", "") != "row lang_English chapter_row")
                    {
                        continue;
                    }

                    foreach (HtmlNode link in element.Descendants("a"))
                    {
                        //if (link.Name == "tr" && link.Attributes["href"] != null && link.InnerText.Trim().ToUpper().Contains(manga.Name.Trim().ToUpper()))
                        if (link.Name == "a" && link.Attributes["href"] != null && link.InnerText.Trim().ToUpper().Contains("Ch.".ToUpper()))
                        {
                            string sLink = link.Attributes["href"].Value.Replace("&amp;", "&");

                            MangaChapter chapter = MangaFactory.CreateMangaChapter(manga, new System.Uri(sLink), MangaConstants.MangaPage.Batoto);
                            if(chapter.Chapter == 0)
                            {
                                chapter.GetChapterOfDescription(link.InnerText.Trim());
                            }

                            chapter.MangaPage = MangaConstants.MangaPage.Batoto;
                            lMangaChapters.Add(chapter);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Error while getting chapters - download html file '{0}'{1}{2}", uMangaUrl.AbsoluteUri, Environment.NewLine, ex.Message);
            }

            logger.LogInformation("'{0}' Chapters found on WebSite '{1}'", lMangaChapters.Count, uMangaUrl.AbsoluteUri);

            return lMangaChapters;
        }

        public List<KeyValuePair<int, Uri>> GetFiles(Uri uChapterUrl)
        {
            if (uChapterUrl == null)
                return new List<KeyValuePair<int, Uri>>();

            // Load Files
            
            List<Uri> lFileUrl = new List<Uri>();

            HtmlDocument doc = new HtmlDocument();
            string sTest = Helper.WebHelper.DownloadString(uChapterUrl.AbsoluteUri);

            doc.LoadHtml(sTest);

            List<KeyValuePair<int, Uri>> iuFiles = new List<KeyValuePair<int, Uri>>();

            // Wenn das Element existiert, dann werden die Chapters auf mehreren Seiten angezeigt
            // Wenn nicht, dann werden die Bilder untereinander angezeigt und sind auf 1ner Seite
            if (doc.GetElementbyId("page_select") != null)
            {
                foreach (HtmlNode option in doc.GetElementbyId("page_select").Descendants())
                {
                    if (option.Name == "option" && option.Attributes["value"] != null)
                    {
                        string sLink = option.Attributes["value"].Value.Replace("&amp;", "&");

                        lFileUrl.Add(new System.Uri(sLink));

                    }
                }

                // Get Files
                for (int i = 0; i < lFileUrl.Count; i++)
                {
                    logger.LogInformation("Loading file '{0}'", lFileUrl[i].AbsoluteUri);
                    string sTemp = Helper.WebHelper.DownloadString(lFileUrl[i].AbsoluteUri);
                    doc.LoadHtml(sTemp);
                    HtmlNode image = doc.GetElementbyId("comic_page");

                    if (image.Name != "img")
                        continue;

                    if (image.Attributes["src"] != null)
                    {
                        iuFiles.Add(new KeyValuePair<int, Uri>(i + 1, new Uri(image.Attributes["src"].Value)));
                    }

                }
            }
            else
            {
                int i = 1;
                foreach (HtmlNode image in doc.GetElementbyId("content").Descendants())
                {
                    if (image.Name != "img")
                        continue;

                    if (image.Attributes["src"] != null && image.Attributes["alt"] != null)
                    {
                        iuFiles.Add(new KeyValuePair<int, Uri>(i, new Uri(image.Attributes["src"].Value)));
                        i++;
                    }
                }
            }


            return iuFiles;
        }

        public Uri GetMangaPictureUrl(Uri mangaUrl)
        {
            try
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(Helper.WebHelper.DownloadString(mangaUrl));
                foreach (HtmlNode div in doc.DocumentNode.SelectNodes("//div"))
                {
                    try
                    {
                        if (div.Attributes["class"].Value == "ipsBox")
                        {
                            foreach (HtmlNode img in div.Descendants())
                            {
                                if (img.Name == "img")
                                {
                                    return new Uri(img.Attributes["src"].Value);
                                }

                            }
                        }
                    }
                    catch
                    { }
                }
            }
            catch
            { }

            return null;
        }

        public void PrepareCache()
        {
            FileInfo fi = new FileInfo(_FILENAME);

            if (!fi.Exists || fi.LastWriteTime.Year != DateTime.Now.Year || fi.LastWriteTime.Day < DateTime.Now.Day - 7)
            {
                StringBuilder sb = new StringBuilder();

                sb.Append("<html><body><table><tbody>");
                List<Task<Tuple<bool, string>>> tasks = new List<Task<Tuple<bool, string>>>();
                do
                {
                    tasks.Add(Task.Factory.StartNew<Tuple<bool, string>>(() => DoAjaxRequest("http://bato.to/search_ajax?&p=")));

                    if (tasks.Count == 10)
                    {
                        Task.WaitAll(tasks.ToArray());

                        bool bBreak = false;
                        foreach (Task<Tuple<bool, string>> t in tasks)
                        {
                            if (t.Result.Item1 == false)
                                bBreak = true;

                            sb.Append(t.Result.Item2);
                        }

                        tasks.Clear();

                        if (bBreak)
                            break;
                    }
                } while (true);

                sb.Append("</tbody></table></body></html>");
                File.WriteAllText(_FILENAME, sb.ToString());
                //MangaListCache.LoadHtml(sb.ToString());
                MangaListCache = WebUtility.HtmlDecode(sb.ToString());
            }
            else
            {
                //string sText = File.ReadAllText(fi.FullName);
                //MangaListCache.LoadHtml(sText);
                MangaListCache = WebUtility.HtmlDecode(File.ReadAllText(fi.FullName));
            }
        }

        private int _searchCount = 0;
        private object lockObject = new object();

        private Tuple<bool, string> DoAjaxRequest(string sUrl)
        {
            lock (lockObject)
            {
                _searchCount++;
                sUrl += _searchCount;
            }

            logger.LogInformation("Loading Cache Part '{0}'", sUrl);
            string sHtml = Helper.WebHelper.DownloadString(sUrl);

            if (sHtml.Contains("No (more) comics found!"))
                return new Tuple<bool, string>(false, String.Empty);

            int iStart = sHtml.IndexOf("<tbody>") + 7;
            int iEnd = sHtml.IndexOf("</tbody>");
            string sContent = sHtml.Substring(iStart, iEnd - iStart);

            return new Tuple<bool, string>(true, sContent);
        }

    }
}
