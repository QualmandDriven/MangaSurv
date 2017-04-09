using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MangaSurvWebApi.Model;
using Microsoft.AspNetCore.Authorization;
using MangaSurvWebApi.Service;

namespace MangaSurvWebApi.Controllers
{
    [Route("api/mangas")]
    public class MangasChaptersFilesController : Controller
    {
        private readonly MangaSurvContext _context;
        public MangasChaptersFilesController(MangaSurvContext context)
        {
            this._context = context;
        }
        
        // GET api/mangas/5/chapters/2/files
        [HttpGet("{mangaid}/chapters/{chapterid}/files")]
        public IActionResult Get(int mangaid, int chapterid)
        {
            var files = this._context.Files.Where(f => f.ChapterId == chapterid);
            return this.Ok(files);
        }

        // GET api/mangas/5/chapters/2/files/4
        [HttpGet("{mangaid}/chapters/{chapterid}/files/{fileid}", Name ="MangaChapterFileLink")]
        public IActionResult Get(int mangaid, int chapterid, int fileid)
        {
            //if(Request.QueryString.HasValue)
            //    return Request.QueryString.Value;

            var file = this._context.Files.FirstOrDefault(d => d.Id == fileid);
            return this.Ok(file);
        }

        // POST api/mangas/5/chapters/2/files
        [Authorize(Roles = WebApiAccess.WRITE_ROLE)]
        [HttpPost("{mangaid}/chapters/{chapterid}/files")]
        public IActionResult Post(int mangaid, int chapterid, [FromBody]File value)
        {
            try
            {
                value.ChapterId = chapterid;

                if (!ModelState.IsValid)
                    return this.BadRequest(ModelState);

                MangaSurvWebApi.Model.File.AddFile(value);

                return this.CreatedAtRoute("MangaChapterFileLink", new { mangaid = mangaid, chapterid = chapterid, fileid = value.Id }, value);
            }
            catch(Exception ex)
            {
                return this.BadRequest(ex);
            }
        }

        // PUT api/mangas/5/chapters/2/files/34
        [Authorize(Roles = WebApiAccess.WRITE_ROLE)]
        [HttpPut("{mangaid}/chapters/{chapterid}/files/{fileid}")]
        public void Put(int mangaid, int chapterid, int fileid, [FromBody]Manga value)
        {
        }

        // DELETE api/mangas/5/chapters/2/files/34
        [Authorize(Roles = WebApiAccess.WRITE_ROLE)]
        [HttpDelete("{mangaid}/chapters/{chapterid}/files/{fileid}")]
        public void Delete(int mangaid, int chapterid, int fileid)
        {
            var file = this._context.Files.FirstOrDefault(f => f.Id == fileid);
            if (file != null)
                this._context.Files.Remove(file);
        }
    }
}
