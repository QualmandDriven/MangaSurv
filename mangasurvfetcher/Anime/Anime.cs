using System;
using System.Collections.Generic;
using System.IO;
using mangasurvlib.Helper;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace mangasurvlib.Anime
{
    /// <summary>
    /// Manga name from MangaPanda
    /// </summary>
    public class Anime
    {
        private bool _episodesLoaded;
        private static ILogger logger = Logging.ApplicationLogging.CreateLogger<Anime>();
        private delegate void DelegateDownloadEpisode(AnimeEpisode Episode);
        private delegate void DelegateLoadEpisode(AnimeEpisode Episode);


        public Anime(string Name, string SavePath)
        {
            Page = AnimeConstants.AnimePage.AnimefansFtw;
            this.Name = Name;
            this.SavePath = SavePath;
        }

        public string Name { get; set; }

        public string SavePath { get; set; }

        public int ID { get; set; }

        public AnimeConstants.AnimePage Page { get; set; }

        public List<AnimeEpisode> Episodes { get; set; }

        /// <summary>
        /// Loads and then Downloads every Episode of Anime.
        /// </summary>
        public void DownloadAnime()
        {
            logger.LogInformation("Loading URL's and Downloading every Episode of Anime '{0}'", Name);

            if (!_episodesLoaded)
                LoadEpisodes();

            DownloadEpisode(Episodes);
        }

        /// <summary>
        /// Downloads Episodes asynchronous.
        /// </summary>
        /// <param name="AnimeEpisode"></param>
        public void DownloadEpisode(List<AnimeEpisode> AnimeEpisode)
        {
            List<Task> tasks = new List<Task>();

            foreach (AnimeEpisode episode in AnimeEpisode)
            {
                tasks.Add(Task.Factory.StartNew(() => DownloadEpisode(episode)));
            }

            Task.WaitAll(tasks.ToArray());
        }

        /// <summary>
        /// Downloads the Episode.
        /// </summary>
        /// <param name="Episode"></param>
        public void DownloadEpisode(AnimeEpisode Episode)
        {
            logger.LogInformation("Downloading Episode " + Episode.Episode);
            Episode.Download();
        }

        /// <summary>
        /// Gets all Episodes and loads the URL's of them.
        /// </summary>
        public void LoadEpisodes()
        {
            Episodes = GetAllEpisodes();
            LoadEpisodes(Episodes);

            _episodesLoaded = true;
        }

        /// <summary>
        /// Loads Episodes asynchronous.
        /// </summary>
        /// <param name="AnimeEpisodes"></param>
        private void LoadEpisodes(IEnumerable<AnimeEpisode> AnimeEpisodes)
        {
            if (AnimeEpisodes == null) throw new ArgumentNullException("AnimeEpisodes");

            List<Task> tasks = new List<Task>();

            foreach (AnimeEpisode episode in AnimeEpisodes)
            {
                tasks.Add(Task.Factory.StartNew(() => LoadEpisode(episode)));
            }

            Task.WaitAll(tasks.ToArray());
        }

        /// <summary>
        /// Loads URL's of Episode.
        /// </summary>
        /// <param name="Episode"></param>
        public void LoadEpisode(AnimeEpisode Episode)
        {
            logger.LogInformation("Loading Episode " + Episode.Episode);
            Episode.LoadUrls();
        }

        /// <summary>
        /// Gets a list of all Episodes from Anime (gets it from WebSite).
        /// </summary>
        /// <returns></returns>
        private List<AnimeEpisode> GetAllEpisodes()
        {
            return AnimeHelper.GetAnimeClass(Page).GetEpisodes(this, GetAnimeUrl());
        }

        /// <summary>
        /// Gets URL of Anime to find Episodes.
        /// </summary>
        /// <returns></returns>
        private Uri GetAnimeUrl()
        {
            return AnimeHelper.GetAnimeClass(Page).GetAnime(Name);
        }

        public override string ToString()
        {
            return String.Format("Anime '{0}'", Name);
        }

        /// <summary>
        /// Finds, loads and downloads new Episodes.
        /// </summary>
        /// <returns>List of new Episodes found.</returns>
        public List<AnimeEpisode> DownloadNewEpisodes()
        {
            List<AnimeEpisode> lNewEpisodes = this.SearchNewEpisodes();

            return this.DownloadEpisodes(lNewEpisodes);
        }

        public List<AnimeEpisode> DownloadEpisodes(List<AnimeEpisode> lEpisodes)
        {
            if (lEpisodes.Count > 0)
            {
                Episodes.AddRange(lEpisodes);
                DownloadEpisode(lEpisodes);
                logger.LogInformation("'{0}' new Episodes downloaded!", lEpisodes.Count);
            }

            return lEpisodes;
        }

        public List<AnimeEpisode> SearchNewEpisodes()
        {
            logger.LogInformation("Checking for new Episodes for Anime '{0}'", Name);

            List<AnimeEpisode> lNewEpisodes = new List<AnimeEpisode>();
            List<AnimeEpisode> lTempEpisodes = this.GetAllEpisodes();
            foreach (AnimeEpisode episode in lTempEpisodes)
            {
                if (!Episodes.Contains(episode) && !lNewEpisodes.Contains(episode))
                {
                    logger.LogInformation("New Episode '{0}' found!", episode.Episode);
                    lNewEpisodes.Add(episode);
                }
            }

            if(lNewEpisodes.Count == 0)
                logger.LogInformation("No new Episodes found!");

            return lNewEpisodes;
        }

        public void SerializeEpisodes(string FileName)
        {
            JsonHelper.Serialize(FileName, Episodes);
        }

        public void DeserializeEpisodes(string FileName)
        {
            Episodes = JsonHelper.DeserializeList<AnimeEpisode>(FileName);
            _episodesLoaded = true;
        }

        public List<AnimeEpisode> LoadEpisodesWithPath(string Path)
        {
            logger.LogInformation("Loading Episodes with folders from path '{0}'", Path);

            List<AnimeEpisode> lAnimeEpisodes = new List<AnimeEpisode>();

            foreach (DirectoryInfo diEpisode in new DirectoryInfo(Path).GetDirectories())
            {
                lAnimeEpisodes.Add(new AnimeEpisode(Name, Convert.ToInt32(diEpisode.Name.Replace("Episode ", String.Empty))));
            }

            if (Episodes == null)
                Episodes = new List<AnimeEpisode>();

            Episodes.AddRange(lAnimeEpisodes);

            logger.LogInformation("'{0}' Episodes loaded", lAnimeEpisodes.Count);

            return lAnimeEpisodes;
        }

        public void LoadAnimePicture()
        {
            if (this.ID == 0)
                return;

            // If Image does not exists we download it
            if (Helper.WebHelper.UriExists(new Uri("http://www.mangasurv.com/img/anime/" + this.ID + ".jpg")) == false)
            {
                IAnimeLoader animeLoader = AnimeHelper.GetAnimeClass(this.Page);
                Uri imageUri = animeLoader.GetAnimePictureUrl(animeLoader.GetAnime(this.Name));

                if (imageUri != null)
                {
                    logger.LogInformation("Uploading image for anime '{0}' '{1}' from '{2}'", this.ID, this.Name, imageUri.AbsoluteUri);
                    // TODO: Bild laden
                    //service.UploadAnimeImageUrl(this.ID + System.IO.Path.GetExtension(imageUri.AbsoluteUri), imageUri.AbsoluteUri);
                }

            }
        }
    }
}
