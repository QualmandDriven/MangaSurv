using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using HtmlAgilityPack;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace mangasurvlib.Manga
{
    /// <summary>
    /// Class which contains functionality to load Mangas from Internet Pages.
    /// </summary>
    public class Manga
    {
        /// <summary>
        /// Collection of Internet MangaPages.
        /// </summary>
        private static ILogger logger = Logging.ApplicationLogging.CreateLogger<Manga>();
        private string _Name;
        private string _SavePath;
        private List<MangaChapter> _Chapters;
        private bool ChaptersLoaded = false;
        private MangaConstants.MangaPage _Page = MangaConstants.MangaPage.MangaPanda;
        public int ID { get; set; }

        private delegate void DelegateDownloadChapter(MangaChapter chapter);
        private delegate void DelegateLoadChapter(MangaChapter chapter);

        /// <summary>
        /// Creates new instance of Manga.
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="SavePath"></param>
        public Manga(string Name, string SavePath)
        {
            this.Name = Name;
            this.SavePath = SavePath;
            this.Chapters = new List<MangaChapter>();
        }

        /// <summary>
        /// Name of Manga.
        /// </summary>
        public string Name
        {
            get
            {
                return this._Name;
            }
            set
            {
                this._Name = value;
            }
        }

        /// <summary>
        /// Save Path for Chapters and Serialization.
        /// </summary>
        public string SavePath
        {
            get
            {
                return this._SavePath;
            }
            set
            {
                this._SavePath = value;
            }
        }

        /// <summary>
        /// this.SavePath + FileSystemMangaName
        /// </summary>
        public string MangaSavePath
        {
            get { return System.IO.Path.Combine(this.SavePath, Helper.StringHelper.ReplaceSpecialCharacters(this.Name)); }
            set { }
        }

        /// <summary>
        /// Internet Page form which Manga was/will be loaded.
        /// </summary>
        public MangaConstants.MangaPage Page
        {
            get
            {
                return this._Page;
            }
            set
            {
                this._Page = value;
            }
        }

        /// <summary>
        /// List of all Chapters.
        /// </summary>
        public List<MangaChapter> Chapters
        {
            get
            {
                return this._Chapters;
            }
            set
            {
                this._Chapters = value;
            }
        }

        public void InitChapters()
        {
            foreach (MangaChapter chapter in this.Chapters)
            {
                chapter.Init(this);
            }
        }

        /// <summary>
        /// Loads and then Downloads every Chapter of Manga.
        /// </summary>
        public void DownloadManga()
        {
            logger.LogInformation("Loading URL's and Downloading every chapter of Manga '{0}'", this.Name);

            if (!this.ChaptersLoaded)
                this.LoadChapters();

            this.DownloadChapter(this.Chapters);
        }

        /// <summary>
        /// Downloads chapters asynchronous.
        /// </summary>
        /// <param name="Chapter"></param>
        public void DownloadChapter(List<MangaChapter> lMangaChapter)
        {
            List<Task> tasks = new List<Task>();
            foreach (MangaChapter chapter in lMangaChapter)
            {
                tasks.Add(Task.Factory.StartNew(() => DownloadChapter(chapter)));
            }

            Task.WaitAll(tasks.ToArray());
        }

        /// <summary>
        /// Downloads the chapter.
        /// </summary>
        /// <param name="Chapter"></param>
        public void DownloadChapter(MangaChapter Chapter)
        {
            logger.LogInformation("Downloading Chapter " + Chapter.Chapter);
            Chapter.Download();
        }

        /// <summary>
        /// Gets all chapters and loads the URL's of them.
        /// </summary>
        public void LoadChapters()
        {
            this.Chapters = this.GetAllChapters();
            this.LoadChapters(this.Chapters);

            this.ChaptersLoaded = true;
        }

        /// <summary>
        /// Loads chapters asynchronous.
        /// </summary>
        /// <param name="lMangaChapters"></param>
        private void LoadChapters(List<MangaChapter> lMangaChapters)
        {
            List<Task> tasks = new List<Task>();

            foreach (MangaChapter chapter in lMangaChapters)
            {
                tasks.Add(Task.Factory.StartNew(() => LoadChapter(chapter)));
            }

            Task.WaitAll(tasks.ToArray());
        }

        /// <summary>
        /// Loads URL's of chapter.
        /// </summary>
        /// <param name="chapter"></param>
        public void LoadChapter(MangaChapter chapter)
        {
            logger.LogInformation("Loading Chapter " + chapter.Chapter);
            chapter.LoadUrls();
        }

        /// <summary>
        /// Gets a list of all Chapters from Manga (gets it from WebSite).
        /// </summary>
        /// <returns></returns>
        private List<MangaChapter> GetAllChapters()
        {
            logger.LogInformation("Getting all Chapters for Manga '{0}'", this.Name);

            List<MangaChapter> lChapters = new List<MangaChapter>();

            try
            {
                List<Task<List<MangaChapter>>> tasks = new List<Task<List<MangaChapter>>>();
                foreach (MangaConstants.MangaPage mangaPage in (MangaConstants.MangaPage[])Enum.GetValues(typeof(MangaConstants.MangaPage)))
                {
                    IMangaLoader mangaLoader = MangaHelper.GetMangaClass(mangaPage);
                    Uri uriManga = mangaLoader.GetManga(this.Name);
                    Task<List<MangaChapter>> t = Task.Factory.StartNew<List<MangaChapter>>(() => mangaLoader.GetChapters(this, uriManga));
                    tasks.Add(t);
                    //List<MangaChapter> lAllChapters = mangaLoader.GetChapters(this, uriManga);
                    //if (lAllChapters != null)
                    //    lChapters.AddRange(lAllChapters);
                }

                Task.WaitAll(tasks.ToArray());

                foreach (Task<List<MangaChapter>> t in tasks)
                {
                    if (t.Result != null)
                        lChapters.AddRange(t.Result);
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Getting all chapters for manga '{0}'\r\n'{1}'", this.Name, ex.Message);
            }

            return lChapters;
        }

        public override string ToString()
        {
            return String.Format("Manga '{0}'", this.Name);
        }

        /// <summary>
        /// Finds, loads and downloads new chapters.
        /// </summary>
        /// <returns>List of new chapters found.</returns>
        public List<MangaChapter> DownloadNewChapters()
        {
            List<MangaChapter> lNewChapters = this.SearchNewChapters();
            return this.DownloadChapters(lNewChapters);
        }

        /// <summary>
        /// Downloads chapters.
        /// </summary>
        /// <returns>List of new chapters found.</returns>
        public List<MangaChapter> DownloadChapters(List<MangaChapter> lChapters)
        {
            if (lChapters.Count > 0)
            {
                this.Chapters.AddRange(lChapters);
                this.DownloadChapter(lChapters);
                logger.LogInformation("'{0}' new chapters downloaded!", lChapters.Count);
            }

            return lChapters;
        }

        /// <summary>
        /// Search for new manga chapters on website.
        /// Return new chapters found.
        /// </summary>
        /// <returns></returns>
        public List<MangaChapter> SearchNewChapters()
        {
            logger.LogInformation("Checking for new chapters for Manga '{0}'", this.Name);

            List<MangaChapter> lNewChapters = new List<MangaChapter>();

            List<MangaChapter> lTemp = this.GetAllChapters();
            foreach (MangaChapter chapter in lTemp)
            {
                if (!this.Chapters.Contains(chapter) && !lNewChapters.Contains(chapter))
                {
                    logger.LogInformation("New Chapter '{0}' found!", chapter.Chapter);
                    lNewChapters.Add(chapter);
                }
            }

            if(lNewChapters.Count == 0)
                logger.LogInformation("No new chapters found!");

            return lNewChapters;
        }

        public void SerializeChapters(string sFileName)
        {
            Helper.JsonHelper.Serialize(sFileName, this.Chapters);
        }

        public void DeserializeChapters(string sFileName)
        {
            this.Chapters = Helper.JsonHelper.DeserializeList<MangaChapter>(sFileName);
            this.ChaptersLoaded = true;
        }

        public List<MangaChapter> LoadChaptersWithPath(string sPath)
        {
            logger.LogInformation("Loading Chapters with folders from path '{0}'", sPath);

            List<MangaChapter> lMangaChapters = new List<MangaChapter>();

            foreach (System.IO.DirectoryInfo diChapter in new System.IO.DirectoryInfo(sPath).GetDirectories())
            {
                lMangaChapters.Add(MangaFactory.CreateMangaChapter(this, Convert.ToInt32(diChapter.Name.Replace("Chapter ", String.Empty))));
            }

            if (this.Chapters == null)
                this.Chapters = new List<MangaChapter>();

            this.Chapters.AddRange(lMangaChapters);

            logger.LogInformation("'{0}' Chapters loaded", lMangaChapters.Count);

            return lMangaChapters;
        }

        /// <summary>
        /// Check if image for manga exists
        /// If not -> downloads it
        /// </summary>
        public void LoadMangaImage()
        {
            if (this.ID == 0)
                return;

            // If Image does not exists we download it
            if (Helper.WebHelper.UriExists(new Uri("http://www.mangasurv.com/img/manga/" + this.ID + ".jpg")) == false)
            {
                //MangaSurv.ImageService.ImageServicePortTypeClient service = new MangaSurv.ImageService.ImageServicePortTypeClient();

                foreach (MangaConstants.MangaPage mangaPage in (MangaConstants.MangaPage[])Enum.GetValues(typeof(MangaConstants.MangaPage)))
                {
                    IMangaLoader mangaLoader = MangaHelper.GetMangaClass(mangaPage);
                    Uri imageUri = mangaLoader.GetMangaPictureUrl(mangaLoader.GetManga(this._Name));

                    if (imageUri != null)
                    {
                        if (System.IO.Path.GetExtension(imageUri.AbsoluteUri) == ".jpg")
                        {
                            logger.LogInformation("Uploading image for manga '{0}' '{1}' from '{2}'", this.ID, this.Name, imageUri.AbsoluteUri);
                            //TODO: Bild laden
                            //service.UploadMangaImageUrl(this.ID + System.IO.Path.GetExtension(imageUri.AbsoluteUri), imageUri.AbsoluteUri);
                            break;
                        }
                    }
                }
            }
        }

        public static Manga DeserializeJsonManga(string sFilePath)
        {
            return Helper.JsonHelper.Deserialize(sFilePath, typeof(Manga)) as Manga;
        }

        public static void SerializeJsonManga(string sFilePath, Manga manga)
        {
            Helper.JsonHelper.Serialize(sFilePath, manga);
        }
    }

    /// <summary>
    /// Constants for Mangas.
    /// </summary>
    public class MangaConstants
    {
        /// <summary>
        /// Collection of Internet Manga Pages.
        /// </summary>
        public enum MangaPage
        {
            Batoto, MangaPanda, MangaBB, MangaReader
        }

        /// <summary>
        /// @"\\NAS\Multimedia\MANGA"
        /// </summary>
        public const string _MANGAPATH = @"\\NAS\Multimedia\MANGA";
    }
}
