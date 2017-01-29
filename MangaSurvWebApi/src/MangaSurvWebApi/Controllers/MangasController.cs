using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MangaSurvWebApi.Model;

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
                Helper.QueryString queryString = new Helper.QueryString(Request);
                if (queryString.ContainsKey("CHAPTERSTATEID"))
                {
                    List<Chapter> lNewChapters = this._context.Chapters.Where(c => c.StateId == int.Parse(queryString.GetValue("CHAPTERSTATEID"))).ToList();
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
                                lMangas.Add(manga);
                                lMangaIds.Add(manga.Id);
                            }
                        }
                    }

                    return this.Ok(lMangas);
                }
                else if (queryString.ContainsKey("INCLUDE"))
                {
                    return this.Ok(this._context.Mangas.Include(m => m.Chapters));
                }
                else if(queryString.ContainsKey("NAME"))
                {
                    return this.Ok(this._context.Mangas.Where(m => m.Name == queryString.GetValue("NAME")));
                }
            }

            return this.Ok(this._context.Mangas.ToList());
        }

        // GET api/mangas/5
        [HttpGet("{id}", Name ="MangaLink")]
        [Produces(typeof(Manga))]
        public IActionResult Get(int id)
        {
            Manga manga = this._context.Mangas.FirstOrDefault(m => m.Id == id);
            if (manga == null)
                return this.NotFound();

            Helper.QueryString queryString = new Helper.QueryString(Request);
            if(queryString.ContainsKeys())
            {
                if (queryString.ContainsKey("INCLUDE"))
                {
                    return this.Ok(this._context.Mangas.Where(m=> m.Id == id).Include(m => m.Chapters).FirstOrDefault());
                }
            }

            return this.Ok(manga);
        }

        // POST api/mangas
        [HttpPost]
        public IActionResult Post([FromBody]Manga value)
        {
            try
            {
                if (!ModelState.IsValid)
                    return this.BadRequest(ModelState);

                Manga.AddManga(this._context, value, true);

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
