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
                List<Episode> results = new List<Episode>();

                DateTime dt = new DateTime();
                if (queryString.ContainsKey("FROM") && DateTime.TryParse(queryString.GetValue("FROM"), out dt))
                {
                    results = (from chapter in _context.Episodes
                               where chapter.EnterDate >= dt
                               orderby chapter.EnterDate
                               select chapter).ToList();

                    HashSet<long> hsAnimeIds = new HashSet<long>();
                    results.ForEach(c => hsAnimeIds.Add(c.AnimeId));

                    var animes = (from anime in _context.Animes
                                  where hsAnimeIds.Contains(anime.Id)
                                  select anime).ToList();

                    Dictionary<Tuple<int, long>, Anime> dicAnimes = new Dictionary<Tuple<int, long>, Anime>();
                    foreach (Episode c in results)
                    {
                        Tuple<int, long> t = new Tuple<int, long>(c.EnterDate.DayOfYear, c.AnimeId);
                        if (dicAnimes.ContainsKey(t))
                        {
                            dicAnimes[t].Episodes.Add(c);
                        }
                        else
                        {
                            //var ma = _context.Animes.FirstOrDefault(m => m.Id == c.AnimeId);
                            var ma = animes.First(m => m.Id == c.AnimeId).Copy();
                            ma.Episodes.Clear();
                            ma.Episodes.Add(c);
                            dicAnimes.Add(t, ma);
                        }
                    }

                    List<Anime> lAnimes = new List<Anime>();
                    foreach (var kv in dicAnimes)
                    {
                        lAnimes.Add(kv.Value);
                    }

                    return this.Ok(lAnimes);

                }
                else
                {
                    results = this._context.Episodes.ToList();
                }

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

                if (queryString.ContainsKey("SORTBY") && queryString.GetValue("SORTBY").ToUpper().Equals("ANIME"))
                {
                    return this.Ok(results.SortByAnime());
                }

                return this.Ok(results);
            }

            return this.Ok(this._context.Episodes.ToList());

            //Helper.QueryString queryString = new Helper.QueryString(Request);
            //if (queryString.ContainsKeys())
            //{
            //    List<Episode> results = new List<Episode>();
            //    DateTime dt = new DateTime();

            //    if (queryString.ContainsKey("STATEID"))
            //    {
            //        return this.Ok(this._context.Episodes.Where(o => o.StateId == int.Parse(queryString.GetValue("STATEID"))).ToList());
            //    }
            //    else if (queryString.ContainsKey("FROM") && DateTime.TryParse(queryString.GetValue("FROM"), out dt))
            //    {
            //        results = (from episode in _context.Episodes
            //                   where episode.EnterDate >= dt
            //                   orderby episode.EnterDate
            //                   select episode).ToList();
            //    }
            //    else
            //    {
            //        return this.Ok(this._context.Episodes.ToList());
            //    }

            //    if (queryString.ContainsKey("SORTBY") && queryString.GetValue("SORTBY").ToUpper().Equals("ANIME"))
            //    {
            //        return this.Ok(results.SortByAnime());
            //    }
            //}

            //return this.Ok(this._context.Episodes.ToList());
        }

        // GET api/episodes/5
        [HttpGet("{id}", Name ="EpisodeLink")]
        public Episode Get(int id)
        {
            var result = this._context.Episodes.FirstOrDefault(d => d.Id == id);
            return result;
        }

        // POST api/episodes
        [Authorize(Roles = WebApiAccess.WRITE_ROLE)]
        [HttpPost]
        public IActionResult Post([FromBody]Episode value)
        {
            try
            {
                if (!ModelState.IsValid)
                    return this.BadRequest(ModelState);

                Episode.AddEpisode(value);

                return this.CreatedAtRoute("EpisodeLink", new { id = value.Id }, value);
            }
            catch(Exception ex)
            {
                return this.BadRequest(ex);
            }
        }

        // PUT api/episodes/5
        [Authorize(Roles = WebApiAccess.WRITE_ROLE)]
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
        [Authorize(Roles = WebApiAccess.WRITE_ROLE)]
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            var result = this._context.Episodes.FirstOrDefault(c => c.Id == id);
            if (result != null)
                this._context.Episodes.Remove(result);
        }
    }
}
