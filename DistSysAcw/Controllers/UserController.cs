using DistSysAcw.Models;
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
        public string insert([FromBody] string newuser )
        {
            return UserDatabaseAccess.CreateUser(newuser);
        }




    }
}
