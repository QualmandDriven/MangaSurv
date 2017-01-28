using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MangaSurvWebApi.Model;

namespace AnimeSurvWebApi.Controllers
{
    [Route("api/animes")]
    public class AnimesEpisodesController : Controller
    {
        private readonly MangaSurvContext _context;
        public AnimesEpisodesController(MangaSurvContext context)
        {
            this._context = context;
        }
        
        // GET api/animes/5
        [HttpGet("{animeid}/episodes")]
        public IActionResult Get(int animeid)
        {
            var episodes = this._context.Episodes.Where(d => d.AnimeId == animeid);
            return this.Ok(episodes);
        }

        // GET api/animes/5
        [HttpGet("{animeid}/episodes/{episodeid}", Name ="AnimeEpisodeLink")]
        public IActionResult Get(int animeid, int episodeid)
        {
            Episode episode = this._context.Episodes.FirstOrDefault(d => d.Id == episodeid && d.AnimeId == animeid);
            return this.Ok(episode);
        }

        // POST api/animes
        [HttpPost("{animeid}/episodes")]
        public IActionResult Post(int animeid, [FromBody]Episode value)
        {
            try
            {
                value.AnimeId = animeid;

                if (!ModelState.IsValid)
                    return this.BadRequest(ModelState);

                var anime = this.Get(animeid);
                if (anime is NotFoundResult)
                    return this.NotFound();

                Episode.AddEpisode(this._context, value, true);

                return this.CreatedAtRoute("AnimeEpisodeLink", new { animeid = animeid, episodeid = value.Id }, value);
            }
            catch(Exception ex)
            {
                return this.BadRequest(ex);
            }
        }

        // PUT api/animes/5
        [HttpPut("{animeid}/episodes/{episodeid}")]
        public IActionResult Put(int animeid, int episodeid, [FromBody]Episode value)
        {
            this._context.Episodes.Attach(value);

            var episode = this._context.Episodes.FirstOrDefault(o => o.Id == episodeid);

            this._context.Entry(episode).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            this._context.SaveChanges();
            return this.Ok(episode);
        }

        // DELETE api/animes/5
        [HttpDelete("{animeid}/episodes/{episodeid}")]
        public void Delete(int animeid, int episodeid)
        {
            Episode episode = this._context.Episodes.FirstOrDefault(c => c.Id == episodeid && c.AnimeId == animeid);
            if (episode != null)
                this._context.Episodes.Remove(episode);
        }
    }
}
