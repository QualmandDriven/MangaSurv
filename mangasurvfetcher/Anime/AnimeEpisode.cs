using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace mangasurvlib.Anime
{
    public class AnimeEpisode : IComparable
    {
        private static ILogger logger = Logging.ApplicationLogging.CreateLogger<AnimeEpisode>();
        private bool _filesLoaded = false;

        internal AnimeEpisode()
        { }

        internal AnimeEpisode(Anime Anime, Uri Url)
        {
            this.AnimeName = Anime.Name;
            this.SavePath = Anime.SavePath;
            this.Url = Url;

            this.GetEpisode();

            //this.LoadUrls();
        }

        internal AnimeEpisode(Anime Anime, Uri Url, double episode)
        {
            this.AnimeName = Anime.Name;
            this.SavePath = Anime.SavePath;
            this.Url = Url;

            this.Episode = episode;
        }

        internal AnimeEpisode(string Anime, double Episode)
        {
            this.AnimeName = Anime;
            this.Episode = Episode;
        }

        public double Episode { get; set; }

        public Uri Url { get; set; }

        public string AnimeName { get; set; }

        public string SavePath { get; set; }

        public List<KeyValuePair<int, Uri>> Files { get; set; }

        private void GetEpisode()
        {
            // Get Episode
            List<string> lSplittedUrl = this.SplitUrl();
            const string sEpisode = "episode-";

            if (lSplittedUrl[lSplittedUrl.Count - 2].ToUpper().Contains(sEpisode.ToUpper()))
            {
                // Url looks like http://www.animefansftw.com/naruto-shippuden-episode-348/
                string sLink = lSplittedUrl[lSplittedUrl.Count - 2];
                lSplittedUrl = sLink.Split('-').ToList();
                this.Episode = double.Parse(lSplittedUrl[lSplittedUrl.Count - 1]);
                //this.Episode = double.Parse(lSplittedUrl[lSplittedUrl.Count - 1].Replace("Episode-", "").Replace(".html", ""));

                return;
            }

            throw new Exception("Episode of Url could not be loaded " + this.Url.AbsolutePath);    
        }

        public void LoadUrls()
        {
            if (this.Url == null)
                return;

            this.Files = new List<KeyValuePair<int, Uri>>();

            this.Files = AnimeHelper.GetAnimeClass().GetFiles(this.AnimeName, this.Episode, this.Url);
            if (this.Files.Count == 0)
                logger.LogWarning("No available torrent for '{0}'", this.Url);

            this._filesLoaded = true;
        }

        public void Download()
        {
            if (!_filesLoaded)
                this.LoadUrls();

            DirectoryInfo info = Directory.CreateDirectory(Path.Combine(this.SavePath, Helper.StringHelper.ReplaceSpecialCharacters(this.AnimeName)));
            
            foreach (KeyValuePair<int, Uri> pair in this.Files)
            {
                try
                {
                    string sPath = AnimeConstants._DOWNLOADFOLDER + "\\" + this.AnimeName.Replace(":", "_") + "_" + this.Episode + ".torrent";
                    logger.LogInformation("Downloading torrent '{0}' to '{1}'", pair.Value, sPath);
                    Helper.WebHelper.DownloadFile(pair.Value, sPath);
                    //client.DownloadFile(pair.Value, String.Concat(info.FullName, "\\", Helper.StringHelper.ReplaceSpecialCharacters(this.AnimeName), "_", this.Episode, "_", pair.Key, Path.GetExtension((pair.Value.AbsoluteUri))));
                    logger.LogInformation("Starting process for '{0}'", sPath);
                    Process.Start(sPath);
                }
                catch(Exception ex)
                {
                    logger.LogError("ERROR while loading Episode '{0}' Page '{1}'\r\n{2}", this.Episode, pair.Value, ex.Message);
                }
            }
        }

        private List<string> SplitUrl()
        {
            return this.Url.AbsoluteUri.Replace("-final", "").Split('/').ToList();
        }

        public override string ToString()
        {
            return String.Format("Anime '{0}' Episode '{1}'", this.AnimeName, this.Episode);
        }

        public override bool Equals(object Obj)
        {
            if (!(Obj is AnimeEpisode))
                return false;

            AnimeEpisode temp = Obj as AnimeEpisode;

            if (temp.Episode == this.Episode && temp.AnimeName == this.AnimeName)
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public int CompareTo(object obj)
        {
            AnimeEpisode temp = obj as AnimeEpisode;


            if (this.Episode < temp.Episode)
                return -1;
            else
                return 1;
        }

        public static List<AnimeEpisode> DeserializeJsonAnimeEpisode(string sFilePath)
        {
            return Helper.JsonHelper.Deserialize(sFilePath, typeof(List<AnimeEpisode>)) as List<AnimeEpisode>;
        }

        public static void SerializeJsonAnimeEpisode(string sFilePath, List<AnimeEpisode> anime)
        {
            Helper.JsonHelper.Serialize(sFilePath, anime);
        }
    }
}
