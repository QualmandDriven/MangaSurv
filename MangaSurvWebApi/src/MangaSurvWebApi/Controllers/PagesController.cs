using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MangaSurvWebApi.Controllers
{
    [Route("api/[controller]")]
    public class PagesController : Controller
    {
        private readonly MangaSurvContext _context;
        public PagesController(MangaSurvContext context)
        {
            this._context = context;
        }

        // GET api/pages
        [HttpGet]
        public IActionResult Get()
        {
            var results = this._context.Pages.ToList();
            foreach (KeyValuePair<string, Microsoft.Extensions.Primitives.StringValues> pair in Request.Query)
            {
                switch (pair.Key.ToUpper())
                {
                    case "CONTENTTYPE":
                        results = results.Where(o => o.ContentType.ToUpper() == pair.Value.ToString().ToUpper()).ToList();
                        break;
                    default:
                        break;
                }
            }

            return this.Ok(results);
        }

        // GET api/pages/5
        [HttpGet("{id}", Name ="PageLink")]
        [Produces(typeof(Manga))]
        public IActionResult Get(int id)
        {
            var result = this._context.Pages.FirstOrDefault(o => o.Id == id);
            if (result == null)
                return this.NotFound();
            
            return this.Ok(result);
        }

        // POST api/pages
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Page value)
        {
            try
            {
                if (!ModelState.IsValid)
                    return this.BadRequest(ModelState);

                await this._context.Pages.AddAsync(value);
                await this._context.SaveChangesAsync();
                return this.CreatedAtRoute("PageLink", new { id = value.Id }, value);
            }
            catch(Exception ex)
            {
                return this.BadRequest(ex);
            }
        }

        // PUT api/pages/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]Page value)
        {
            this._context.Pages.Attach(value);

            var result = this._context.Pages.FirstOrDefault(o => o.Id == id);

            this._context.Entry(result).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            this._context.SaveChanges();
            return this.Ok(result);
        }

        // DELETE api/pages/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            var result = this._context.Pages.FirstOrDefault(o => o.Id == id);
            if (result != null)
            { 
                this._context.Pages.Remove(result);
                this._context.SaveChanges();
            }
        }
    }
}
