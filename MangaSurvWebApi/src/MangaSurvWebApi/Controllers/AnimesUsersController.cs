using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MangaSurvWebApi.Model;
using Microsoft.AspNetCore.Authorization;
using MangaSurvWebApi.Service;

namespace MangaSurvWebApi.Controllers
{
    [Route("api/animes")]
    public class AnimesUsersController : Controller
    {
        private readonly MangaSurvContext _context;
        public AnimesUsersController(MangaSurvContext context)
        {
            this._context = context;
        }

        // GET api/mangas
        [Authorize(Roles = WebApiAccess.WRITE_ROLE)]
        [HttpGet("{animeid}/users")]
        public IActionResult Get(int animeid)
        {
            var users = from ufa in this._context.UserFollowAnimes
                        where ufa.AnimeId == animeid
                        select ufa.UserId;

            return this.Ok(users);
        }
    }
}
