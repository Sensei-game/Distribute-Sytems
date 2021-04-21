using DistSysAcw.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
        public static RSACryptoServiceProvider RSAserver = new RSACryptoServiceProvider(new CspParameters()
        {
            KeyContainerName = "PublicPrivateKey",
            Flags = CspProviderFlags.UseMachineKeyStore
            //Flags = CspProviderFlags.UseDefaultKeyContainer 
        }
            )
        {
            PersistKeyInCsp = true,

        };
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
           //     RSAserver.PersistKeyInCsp = true;
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

        [ActionName("AddFifty")]
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult AddFifty([FromHeader(Name = "ApiKey")] string ApiKey, [FromQuery] string encryptedInteger, [FromQuery] string encryptedSymKey, [FromQuery] string encryptedIV)
        {
            if(UserDatabaseAccess.CheckApiKey(ApiKey) != null)
            {

                byte[] encrypted;
                //HEXADECIMAL and encrypt 

                //Intgere

                encryptedInteger = encryptedInteger.TrimStart('"').TrimEnd('"');
                encryptedInteger = encryptedInteger.Replace("-", "");
               
                int length = encryptedInteger.Length >> 1;
                byte[] buyt = new byte[length];

                for (int i = 0; i < length; i++)
                {
                    buyt[i] = Byte.Parse(encryptedInteger.Substring(i * 2, 2), NumberStyles.HexNumber);
                }

                var decryptedInteger = RSAserver.Decrypt(buyt, false);
                var textInt = BitConverter.ToInt32(decryptedInteger);

                textInt += 50;
                
                // Turn back to hexadecimal later

                //Symmetric key
                encryptedSymKey = encryptedSymKey.TrimStart('"').TrimEnd('"');
                encryptedSymKey = encryptedSymKey.Replace("-", "");

                int length_Key = encryptedSymKey.Length >> 1;
                byte[] buyt_key = new byte[length_Key];

                for (int i = 0; i < length_Key; i++)
                {
                    buyt_key[i] = Byte.Parse(encryptedSymKey.Substring(i * 2, 2), NumberStyles.HexNumber);
                }
            
                var decrypted_SYm = RSAserver.Decrypt(buyt_key, false);
              

                //IV
                encryptedIV = encryptedIV.TrimStart('"').TrimEnd('"');
                encryptedIV = encryptedIV.Replace("-", "");

                int length1 = encryptedIV.Length >> 1;
                byte[] buyt_IV = new byte[length1];
            
                for (int i = 0; i < length1; i++)
                {
                    buyt_IV[i] = Byte.Parse(encryptedIV.Substring(i * 2, 2), NumberStyles.HexNumber);
                }

                var decryptedIV = RSAserver.Decrypt(buyt_IV, false);

                //ENCRYPTION WITH AES
                using (AesManaged aesAlg = new AesManaged())
                {
                    aesAlg.Key = decrypted_SYm;
                    aesAlg.IV = decryptedIV;

                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt =
                                new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {
                                swEncrypt.Write(textInt.ToString());
                            }
                            encrypted = msEncrypt.ToArray();
                        }
                    }
                }
                string response = BitConverter.ToString(encrypted);
                return Ok(response);

            }
            else
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Bad Request");
            }
        }
    }
}
