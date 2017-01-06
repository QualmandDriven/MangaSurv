using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mangasurvlib.Manga
{
    internal interface IMangaLoader : Helper.ICache
    {
        Uri GetManga(string sName);
        List<MangaChapter> GetChapters(Manga manga, Uri uMangaUrl);
        List<KeyValuePair<int, Uri>> GetFiles(Uri uChapterUrl);
        Uri GetMangaPictureUrl(Uri mangaUrl);
    }
}
