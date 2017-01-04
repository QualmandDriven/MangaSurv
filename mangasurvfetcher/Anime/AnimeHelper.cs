using System.Collections.Generic;
namespace mangasurvlib.Anime
{
    internal class AnimeHelper
    {
        private static AnimeConstants.AnimePage _page = AnimeConstants.AnimePage.AnimefansFtw;
        private static Dictionary<AnimeConstants.AnimePage, IAnimeLoader> InitializedAnimeLoaders = new Dictionary<AnimeConstants.AnimePage, IAnimeLoader>();

        public static IAnimeLoader GetAnimeClass()
        {
            if (InitializedAnimeLoaders.ContainsKey(_page))
                return InitializedAnimeLoaders[_page];

            IAnimeLoader loader = null;

            switch (_page)
            {
                case AnimeConstants.AnimePage.AnimefansFtw:
                    loader = new AnimeAnimefansFtw();
                    break;
            }

            if (loader != null)
                InitializedAnimeLoaders.Add(_page, loader);

            return loader;
        }

        public static IAnimeLoader GetAnimeClass(AnimeConstants.AnimePage AnimePage)
        {
            _page = AnimePage;

            return GetAnimeClass();
        }
    }
}
