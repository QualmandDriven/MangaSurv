using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MangaSurvWebApi.Model;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using MangaSurvWebApi.Service;

namespace MangaSurvWebApi.Controllers
{
    [Route("api/users")]
    public class UsersMangasController : Controller
    {
        private readonly MangaSurvContext _context;
        public UsersMangasController(MangaSurvContext context)
        {
            this._context = context;
        }

        // GET api/mangas
        [Authorize(Roles = WebApiAccess.USER_ROLE)]
        [HttpGet("{userid}/mangas/")]
        public IActionResult Get(int userid)
        {
            UserTokenDetails userDetails = new UserTokenDetails(User);
            User user = Model.User.GetUser(userid, userDetails);

            if (user == null)
                return this.Forbid();

            List<Manga> mangaslist = (from manga in _context.UserFollowMangas
                                      where manga.UserId == user.Id
                                      orderby manga.Manga.Name
                                      select manga.Manga).ToList();

            List<dynamic> lMangas = new List<dynamic>();
            foreach(Manga manga in mangaslist)
            {
                dynamic m = new {
                    manga.Chapters,
                    manga.EnterDate,
                    manga.FileSystemName,
                    manga.Id,
                    manga.LastUpdate,
                    manga.Name,
                    followed = true
                };

                lMangas.Add(m);
            }

            return this.Ok(lMangas);
        }

        // GET api/mangas/5
        [Authorize(Roles = WebApiAccess.USER_ROLE)]
        [HttpGet("{userid}/mangas/{mangaid}", Name ="UserMangaLink")]
        [Produces(typeof(Manga))]
        public IActionResult Get(int userid, int mangaid)
        {
            UserTokenDetails userDetails = new UserTokenDetails(User);
            User user = Model.User.GetUser(userid, userDetails);

            if (user == null)
                return this.Forbid();

            List<Manga> mangaslist = (from manga in _context.UserFollowMangas
                                      where manga.UserId == user.Id
                                      && manga.MangaId == mangaid
                                      orderby manga.Manga.Name
                                      select manga.Manga).ToList();

            return this.Ok(mangaslist.FirstOrDefault());
        }

        // POST api/mangas
        [Authorize(Roles = WebApiAccess.USER_ROLE)]
        [HttpPost("{userid}/mangas/")]
        public IActionResult Post(int userid, [FromBody]Manga value)
        {
            try
            {
                UserTokenDetails userDetails = new UserTokenDetails(User);
                User user = Model.User.GetUser(userid, userDetails);

                if (user == null)
                    return this.Forbid();

                var manga = this._context.Mangas.FirstOrDefault(m => m.Id == value.Id);
                if (manga == null)
                    return this.NotFound();

                var entry = UserFollowMangas.AddMangaToUser(manga, user);

                return this.CreatedAtRoute("UserMangaLink", new { userid = entry.UserId, mangaid = entry.MangaId }, entry);
            }
            catch(Exception ex)
            {
                return this.BadRequest(ex);
            }
        }

        // PUT api/mangas/5
        //[Authorize(Roles = WebApiAccess.USER_ROLE)]
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody]Manga value)
        //{
        //}

        // DELETE api/mangas/5
        [Authorize]
        [HttpDelete("{userid}/mangas/{mangaid}")]
        public IActionResult Delete(int userid, int mangaid)
        {
            UserTokenDetails userDetails = new UserTokenDetails(User);
            User user = Model.User.GetUser(userid, userDetails);

            if (user == null)
                return this.NotFound();

            var usermanga = this._context.UserFollowMangas.FirstOrDefault(u => u.UserId == user.Id && u.MangaId == mangaid);
            if (usermanga == null)
                return this.NotFound();

            this._context.UserFollowMangas.Remove(usermanga);
            this._context.SaveChanges();

            return this.Ok();
        }

        [HttpOptions("{userid}/mangas")]
        public IActionResult Options(int userid)
        {
            return this.Ok();
        }
    }
}
