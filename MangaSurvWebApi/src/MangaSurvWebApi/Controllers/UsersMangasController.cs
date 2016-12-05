using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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
            User user = this._context.Users.FirstOrDefault(u => u.Id == userid);

            if (user == null)
                return this.NotFound();
            
            return this.Ok(user.FollowedMangas);
        }
        
        // GET api/mangas/5
        [HttpGet("{userid}/mangas/{mangaid}")]
        [Produces(typeof(Manga))]
        public IActionResult Get(int userid, int mangaid)
        {
            //if(Request.QueryString.HasValue)
            //    return Request.QueryString.Value;

            User user = this._context.Users.FirstOrDefault(u => u.Id == userid);

            if (user == null)
                return this.NotFound();

            var manga = user.FollowedMangas.FirstOrDefault(m => m.Id == mangaid);

            return this.Ok(manga);
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


                user.FollowedMangas.Add(manga);
                await this._context.SaveChangesAsync();
                return this.CreatedAtAction("POST", value);
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
            var user = this._context.Users.FirstOrDefault(u => u.Id == userid);
            if (user == null)
                return;

            var manga = user.FollowedMangas.FirstOrDefault(m => m.Id == mangaid);

            if (manga != null)
            {
                user.FollowedMangas.Remove(manga);
                this._context.SaveChanges();
            }
        }
    }
}
