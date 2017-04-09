using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mangasurvlib.Anime
{
    public interface IAnimeManager : IManager
    {
        /// <summary>
        /// Add a Manga to list.
        /// </summary>
        /// <param name="Anime"></param>
        void AddAnime(Anime Anime);

        /// <summary>
        /// Loads present Animes.
        /// </summary>
        void LoadAnimes();

        /// <summary>
        /// Load new versions of Animes.
        /// </summary>
        void LoadNewVersion();

        /// <summary>
        /// Save all Animes to file system.
        /// </summary>
        void SaveAnimes();

        /// <summary>
        /// Create new Anime.
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Page"></param>
        /// <returns></returns>
        Anime CreateNewAnime(string Name, AnimeConstants.AnimePage Page);
        
        /// <summary>
        /// Search new chapters for given Anime.
        /// </summary>
        /// <returns></returns>
        List<AnimeEpisode> Search();

        /// <summary>
        /// Get new episodes which were searched.
        /// </summary>
        /// <returns></returns>
        List<AnimeEpisode> GetNewEpisodes();

        /// <summary>
        /// Get loaded Animes.
        /// </summary>
        /// <returns></returns>
        List<Anime> GetAnimes();

        /// <summary>
        /// Writes given Chapters to DB.
        /// </summary>
        /// <param name="lChapters"></param>
        void WriteEpisodesToDb(List<AnimeEpisode> lEpisodes);

        /// <summary>
        /// Searches chapters in DB with state "Downloadable" and downloads it.
        /// </summary>
        void DownloadDownloadableEpisodes();

        /// <summary>
        /// Searches for new chapters and writes them with state "Downloadable" to DB.
        /// </summary>
        void SearchNewEpisodes();

        /// <summary>
        /// Checks if image of anime exists and downloads it if not
        /// </summary>
        void LoadAnimeImages();
    }
}
