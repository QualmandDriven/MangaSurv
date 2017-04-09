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
    public class FilesController : Controller
    {
        private readonly MangaSurvContext _context;
        public FilesController(MangaSurvContext context)
        {
            this._context = context;
        }

        // GET api/files
        [HttpGet]
        public IEnumerable<File> Get()
        {
            return this._context.Files.ToList();
        }

        // GET api/files/5
        [HttpGet("{id}", Name ="FileLink")]
        public File Get(int id)
        {
            //if(Request.QueryString.HasValue)
            //    return Request.QueryString.Value;

            File file = this._context.Files.FirstOrDefault(f => f.Id == id);
            return file;
        }

        // POST api/files
        [Authorize(Roles = WebApiAccess.WRITE_ROLE)]
        [HttpPost]
        public IActionResult Post([FromBody]File value)
        {
            try
            {
                if (!ModelState.IsValid)
                    return this.BadRequest(ModelState);

                MangaSurvWebApi.Model.File.AddFile(value);

                return this.CreatedAtRoute("FileLink", new { id = value.Id }, value);
            }
            catch(Exception ex)
            {
                return this.BadRequest(ex);
            }
        }

        // PUT api/files/5
        [Authorize(Roles = WebApiAccess.WRITE_ROLE)]
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]File value)
        {
        }

        // DELETE api/files/5
        [Authorize(Roles = WebApiAccess.WRITE_ROLE)]
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            File file = this._context.Files.FirstOrDefault(f => f.Id == id);
            if (file != null)
                this._context.Files.Remove(file);
        }
    }
}
