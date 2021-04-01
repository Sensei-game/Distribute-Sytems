using DistSysAcw.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DistSysAcw.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : BaseController
    {
        public static int output = 0;

        public UserController(Models.UserContext dbcontext) : base(dbcontext)
        {

        }




        //GET request User/New?username=blahblah
        [ActionName ("New")]
        [HttpGet]
        public string Check([FromQuery] string username)
        {
            return UserDatabaseAccess.CheckUser(username);
        }


        //POST request User/New
        //Body adds items for User, either one element or more
        [ActionName("New")]
        [HttpPost]
        public IActionResult Insert([FromBody] string newuser )
        {
             UserDatabaseAccess.CreateUser(newuser);
          
            if(output == 1) 
            {
                return Ok(UserDatabaseAccess.GuidKey.ToString());
            }
            else if (output == 2)
            {
                return StatusCode(StatusCodes.Status403Forbidden, "Oops. This username is already in use. Please try again with a new username.");//403 code;
            }
            else
            { 
                return BadRequest("Oops. Make sure your body contains a string with your username and your Content-Type is Content-Type:application/json");
            }
        }




    }
}
