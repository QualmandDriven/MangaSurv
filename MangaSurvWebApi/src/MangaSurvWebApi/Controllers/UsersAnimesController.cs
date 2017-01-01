using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AnimeSurvWebApi.Controllers
{
    [Route("api/users")]
    public class UsersAnimesController : Controller
    {
        private readonly MangaSurvContext _context;
        public UsersAnimesController(MangaSurvContext context)
        {
            this._context = context;
        }

        // GET api/animes
        [HttpGet("{userid}/animes/")]
        public IActionResult Get(int userid)
        {
            var animeslist = (from anime in _context.UserFollowAnimes
                              where anime.UserId == userid
                              select anime.Anime).ToList();

            return this.Ok(animeslist);
        }
        
        // GET api/animes/5
        [HttpGet("{userid}/animes/{animeid}", Name ="UserAnimeLink")]
        [Produces(typeof(Anime))]
        public IActionResult Get(int userid, int animeid)
        {
            var animeslist = (from anime in _context.UserFollowAnimes
                              where anime.UserId == userid
                              && anime.AnimeId == animeid
                              select anime.Anime).ToList();

            return this.Ok(animeslist.FirstOrDefault());
        }

        // POST api/animes
        [HttpPost("{userid}/animes/")]
        public async Task<IActionResult> Post(int userid, [FromBody]Anime value)
        {
            try
            {
                var user = this._context.Users.FirstOrDefault(u => u.Id == userid);
                if (user == null)
                    return this.NotFound();

                var anime = this._context.Animes.FirstOrDefault(m => m.Id == value.Id);
                if (anime == null)
                    return this.NotFound();

                var entry = this._context.UserFollowAnimes.FirstOrDefault(u => u.UserId == userid && u.AnimeId == anime.Id);

                // Add entry only when it does not exist yet
                if (entry == null)
                {
                    UserFollowAnimes ufm = new UserFollowAnimes();
                    ufm.Anime = anime;
                    ufm.AnimeId = anime.Id;
                    ufm.User = user;
                    ufm.UserId = user.Id;

                    this._context.UserFollowAnimes.Add(ufm);
                    await this._context.SaveChangesAsync();

                    entry = ufm;
                }
                return this.CreatedAtRoute("UserAnimeLink", new { userid = entry.UserId, animeid = entry.AnimeId }, entry);
            }
            catch(Exception ex)
            {
                return this.BadRequest(ex);
            }
        }

        // PUT api/animes/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]Anime value)
        {
        }

        // DELETE api/animes/5
        [HttpDelete("{userid}/animes/{animeid}")]
        public void Delete(int userid, int animeid)
        {
            var useranime = this._context.UserFollowAnimes.FirstOrDefault(u => u.UserId == userid && u.AnimeId == animeid);
            if (useranime == null)
                return;

            this._context.UserFollowAnimes.Remove(useranime);
            this._context.SaveChanges();
        }
    }
}
