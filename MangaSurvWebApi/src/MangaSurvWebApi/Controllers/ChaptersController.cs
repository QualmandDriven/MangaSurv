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
    [Route("api/[controller]")]
    public class ChaptersController : Controller
    {
        private readonly MangaSurvContext _context;
        public ChaptersController(MangaSurvContext context)
        {
            this._context = context;
        }

        // GET api/chapters
        [HttpGet]
        public IActionResult Get()
        {
            if(Request.QueryString.HasValue)
            {
                var results = this._context.Chapters.ToList();
                foreach (KeyValuePair<string, Microsoft.Extensions.Primitives.StringValues> pair in Request.Query)
                {
                    switch (pair.Key.ToUpper())
                    {
                        case "STATEID":
                            results = results.Where(o => o.StateId == int.Parse(pair.Value.ToString())).ToList();
                            break;
                        default:
                            break;
                    }
                }

                return this.Ok(results);
            }

            return this.Ok(this._context.Chapters.ToList());
        }

        // GET api/chapters/5
        [HttpGet("{id}", Name ="ChapterLink")]
        public Chapter Get(int id)
        {
            var result = this._context.Chapters.FirstOrDefault(d => d.Id == id);
            return result;
        }

        // POST api/chapters
        [Authorize(Roles = WebApiAccess.WRITE_ROLE)]
        [HttpPost]
        public IActionResult Post([FromBody]Chapter value)
        {
            try
            {
                if (!ModelState.IsValid)
                    return this.BadRequest(ModelState);

                Chapter.AddChapter(value);

                return this.CreatedAtRoute("ChapterLink", new { id = value.Id }, value);
            }
            catch(Exception ex)
            {
                return this.BadRequest(ex);
            }
        }

        // PUT api/chapters/5
        [Authorize(Roles = WebApiAccess.WRITE_ROLE)]
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]Chapter value)
        {
            this._context.Chapters.Attach(value);

            var chapter = this._context.Chapters.FirstOrDefault(o => o.Id == id);

            this._context.Entry(chapter).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            this._context.SaveChanges();
            return this.Ok(chapter);
        }

        // DELETE api/chapters/5
        [Authorize(Roles = WebApiAccess.WRITE_ROLE)]
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            var result = this._context.Chapters.FirstOrDefault(c => c.Id == id);
            if (result != null)
                this._context.Chapters.Remove(result);
        }
    }
}
