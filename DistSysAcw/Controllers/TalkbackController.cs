using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DistSysAcw.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TalkbackController : BaseController
    {
        /// <summary>
        /// Constructs a TalkBack controller, taking the UserContext through dependency injection
        /// </summary>
        /// <param name="context">DbContext set as a service in Startup.cs and dependency injected</param>
        /// 
        
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}
        public TalkbackController(Models.UserContext dbcontext) : base(dbcontext)
        {
            
        }

        
        [ActionName("Hello")]
        [HttpGet]
        public string hello()
        {
            #region TASK1
            // TODO: add api/talkback/hello response
            return "Hello World";
            #endregion
        }

        //[ActionName("Hello")]
        //public string Get()
        //{
        //    #region TASK1
        //    // TODO: add api/talkback/hello response
        //    #endregion
        //}

        [ActionName("Sort")]
        [HttpGet]
        public string sort([FromQuery]int[] integers)
        {
            #region TASK1

            string message = "";

            Array.Sort(integers);


            foreach(var num in integers)
            {
                if (num != integers.Last<int>())
                {
                    message = message + num.ToString() + ",";
                }
                else
                {
                    message = message + num.ToString();
                }
                
            }

            //TODO:
            //   add a parameter to get integers from the URI query
            //   sort the integers into ascending order
            //   send the integers back as the api/talkback/sort response
            //   conform to the error handling requirements in the spec

                return "[" + message + "]"; 

           

            #endregion
        }

        //[ActionName("Sort")]
        //    #region TASK1
        //       TODO:
        //       add a parameter to get integers from the URI query
        //       sort the integers into ascending order
        //       send the integers back as the api/talkback/sort response
        //       conform to the error handling requirements in the spec
        //    #endregion
    }
}
