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
    [Route("api/mangas")]
    public class MangasUsersController : Controller
    {
        private readonly MangaSurvContext _context;
        public MangasUsersController(MangaSurvContext context)
        {
            this._context = context;
        }

        // GET api/mangas
        [Authorize(Roles = WebApiAccess.WRITE_ROLE)]
        [HttpGet("{mangaid}/users")]
        public IActionResult Get(int mangaid)
        {
            var users = from ufm in this._context.UserFollowMangas
                        where ufm.MangaId == mangaid
                        select ufm.UserId;

            return this.Ok(users);
        }
    }
}
