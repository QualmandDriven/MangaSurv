using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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
