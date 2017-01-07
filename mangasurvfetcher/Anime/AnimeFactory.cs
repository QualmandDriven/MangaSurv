using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mangasurvlib.Anime
{
    public class AnimeFactory
    {
        private static ILogger logger = Logging.ApplicationLogging.CreateLogger<AnimeFactory>();

        public static Anime CreateAnime(string Name, string SavePath, AnimeConstants.AnimePage AnimePage)
        {
            Anime anime = new Anime(Name, SavePath) {Page = AnimePage};

            return anime;
        }

        internal static List<Anime> ReadAnimesFromDb()
        {
            logger.LogInformation("Get animes from Database");

            Rest.RestController ctr = Rest.RestController.GetRestController();
            string sAnimes = ctr.Get("animes").Item2;
            List<dynamic> restAnimes = Helper.JsonHelper.DeserializeString<List<dynamic>>(sAnimes);

            logger.LogInformation("Found '{0}' animes", restAnimes.Count);

            List<Anime> lAnimes = new List<Anime>();
            foreach (dynamic dbAnime in restAnimes)
            {
                try
                {
                    Anime anime = CreateAnime((string)dbAnime.name, AnimeConstants._ANIMEPATH, AnimeConstants.AnimePage.AnimefansFtw);

                    logger.LogInformation("Loading anime '{0}'", anime.Name);

                    anime.ID = dbAnime.id;
                    anime.Episodes = new List<AnimeEpisode>();

                    string sEpisodes = ctr.Get(String.Format("animes/{0}/episodes", anime.ID)).Item2;
                    List<dynamic> restEpisodes = Helper.JsonHelper.DeserializeString<List<dynamic>>(sEpisodes);

                    if (restEpisodes != null)
                    {
                        foreach (dynamic episode in restEpisodes)
                        {
                            AnimeEpisode newEpisode = CreateAnimeEpisode(anime.Name, (double)episode.episodeNo);
                            anime.Episodes.Add(newEpisode);
                        }
                    }

                    lAnimes.Add(anime);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message);
                }

            }

            return lAnimes;
        }

        public static IAnimeManager CreateAnimeManager()
        {
            return new AnimeManager();
        }

        public static AnimeEpisode CreateAnimeEpisode()
        {
            return new AnimeEpisode();
        }

        public static AnimeEpisode CreateAnimeEpisode(Anime Anime, Uri Uri)
        {
            return new AnimeEpisode(Anime, Uri);
        }

        public static AnimeEpisode CreateAnimeEpisode(string AnimeName, double Episode)
        {
            return new AnimeEpisode(AnimeName, Episode);
        }

        internal static IAnimeLoader CreateLoaderAnimeFansFtw()
        {
            return new AnimeAnimefansFtw();
        }
    }
}
