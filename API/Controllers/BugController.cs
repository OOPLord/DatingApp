using API.Data;
using API.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class BugController : BaseApiController
    {
        private readonly DataContext _context;

        public BugController(DataContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet("auth")]
        public ActionResult<string> GetSecret()
        {
            return "secret text";
        }

        [HttpGet("not-found")]
        public ActionResult<AppUser> GetNotFound()
        {
            var notExist = _context.Users.Find(-1);

            if(notExist == null)
            {
                return NotFound();
            }

            return Ok(notExist);
        }

        [HttpGet("server-error")]
        public ActionResult<string> GetServerError()
        {
            var notExist = _context.Users.Find(-1);
            var notExistReturn = notExist.ToString();

            return notExistReturn;
        }

        [HttpGet("bad-request")]
        public ActionResult<string> GetBadRequest()
        {
            return BadRequest("This was not a good request");
        }
    }
}
