using Microsoft.AspNetCore.Mvc;

namespace DistSysAcw.Controllers
{
    [Route("api/[Controller]/[Action]")]
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        /// <summary>
        /// This DbContext contains the database context defined in UserContext.cs
        /// You can use it inside your controllers to perform database CRUD functionality
        /// </summary>
        protected Models.UserContext DbContext { get; set; }
        public BaseController(Models.UserContext dbcontext)
        {
            DbContext = dbcontext;
        }
    }
}