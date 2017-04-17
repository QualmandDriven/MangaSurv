using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MangaSurvWebApi.Model;
using Microsoft.AspNetCore.Authorization;
using MangaSurvWebApi.Service;

namespace MangaSurvWebApi.Controllers
{
    [Route("api/users")]
    public class UsersEpisodesController : Controller
    {
        private readonly MangaSurvContext _context;
        public UsersEpisodesController(MangaSurvContext context)
        {
            this._context = context;
        }

        // GET api/episodes
        [Authorize(Roles = WebApiAccess.USER_ROLE)]
        [HttpGet("{userid}/episodes/")]
        public IActionResult Get(int userid)
        {
            UserTokenDetails userDetails = new UserTokenDetails(User);
            User user = Model.User.GetUser(userid, userDetails);

            if (user == null)
                return this.Forbid();

            if (Request.QueryString.HasValue)
            {
                Helper.QueryString queryString = new Helper.QueryString(Request);
                if (queryString.ContainsKey("SORTBY") && queryString.GetValue("SORTBY").ToUpper().Equals("ANIME"))
                {
                    List<Episode> lNewEpisodes = (from epidose in _context.UserNewEpisodes
                                                  where epidose.UserId == user.Id
                                                  orderby epidose.Episode
                                                  select epidose.Episode).ToList();

                    List<Anime> lAnimes = new List<Anime>();
                    List<long> lAnimeIds = new List<long>();

                    foreach (Episode episode in lNewEpisodes)
                    {
                        if (lAnimeIds.Contains(episode.AnimeId))
                        {
                            Anime anime = lAnimes.Find(a => a.Id == episode.AnimeId);
                            if (!anime.Episodes.Contains(episode))
                                anime.Episodes.Add(episode);

                            anime.Episodes.Sort();
                        }
                        else
                        {
                            Anime anime = this._context.Animes.FirstOrDefault(m => m.Id == episode.AnimeId);
                            if (anime != null)
                            {
                                lAnimes.Add(anime);
                                lAnimeIds.Add(anime.Id);
                            }
                        }
                    }

                    return this.Ok(lAnimes.OrderBy(anime => anime.Name));
                }
            }

            List<Episode> episodeslist = (from episode in _context.UserNewEpisodes
                                          where episode.UserId == user.Id
                                          orderby episode.Episode
                                          select episode.Episode).ToList();

            return this.Ok(episodeslist);
        }

        // GET api/episodes/5
        [Authorize(Roles = WebApiAccess.USER_ROLE)]
        [HttpGet("{userid}/episodes/{episodeid}", Name ="UserEpisodeLink")]
        [Produces(typeof(Episode))]
        public IActionResult Get(int userid, int episodeid)
        {
            UserTokenDetails userDetails = new UserTokenDetails(User);
            User user = Model.User.GetUser(userid, userDetails);

            if (user == null)
                return this.Forbid();

            List<Episode> episodeslist = (from episode in _context.UserNewEpisodes
                                          where episode.UserId == user.Id
                                          && episode.EpisodeId == episodeid
                                          select episode.Episode).ToList();

            return this.Ok(episodeslist.FirstOrDefault());
        }

        // POST api/episodes
        [Authorize(Roles = WebApiAccess.USER_ROLE)]
        [HttpPost("{userid}/episodes/")]
        public IActionResult Post(int userid, [FromBody]Episode value)
        {
            try
            {
                UserTokenDetails userDetails = new UserTokenDetails(User);
                User user = Model.User.GetUser(userid, userDetails);

                if (user == null)
                    return this.Forbid();

                var episode = this._context.Episodes.FirstOrDefault(m => m.Id == value.Id);
                if (episode == null)
                    return this.NotFound();

                var entry = UserNewEpisodes.AddEpisodeToUser(episode, user.Id).Result;

                return this.CreatedAtRoute("UserEpisodeLink", new { userid = entry.UserId, episodeid = entry.EpisodeId }, entry);
            }
            catch(Exception ex)
            {
                return this.BadRequest(ex);
            }
        }

        // PUT api/episodes/5
        //[Authorize(Roles = WebApiAccess.USER_ROLE)]
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody]Episode value)
        //{
        //}

        // DELETE api/episodes/5
        [Authorize(Roles = WebApiAccess.USER_ROLE)]
        [HttpDelete("{userid}/episodes/{episodeid}")]
        public void Delete(int userid, int episodeid)
        {
            UserTokenDetails userDetails = new UserTokenDetails(User);
            User user = Model.User.GetUser(userid, userDetails);

            if (user == null)
                return;

            var userepisode = this._context.UserNewEpisodes.FirstOrDefault(u => u.UserId == user.Id && u.EpisodeId == episodeid);
            if (userepisode == null)
                return;

            this._context.UserNewEpisodes.Remove(userepisode);
            this._context.SaveChanges();
        }
    }
}
