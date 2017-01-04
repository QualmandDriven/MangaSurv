using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

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
            if(Request.QueryString.HasValue)
            {
                var results = this._context.Episodes.ToList();
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

                this._context.Episodes.Add(value);
                this._context.SaveChanges();
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
