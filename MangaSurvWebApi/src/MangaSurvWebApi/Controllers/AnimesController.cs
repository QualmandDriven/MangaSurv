using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MangaSurvWebApi.Controllers
{
    [Route("api/[controller]")]
    public class AnimesController : Controller
    {
        private readonly MangaSurvContext _context;
        public AnimesController(MangaSurvContext context)
        {
            this._context = context;
        }

        // GET api/mangas
        [HttpGet]
        public IActionResult GetAnimes()
        {
            if (HttpContext.Request.Query.ContainsKey("include") && HttpContext.Request.Query["include"].ToString() == "1") { 
                var results = _context.Animes.Select(a => new { a.Id, a.Name, a.FileSystemName, a.Episodes }).ToList();
                return this.Ok(results);
            }
            
            if (HttpContext.Request.Query.ContainsKey("name"))
            {
                return this.Ok(this._context.Animes.Where(a => a.Name == HttpContext.Request.Query["name"].ToString()));
            }

            return this.Ok(this._context.Animes.ToList());
        }

        // GET api/animes/5
        [HttpGet("{id}", Name ="AnimeLink")]
        [Produces(typeof(Manga))]
        public IActionResult GetAnime(int id)
        {
            var anime = this._context.Animes.FirstOrDefault(a => a.Id == id);
            if (anime == null)
                return this.NotFound();
            
            return this.Ok(anime);
        }

        // POST api/mangas
        [HttpPost]
        public async Task<IActionResult> PostAnime([FromBody]Anime value)
        {
            try
            {
                if (!ModelState.IsValid)
                    return this.BadRequest(ModelState);

                List<Episode> lEpisodes = value.Episodes;
                lEpisodes.ForEach(e => e.DoDefaultDate());

                value.Episodes = new List<Episode>();

                await this._context.Animes.AddAsync(value);
                await this._context.SaveChangesAsync();
                var newAnime = this._context.Animes.FirstOrDefault(a => a.Id == value.Id);
                newAnime.Episodes.AddRange(lEpisodes);
                await this._context.SaveChangesAsync();
                return this.CreatedAtRoute("AnimeLink", new { animeid = value.Id }, value);
            }
            catch(Exception ex)
            {
                return this.BadRequest(ex);
            }
        }

        // PUT api/mangas/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]Anime value)
        {
            var anime = this._context.Animes.FirstOrDefault(m => m.Id == id);
            if(anime != null)
            { 
                this._context.Animes.Attach(value);
                this._context.Entry(anime).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                this._context.SaveChanges();
                this.Ok(anime);
            }
            else
            {
                this.BadRequest();
            }
        }

        // DELETE api/mangas/5
        [HttpDelete("{animeid}")]
        public void DeleteAnime(int animeid)
        {
            var anime = this._context.Animes.FirstOrDefault(a => a.Id == animeid);
            if (anime != null)
            { 
                this._context.Animes.Remove(anime);
                this._context.SaveChanges();
            }
        }
    }
}
