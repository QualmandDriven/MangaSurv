using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MangaSurvWebApi.Model;

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
        [HttpGet("{userid}/mangas/")]
        public IActionResult Get(int userid)
        {
            var mangaslist = (from manga in _context.UserFollowMangas
                              where manga.UserId == userid
                              select manga.Manga).ToList();

            return this.Ok(mangaslist);
        }
        
        // GET api/mangas/5
        [HttpGet("{userid}/mangas/{mangaid}", Name ="UserMangaLink")]
        [Produces(typeof(Manga))]
        public IActionResult Get(int userid, int mangaid)
        {
            var mangaslist = (from manga in _context.UserFollowMangas
                              where manga.UserId == userid
                              && manga.MangaId == mangaid
                              select manga.Manga).ToList();

            return this.Ok(mangaslist.FirstOrDefault());
        }

        // POST api/mangas
        [HttpPost("{userid}/mangas/")]
        public async Task<IActionResult> Post(int userid, [FromBody]Manga value)
        {
            try
            {
                var user = this._context.Users.FirstOrDefault(u => u.Id == userid);
                if (user == null)
                    return this.NotFound();

                var manga = this._context.Mangas.FirstOrDefault(m => m.Id == value.Id);
                if (manga == null)
                    return this.NotFound();

                var entry = this._context.UserFollowMangas.FirstOrDefault(u => u.UserId == userid && u.MangaId == manga.Id);

                // Add entry only when it does not exist yet
                if (entry == null)
                {
                    UserFollowMangas ufm = new UserFollowMangas();
                    ufm.Manga = manga;
                    ufm.MangaId = manga.Id;
                    ufm.User = user;
                    ufm.UserId = user.Id;

                    this._context.UserFollowMangas.Add(ufm);
                    await this._context.SaveChangesAsync();

                    entry = ufm;
                }
                return this.CreatedAtRoute("UserMangaLink", new { userid = entry.UserId, mangaid = entry.MangaId }, entry);
            }
            catch(Exception ex)
            {
                return this.BadRequest(ex);
            }
        }

        // PUT api/mangas/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]Manga value)
        {
        }

        // DELETE api/mangas/5
        [HttpDelete("{userid}/mangas/{mangaid}")]
        public void Delete(int userid, int mangaid)
        {
            var usermanga = this._context.UserFollowMangas.FirstOrDefault(u => u.UserId == userid && u.MangaId == mangaid);
            if (usermanga == null)
                return;

            this._context.UserFollowMangas.Remove(usermanga);
            this._context.SaveChanges();
        }
    }
}
