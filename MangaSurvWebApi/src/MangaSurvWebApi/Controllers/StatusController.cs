using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MangaSurvWebApi.Controllers
{
    [Route("api/[controller]")]
    public class StatusController : Controller
    {
        
        public StatusController()
        {
            
        }

        // GET api/mangas
        [HttpGet]
        public IActionResult Get()
        {
            return this.Ok();
        }
    }
}
