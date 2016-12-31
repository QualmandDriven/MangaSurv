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
        public IActionResult GetEpisodes(int animeid)
        {
            var anime = this.GetAnime(animeid);
            if(anime is NotFoundResult)
                return this.NotFound();

            Anime foundAnime = ((anime as ObjectResult).Value as Anime);
            var episodes = this._context.Episodes.Where(c => c.AnimeId == foundAnime.Id);

            return this.Ok(episodes);
        }

        // GET api/animes/5
        [HttpGet("{animeid}/episodes/{episodeid}", Name = "AnimeEpisodeLink")]
        [Produces(typeof(List<Episode>))]
        public IActionResult GetEpisode(int animeid, int episodeid)
        {
            var anime = this.GetAnime(animeid);
            if (anime is NotFoundResult)
                return this.NotFound();

            Anime foundAnime = ((anime as ObjectResult).Value as Anime);
            var episodes = this._context.Episodes.Where(c => c.AnimeId == foundAnime.Id && c.Id == episodeid);

            return this.Ok(episodes);
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

        // POST api/mangas
        [HttpPost("{animeid}/episodes")]
        public async Task<IActionResult> PostEpisode(int animeid, [FromBody]Episode value)
        {
            try
            {
                if (!ModelState.IsValid)
                    return this.BadRequest(ModelState);

                var anime = this.GetAnime(animeid);
                if (anime is NotFoundResult)
                    return this.NotFound();

                Anime foundAnime = ((anime as ObjectResult).Value as Anime);

                // Wenn kein Datum übergeben wurde, dann setzen wir das jetzige
                value.DoDefaultDate();

                foundAnime.Episodes.Add(value);
                await this._context.SaveChangesAsync();

                return this.CreatedAtRoute("AnimeEpisodeLink", new { animeid = value.AnimeId, episodeid = value.Id }, value);
            }
            catch (Exception ex)
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

        // DELETE api/mangas/5
        [HttpDelete("{animeid}/episodes/{episodeid}")]
        public void DeleteEpisode(int animeid, int episodeid)
        {
            var episode = this._context.Episodes.FirstOrDefault(e => e.AnimeId == animeid && e.Id == episodeid);
            if (episode != null)
            {
                this._context.Episodes.Remove(episode);
                this._context.SaveChanges();
            }
        }
    }
}
