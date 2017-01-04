using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MangaSurvWebApi.Controllers
{
    [Route("api/[controller]")]
    public class MangasController : Controller
    {
        private readonly MangaSurvContext _context;
        public MangasController(MangaSurvContext context)
        {
            this._context = context;
        }

        // GET api/mangas
        [HttpGet]
        public IActionResult Get()
        {
            if (HttpContext.Request.Query.ContainsKey("include") && HttpContext.Request.Query["include"].ToString() == "1") { 
                var results = _context.Mangas.Select(m => new { m.Id, m.Name, m.FileSystemName, m.Chapters }).ToList();
                return this.Ok(results);
            }

            var mangas = this._context.Mangas.ToList();

            if (HttpContext.Request.Query.ContainsKey("name"))
            {
                return this.Ok(mangas.Where(m => m.Name == HttpContext.Request.Query["name"].ToString()));
            }
            
            return this.Ok(mangas);
        }

        // GET api/mangas/5
        [HttpGet("{id}", Name ="MangaLink")]
        [Produces(typeof(Manga))]
        public IActionResult Get(int id)
        {
            //if(Request.QueryString.HasValue)
            //    return Request.QueryString.Value;
            
            Manga manga = this._context.Mangas.FirstOrDefault(m => m.Id == id);
            if (manga == null)
                return this.NotFound();
            
            return this.Ok(manga);
        }

        // POST api/mangas
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Manga value)
        {
            try
            {
                if (!ModelState.IsValid)
                    return this.BadRequest(ModelState);

                List<Chapter> lChapters = value.Chapters;
                value.Chapters = new List<Chapter>();

                await this._context.Mangas.AddAsync(value);
                await this._context.SaveChangesAsync();
                Manga newManga = this._context.Mangas.FirstOrDefault(m => m.Id == value.Id);
                newManga.Chapters.AddRange(lChapters);
                await this._context.SaveChangesAsync();
                return this.CreatedAtRoute("MangaLink", new { id = value.Id }, value);
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
            this._context.Mangas.Attach(value);

            var manga = this._context.Mangas.FirstOrDefault(m => m.Id == id);

            this._context.Entry(manga).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            this._context.SaveChanges();
            this.Ok(manga);
        }

        // DELETE api/mangas/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            Manga manga = this._context.Mangas.FirstOrDefault(m => m.Id == id);
            if (manga != null)
            { 
                this._context.Mangas.Remove(manga);
                this._context.SaveChanges();
            }
        }
    }
}
