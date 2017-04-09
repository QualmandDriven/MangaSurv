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
    public class UsersAnimesController : Controller
    {
        private readonly MangaSurvContext _context;
        public UsersAnimesController(MangaSurvContext context)
        {
            this._context = context;
        }

        // GET api/animes
        [Authorize(Roles = WebApiAccess.USER_ROLE)]
        [HttpGet("{userid}/animes/")]
        public IActionResult Get(int userid)
        {
            UserTokenDetails userDetails = new UserTokenDetails(User);
            User user = Model.User.GetUser(userid, userDetails);

            if (user == null)
                return this.Forbid();

            List<Anime> animeslist = (from anime in _context.UserFollowAnimes
                                      where anime.UserId == user.Id
                                      orderby anime.Anime.Name
                                      select anime.Anime).ToList();

            return this.Ok(animeslist);
        }

        // GET api/animes/5
        [Authorize(Roles = WebApiAccess.USER_ROLE)]
        [HttpGet("{userid}/animes/{animeid}", Name ="UserAnimeLink")]
        [Produces(typeof(Anime))]
        public IActionResult Get(int userid, int animeid)
        {
            UserTokenDetails userDetails = new UserTokenDetails(User);
            User user = Model.User.GetUser(userid, userDetails);

            if (user == null)
                return this.Forbid();

            List<Anime> animeslist = (from anime in _context.UserFollowAnimes
                                      where anime.UserId == user.Id
                                      && anime.AnimeId == animeid
                                      select anime.Anime).ToList();

            return this.Ok(animeslist.FirstOrDefault());
        }

        // POST api/animes
        [Authorize(Roles = WebApiAccess.USER_ROLE)]
        [HttpPost("{userid}/animes/")]
        public IActionResult Post(int userid, [FromBody]Anime value)
        {
            try
            {
                UserTokenDetails userDetails = new UserTokenDetails(User);
                User user = Model.User.GetUser(userid, userDetails);

                if (user == null)
                    return this.Forbid();

                var anime = this._context.Animes.FirstOrDefault(m => m.Id == value.Id);
                if (anime == null)
                    return this.NotFound();

                var entry = UserFollowAnimes.AddAnimeToUser(anime, user).Result;

                return this.CreatedAtRoute("UserAnimeLink", new { userid = entry.UserId, animeid = entry.AnimeId }, entry);
            }
            catch(Exception ex)
            {
                return this.BadRequest(ex);
            }
        }

        // PUT api/animes/5
        //[Authorize(Roles = WebApiAccess.USER_ROLE)]
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody]Anime value)
        //{
        //}

        // DELETE api/animes/5
        [Authorize(Roles = WebApiAccess.USER_ROLE)]
        [HttpDelete("{userid}/animes/{animeid}")]
        public void Delete(int userid, int animeid)
        {
            UserTokenDetails userDetails = new UserTokenDetails(User);
            User user = Model.User.GetUser(userid, userDetails);

            if (user == null)
                return;

            var useranimes = this._context.UserFollowAnimes.Where(u => u.UserId == user.Id && u.AnimeId == animeid);
            if (useranimes == null || useranimes.Count() == 0)
                return;

            this._context.UserFollowAnimes.RemoveRange(useranimes);
            this._context.SaveChanges();
        }
    }
}
