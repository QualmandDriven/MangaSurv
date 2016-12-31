using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MangaSurvWebApi.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly MangaSurvContext _context;
        public UsersController(MangaSurvContext context)
        {
            this._context = context;
        }

        // GET api/mangas
        [HttpGet]
        public IActionResult Get()
        {
            if (HttpContext.Request.Query.ContainsKey("include") && HttpContext.Request.Query["include"].ToString() == "1") { 
                var results = _context.Users.Select(u => new { u.Id, u.Name, u.FollowedMangas, u.NewChapters }).ToList();
                return this.Ok(results);
            }

            var users = this._context.Users.ToList();

            if (HttpContext.Request.Query.ContainsKey("name"))
            {
                return this.Ok(users.Where(u => u.Name == HttpContext.Request.Query["name"].ToString()));
            }

            return this.Ok(users);
        }

        // GET api/mangas/5
        [HttpGet("{id}", Name ="UserLink")]
        [Produces(typeof(Manga))]
        public IActionResult Get(int id)
        {
            //if(Request.QueryString.HasValue)
            //    return Request.QueryString.Value;
            
            User user = this._context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return this.NotFound();
            
            return this.Ok(user);
        }
        
        // POST api/mangas
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]User value)
        {
            try
            {
                if (!ModelState.IsValid)
                    return this.BadRequest(ModelState);

                await this._context.Users.AddAsync(value);
                await this._context.SaveChangesAsync();
                return this.CreatedAtRoute("UserLink", new { id = value.Id }, value);
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
            User user = this._context.Users.FirstOrDefault(m => m.Id == id);
            if (user != null)
            { 
                this._context.Users.Remove(user);
                this._context.SaveChanges();
            }
        }
    }
}
