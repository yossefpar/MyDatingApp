using System.Net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{

    public class BuggyController: BaseApiController
    {
        private readonly DataContext _context;

        public BuggyController(DataContext context)
        {
            _context = context;
        }

        //  401 unauthorized
        [Authorize]
        [HttpGet("auth")]// api/buggy/auth
        public ActionResult<string> GetSecret()
        {
            return "Secret String";
        }


        // 404 not found
        [HttpGet("not-found")] // api/buggy/not-found
        public ActionResult<AppUser> GetNotFound()
        {
            var thing = _context.Users.Find(-1);
            if (thing == null)
            {
                return NotFound();
            }

            return Ok(); //🤣
        }


        // 500 server error
        [HttpGet("server-error")]//api/buggy/server-error
        public ActionResult<string> GetServerError()
        {
             var thing = _context.Users.Find(-1);
             var thingToString = thing.ToString(); //NullReferenceExaption
             return thingToString; // 🤣
        }


        [HttpGet("bad-request")]//api/buggy/bad-request
        public ActionResult<string> GetBadRequest() {
            return BadRequest("this was not a good request");
        }


    }
}