using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace DistSysAcwClient
{
    #region Task 10 and beyond
    class Client
    {
        static HttpClient client = new HttpClient();
        public static RSACryptoServiceProvider RSAclient = new RSACryptoServiceProvider();


        static void Main(string[] args)
        {
            RunAsync().Wait();
            //Console.WriteLine("You typed Exit, please press any key to exit...");
            //Console.ReadKey();
            Environment.Exit(0);
            
        }
        static async Task RunAsync()
        {
            bool exit = false;

            //http://150.237.94.9/4702207/api/talkback/hello
            //client.BaseAddress = new Uri("http://localhost:44394/");

            client.BaseAddress = new Uri("http://150.237.94.9/4702207/");

           // client.DefaultRequestHeaders.Accept.Clear();

            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            string currentUsername ="";
            string currentApiKey ="";

            Console.WriteLine("Hello.What would you like to do?");
            while (exit == false)
            {
                string command = Console.ReadLine();
                if (command != "Exit")
                {
                    Console.Clear();
                    Console.WriteLine("…please wait…");
                    

                    string path = "";


                    if (command.Contains("Sort"))
                    {
                        path = "api/talkback/sort?";
                        //string b = "";

                        command = command.Split(" ")[2];

                        command = command.TrimStart('[');
                        command = command.TrimEnd(']');

                        command = command.Replace(" ", String.Empty);


                        var c = command.Split(",");
                        c = c.Select(value => "integers=" + value).ToArray();

                        path = path + String.Join('&', c);



                        //This is for GET requests
                        try
                        {
                            //Task<HttpResponseMessage> task = GetStringAsync(path);

                            var message = await GetStringAsync(path);

                            Console.WriteLine(message);

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
                    else if (command.Contains("PublicKey"))
                    {
                        if (currentApiKey == "")
                        {
                            Console.WriteLine("You need to do a User Post or User Set first");
                        }
                        else
                        {
                            path = "api/protected/getpublickey";

                            try
                            {
                                HttpResponseMessage response = await client.GetAsync(path);

                                string final_Response = await response.Content.ReadAsStringAsync();

                                if (response.StatusCode == HttpStatusCode.OK)
                                {
                                    string RSApublicKey = final_Response.TrimStart('"').TrimEnd('"');

                                    RSAclient.FromXmlString(RSApublicKey);

                                    Console.WriteLine("Got Public Key");
                                }
                                else
                                {
                                    Console.WriteLine("Couldn’t Get the Public Key");
                                }

                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.GetBaseException().Message);
                            }
                        }
                    }
                    else if (command.Contains("Get"))
                    {
                        string username = command.Split(" ")[2];
                        path = "api/user/new?username=" + username;

                        //This is for GET requests
                        try
                        {
                            //Task<HttpResponseMessage> task = GetStringAsync(path);

                            var message = await GetStringAsync(path);

                            Console.WriteLine(message);

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
                    else if (command.Contains("Post"))
                    {
                        string usernameBody = command.Split(" ")[2];
          
                        path = "api/user/new";

                        try
                        {
                            //Task<HttpResponseMessage> task = GetStringAsync(path);

                            string json = JsonConvert.SerializeObject(usernameBody);

                            StringContent message_json = new StringContent(json, Encoding.UTF8, "application/json");
                            HttpResponseMessage response = await client.PostAsync(path, message_json);

                            string final_Response = await response.Content.ReadAsStringAsync();

                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                currentUsername = usernameBody;
                                currentApiKey = (string)JsonConvert.DeserializeObject(final_Response);

                                client.DefaultRequestHeaders.Remove("ApiKey");
                                client.DefaultRequestHeaders.Add("ApiKey", currentApiKey);

                                Console.WriteLine("Got API Key");
                            }
                            else
                            {
                                Console.WriteLine(final_Response);
                            }
                         
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
                    else if (command.Contains("Set"))
                    {
                        currentUsername = command.Split(" ")[2];
                        currentApiKey = command.Split(" ")[3];

                        client.DefaultRequestHeaders.Remove("ApiKey");
                        client.DefaultRequestHeaders.Add("ApiKey", currentApiKey);

                        Console.WriteLine("Stored");
                    }
                    else if (command.Contains("Delete"))
                    {
                         if (currentApiKey == "" || currentUsername == "")
                         {
                            Console.WriteLine("You need to do a User Post or User Set first");
                         }
                        else
                        {
                            path = "api/user/removeuser?username=" + currentUsername;

                            try
                            {
                                HttpResponseMessage response = await DeleteAsync(path);

                                string final_Response = await response.Content.ReadAsStringAsync();

                                Console.WriteLine(final_Response);

                                currentApiKey = "";
                                currentUsername = "";
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.GetBaseException().Message);
                            }
                        }

                       
                    }
                    else if (command.Contains("Role"))
                    {
                        if (currentApiKey == "")
                        {
                            Console.WriteLine("You need to do a User Post or User Set first");
                        }
                        else 
                        {
                            string username = command.Split(" ")[2];
                            string role = command.Split(" ")[3];

                            path = "api/user/changerole";

                            try
                            {
                                User jsonUser = new User(username, role);

                                string json = JsonConvert.SerializeObject(jsonUser);

                                StringContent message_json = new StringContent(json, Encoding.UTF8, "application/json");

                                HttpResponseMessage response = await client.PostAsync(path, message_json);

                                string final_Response = await response.Content.ReadAsStringAsync();

                                //string trueOrFalse = (string)JsonConvert.DeserializeObject(responseBody);

                                Console.WriteLine(final_Response);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.GetBaseException().Message);
                            }
                        }

                        
                    }
                    else if (command.Contains("SHA1"))
                    {
                        if (currentApiKey == "")
                        {
                            Console.WriteLine("You need to do a User Post or User Set first");
                        }
                        else
                        {
                            string message = command.Split(" ")[2];
                            path = "api/protected/sha1?message=" + message;

                            try
                            {
                                string final_Response = await GetStringAsync(path);

                                Console.WriteLine(final_Response);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.GetBaseException().Message);
                            }
                        }
                    }
                    else if (command.Contains("SHA256"))
                    {
                        if (currentApiKey == "")
                        {
                            Console.WriteLine("You need to do a User Post or User Set first");
                        }
                        else
                        {
                            string message = command.Split(" ")[2];
                            path = "api/protected/sha256?message=" + message;

                            try
                            {
                                string final_Response = await GetStringAsync(path);

                                Console.WriteLine(final_Response);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.GetBaseException().Message);
                            }
                        }
                        
                    }
                    
                    else if(command.Contains("Sign"))
                    {
                        if (currentApiKey == "")
                        {
                            Console.WriteLine("You need to do a User Post or User Set first");
                        }
                        else if(RSAclient.ToXmlString(false) == "")
                        {
                            Console.WriteLine("Client doesn’t yet have the public key");
                        }
                        else
                        {
                            string message = command.Split(" ")[2];

                            path = "api/protected/sign?message=" + message;

                            HttpResponseMessage response = await client.GetAsync(path);

                            
                            var Hash_string = await response.Content.ReadAsStringAsync();

                            //Trim
                            Hash_string = Hash_string.TrimStart('"').TrimEnd('"');
                            Hash_string = Hash_string.Replace("-", "");

                            int length = Hash_string.Length >> 1;
                            byte[] signature = new byte[length];

                            for (int i = 0; i < length; i++)
                            {
                                signature[i] = Byte.Parse(Hash_string.Substring(i * 2, 2), NumberStyles.HexNumber);
                            }


                            

                            //String is a hex string 

                            //Sign the message

                           byte[] bytes = Encoding.UTF8.GetBytes(message);


                            bool signed = RSAclient.VerifyData(bytes, CryptoConfig.MapNameToOID("SHA1"), signature);

                            if (signed == true)
                            {
                                Console.WriteLine("Message was successfully signed");
                            }
                            else
                            {
                                Console.WriteLine("Message was not successfully signed");
                            }
                        }
                    }

                    else if (command == "TalkBack Hello")
                    {
                        path = "api/talkback/hello";
                        try
                        {
                            var message = await GetStringAsync(path);

                            Console.WriteLine(message);
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
                        if (currentApiKey == "")
                        {
                            Console.WriteLine("You need to do a User Post or User Set first");
                        }
                        else
                        {
                            path = "api/protected/hello";
                            try
                            {
                                string response = await GetStringAsync(path);
                                Console.WriteLine(response);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.GetBaseException().Message);
                            }
                        }
                    }

                    Console.WriteLine("What would you like to do next ?");
                }
                else
                {
                    exit = true;
                }
            }   
        }

        //GET REQEUEST
        static async Task<string> GetStringAsync(string path)
        {
            string responsestring = "";
            HttpResponseMessage response = await client.GetAsync(path);
            //You will have falis too
           // response.EnsureSuccessStatusCode();

            responsestring = await response.Content.ReadAsStringAsync();

            return responsestring;
        }

        //POST REQUEST
        static async Task<HttpResponseMessage> PostAsync(string path, StringContent Body)
        {
            string responsestring = "";
            HttpResponseMessage response = await client.PostAsync(path, Body);
            //response.EnsureSuccessStatusCode();
            responsestring = await response.Content.ReadAsStringAsync();
            return response;
        }

        //DELETE REQUEST
        static async Task<HttpResponseMessage> DeleteAsync(string path)
        {
            string responsestring = "";
            HttpResponseMessage response = await client.DeleteAsync(path);
            //response.EnsureSuccessStatusCode();
            responsestring = await response.Content.ReadAsStringAsync();
            return response;
        }

    }
    #endregion
    public class User
    {
        public string username;
        public string role;
        public User(string username, string role)
        {
            this.username = username;
            this.role = role;
        }

    }
}
