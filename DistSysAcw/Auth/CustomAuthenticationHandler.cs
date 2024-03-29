﻿using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using DistSysAcw.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace DistSysAcw.Auth
{
    /// <summary>
    /// Authenticates clients by API Key
    /// </summary>
    public class CustomAuthenticationHandler
        : AuthenticationHandler<AuthenticationSchemeOptions>, IAuthenticationHandler
    {
        private Models.UserContext DbContext { get; set; }

        public CustomAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            Models.UserContext dbContext)
            : base(options, logger, encoder, clock) 
        {
            DbContext = dbContext;
        }


        //[Authorize(Roles = "Admin, User")]
        //Authorize future requests which will need to use ApiKeys
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var re = Request;
            var headers = re.Headers;
            var query = re.Query;

            if (headers.ContainsKey("ApiKey") == true)
            {
                headers.TryGetValue("ApiKey", out var dbApiKey);

                //query.TryGetValue("username", out var username);

                //Use the User Acces class to get the User of that API

                if(UserDatabaseAccess.CheckApiKey(dbApiKey) != null /*&& UserDatabaseAccess.CheckUser(username) != "False - User Does Not Exist! Did you mean to do a POST to create a new user?"*/)
                {
                    User foundUser = UserDatabaseAccess.CheckApiKey(dbApiKey);
                    Claim[] claims = { new Claim(ClaimTypes.Name, foundUser.UserName), new Claim(ClaimTypes.Role, foundUser.Role) };
                    var identity = new ClaimsIdentity(claims, dbApiKey);
                    var principal = new ClaimsPrincipal(identity);

                    AuthenticationTicket ticket = new AuthenticationTicket(principal, this.Scheme.Name);
                    return Task.FromResult(AuthenticateResult.Success(ticket));
                }
                
            }
            //Continue with 401 error             

            return Task.FromResult(AuthenticateResult.Fail("Unauthorized. Check ApiKey in Header is correct."));


            #region Task5
            // TODO:  Find if a header ‘ApiKey’ exists, and if it does, check the database to determine if the given API Key is valid
            //        Then create the correct Claims, add these to a ClaimsIdentity, create a ClaimsPrincipal from the identity 
            //        Then use the Principal to generate a new AuthenticationTicket to return a Success AuthenticateResult
            #endregion
        }

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            byte[] messagebytes = Encoding.ASCII.GetBytes("Unauthorized. Check ApiKey in Header is correct.");
            Context.Response.StatusCode = 401;
            Context.Response.ContentType = "application/json";
            await Context.Response.Body.WriteAsync(messagebytes, 0, messagebytes.Length);
        }
    }
}