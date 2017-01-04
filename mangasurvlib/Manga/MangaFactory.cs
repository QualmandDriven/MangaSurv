using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Microsoft.Extensions.Logging;
using System.Dynamic;

namespace mangasurvlib.Manga
{
    public class MangaFactory
    {
        private static ILogger logger = Logging.ApplicationLogging.CreateLogger<MangaFactory>();

        public static Manga CreateManga(string Name, MangaConstants.MangaPage MangaPage)
        {
            return CreateManga(Name, MangaConstants._MANGAPATH, MangaPage);
        }

        internal static Manga CreateManga(string Name, string SavePath, MangaConstants.MangaPage MangaPage)
        {
            Manga manga = new Manga(Name, SavePath);
            manga.Page = MangaPage;
            
            return manga;
        }

        internal static List<Manga> ReadMangasFromDb()
        {
            Rest.RestController ctr = Rest.RestController.GetRestController();
            string sMangas = ctr.Get("mangas").Item2;
            List<dynamic> restMangas = Helper.JsonHelper.DeserializeString<List<dynamic>>(sMangas);

            logger.LogInformation("Found '{0}' mangas", restMangas.Count);

            List<Manga> lMangas = new List<Manga>();
            foreach (dynamic dbManga in restMangas)
            {
                string sName = dbManga.name;
                logger.LogInformation("Loading manga '{0}'", sName);
                Manga manga = CreateManga(sName, MangaConstants._MANGAPATH, MangaConstants.MangaPage.MangaBB);
                manga.ID = dbManga.ID;

                string sChapters = ctr.Get("mangas/" + manga.ID + "/chapters").Item2;
                List<dynamic> restChapters = Helper.JsonHelper.DeserializeString< List<dynamic>>(sChapters);

                if (dbManga.Chapters != null)
                {
                    foreach (dynamic chapter in restChapters)
                    {
                        MangaChapter newChapter = CreateMangaChapter(manga, Convert.ToDouble(chapter.ChapterNo));
                        newChapter.MangaFiles = new List<MangaFile>();

                        // Add files
                        string sFiles = ctr.Get(String.Format("mangas/{0}/chapters/{1}/files", manga.ID, chapter.id)).Item2;
                        List<dynamic> restFiles = Helper.JsonHelper.DeserializeString< List<dynamic>>(sChapters);

                        if (restFiles != null)
                            foreach(dynamic file in restFiles)
                                newChapter.MangaFiles.Add(new MangaFile() { FileName = file.name, FileNumber = file.fileno });

                        manga.Chapters.Add(newChapter);
                    }
                }

                lMangas.Add(manga);
            }

            return lMangas;
        }

        public static IMangaManager CreateMangaManager()
        {
            return new MangaManager();
        }

        public static MangaChapter CreateMangaChapter()
        {
            return new MangaChapter();
        }

        public static MangaChapter CreateMangaChapter(Manga Manga, Uri uUri)
        {
            return new MangaChapter(Manga, uUri);
        }

        public static MangaChapter CreateMangaChapter(Manga manga, double Chapter)
        {
            return new MangaChapter(manga, Chapter);
        }

        public static MangaChapter CreateMangaChapter(Manga manga, Uri uri, MangaConstants.MangaPage mangaPage)
        {
            return new MangaChapter(manga, uri, mangaPage);
        }

        internal static IMangaLoader CreateLoaderMangaBB()
        {
            return new MangaBB();
        }

        internal static IMangaLoader CreateLoaderMangaPanda()
        {
            return new MangaPanda();
        }

        internal static IMangaLoader CreateLoaderMangaReader()
        {
            return new MangaReader();
        }

        internal static IMangaLoader CreateLoaderBatoto()
        {
            return new Batoto();
        }
    }
}
