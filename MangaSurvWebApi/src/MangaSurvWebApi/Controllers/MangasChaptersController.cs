using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MangaSurvWebApi.Controllers
{
    [Route("api/mangas")]
    public class MangasChaptersController : Controller
    {
        private readonly MangaSurvContext _context;
        public MangasChaptersController(MangaSurvContext context)
        {
            this._context = context;
        }
        
        // GET api/mangas/5
        [HttpGet("{mangaid}/chapters")]
        public IActionResult Get(int mangaid)
        {
            //if(Request.QueryString.HasValue)
            //    return Request.QueryString.Value;

            var chapters = this._context.Chapters.Where(d => d.MangaId == mangaid);
            return this.Ok(chapters);
        }

        // GET api/mangas/5
        [HttpGet("{mangaid}/chapters/{chapterid}")]
        public IActionResult Get(int mangaid, int chapterid)
        {
            //if(Request.QueryString.HasValue)
            //    return Request.QueryString.Value;

            Chapter chapter = this._context.Chapters.FirstOrDefault(d => d.Id == chapterid && d.MangaId == mangaid);
            return this.Ok(chapter);
        }

        // POST api/mangas
        [HttpPost("{mangaid}/chapters")]
        public IActionResult Post(int mangaid, [FromBody]Chapter value)
        {
            try
            {
                value.MangaId = mangaid;

                if (!ModelState.IsValid)
                    return this.BadRequest(ModelState);

                this._context.Chapters.Add(value);
                this._context.SaveChanges();
                return this.CreatedAtAction("POST", value);
            }
            catch(Exception ex)
            {
                return this.BadRequest(ex);
            }
        }

        // PUT api/mangas/5
        [HttpPut("{mangaid}/chapters/{chapterid}")]
        public void Put(int mangaid, int chapterid, [FromBody]Manga value)
        {
        }

        // DELETE api/mangas/5
        [HttpDelete("{mangaid}/chapters/{chapterid}")]
        public void Delete(int mangaid, int chapterid)
        {
            Chapter chapter = this._context.Chapters.FirstOrDefault(c => c.Id == chapterid && c.MangaId == mangaid);
            if (chapter != null)
                this._context.Chapters.Remove(chapter);
        }
    }
}
