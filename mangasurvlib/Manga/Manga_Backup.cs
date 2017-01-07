//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Net;
//using HtmlAgilityPack;

//namespace mangasurvlib.Manga
//{
//    /// <summary>
//    /// Manga name from MangaPanda
//    /// </summary>
//    public class Manga
//    {
//        //internal const string _MANGALISTURL = "http://www.mangapanda.com/alphabetical";
//        //internal const string _MANGAPANDAURL = "http://www.mangapanda.com";

//        internal const string _MANGALISTURL = "http://www.mangabb.me/manga-list";
//        internal const string _MANGAPANDAURL = "http://www.mangabb.me/";

//        private string _Name;
//        private string _SavePath;
//        private List<MangaChapter> _Chapters;
//        private bool ChaptersLoaded = false;

//        private delegate void DelegateDownloadChapter(MangaChapter chapter);
//        private delegate void DelegateLoadChapter(MangaChapter chapter);

//        public Manga(string Name, string SavePath)
//        {
//            this.Name = Name;
//            this.SavePath = SavePath;
//        }

//        public string Name
//        {
//            get
//            {
//                return this._Name;
//            }
//            set
//            {
//                this._Name = value;
//            }
//        }

//        public string SavePath
//        {
//            get
//            {
//                return this._SavePath;
//            }
//            set
//            {
//                this._SavePath = value;
//            }
//        }

//        public List<MangaChapter> Chapters
//        {
//            get
//            {
//                return this._Chapters;
//            }
//            set
//            {
//                this._Chapters = value;
//            }
//        }

//        /// <summary>
//        /// Loads and then Downloads every Chapter of Manga.
//        /// </summary>
//        public void DownloadManga()
//        {
//            ParadiseLib.Logger.Logger.AddEntry("Loading URL's and Downloading every chapter of Manga '{0}'", this.Name);

//            if (!this.ChaptersLoaded)
//                this.LoadChapters();

//            this.DownloadChapter(this.Chapters);
//        }

//        /// <summary>
//        /// Downloads chapters asynchronous.
//        /// </summary>
//        /// <param name="Chapter"></param>
//        public void DownloadChapter(List<MangaChapter> lMangaChapter)
//        {
//            Threading.DelegateManager manager = new Threading.DelegateManager(20);
//            foreach (MangaChapter chapter in lMangaChapter)
//            {
//                manager.WaitForFreeSpot();

//                DelegateDownloadChapter delDownload = new DelegateDownloadChapter(DownloadChapter);
//                manager.AddDelegate(delDownload.BeginInvoke(chapter, null, null));
//            }

//            manager.WaitTillAllEnd();
//        }

//        /// <summary>
//        /// Downloads the chapter.
//        /// </summary>
//        /// <param name="Chapter"></param>
//        public void DownloadChapter(MangaChapter Chapter)
//        {
//            Logger.Logger.AddEntry("Downloading Chapter " + Chapter.Chapter);
//            Chapter.Download();
//        }

//        /// <summary>
//        /// Gets all chapters and loads the URL's of them.
//        /// </summary>
//        public void LoadChapters()
//        {
//            this.Chapters = this.GetAllChapters();
//            this.LoadChapters(this.Chapters);

//            this.ChaptersLoaded = true;
//        }

//        /// <summary>
//        /// Loads chapters asynchronous.
//        /// </summary>
//        /// <param name="lMangaChapters"></param>
//        private void LoadChapters(List<MangaChapter> lMangaChapters)
//        {
//            Threading.DelegateManager manager = new Threading.DelegateManager(25);

//            foreach (MangaChapter chapter in lMangaChapters)
//            {
//                manager.WaitForFreeSpot();

//                DelegateLoadChapter downloadDelegate = new DelegateLoadChapter(LoadChapter);
//                manager.AddDelegate(downloadDelegate.BeginInvoke(chapter, null, null));
//            }

//            manager.WaitTillAllEnd();
//        }

//        /// <summary>
//        /// Loads URL's of chapter.
//        /// </summary>
//        /// <param name="chapter"></param>
//        public void LoadChapter(MangaChapter chapter)
//        {
//            ParadiseLib.Logger.Logger.AddEntry("Loading Chapter " + chapter.Chapter);
//            chapter.LoadUrls();
//        }

//        /// <summary>
//        /// Gets a list of all Chapters from Manga (gets it from WebSite).
//        /// </summary>
//        /// <returns></returns>
//        private List<MangaChapter> GetAllChapters()
//        {
//            ParadiseLib.Logger.Logger.AddEntry("Getting all Chapters for Manga '{0}'", this.Name);

//            List<MangaChapter> lMangaChapters = new List<MangaChapter>();

//            Uri uriManga = this.GetMangaUrl();

//            if (uriManga == null)
//                throw new Exception("Manga not found in Manga list!");

//            WebClient client = new WebClient();

//            HtmlDocument doc = new HtmlDocument();
//            doc.LoadHtml(client.DownloadString(uriManga.AbsoluteUri));

//            Threading.DelegateManager manager = new Threading.DelegateManager(25);

//            foreach (HtmlNode link in doc.GetElementbyId("chapterlist").Descendants())
//            {
//                if (link.Name == "a" && link.Attributes["href"] != null && link.InnerText.Contains(this.Name))
//                {
//                    string sLink = link.Attributes["href"].Value.Replace("&amp;", "&");

//                    MangaChapter chapter = new MangaChapter(this, new System.Uri(_MANGAPANDAURL + sLink));
//                    lMangaChapters.Add(chapter);
//                }
//            }

//            ParadiseLib.Logger.Logger.AddEntry("'{0}' Chapters found on WebSite '{1}'", lMangaChapters.Count, uriManga.AbsoluteUri);

//            return lMangaChapters;
//        }

//        /// <summary>
//        /// Gets URL of Manga to find chapters.
//        /// </summary>
//        /// <returns></returns>
//        private Uri GetMangaUrl()
//        {
//            WebClient client = new WebClient();

//            HtmlDocument doc = new HtmlDocument();
//            doc.LoadHtml(client.DownloadString(_MANGALISTURL));

//            foreach (HtmlNode link in doc.GetElementbyId("wrapper_body").Descendants())
//            {
//                if (link.Name == "a" && link.Attributes["href"] != null && link.InnerText == this.Name)
//                {
//                    string sLink = link.Attributes["href"].Value.Replace("&amp;", "&");

//                    return new System.Uri(_MANGAPANDAURL + sLink);
//                }
//            }

//            return null;
//        }

//        public override string ToString()
//        {
//            return String.Format("Manga '{0}'", this.Name);
//        }

//        /// <summary>
//        /// Finds, loads and downloads new chapters.
//        /// </summary>
//        /// <returns>List of new chapters found.</returns>
//        public List<MangaChapter> DownloadNewChapters()
//        {
//            ParadiseLib.Logger.Logger.AddEntry("Checking for new chapters for Manga '{0}'", this.Name);
            
//            List<MangaChapter> lNewChapters = new List<MangaChapter>();

//            foreach (MangaChapter chapter in this.GetAllChapters())
//            {
//                if (!this.Chapters.Contains(chapter))
//                {
//                    ParadiseLib.Logger.Logger.AddEntry("New Chapter '{0}' found!", chapter.Chapter);
//                    lNewChapters.Add(chapter);
//                }
//            }

//            if (lNewChapters.Count > 0)
//            {
//                this.Chapters.AddRange(lNewChapters);
//                this.DownloadChapter(lNewChapters);
//                ParadiseLib.Logger.Logger.AddEntry("'{0}' new chapters downloaded!", lNewChapters.Count);
//            }
//            else
//            {
//                ParadiseLib.Logger.Logger.AddEntry("No new chapters found!");
//            }

//            return lNewChapters;
//        }

//        public void SerializeChapters(string sFileName)
//        {
//            Helper.JSONHelper.Serialize(sFileName, this.Chapters);
//        }

//        public void DeserializeChapters(string sFileName)
//        {
//            this.Chapters = Helper.JSONHelper.DeserializeList<MangaChapter>(sFileName);
//            this.ChaptersLoaded = true;
//        }

//        public List<MangaChapter> LoadChaptersWithPath(string sPath)
//        {
//            ParadiseLib.Logger.Logger.AddEntry("Loading Chapters with folders from path '{0}'", sPath);

//            List<MangaChapter> lMangaChapters = new List<MangaChapter>();

//            foreach (System.IO.DirectoryInfo diChapter in new System.IO.DirectoryInfo(sPath).GetDirectories())
//            {
//                lMangaChapters.Add(new MangaChapter(this.Name, Convert.ToInt32(diChapter.Name.Replace("Chapter ", String.Empty))));
//            }

//            if (this.Chapters == null)
//                this.Chapters = new List<MangaChapter>();

//            this.Chapters.AddRange(lMangaChapters);

//            ParadiseLib.Logger.Logger.AddEntry("'{0}' Chapters loaded", lMangaChapters.Count);

//            return lMangaChapters;
//        }
//    }
//}
