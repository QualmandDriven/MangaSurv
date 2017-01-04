using System;

namespace mangasurvfetcher
{
    public class Program
    {
        public static void Main(string[] args)
        {
            mangasurvlib.Logging.ApplicationLogging.ConfigureLogger();

            mangasurvlib.Manga.IMangaManager mangaManager = mangasurvlib.Manga.MangaFactory.CreateMangaManager();
            mangaManager.LoadMangas();
            //mangaManager.LoadMangaImages();
            mangaManager.SearchNewChapters();

            mangasurvlib.Anime.IAnimeManager animeManager = mangasurvlib.Anime.AnimeFactory.CreateAnimeManager();
            animeManager.LoadAnimes();
            //animeManager.LoadAnimeImages();
            animeManager.SearchNewEpisodes();
        }
    }
}
