using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MangaSurvWebApi.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using MangaSurvWebApi.Service;

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
        [Authorize]
        public IActionResult Get()
        {
            return this.Ok();
        }

        [Authorize(Roles = WebApiAccess.USER_ROLE)]
        [HttpGet("login")]
        public IActionResult CheckLogin()
        {
            return this.Ok();
        }
    }
}
