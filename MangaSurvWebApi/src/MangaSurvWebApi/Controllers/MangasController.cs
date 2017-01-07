using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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
            if (Request.QueryString.HasValue)
            {
                IQueryable<Manga> results = this._context.Mangas.Where(m => 1 == 1);
                foreach (KeyValuePair<string, Microsoft.Extensions.Primitives.StringValues> pair in Request.Query)
                {
                    switch (pair.Key.ToUpper())
                    {
                        case "CHAPTERSTATEID":

                            List<Chapter> lNewChapters = this._context.Chapters.Where(c => c.StateId == int.Parse(pair.Value.ToString())).ToList();
                            List<Manga> lMangas = new List<Manga>();
                            List<long> lMangaIds = new List<long>();

                            foreach (Chapter chapter in lNewChapters)
                            {
                                if (lMangaIds.Contains(chapter.MangaId))
                                {
                                    Manga manga = lMangas.Find(m => m.Id == chapter.MangaId);
                                    manga.Chapters.Add(chapter);
                                }
                                else
                                {
                                    Manga manga = this._context.Mangas.FirstOrDefault(m => m.Id == chapter.MangaId);
                                    if (manga != null)
                                    {
                                        //manga.Chapters.Add(chapter);
                                        lMangas.Add(manga);
                                        lMangaIds.Add(manga.Id);
                                    }
                                }
                            }

                            return this.Ok(lMangas);
                            //List<dynamic> dynMangas = new List<dynamic>();
                            //foreach(Manga manga in lMangas)
                            //{
                            //    List<dynamic> dynChapters = new List<dynamic>();
                            //    foreach(Chapter chapter in manga.Chapters)
                            //    {
                            //        dynChapters.Add(new { ChapterNo = chapter.ChapterNo, Address = chapter.Address, EnterDate=chapter.EnterDate });
                            //    }

                            //    dynamic dynManga = new { Id = manga.Id, Name = manga.Name, FileSystemName = manga.FileSystemName, EnterDate = manga.EnterDate, Chapters = dynChapters };
                            //    dynMangas.Add(dynManga);
                            //}

                            //return this.Ok(dynMangas);
                        case "INCLUDE":
                            results.Include(m => m.Chapters);
                            break;
                        default:
                            break;
                    }
                }

                return this.Ok(results);
            }

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
