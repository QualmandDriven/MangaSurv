using mangasurvlib.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace mangasurvlib.Manga
{
    public class MangaChapter : IComparable
    {
        private static ILogger logger = Logging.ApplicationLogging.CreateLogger<MangaChapter>();
        private double _Chapter;
        private string _MangaName;
        private string _SavePath;
        private string _MangaSavePath;
        private Uri _Url;
        private List<KeyValuePair<int, Uri>> _Files;
        private bool FilesLoaded = false;

        public List<MangaFile> MangaFiles;

        /// <summary>
        /// Creates new instance of MangaChapter with no information.
        /// </summary>
        internal MangaChapter()
        { }

        /// <summary>
        /// Creats new instance of MangaChapter.
        /// </summary>
        /// <param name="manga"></param>
        /// <param name="url"></param>
        internal MangaChapter(Manga manga, Uri url)
        {
            this.Init(manga);
            this.Url = url;

            this.GetChapter();
            this.SetSavePath();

            //this.LoadUrls();
        }

        internal MangaChapter(Manga manga, Uri url, MangaConstants.MangaPage mangaPage)
        {
            this.Init(manga);
            this.Url = url;
            this.MangaPage = mangaPage;

            this.GetChapter();
            this.SetSavePath();

            //this.LoadUrls();
        }

        /// <summary>
        /// Creates new instance of MangaChapter.
        /// </summary>
        /// <param name="Manga"></param>
        /// <param name="Chapter"></param>
        internal MangaChapter(Manga manga, double Chapter)
        {
            this.Init(manga);
            this.Chapter = Chapter;
            
            this.SetSavePath();
        }

        public void Init(Manga manga)
        {
            this.MangaName = manga.Name;
            this.MangaSavePath = manga.SavePath;
        }

        private void SetSavePath()
        {
            
            this.SavePath = Path.Combine(this.MangaSavePath, Helper.StringHelper.ReplaceSpecialCharacters(this.MangaName), "Chapter " + this.Chapter.ToFolderString());
        }

        /// <summary>
        /// Chapter number of Manga.
        /// </summary>
        public double Chapter
        {
            get
            {
                return this._Chapter;
            }
            set
            {
                this._Chapter = value;
            }
        }

        /// <summary>
        /// URL to Chapter.
        /// </summary>
        public Uri Url
        {
            get
            {
                return this._Url;
            }
            set
            {
                this._Url = value;
            }
        }

        /// <summary>
        /// Manga to which referers this Chapter.
        /// </summary>
        public string MangaName
        {
            get
            {
                return this._MangaName;
            }
            set
            {
                this._MangaName = value;
            }
        }

        /// <summary>
        /// Save Path to which the Chapter will be downloaded.
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
        /// Save Path of Manga.
        /// </summary>
        public string MangaSavePath
        {
            get
            {
                return this._MangaSavePath;
            }
            set
            {
                this._MangaSavePath = value;
            }
        }

        /// <summary>
        /// List of found Files (Pictures) of Chapter.
        /// </summary>
        public List<KeyValuePair<int, Uri>> Files
        {
            get
            {
                return this._Files;
            }
            set
            {
                this._Files = value;
            }
        }

        public MangaConstants.MangaPage MangaPage { get; set; }

        /// <summary>
        /// Get Chapter number of URL.
        /// </summary>
        private void GetChapter()
        {
            try
            {
                // Get Chapter

                if (this.MangaPage == MangaConstants.MangaPage.Batoto)
                {
                    // Batoto Link http://bato.to/read/_/9379/fairy-tail_ch096_by_franky-house
                    string sLink = this.Url.AbsoluteUri;
                    int iStart = sLink.IndexOf("_ch") + 3;
                    int iEnd = sLink.IndexOf("_", iStart);

                    this.Chapter = double.Parse(sLink.Substring(iStart, iEnd - iStart).Replace(".", ","), new System.Globalization.CultureInfo("en-US"));
                }
                else
                {
                    List<string> lSplittedUrl = this.SplitUrl();
                    if (lSplittedUrl[lSplittedUrl.Count - 1].ToUpper().Contains("CHAPTER"))
                    {
                        // Url looks like http://www.mangapanda.com/94-36557-3/bleach/chapter-379.html
                        this.Chapter = double.Parse(lSplittedUrl[lSplittedUrl.Count - 1].Replace("chapter-", "").Replace(".html", "").Replace(".", ","), new System.Globalization.CultureInfo("en-US"));
                    }
                    else
                    {
                        // Url looks like http://www.mangapanda.com/bleach/560
                        this.Chapter = double.Parse(lSplittedUrl[lSplittedUrl.Count - 1].Replace(".", ","), new System.Globalization.CultureInfo("en-US"));
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Failed to get chapter no of '{0}' {1}", this.Url.AbsoluteUri, ex.Message);
            }
        }

        /// <summary>
        /// Loads Picture URLs.
        /// </summary>
        public void LoadUrls()
        {
            if (this.Url == null)
                return;

            this.Files = new List<KeyValuePair<int, Uri>>();

            IMangaLoader mangaLoader = MangaHelper.GetMangaClass(this.MangaPage);
            this.Files = mangaLoader.GetFiles(this.Url);

            this.FilesLoaded = true;
        }

        /// <summary>
        /// Download all pictures.
        /// If Load URLs was not called, it will be called here before.
        /// </summary>
        public void Download()
        {
            if (!FilesLoaded)
                this.LoadUrls();

            //DirectoryInfo info = Directory.CreateDirectory(Path.Combine(this.SavePath, Helper.StringHelper.ReplaceSpecialCharacters(this.MangaName), "Chapter " + this.Chapter.ToString()));
            this.SetSavePath();
            DirectoryInfo info = Directory.CreateDirectory(this.SavePath);

            foreach (KeyValuePair<int, Uri> pair in this.Files)
            {
                string sPath = String.Concat(info.FullName, "\\", Helper.StringHelper.ReplaceSpecialCharacters(this.MangaName), "_", this.Chapter, "_", pair.Key, Path.GetExtension((pair.Value.AbsoluteUri)));
                try
                {
                    logger.LogInformation("Downloading manga file '{0}' to '{1}'", pair.Value, sPath);
                    Helper.WebHelper.DownloadFile(pair.Value, sPath);
                }
                catch
                {
                    try
                    {
                        Helper.WebHelper.DownloadFile(pair.Value, sPath);
                    }
                    catch
                    {
                        logger.LogError("ERROR while loading Chapter '{0}' Page '{1}'", this.Chapter, pair.Value);
                    }
                }
            }
        }

        public void GetFiles()
        {
            this.SetSavePath();
            this.MangaFiles = new List<MangaFile>();
            string[] arrExtensions = new string[] {".jpg", ".bmp", ".png", ".gif"};

            FileInfo[] fiFiles = new DirectoryInfo(this.SavePath).GetFiles();
            foreach (FileInfo file in fiFiles)
            {
                if(arrExtensions.Contains(file.Extension))
                    this.MangaFiles.Add(new MangaFile(file.FullName));
            }
        }

        /// <summary>
        /// Split URL at "/".
        /// </summary>
        /// <returns></returns>
        private List<string> SplitUrl()
        {
            return this.Url.AbsoluteUri.Split('/').ToList();
        }

        public override string ToString()
        {
            return String.Format("Manga '{0}' Chapter '{1}'", this.MangaName, this.Chapter);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is MangaChapter))
                return false;

            MangaChapter temp = obj as MangaChapter;

            if (temp.Chapter == this.Chapter)
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public int CompareTo(object obj)
        {
            MangaChapter temp = obj as MangaChapter;


            if (this.Chapter < temp.Chapter)
                return -1;
            else
                return 1;
        }

        public static List<MangaChapter> DeserializeJsonManga(string sFilePath)
        {
            return Helper.JsonHelper.Deserialize(sFilePath, typeof(List<MangaChapter>)) as List<MangaChapter>;
        }

        public static void SerializeJsonManga(string sFilePath, List<MangaChapter> manga)
        {
            Helper.JsonHelper.Serialize(sFilePath, manga);
        }
    }
}
