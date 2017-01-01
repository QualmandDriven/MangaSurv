using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ChapterSurvWebApi.Controllers
{
    [Route("api/users")]
    public class UsersChaptersController : Controller
    {
        private readonly MangaSurvContext _context;
        public UsersChaptersController(MangaSurvContext context)
        {
            this._context = context;
        }

        // GET api/chapters
        [HttpGet("{userid}/chapters/")]
        public IActionResult Get(int userid)
        {
            var chapterslist = (from chapter in _context.UserNewChapters
                                where chapter.UserId == userid
                                select chapter.Chapter).ToList();

            return this.Ok(chapterslist);
        }
        
        // GET api/chapters/5
        [HttpGet("{userid}/chapters/{chapterid}", Name ="UserChapterLink")]
        [Produces(typeof(Chapter))]
        public IActionResult Get(int userid, int chapterid)
        {
            var chapterslist = (from chapter in _context.UserNewChapters
                                where chapter.UserId == userid
                                && chapter.ChapterId == chapterid
                                select chapter.Chapter).ToList();

            return this.Ok(chapterslist.FirstOrDefault());
        }

        // POST api/chapters
        [HttpPost("{userid}/chapters/")]
        public async Task<IActionResult> Post(int userid, [FromBody]Chapter value)
        {
            try
            {
                var user = this._context.Users.FirstOrDefault(u => u.Id == userid);
                if (user == null)
                    return this.NotFound();

                var chapter = this._context.Chapters.FirstOrDefault(m => m.Id == value.Id);
                if (chapter == null)
                    return this.NotFound();

                var entry = this._context.UserNewChapters.FirstOrDefault(u => u.UserId == userid && u.ChapterId == chapter.Id);

                // Add entry only when it does not exist yet
                if (entry == null)
                {
                    UserNewChapters ufm = new UserNewChapters();
                    ufm.Chapter = chapter;
                    ufm.ChapterId = chapter.Id;
                    ufm.User = user;
                    ufm.UserId = user.Id;

                    this._context.UserNewChapters.Add(ufm);
                    await this._context.SaveChangesAsync();

                    entry = ufm;
                }
                return this.CreatedAtRoute("UserChapterLink", new { userid = entry.UserId, chapterid = entry.ChapterId }, entry);
            }
            catch(Exception ex)
            {
                return this.BadRequest(ex);
            }
        }

        // PUT api/chapters/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]Chapter value)
        {
        }

        // DELETE api/chapters/5
        [HttpDelete("{userid}/chapters/{chapterid}")]
        public void Delete(int userid, int chapterid)
        {
            var userchapter = this._context.UserNewChapters.FirstOrDefault(u => u.UserId == userid && u.ChapterId == chapterid);
            if (userchapter == null)
                return;

            this._context.UserNewChapters.Remove(userchapter);
            this._context.SaveChanges();
        }
    }
}
