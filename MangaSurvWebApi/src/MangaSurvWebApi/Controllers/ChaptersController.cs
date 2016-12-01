using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MangaSurvWebApi.Controllers
{
    [Route("api/[controller]")]
    public class ChaptersController : Controller
    {
        private readonly MangaSurvContext _context;
        public ChaptersController(MangaSurvContext context)
        {
            this._context = context;
        }

        // GET api/mangas
        [HttpGet]
        public IEnumerable<Chapter> Get()
        {
            return this._context.Chapters.ToList();
        }

        // GET api/mangas/5
        [HttpGet("{id}")]
        public Chapter Get(int id)
        {
            //if(Request.QueryString.HasValue)
            //    return Request.QueryString.Value;

            Chapter chapter = this._context.Chapters.FirstOrDefault(d => d.Id == id);
            return chapter;
        }

        // POST api/mangas
        [HttpPost]
        public IActionResult Post([FromBody]Chapter value)
        {
            try
            {
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
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]Manga value)
        {
        }

        // DELETE api/mangas/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            Chapter chapter = this._context.Chapters.FirstOrDefault(c => c.Id == id);
            if (chapter != null)
                this._context.Chapters.Remove(chapter);
        }
    }
}
