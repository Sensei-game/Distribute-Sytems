using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.AspNetCore.Mvc;

namespace DistSysAcw.Models
{
    

    public class User
    {
        [Key]
        public string ApiKey { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
        public User() {}

        #region Task2
        // TODO: Create a User Class for use with Entity Framework
        // Note that you can use the [key] attribute to set your ApiKey Guid as the primary key 
        #endregion
    }

    #region Task13?
    // TODO: You may find it useful to add code here for Logging
    #endregion

    //Make an intsnace in the controller and then call all the bellow methods
    public static class UserDatabaseAccess
    {
        private static UserContext ctx = new UserContext();

        //GET Request User/New?username=blahblah
        public static string CheckUser(string username)
        {
            using (ctx)
            {
                if (ctx.Users.Where(x => x.UserName == username).FirstOrDefault() == null)
                {
                    return "False - User Does Not Exist! Did you mean to do a POST to create a new user?";
                }
                else if(username == "")
                {
                    return "True - User Does Exist! Did you mean to do a POST to create a new user?";
                }
                else
                {
                    return "True - User Does Exist! Did you mean to do a POST to create a new user?";
                }
            }
        }

        //POST Request User/New
        public static string CreateUser(string newuser)
        {
            Guid GuidKey = Guid.NewGuid();

            using (ctx)
            {
                //change this with bits in the body

                if (newuser is string)
                {
                    if (ctx.Users.Where(x => x.UserName == newuser).FirstOrDefault() == null)
                    {
                        User usrn = new User { ApiKey = GuidKey.ToString(), UserName = newuser, Role = "Admin" };
                        ctx.Users.Add(usrn);
                        ctx.SaveChanges();
                        return GuidKey.ToString();
                    }
                    else
                    {
                        throw new HttpException("Oops. This username is already in use. Please try again with a new username.", 403);
                    }

                }
                else
                {
                    throw new HttpException("Oops. Make sure your body contains a string with your username and your Content-Type is Content-Type:application/json", 400);
                }
            }
        }


        #region Task3 
        // TODO: Make methods which allow us to read from/write to the database 
        #endregion


    }


}