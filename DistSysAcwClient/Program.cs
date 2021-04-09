using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace DistSysAcwClient
{
    #region Task 10 and beyond
    class Client
    {
        static HttpClient client = new HttpClient();
        static void Main(string[] args)
        {
            RunAsync().Wait();
            Console.ReadKey();
            //Environment.Exit(0);
            
        }
        static async Task RunAsync()
        {
            bool exit = false;
            client.BaseAddress = new Uri("http://localhost:44394/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            Console.WriteLine("Hello.What would you like to do?");
            {
                string command = Console.ReadLine();
                if (command != "Exit")
                {
                    Console.Clear();
                    Console.WriteLine("…please wait…");
                    

                    string path = "";


                    if (command.Contains("Sort") == true)
                    {
                        path = "api/talkback/sort?";
                        string b = "";

                        for (int i = 0; i < command.Length; i++)
                        {
                            if (i == command.Length)
                            {
                                if (Char.IsDigit(command[i]))
                                {
                                    b += command[i];
                                }
                            }
                            else
                            {
                                if (Char.IsDigit(command[i]))
                                {
                                    b += command[i] + ",";
                                }
                            }
                        }

                        int[] array = Array.ConvertAll(b.Split(','), int.Parse);

                        for (int i = 0; i < array.Length; i++)
                        {
                            if (array[i] != array.Last<int>())
                            {
                                path += "integers=" + array[i].ToString() + "&";
                            }
                            else
                            {
                                path += "integers=" + array[i].ToString();
                            }
                        }

                        //This is for GET requests
                        try
                        {
                            Task<HttpResponseMessage> task = GetStringAsync(path);



                            if (await Task.WhenAny(task, Task.Delay(20000)) == task)
                            { Console.WriteLine(task.Result); }
                            else
                            { Console.WriteLine("Request Timed Out"); }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.GetBaseException().Message);
                        }

                    }
                    else if (command.Contains("Get") == true)
                    {
                        string username = command.Substring(command.IndexOf("Get ") + 1);
                        path = "api/user/new?username=" + username;

                        //This is for GET requests
                        try
                        {
                            Task<HttpResponseMessage> task = GetStringAsync(path);

                            if (await Task.WhenAny(task, Task.Delay(20000)) == task)
                            { Console.WriteLine(task.Result); }
                            else
                            { Console.WriteLine("Request Timed Out"); }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.GetBaseException().Message);
                        }
                    }
                    else if (command.Contains("Post"))
                    {
                        string usernameBody = command.Substring(command.IndexOf("Post ") + 1);
                        path = "api/user/new";

                    }
                    else if (command.Contains("Set"))
                    {

                    }
                    else if (command.Contains("Delete"))
                    {

                    }
                    else if (command.Contains("Role"))
                    {

                    }
                    else if (command.Contains("SHA1"))
                    {

                    }
                    else if (command.Contains("SHA256"))
                    {

                    }
                    else if (command == "TalkBack Hello")
                    {
                        path = "api/talkback/hello";
                        try
                        {
                            Task<HttpResponseMessage> task = GetStringAsync(path);

                            Console.WriteLine(task.Result);
                            //if (await Task.WhenAny(task, Task.Delay(20000)) == task)
                            //{ Console.WriteLine(task.Result); }
                            //else
                            //{ Console.WriteLine("Request Timed Out"); }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.GetBaseException().Message);
                        }
                    }
                    else if (command == "Protected Hello")
                    {
                        try
                        {
                            Task<HttpResponseMessage> task = GetStringAsync(path);



                            if (await Task.WhenAny(task, Task.Delay(20000)) == task)
                            { Console.WriteLine(task.Result); }
                            else
                            { Console.WriteLine("Request Timed Out"); }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.GetBaseException().Message);
                        }
                    }

                    Console.WriteLine("What would you like to do next ?");
                }
                else
                {
                    exit = true;
                }


            } while (exit == false) ;

              
            
        }

        //GET REQEUEST
        static async Task<HttpResponseMessage> GetStringAsync(string path)
        {
            string responsestring = "";
            HttpResponseMessage response = await client.GetAsync(path);
            response.EnsureSuccessStatusCode();

            responsestring = await response.Content.ReadAsStringAsync();
            
            return response;
        }

        //POST REQUEST
        static async Task<string> PostAsync(string path, StringContent Body)
        {
            string responsestring = "";
            HttpResponseMessage response = await client.PostAsync(path, Body);
            response.EnsureSuccessStatusCode();
            responsestring = await response.Content.ReadAsStringAsync();
            return responsestring;
        }

    }
    #endregion
}
