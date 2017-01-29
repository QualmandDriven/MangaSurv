using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MangaSurvWebApi.Model;

namespace MangaSurvWebApi.Controllers
{
    [Route("api/[controller]")]
    public class EpisodesController : Controller
    {
        private readonly MangaSurvContext _context;
        public EpisodesController(MangaSurvContext context)
        {
            this._context = context;
        }

        // GET api/episodes
        [HttpGet]
        public IActionResult Get()
        {
            Helper.QueryString queryString = new Helper.QueryString(Request);
            if (queryString.ContainsKeys())
            {
                if (queryString.ContainsKey("STATEID"))
                {
                    return this.Ok(this._context.Episodes.Where(o => o.StateId == int.Parse(queryString.GetValue("STATEID"))).ToList());
                }
                else
                {
                    return this.Ok(this._context.Episodes.ToList());
                }
            }

            return this.Ok(this._context.Episodes.ToList());
        }

        // GET api/episodes/5
        [HttpGet("{id}", Name ="EpisodeLink")]
        public Episode Get(int id)
        {
            var result = this._context.Episodes.FirstOrDefault(d => d.Id == id);
            return result;
        }

        // POST api/episodes
        [HttpPost]
        public IActionResult Post([FromBody]Episode value)
        {
            try
            {
                if (!ModelState.IsValid)
                    return this.BadRequest(ModelState);

                Episode.AddEpisode(this._context, value, true);

                return this.CreatedAtRoute("EpisodeLink", new { id = value.Id }, value);
            }
            catch(Exception ex)
            {
                return this.BadRequest(ex);
            }
        }

        // PUT api/episodes/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]Episode value)
        {
            this._context.Episodes.Attach(value);

            var episode = this._context.Episodes.FirstOrDefault(o => o.Id == id);

            this._context.Entry(episode).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            this._context.SaveChanges();
            return this.Ok(episode);
        }

        // DELETE api/episodes/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            var result = this._context.Episodes.FirstOrDefault(c => c.Id == id);
            if (result != null)
                this._context.Episodes.Remove(result);
        }
    }
}
