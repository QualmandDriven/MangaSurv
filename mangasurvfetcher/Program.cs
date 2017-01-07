using Microsoft.Extensions.Logging;
using mangasurvlib.Logging;
using System;

namespace mangasurvfetcher
{
    public class Program
    {
        private static ILogger logger = mangasurvlib.Logging.ApplicationLogging.CreateLogger<Program>();

        public static void Main(string[] args)
        {
            mangasurvlib.Logging.ApplicationLogging.ConfigureLogger();
            logger.StartLogging(System.Reflection.Assembly.GetEntryAssembly());

            mangasurvlib.Manga.IMangaManager mangaManager = mangasurvlib.Manga.MangaFactory.CreateMangaManager();
            mangaManager.LoadMangas();
            //mangaManager.LoadMangaImages();
            mangaManager.SearchNewChapters();

            mangasurvlib.Anime.IAnimeManager animeManager = mangasurvlib.Anime.AnimeFactory.CreateAnimeManager();
            animeManager.LoadAnimes();
            //animeManager.LoadAnimeImages();
            animeManager.SearchNewEpisodes();
            
            logger.EndLogging();
        }
    }
}
