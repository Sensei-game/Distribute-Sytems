using DistSysAcw.Auth;
using DistSysAcw.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
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
        [ActionName("New")]
        [HttpGet]
        public IActionResult Check([FromQuery] string username)
        {
            //Console.WriteLine(UserDatabaseAccess.CheckUser(username));
            return Ok(UserDatabaseAccess.CheckUser(username));
        }


        //POST request User/New
        //Body adds items for User, either one element or more
        [ActionName("New")]
        [HttpPost]
        public IActionResult Insert([FromBody] string newuser)
        {
            UserDatabaseAccess.CreateUser(newuser);

            if (output == 1)
            {
                //Console.WriteLine("Got API Key");
                return Ok(UserDatabaseAccess.GuidKey.ToString());
            }
            else if (output == 2)
            {
                //Console.WriteLine("Oops. This username is already in use. Please try again with a new username.");
                return StatusCode(StatusCodes.Status403Forbidden, "Oops. This username is already in use. Please try again with a new username.");//403 code;
            }
            else
            {
                //Console.WriteLine("Oops. Make sure your body contains a string with your username and your Content-Type is Content-Type:application/json");
                return BadRequest("Oops. Make sure your body contains a string with your username and your Content-Type is Content-Type:application/json");
            }
        }


        ///api/user/removeuser?username=<username>
        [ActionName("RemoveUser")]
        [HttpDelete]
        [Authorize(Roles = "Admin, User")]
        public IActionResult Delete([FromQuery] string username, [FromHeader(Name = "ApiKey")] string ApiKey)
        {
            if (UserDatabaseAccess.CheckApiKey(ApiKey) != null)
            {
                var key = UserDatabaseAccess.CheckApiKey(ApiKey);
                if( key.UserName == username)
                {
                    UserDatabaseAccess.DeleteUser(key);
                    return Ok(true);
                }
                else
                {
                    return Ok(false); 
                }
            }
            return Ok(false);
        }




        [ActionName("ChangeRole")]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult ChangeRole([FromHeader(Name = "ApiKey")] string ApiKey, [FromBody] ObjectJ bod)
        {
            if (UserDatabaseAccess.CheckApiKey(ApiKey) != null)
            {
                if (bod.role == "Admin" || bod.role == "User")
                {
                    if (UserDatabaseAccess.CheckUser(bod.username) != "False - User Does Not Exist! Did you mean to do a POST to create a new user?")
                    {
                        var user = UserDatabaseAccess.CheckApiKey(ApiKey);
                        UserDatabaseAccess.ChangeRole(user, bod.role);
                        return Ok("DONE");
                    }
                    else
                    {
                        return BadRequest("NOT DONE: Username does not exist");
                    }
                }
                else
                {
                    return BadRequest("NOT DONE: Role does not exist");
                }

            }
            return BadRequest("NOT DONE: An error occured");
        }
    }

    public class ObjectJ
    {
        public string username { get; set; }
        public string role { get; set; }
    }
}
