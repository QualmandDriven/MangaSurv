using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MangaSurvWebApi.Model;
using Microsoft.AspNetCore.Authorization;
using MangaSurvWebApi.Service;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

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
                List<Chapter> results = new List<Chapter>();

                Helper.QueryString queryString = new Helper.QueryString(Request);
                DateTime dt = new DateTime();
                if (queryString.ContainsKey("FROM") && DateTime.TryParse(queryString.GetValue("FROM"), out dt))
                {
                    results = (from chapter in _context.Chapters
                                where chapter.EnterDate >= dt
                                orderby chapter.EnterDate
                                select chapter).ToList();
                    
                    HashSet<long> hsMangaIds = new HashSet<long>();
                    results.ForEach(c => hsMangaIds.Add(c.MangaId));

                    var mangas = (from manga in _context.Mangas
                                 where hsMangaIds.Contains(manga.Id)
                                 select manga).ToList();

                    Dictionary<Tuple<int, long>, Manga> dicMangas = new Dictionary<Tuple<int, long>, Manga>();
                    foreach(Chapter c in results)
                    {
                        Tuple<int, long> t = new Tuple<int, long>(c.EnterDate.DayOfYear, c.MangaId);
                        if (dicMangas.ContainsKey(t))
                        {
                            dicMangas[t].Chapters.Add(c);
                        }
                        else
                        {
                            //var ma = _context.Mangas.FirstOrDefault(m => m.Id == c.MangaId);
                            var ma = mangas.First(m => m.Id == c.MangaId).Copy();
                            ma.Chapters.Clear();
                            ma.Chapters.Add(c);
                            dicMangas.Add(t, ma);
                        }
                    }

                    List<Manga> lMangas = new List<Manga>();
                    foreach(var kv in dicMangas)
                    {
                        lMangas.Add(kv.Value);
                    }

                    return this.Ok(lMangas);

                }
                else
                {
                    results = this._context.Chapters.ToList();
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

                if (queryString.ContainsKey("SORTBY") && queryString.GetValue("SORTBY").ToUpper().Equals("MANGA"))
                {
                    return this.Ok(results.SortByManga());
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
