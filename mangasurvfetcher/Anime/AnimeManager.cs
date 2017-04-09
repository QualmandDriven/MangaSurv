using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using mangasurvlib.Helper;

namespace mangasurvlib.Anime
{
    public class AnimeManager : IAnimeManager
    {
        private static ILogger logger = Logging.ApplicationLogging.CreateLogger<AnimeManager>();
        private static Rest.RestController ctr;

        private const string SaveFile = "Animelist.json";

        public List<Anime> Animes { get; private set; }
        public List<AnimeEpisode> NewEpisodes { get; private set; }

        public AnimeManager(string sApiToken) : this()
        {
            this.ConfigureApiController(sApiToken);
        }

        public AnimeManager()
        {
            this.NewEpisodes = new List<AnimeEpisode>();
        }

        public void AddAnime(Anime Anime)
        {
            this.Animes.Add(Anime);
        }

        public void LoadAnimes()
        {
            logger.LogInformation("Loading Mangas from service");
            this.InitializeAnimeLoaders();

            this.Animes = AnimeFactory.ReadAnimesFromDb();
        }

        public void LoadNewVersion()
        {
            this.NewEpisodes = new List<AnimeEpisode>();

            foreach (Anime anime in this.Animes)
            {
                try
                {
                    logger.LogInformation("");
                    this.LoadNewVersion(anime);
                }
                catch (Exception ex)
                {
                    logger.LogWarning("Moving to next Anime" + Environment.NewLine + ex.Message);
                }
            }
        }

        public void SetSavePath(string sSavePath)
        {
            foreach (Anime anime in this.Animes)
            {
                try
                {
                    logger.LogInformation("");
                    anime.SavePath = sSavePath;

                    foreach (AnimeEpisode episode in anime.Episodes)
                    {
                        episode.SavePath = sSavePath;
                    }
                }
                catch (Exception ex)
                {
                    logger.LogWarning("Moving to next Anime" + Environment.NewLine + ex.Message);
                }
            }
        }

        /// <summary>
        /// Checks specific Anime.
        /// If newer version available, download it.
        /// </summary>
        /// <param name="Anime"></param>
        public void LoadNewVersion(Anime Anime)
        {
            this.NewEpisodes.AddRange(Anime.DownloadNewEpisodes());
        }

        public void SaveAnimes()
        {
            Helper.JsonHelper.Serialize(Path.Combine(AnimeConstants._ANIMEPATH, SaveFile), this.Animes);
        }

        /// <summary>
        /// Creates a new anime.
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Page"></param>
        /// <returns></returns>
        public Anime CreateNewAnime(string Name, AnimeConstants.AnimePage Page)
        {
            foreach (Anime t in this.Animes)
            {
                if (t.Name == Name)
                    return null;
            }

            Anime anime = AnimeFactory.CreateAnime(Name, AnimeConstants._ANIMEPATH, Page);
            
            anime.DownloadAnime();
            this.AddAnime(anime);

            return anime;
        }

        /// <summary>
        /// Search new versions of animes.
        /// </summary>
        /// <returns></returns>
        public List<AnimeEpisode> Search()
        {
            this.LoadAnimes();
            this.LoadNewVersion();
            logger.LogInformation("");
            this.SaveAnimes();

            return this.NewEpisodes;
        }

        public List<AnimeEpisode> GetNewEpisodes()
        {
            return this.NewEpisodes;
        }

        public List<Anime> GetAnimes()
        {
            return this.Animes;
        }


        public void WriteEpisodesToDb(List<AnimeEpisode> lEpisodes)
        {
            this.WriteEpisodesToDb(lEpisodes, State.Complete);
        }

        private void WriteEpisodesToDb(List<AnimeEpisode> lEpisodes, State state)
        {
            if (lEpisodes == null || lEpisodes.Count == 0)
                return;

            string sAnimes = ctr.Get("animes").Item2;
            List<dynamic> lAnimes = Helper.JsonHelper.DeserializeString<List<dynamic>>(sAnimes);

            foreach (AnimeEpisode episode in lEpisodes)
            {
                dynamic anime = lAnimes.FirstOrDefault(m => m.name == episode.AnimeName);

                dynamic episodeRest = new
                {
                    episodeno = Convert.ToDecimal(episode.Episode),
                    animeid = anime.id,
                    stateid = Convert.ToInt32(state),
                    address = episode.Url.AbsoluteUri,
                    enterdate = DateTime.Now.ToStringUtc()
                };

                string sEpisode = ctr.Post("animes/" + anime.id + "/episodes", episodeRest).Item2;
            }
        }

        public void DownloadDownloadableEpisodes()
        {
            logger.LogInformation("Get downloadable episodes...");

            string sEpisodes = ctr.Get("episodes?stateid=" + State.Downloadable).Item2;
            List<dynamic> newEpisodes = Helper.JsonHelper.DeserializeString<List<dynamic>>(sEpisodes);

            logger.LogInformation("'{0}' downloadable episodes found", newEpisodes.Count);

            foreach (dynamic newEpisode in newEpisodes)
            {
                string sAnime = ctr.Get("animes/" + newEpisode.animeid).Item2;
                dynamic dbAnime = Helper.JsonHelper.DeserializeString<dynamic>(sAnime);

                AnimeConstants.AnimePage animePage = AnimeConstants.AnimePage.AnimefansFtw;
                Anime anime = AnimeFactory.CreateAnime(dbAnime.Name, AnimeConstants._ANIMEPATH, animePage);

                AnimeEpisode animeEpisode = AnimeFactory.CreateAnimeEpisode(dbAnime.Name, Convert.ToDouble(newEpisode.EpisodeNo));
                animeEpisode.Url = new Uri(newEpisode.Address);
                animeEpisode.SavePath = AnimeConstants._ANIMEPATH;

                // In Datenbank auf "Downloading" setzen
                ctr.Put("episodes/" + newEpisode.id, new { stateid = State.Downloading });

                // Und jetzt die Episode laden
                animeEpisode.Download();

                if (animeEpisode.Files == null || animeEpisode.Files.Count == 0)
                {
                    ctr.Put("episodes/" + newEpisode.id, new { stateid = State.Downloadable });
                }

                ctr.Put("episodes/" + newEpisode.id, new { stateid = State.Complete });
                
                this.NewEpisodes.Add(animeEpisode);

                if (this.NewEpisodes.Count > 0)
                {
                    this.WriteEpisodesToDb(new List<AnimeEpisode>() { animeEpisode });
                    this.SetAnimeEpisodesForUser(newEpisode);
                }
            }
        }

        /// <summary>
        /// Set the new chapters for users.
        /// So they get informed, that a new chapter is available.
        /// </summary>
        /// <param name="lNewChapters"></param>
        public void SetAnimeEpisodesForUser(dynamic newEpisode)
        {
            // Get all users which follow the specific anime
            string sUsers = ctr.Get("animes/" + newEpisode.animeid + "/users").Item2;
            List<dynamic> lUsers = Helper.JsonHelper.DeserializeString<List<dynamic>>(sUsers);

            // Set new episodes for user
            lUsers.ForEach(user =>
            {
                ctr.Post("users/" + user.id + "/episodes", new { id = newEpisode.id });
            });
        }

        public void SearchNewEpisodes()
        {
            List<Task> tasks = new List<Task>();
            foreach (Anime anime in this.Animes)
            {
                tasks.Add(Task.Factory.StartNew(() => SearchNewAnimeEpisodes(anime)));
            }

            Task.WaitAll(tasks.ToArray());
        }

        public void SearchNewAnimeEpisodes(Anime anime)
        {
            List<AnimeEpisode> lNewEpisodes = new List<AnimeEpisode>();
            lNewEpisodes.AddRange(anime.SearchNewEpisodes());
            logger.LogInformation("");
            this.WriteEpisodesToDb(lNewEpisodes, State.Downloadable);
        }


        public void LoadAnimeImages()
        {
            List<Task> tasks = new List<Task>();
            foreach (Anime anime in this.Animes)
            {
                tasks.Add(Task.Factory.StartNew(() => anime.LoadAnimePicture()));
            }

            Task.WaitAll(tasks.ToArray());
        }

        public void InitializeAnimeLoaders()
        {
            foreach (AnimeConstants.AnimePage animePage in (AnimeConstants.AnimePage[])Enum.GetValues(typeof(AnimeConstants.AnimePage)))
            {
                AnimeHelper.GetAnimeClass(animePage);
            }
        }

        public void ConfigureApiController(string sToken)
        {
             ctr = Rest.RestController.GetRestController(new List<KeyValuePair<System.Net.HttpRequestHeader, string>>() { new KeyValuePair<System.Net.HttpRequestHeader, string>(System.Net.HttpRequestHeader.Authorization, "Bearer " + sToken) });
        }
    }
}
