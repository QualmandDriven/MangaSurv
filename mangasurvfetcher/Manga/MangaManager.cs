using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static mangasurvlib.Manga.MangaConstants;
using mangasurvlib.Helper;

namespace mangasurvlib.Manga
{
    internal class MangaManager : IMangaManager
    {
        private static ILogger logger = Logging.ApplicationLogging.CreateLogger<MangaManager>();
        private static Rest.RestController ctr = Rest.RestController.GetRestController();

        public List<Manga> Mangas
        {
            get;
            private set;
        }

        public List<MangaChapter> NewChapters
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates new instance of MangaManager.
        /// </summary>
        internal MangaManager()
        {
            this.Mangas = new List<Manga>();
            this.NewChapters = new List<MangaChapter>();
        }

        /// <summary>
        /// Loading files from JSON file or if not found try to load from folders.
        /// </summary>
        public void LoadMangas()
        {
            try
            {
                this.InitializeMangaLoaders();

                this.Mangas = MangaFactory.ReadMangasFromDb();

                foreach (Manga manga in this.Mangas)
                {
                    try
                    {
                        manga.InitChapters();
                    }
                    catch (Exception ex)
                    {
                        logger.LogError("While initializing chapters. " + ex.Message);
                    }
                }

                return;
            }
            catch (Exception ex)
            {
                logger.LogError("While loading manga chapters. " + ex.Message);
            }
        }

        /// <summary>
        /// Checks every Manga found.
        /// If newer version available, download it.
        /// </summary>
        public void LoadNewVersion()
        {
            //Parallel.ForEach(this.Mangas, (manga) =>
            //{
            //    try
            //    {
            //        logger.LogInformation("");
            //        this.LoadNewVersion(manga);
            //    }
            //    catch (Exception ex)
            //    {
            //        logger.LogInformation("Moving to next Manga" + Environment.NewLine + ex.Message);
            //    }
            //});
            foreach(Manga manga in this.Mangas)
            {
                try
                {
                    logger.LogInformation("");
                    this.LoadNewVersion(manga);
                }
                catch (Exception ex)
                {
                    logger.LogInformation("Moving to next Manga" + Environment.NewLine + ex.Message);
                }
            };
        }

        /// <summary>
        /// Checks specific Manga.
        /// If newer version available, download it.
        /// </summary>
        /// <param name="sMangaName"></param>
        public void LoadNewVersion(Manga manga)
        {
            this.NewChapters.AddRange(manga.DownloadNewChapters());
            this.WriteMangaChapterToDb(this.NewChapters);
        }

        /// <summary>
        /// Create new manga.
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="SavePath"></param>
        /// <param name="Page"></param>
        public Manga CreateNewManga(string Name, string SavePath, MangaConstants.MangaPage Page)
        {
            Manga manga = MangaFactory.CreateManga(Name, SavePath, Page);
            manga.DownloadManga();
            this.AddManga(manga);

            this.WriteMangaChapterToDb(manga.Chapters);

            return manga;
        }

        /// <summary>
        /// Only one Manga per Call.
        /// Creates manga as well, if not existing.
        /// </summary>
        /// <param name="lChapters"></param>
        public void WriteMangaChapterToDb(List<MangaChapter> lChapters)
        {
            this.WriteMangaChapterToDb(lChapters, State.Complete);
        }

        /// <summary>
        /// Only one Manga per Call.
        /// Creates manga as well, if not existing.
        /// </summary>
        /// <param name="lChapters"></param>
        public void WriteMangaChapterToDb(List<MangaChapter> lChapters, State chapterState)
        {
            if (lChapters.Count == 0)
                return;

            string sMangas = ctr.Get("mangas").Item2;
            List<dynamic> lMangas = Helper.JsonHelper.DeserializeString<List<dynamic>>(sMangas);

            string sMangaPages = ctr.Get("pages", new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("contenttype", "Manga") }).Item2;
            List<dynamic>mangaPages = Helper.JsonHelper.DeserializeString< List<dynamic>>(sMangaPages);

            string sFileSystemName = Helper.StringHelper.ReplaceSpecialCharacters(lChapters[0].MangaName);
            foreach (MangaChapter chapter in lChapters)
            {
                try
                {
                    chapter.GetFiles();
                }
                catch (Exception)
                { }

                //decimal dChapterNo = Convert.ToDecimal(chapter.Chapter);
                int iPageID = 0;

                dynamic page = mangaPages.FirstOrDefault(dbpage => ((string)dbpage.name).ToUpper() == chapter.MangaPage.ToString().ToUpper());
                if (page != null)
                     iPageID = page.id;

                int iStateID = Convert.ToInt32(chapterState);
                string sAddress = String.Empty;
                if (chapter.Url != null)
                    sAddress = chapter.Url.AbsoluteUri;

                dynamic manga = lMangas.FirstOrDefault(m => m.name == chapter.MangaName);
                dynamic chapterRest = new
                {
                    chapterno = chapter.Chapter,
                    mangaid = manga.id,
                    pageid = iPageID,
                    stateid = iStateID,
                    address = sAddress,
                    enterdate = DateTime.Now.ToStringUtc()
                };

                string sChapter = ctr.Post("mangas/" + manga.id + "/chapters", chapterRest).Item2;
                chapterRest = Helper.JsonHelper.DeserializeString<dynamic>(sChapter);

                    
                foreach(MangaFile file in chapter.MangaFiles)
                {
                    sAddress = String.Empty;

                    if (chapter.Files != null)
                    {
                        foreach (KeyValuePair<int, Uri> pair in chapter.Files)
                        {
                            if (pair.Key == file.FileNumber)
                            {
                                sAddress = pair.Value.AbsoluteUri;
                                break;
                            }
                        }
                    }

                    dynamic restFile = new
                    {
                        name = System.IO.Path.GetFileName(file.FileName),
                        fileno = file.FileNumber,
                        address = sAddress
                    };

                    ctr.Post(String.Format("mangas/{0}/chapters/{1}/files", manga.id, chapterRest.id), restFile);
                }
            }
        }

        /// <summary>
        /// Returns new chapters which were loaded with method "Search".
        /// </summary>
        /// <returns></returns>
        public List<MangaChapter> GetNewChapters()
        {
            return this.NewChapters;
        }

        /// <summary>
        /// Returns list of loaded Mangas.
        /// </summary>
        /// <returns></returns>
        public List<Manga> GetMangas()
        {
            return this.Mangas;
        }

        /// <summary>
        /// Adds Manga to list.
        /// </summary>
        /// <param name="Manga"></param>
        public void AddManga(Manga Manga)
        {
            this.Mangas.Add(Manga);
        }

        /// <summary>
        /// Searches in database for downloadable chapters.
        /// Then downloads it.
        /// </summary>
        public void DownloadDownloadableChapters()
        {
            logger.LogInformation("Get downloadable chapters...");

            string sChapters = ctr.Get("chapters?stateid=" + State.Downloadable).Item2;
            List<dynamic> newChapters = Helper.JsonHelper.DeserializeString<List<dynamic>>(sChapters);

            logger.LogInformation("'{0}' downloadable chapters found!", newChapters.Count);

#if RELEASE
            List<Task> tasks = new List<Task>();
            foreach (dynamic newChapter in newChapters)
            {
                tasks.Add(Task.Factory.StartNew(() => DownloadDownloadableChapter(newChapter)));
            }

            Task.WaitAll(tasks.ToArray());
#else
            foreach (dynamic newChapter in newChapters)
            {
                DownloadDownloadableChapter(newChapter);
            }
#endif
        }

        /// <summary>
        /// Searches in database for downloadable chapters.
        /// Then downloads it.
        /// </summary>
        private void DownloadDownloadableChapter(dynamic newChapter)
        {
            if (String.IsNullOrEmpty(newChapter.Address))
            {
                logger.LogError("Downloadable manga chapter '{0}' {1}' has no valid URL '{2}'!", (string)newChapter.MangaID, (string)newChapter.ChapterNo, (string)newChapter.Address);
                return;
            }

            string sManga = ctr.Get("mangas/" + newChapter.mangaid).Item2;
            dynamic dbManga = Helper.JsonHelper.DeserializeString<dynamic>(sManga);

            string sPages = ctr.Get("pages/" + newChapter.PageID, new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("contenttype", "Manga") }).Item2;
            dynamic dbPage = Helper.JsonHelper.DeserializeString<dynamic>(sPages);

            MangaConstants.MangaPage mangaPage = (MangaConstants.MangaPage)Enum.Parse(typeof(MangaConstants.MangaPage), dbPage.Name);

            logger.LogInformation("Downloading chapter '{0} of '{1}'...", (string)newChapter.ChapterNo, (string)dbManga.Name);

            Manga manga = MangaFactory.CreateManga(dbManga.Name, mangaPage);
            MangaChapter mangaChapter = MangaFactory.CreateMangaChapter(manga, Convert.ToDouble(newChapter.ChapterNo));
            mangaChapter.MangaPage = mangaPage;
            mangaChapter.Url = new Uri(newChapter.Address);

            // In Datenbank auf "Downloading" setzen
            ctr.Put("chapters/" + newChapter.id, new { stateid = State.Downloading });

            // Und jetzt die Bilder runterladen
            mangaChapter.Download();

            if (mangaChapter.Files == null || mangaChapter.Files.Count == 0)
            {
                // Wenn keine Bilder gefunden wurden, dann lassen wir es auf Downloadable, vllt. hat die Seite grade einen Fehler
                ctr.Put("chapters/" + newChapter.id, new { stateid = State.Downloadable });
                return;
            }

            ctr.Put("chapters/" + newChapter.id, new { stateid = State.Complete });
            this.NewChapters.Add(mangaChapter);
            
            // Jetzt müssen wir die Infos noch in die Datenbank speichern
            if (this.NewChapters.Count > 0)
            {
                this.WriteMangaChapterToDb(new List<MangaChapter>() { mangaChapter });
                this.SetMangaChaptersForUser(newChapter.id);
            }
        }

        /// <summary>
        /// Set the new chapters for users.
        /// So they get informed, that a new chapter is available.
        /// </summary>
        /// <param name="newChapter"></param>
        public void SetMangaChaptersForUser(dynamic newChapter)
        {
            // Get all users which follow the specific manga
            string sUsers = ctr.Get("mangas/" + newChapter.mangaid + "/users").Item2;
            List<dynamic> lUsers = Helper.JsonHelper.DeserializeString<List<dynamic>>(sUsers);

            // Set new chapters for user
            lUsers.ForEach(user =>
            {
                ctr.Post("users/" + user.id + "/chapters", new { id = newChapter.id });
            });
        }

        /// <summary>
        /// Searches for new chapters.
        /// If new chapters found, writes it to DB with state "Downloadable"
        /// </summary>
        public void SearchNewChapters()
        {
#if RELEASE
            List<Task> tasks = new List<Task>();

            foreach (Manga manga in this.Mangas)
            {
                tasks.Add(Task.Factory.StartNew(() => SearchNewMangaChapters(manga)));
            }

            Task.WaitAll(tasks.ToArray());
#else
            foreach (Manga manga in this.Mangas)
            {
                SearchNewMangaChapters(manga);
            }
#endif
        }

        private void SearchNewMangaChapters(Manga manga)
        {
            List<MangaChapter> lNewMangaChapters = manga.SearchNewChapters();
            this.NewChapters.AddRange(lNewMangaChapters);
            logger.LogInformation("");
            this.WriteMangaChapterToDb(lNewMangaChapters, State.Downloadable);
        }


        public void LoadMangaImages()
        {
#if RELEASE
            List<Task> tasks = new List<Task>();
            foreach (Manga manga in this.Mangas)
            {
                tasks.Add(Task.Factory.StartNew(() => manga.LoadMangaImage()));
            }

            Task.WaitAll(tasks.ToArray());
#else
            foreach (Manga manga in this.Mangas)
            {
                manga.LoadMangaImage();
            }
#endif
        }

        public void InitializeMangaLoaders()
        {
            foreach (MangaConstants.MangaPage mangaPage in (MangaConstants.MangaPage[])Enum.GetValues(typeof(MangaConstants.MangaPage)))
            {
                MangaHelper.GetMangaClass(mangaPage);
            }
        }
    }
}
