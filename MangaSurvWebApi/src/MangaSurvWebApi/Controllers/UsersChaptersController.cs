using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MangaSurvWebApi.Model;

namespace MangaSurvWebApi.Controllers
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
            if (Request.QueryString.HasValue)
            {
                Helper.QueryString queryString = new Helper.QueryString(Request);
                if (queryString.ContainsKey("SORTBY") && queryString.GetValue("SORTBY").ToUpper().Equals("MANGA"))
                {
                    List<Chapter> lNewChapters = (from chapter in _context.UserNewChapters
                                                    where chapter.UserId == userid
                                                    select chapter.Chapter).ToList();

                    List<Manga> lMangas = new List<Manga>();
                    List<long> lMangaIds = new List<long>();

                    foreach (Chapter chapter in lNewChapters)
                    {
                        if (lMangaIds.Contains(chapter.MangaId))
                        {
                            Manga manga = lMangas.Find(m => m.Id == chapter.MangaId);
                            if(!manga.Chapters.Contains(chapter))
                                manga.Chapters.Add(chapter);
                        }
                        else
                        {
                            Manga manga = this._context.Mangas.FirstOrDefault(m => m.Id == chapter.MangaId);
                            if (manga != null)
                            {
                                lMangas.Add(manga);
                                lMangaIds.Add(manga.Id);
                            }
                        }
                    }

                    return this.Ok(lMangas);
                }
            }

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

                var entry = await UserNewChapters.AddChapterToUser(this._context, chapter, userid, true);

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
        public IActionResult Delete(int userid, int chapterid)
        {
            try
            { 
                var userchapters = this._context.UserNewChapters.Where(u => u.UserId == userid && u.ChapterId == chapterid);
                if (userchapters == null || userchapters.Count() == 0)
                    return this.Ok();

                this._context.UserNewChapters.RemoveRange(userchapters);
                this._context.SaveChangesAsync();
            }
            catch
            {
                
            }

            return this.Ok();
        }

        [HttpOptions]
        public IActionResult Options()
        {
            return this.Ok();
        }
    }
}
