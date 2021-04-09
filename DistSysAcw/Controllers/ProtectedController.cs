using DistSysAcw.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace DistSysAcw.Controllers
{
    //Task 9

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProtectedController : BaseController
    {
        public static RSACryptoServiceProvider RSAserver = new RSACryptoServiceProvider();
        //RSACryptoServiceProvider.UseMachineKeyStore = true;

        string publicKeyXml = RSAserver.ToXmlString(false);

        string PrivatePublicKey = RSAserver.ToXmlString(true);

        public ProtectedController(Models.UserContext dbcontext) : base(dbcontext)
        {

        }


        [ActionName("Hello")]
        [HttpGet]
        [Authorize(Roles ="Admin, User")]
        public IActionResult Hello([FromHeader(Name = "ApiKey")]string ApiKey)
        {
            if(UserDatabaseAccess.CheckApiKey(ApiKey) != null)
            {
                return Ok("Hello " + UserDatabaseAccess.CheckApiKey(ApiKey).UserName);
            }

            return null;

        }

        [ActionName("SHA1")]
        [HttpGet]
        [Authorize(Roles = "Admin, User")]
        public IActionResult ProtSHA1([FromQuery] string message)
        {
            if(message != null)
            {


                var hash = SHA1CryptoServiceProvider.Create();
                byte[] bytes = Encoding.ASCII.GetBytes(message);
                byte[] code = hash.ComputeHash(bytes);

                string response = "";

                foreach (byte b in code)
                {
                    //Use X2 for Upper case, x2 for lowercase
                    response += b.ToString("X2");
                }

                return Ok(response);
            }
            else
            {
                return BadRequest("Bad Request");
            }
        }

        [ActionName("SHA256")]
        [HttpGet]
        [Authorize(Roles = "Admin, User")]
        public IActionResult ProtSHA256([FromQuery] string message)
        {
            if (message != null)
            {


                var hash = SHA256CryptoServiceProvider.Create();
                byte[] bytes = Encoding.ASCII.GetBytes(message);
                byte[] code = hash.ComputeHash(bytes);

                string response = "";

                foreach (byte b in code)
                {
                    //Use X2 for Upper case, x2 for lowercase
                    response += b.ToString("X2");
                }

                return Ok(response);
            }
            else
            {
                return BadRequest("Bad Request");
            }
        }

        [ActionName("GetPublicKey")]
        [HttpGet]
        public string RSApublic([FromHeader(Name = "ApiKey")] string ApiKey)
        {
            //New Private/Public key pair generated


            //This to read the key, use flase for public only, true for both parts

            if (UserDatabaseAccess.CheckApiKey(ApiKey) != null)
            {
                RSAserver.PersistKeyInCsp = true;
                return publicKeyXml;
            }

            return null;
        }

        [ActionName("Sign")]
        [HttpGet]
        public string SignMessage([FromHeader(Name = "ApiKey")] string ApiKey, [FromQuery] string message)
        {
            if(UserDatabaseAccess.CheckApiKey(ApiKey)!= null)
            {

                byte[] bytes = Encoding.ASCII.GetBytes(message);

                //No need to hash
                //var hash = SHA1CryptoServiceProvider.Create();
                //byte[] code = hash.ComputeHash(bytes);

                var signature = RSAserver.SignData(bytes, "SHA1");
                string hex = BitConverter.ToString(signature);

                return hex;
            }
            return null;
        }


    }
}
