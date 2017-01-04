using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mangasurvlib.Anime
{
    public interface IAnimeLoader
    {
        Uri GetAnime(string Name);
        List<AnimeEpisode> GetEpisodes(Anime Anime, Uri Url);
        List<KeyValuePair<int, Uri>> GetFiles(Uri EpisodeUrl);
        Uri GetAnimePictureUrl(Uri animeUrl);
    }
}
