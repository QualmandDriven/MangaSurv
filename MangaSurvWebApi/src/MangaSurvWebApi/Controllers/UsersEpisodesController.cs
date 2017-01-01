using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EpisodeSurvWebApi.Controllers
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
        [HttpGet("{userid}/episodes/")]
        public IActionResult Get(int userid)
        {
            var episodeslist = (from episode in _context.UserNewEpisodes
                                where episode.UserId == userid
                                select episode.Episode).ToList();

            return this.Ok(episodeslist);
        }
        
        // GET api/episodes/5
        [HttpGet("{userid}/episodes/{episodeid}", Name ="UserEpisodeLink")]
        [Produces(typeof(Episode))]
        public IActionResult Get(int userid, int episodeid)
        {
            var episodeslist = (from episode in _context.UserNewEpisodes
                                where episode.UserId == userid
                                && episode.EpisodeId == episodeid
                                select episode.Episode).ToList();

            return this.Ok(episodeslist.FirstOrDefault());
        }

        // POST api/episodes
        [HttpPost("{userid}/episodes/")]
        public async Task<IActionResult> Post(int userid, [FromBody]Episode value)
        {
            try
            {
                var user = this._context.Users.FirstOrDefault(u => u.Id == userid);
                if (user == null)
                    return this.NotFound();

                var episode = this._context.Episodes.FirstOrDefault(m => m.Id == value.Id);
                if (episode == null)
                    return this.NotFound();

                var entry = this._context.UserNewEpisodes.FirstOrDefault(u => u.UserId == userid && u.EpisodeId == episode.Id);

                // Add entry only when it does not exist yet
                if (entry == null)
                {
                    UserNewEpisodes ufm = new UserNewEpisodes();
                    ufm.Episode = episode;
                    ufm.EpisodeId = episode.Id;
                    ufm.User = user;
                    ufm.UserId = user.Id;

                    this._context.UserNewEpisodes.Add(ufm);
                    await this._context.SaveChangesAsync();

                    entry = ufm;
                }

                return this.CreatedAtRoute("UserEpisodeLink", new { userid = entry.UserId, episodeid = entry.EpisodeId }, entry);
            }
            catch(Exception ex)
            {
                return this.BadRequest(ex);
            }
        }

        // PUT api/episodes/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]Episode value)
        {
        }

        // DELETE api/episodes/5
        [HttpDelete("{userid}/episodes/{episodeid}")]
        public void Delete(int userid, int episodeid)
        {
            var userepisode = this._context.UserNewEpisodes.FirstOrDefault(u => u.UserId == userid && u.EpisodeId == episodeid);
            if (userepisode == null)
                return;

            this._context.UserNewEpisodes.Remove(userepisode);
            this._context.SaveChanges();
        }
    }
}
