using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using DistSysAcw.Controllers;
using Microsoft.Data.SqlClient;

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
        //private static UserContext ctx = new UserContext();

        //GET Request User/New?username=blahblah
        public static string CheckUser(string username)
        {
            //Need to refresh context or else it gets removed at the end of the using bracket
            using (var ctx = new UserContext())
            {
                if (ctx.Users.Where(x => x.UserName == username).FirstOrDefault() == null)
                {
                    return "False - User Does Not Exist! Did you mean to do a POST to create a new user?";
                }
                else if(username == null)
                {
                    return "False - User Does Not Exist! Did you mean to do a POST to create a new user?";
                }
                else
                {
                    return "True - User Does Exist! Did you mean to do a POST to create a new user?";
                }
            }
        }

        //TASK 7 Remove User
        public static void DeleteUser(User Deleuser)
        {
            using (var ctx = new UserContext())
            {
                ctx.Remove(Deleuser);
                ctx.SaveChanges();
            }
        }

        public static void ChangeRole(User changed_user, string role)
        {
            using(var ctx = new UserContext())
            {
                ctx.Users.Update(changed_user);
                changed_user.Role = role;
                ctx.SaveChanges();
            }
        }

        public static User CheckApiKey(string apiKey)
        {
            //Need to refresh context or else it gets removed at the end of the using bracket
            using (var ctx = new UserContext())
            {
                if (ctx.Users.Where(x => x.ApiKey == apiKey).FirstOrDefault() != null)
                {
                    return ctx.Users.Find(apiKey);
                }
                else
                {
                    return null;
                }
            }
        }


        public static Guid GuidKey;

        //POST Request User/New
        public static void CreateUser(string newuser)
        {
            GuidKey = Guid.NewGuid();
            //Need to refresh context or else it gets removed at the end of the using bracket
            using (var ctx = new UserContext())
            {
                //change this with bits in the body

                if (newuser is string == true)
                {
                    if (ctx.Users.Where(x => x.UserName == newuser).FirstOrDefault() == null)
                    {
                        if (ctx.Users.FirstOrDefault() == null) //SELECT TOP 1000 [ApiKey] ,[UserName] ,[Role] FROM [dbo].[Users]
                        {
                            User usrn = new User { ApiKey = GuidKey.ToString(), UserName = newuser, Role = "Admin" };
                            ctx.Users.Add(usrn);
                            ctx.SaveChanges();
                        }
                        else
                        {
                            User usrn = new User { ApiKey = GuidKey.ToString(), UserName = newuser, Role = "User" };
                            ctx.Users.Add(usrn);
                            ctx.SaveChanges();
                        }
                        
                        UserController.output = 1;
                        
                    }
                    else
                    {
                        //throw new HttpResponseException("Oops. This username is already in use. Please try again with a new username.", HttpStatusCode.Forbidden);
                        UserController.output = 2;
                    }

                }
                else if(newuser is string == false)
                {//need to work on this

                    //throw new HttpResponseException("Oops. Make sure your body contains a string with your username and your Content-Type is Content-Type:application/json", HttpStatusCode.BadRequest);
                    UserController.output = 3;
                }
            }
        }


        #region Task3 
        // TODO: Make methods which allow us to read from/write to the database 
        #endregion


    }


}