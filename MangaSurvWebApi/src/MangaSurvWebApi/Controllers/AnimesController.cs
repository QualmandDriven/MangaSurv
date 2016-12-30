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
        public IActionResult GetAnime()
        {
            if (HttpContext.Request.Query.ContainsKey("include") && HttpContext.Request.Query["include"].ToString() == "1") { 
                var results = _context.Animes.Select(a => new { a.Id, a.Name, a.FileSystemName, a.Episodes }).ToList();
                return this.Ok(results);
            }

            var animes = this._context.Animes.ToList();

            if (HttpContext.Request.Query.ContainsKey("name"))
            {
                return this.Ok(animes.Where(a => a.Name == HttpContext.Request.Query["name"].ToString()));
            }

            return this.Ok(animes);
        }

        // GET api/animes/5
        [HttpGet("{id}")]
        [Produces(typeof(Manga))]
        public IActionResult GetAnime(int id)
        {
            //if(Request.QueryString.HasValue)
            //    return Request.QueryString.Value;
            
            var anime = this._context.Animes.FirstOrDefault(a => a.Id == id);
            if (anime == null)
                return this.NotFound();
            
            return this.Ok(anime);
        }

        // GET api/animes/5
        [HttpGet("{animeid}/episodes")]
        [Produces(typeof(List<Episode>))]
        public IActionResult GetChapters(int animeid)
        {
            var anime = this.GetAnime(animeid);
            if(anime is NotFoundResult)
                return this.NotFound();

            Anime foundAnime = ((anime as OkObjectResult).Value as Anime);
            var episodes = this._context.Episodes.Where(c => c.AnimeId == foundAnime.Id);

            return this.Ok(episodes);
        }

        // POST api/mangas
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Anime value)
        {
            try
            {
                if (!ModelState.IsValid)
                    return this.BadRequest(ModelState);

                List<Episode> lEpisodes = value.Episodes;
                value.Episodes = new List<Episode>();

                await this._context.Animes.AddAsync(value);
                await this._context.SaveChangesAsync();
                var newAnime = this._context.Animes.FirstOrDefault(a => a.Id == value.Id);
                newAnime.Episodes.AddRange(lEpisodes);
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
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            var anime = this._context.Animes.FirstOrDefault(a => a.Id == id);
            if (anime != null)
            { 
                this._context.Animes.Remove(anime);
                this._context.SaveChanges();
            }
        }
    }
}
