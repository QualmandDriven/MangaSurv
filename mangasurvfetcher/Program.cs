using Microsoft.Extensions.Logging;
using mangasurvlib.Logging;
using System;
using mangasurvlib.Rest;
using mangasurvfetcher.Helper;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace mangasurvfetcher
{
    public class Program
    {
        private static ILogger logger = mangasurvlib.Logging.ApplicationLogging.CreateLogger<Program>();

        public static void Main(string[] args)
        {
            //var builder = new ConfigurationBuilder()
            //                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            //var configuration = builder.Build();
            

            //builder.AddEnvironmentVariables();
            //Configuration = builder.Build();

            mangasurvlib.Logging.ApplicationLogging.ConfigureLogger();
            logger.StartLogging(System.Reflection.Assembly.GetEntryAssembly());
            
            ArgumentsManager argMng;
            try
            {
                argMng = new ArgumentsManager();
            }
            catch (Exception ex)
            {
                logger.LogCritical("Could not read arguments! {0} {1}", ex.Message, ex.Source);
                return;
            }

            logger.LogInformation(argMng.GetValue("api-username"));

            // Load Auth0 connection details provided by arguments either of command line arguments or environemnt variables
            API.Auth0Connector auth0Con = new API.Auth0Connector(argMng.GetValue("api-username"), argMng.GetValue("api-password"), argMng.GetValue("api-clientid"), argMng.GetValue("api-secret-key"));
            string sToken = auth0Con.GetIdToken();

            //MySqlTakeOver.TakeOver(sToken);
            //MySqlTakeOver.AddAnimesAndMangasToUser(sToken);

            mangasurvlib.Manga.IMangaManager mangaManager = mangasurvlib.Manga.MangaFactory.CreateMangaManager(sToken);
            mangaManager.LoadMangas();
            //mangaManager.LoadMangaImages();
            mangaManager.SearchNewChapters();

            mangasurvlib.Anime.IAnimeManager animeManager = mangasurvlib.Anime.AnimeFactory.CreateAnimeManager(sToken);
            animeManager.LoadAnimes();
            //animeManager.LoadAnimeImages();
            animeManager.SearchNewEpisodes();
            
            logger.EndLogging();
        }
    }
}
