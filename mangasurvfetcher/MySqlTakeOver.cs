using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace mangasurvfetcher
{
    public class MySqlTakeOver
    {
        public static void TakeOver(string sBearerToken)
        {
            TakeOverManga(sBearerToken);
            TakeOverAnime(sBearerToken);
        }

        public static void TakeOverManga(string sBearerToken)
        {
            MySql.Data.MySqlClient.MySqlConnectionStringBuilder builder = new MySql.Data.MySqlClient.MySqlConnectionStringBuilder();
            builder.Database = "manga";
            builder.UserID = "root";
            builder.Server = "localhost";
            string conStr = builder.GetConnectionString(false);

            using (MySql.Data.MySqlClient.MySqlConnection con = new MySql.Data.MySqlClient.MySqlConnection(conStr))
            {
                con.Open();

                mangasurvlib.Rest.RestController restCtr = mangasurvlib.Rest.RestController.GetRestController("http://h2688485.stratoserver.net:5000/api", new List<KeyValuePair<System.Net.HttpRequestHeader, string>>() { new KeyValuePair<System.Net.HttpRequestHeader, string>(System.Net.HttpRequestHeader.Authorization, "Bearer " + sBearerToken) });
                Tuple<HttpStatusCode, string> result = restCtr.Get("mangas");
                var res = (List<System.Dynamic.ExpandoObject>)mangasurvlib.Helper.JsonHelper.DeserializeString(result.Item2, typeof(List<System.Dynamic.ExpandoObject>));
                foreach (System.Dynamic.ExpandoObject objitem in res)
                {
                    string sId = objitem.FirstOrDefault(o => o.Key == "id").Value.ToString();
                    restCtr.Delete("mangas/" + sId);
                }

                var mangaReader = new MySql.Data.MySqlClient.MySqlCommand("SELECT * from mangas", con).ExecuteReader();
                while (mangaReader.Read())
                {
                    List<dynamic> lChapters = new List<dynamic>();
                    using (var chaptercon = new MySql.Data.MySqlClient.MySqlConnection(conStr))
                    {
                        chaptercon.Open();
                        var chapterReader = new MySql.Data.MySqlClient.MySqlCommand("SELECT * FROM chapters WHERE mangaid=" + mangaReader.GetInt32("id"), chaptercon).ExecuteReader();
                        while (chapterReader.Read())
                        {
                            string sUrl = String.Format("https://www.google.de/#q={0} {1}", mangaReader.GetString("name"), chapterReader.GetInt32("chapterno"));
                            if (!String.IsNullOrEmpty(chapterReader.GetString("address")))
                                sUrl = chapterReader.GetString("address");

                            List<dynamic> lFiles = new List<dynamic>();
                            using (var filecon = new MySql.Data.MySqlClient.MySqlConnection(conStr))
                            {
                                filecon.Open();
                                var fileReader = new MySql.Data.MySqlClient.MySqlCommand("SELECT * FROM files WHERE chapterid=" + chapterReader.GetInt32("id"), filecon).ExecuteReader();
                                while (fileReader.Read())
                                {
                                    lFiles.Add(new { FileNo = fileReader.GetInt32("fileno"), Name = fileReader.GetString("name"), Address = fileReader.GetString("address") });
                                }

                                filecon.Close();
                            }

                            lChapters.Add(new { ChapterNo = chapterReader.GetInt32("chapterno"), Address = sUrl, PageId = chapterReader.GetInt32("Pageid"), EnterDate = chapterReader.GetDateTime("enterdate"), Files = lFiles });
                        }

                        chaptercon.Close();
                    }
                    var manga = new { Name = mangaReader.GetString("name"), FileSystemName = mangaReader.GetString("FileSystemName"), Chapters = lChapters };

                    string mangapost = restCtr.Post("mangas", manga).Item2;
                }

                con.Close();
            }
        }

        public static void TakeOverAnime(string sBearerToken)
        {
            MySql.Data.MySqlClient.MySqlConnectionStringBuilder builder = new MySql.Data.MySqlClient.MySqlConnectionStringBuilder();
            builder.Database = "anime";
            builder.UserID = "root";
            builder.Server = "localhost";
            string conStr = builder.GetConnectionString(false);

            using (MySql.Data.MySqlClient.MySqlConnection con = new MySql.Data.MySqlClient.MySqlConnection(conStr))
            {
                con.Open();

                mangasurvlib.Rest.RestController restCtr = mangasurvlib.Rest.RestController.GetRestController("http://h2688485.stratoserver.net:5000/api", new List<KeyValuePair<System.Net.HttpRequestHeader, string>>() { new KeyValuePair<System.Net.HttpRequestHeader, string>(System.Net.HttpRequestHeader.Authorization, "Bearer " + sBearerToken) });
                Tuple<HttpStatusCode, string> result = restCtr.Get("animes");
                var res = (List<System.Dynamic.ExpandoObject>)mangasurvlib.Helper.JsonHelper.DeserializeString(result.Item2, typeof(List<System.Dynamic.ExpandoObject>));
                foreach (System.Dynamic.ExpandoObject objitem in res)
                {
                    string sId = objitem.FirstOrDefault(o => o.Key == "id").Value.ToString();
                    restCtr.Delete("animes/" + sId);
                }

                var animeReader = new MySql.Data.MySqlClient.MySqlCommand("SELECT * from animes", con).ExecuteReader();
                while (animeReader.Read())
                {
                    List<dynamic> lEpisodes = new List<dynamic>();
                    using (var episodecon = new MySql.Data.MySqlClient.MySqlConnection(conStr))
                    {
                        episodecon.Open();
                        var episodeReader = new MySql.Data.MySqlClient.MySqlCommand("SELECT * FROM episodes WHERE animeid=" + animeReader.GetInt32("id"), episodecon).ExecuteReader();
                        while (episodeReader.Read())
                        {
                            string sUrl = String.Format("https://www.google.de/#q={0} {1}", animeReader.GetString("name"), episodeReader.GetInt32("episodeno"));
                            if (!String.IsNullOrEmpty(episodeReader.GetString("address")))
                                sUrl = episodeReader.GetString("address");

                            try
                            { 
                                lEpisodes.Add(new { EpisodeNo = episodeReader.GetInt32("episodeno"), Address = sUrl, PageId = episodeReader.GetInt32("Pageid"), EnterDate = episodeReader.GetDateTime("enterdate") });
                            }
                            catch
                            {
                                lEpisodes.Add(new { EpisodeNo = episodeReader.GetInt32("episodeno"), Address = sUrl, PageId = episodeReader.GetInt32("Pageid"), EnterDate = DateTime.MinValue });
                            }
                        }

                        episodecon.Close();
                    }
                    var anime = new { Name = animeReader.GetString("name"), FileSystemName = animeReader.GetString("name"), Episodes = lEpisodes };

                    string animepost = restCtr.Post("animes", anime).Item2;
                }

                con.Close();
            }
        }

        public static void AddAnimesAndMangasToUser(string sBearerToken)
        {
            mangasurvlib.Rest.RestController restCtr = mangasurvlib.Rest.RestController.GetRestController("http://h2688485.stratoserver.net:5000/api", new List<KeyValuePair<System.Net.HttpRequestHeader, string>>() { new KeyValuePair<System.Net.HttpRequestHeader, string>(System.Net.HttpRequestHeader.Authorization, "Bearer " + sBearerToken) });
            Tuple<HttpStatusCode, string> result = restCtr.Get("mangas");
            var res = (List<System.Dynamic.ExpandoObject>)mangasurvlib.Helper.JsonHelper.DeserializeString(result.Item2, typeof(List<System.Dynamic.ExpandoObject>));
            foreach (System.Dynamic.ExpandoObject objitem in res)
            {
                string sId = objitem.FirstOrDefault(o => o.Key == "id").Value.ToString();

                restCtr.Post(String.Format("users/{0}/mangas", 1), new { id = int.Parse(sId) });
            }

            result = restCtr.Get("animes");
            res = (List<System.Dynamic.ExpandoObject>)mangasurvlib.Helper.JsonHelper.DeserializeString(result.Item2, typeof(List<System.Dynamic.ExpandoObject>));
            foreach (System.Dynamic.ExpandoObject objitem in res)
            {
                string sId = objitem.FirstOrDefault(o => o.Key == "id").Value.ToString();

                restCtr.Post(String.Format("users/{0}/animes", 1), new { id = int.Parse(sId) });
            }
        }
    }
}
