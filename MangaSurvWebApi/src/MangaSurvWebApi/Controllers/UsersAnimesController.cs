using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MangaSurvWebApi.Model;

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
        public IActionResult Post(int userid, [FromBody]Anime value)
        {
            try
            {
                var user = this._context.Users.FirstOrDefault(u => u.Id == userid);
                if (user == null)
                    return this.NotFound();

                var anime = this._context.Animes.FirstOrDefault(m => m.Id == value.Id);
                if (anime == null)
                    return this.NotFound();

                var entry = UserFollowAnimes.AddAnimeToUser(this._context, anime, user).Result;

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
            var useranimes = this._context.UserFollowAnimes.Where(u => u.UserId == userid && u.AnimeId == animeid);
            if (useranimes == null || useranimes.Count() == 0)
                return;

            this._context.UserFollowAnimes.RemoveRange(useranimes);
            this._context.SaveChanges();
        }
    }
}
