using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mangasurvlib.Manga
{
    public interface IMangaManager : IManager
    {
        /// <summary>
        /// Add a Manga to list.
        /// </summary>
        /// <param name="Manga"></param>
        void AddManga(Manga Manga);

        /// <summary>
        /// Loads present Mangas.
        /// </summary>
        void LoadMangas();

        /// <summary>
        /// Load new versions of Mangas.
        /// </summary>
        void LoadNewVersion();

        /// <summary>
        /// Create new Manga.
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="SavePath"></param>
        /// <param name="Page"></param>
        /// <returns></returns>
        Manga CreateNewManga(string Name, string SavePath, MangaConstants.MangaPage Page);

        /// <summary>
        /// Get new chapters which were searched.
        /// </summary>
        /// <returns></returns>
        List<MangaChapter> GetNewChapters();

        /// <summary>
        /// Get loaded Mangas.
        /// </summary>
        /// <returns></returns>
        List<Manga> GetMangas();

        /// <summary>
        /// Writes given Chapters to DB.
        /// </summary>
        /// <param name="lChapters"></param>
        void WriteMangaChapterToDb(List<MangaChapter> lChapters);

        /// <summary>
        /// Searches chapters in DB with state "Downloadable" and downloads it.
        /// </summary>
        void DownloadDownloadableChapters();

        /// <summary>
        /// Searches for new chapters and writes them with state "Downloadable" to DB.
        /// </summary>
        void SearchNewChapters();

        /// <summary>
        /// Checks if image of manga exists and downloads it if not
        /// </summary>
        void LoadMangaImages();
    }
}
