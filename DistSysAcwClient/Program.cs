using System;
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
                        try
                        {
                            currentUsername = command.Split(" ")[2];
                            currentApiKey = command.Split(" ")[3];

                            client.DefaultRequestHeaders.Remove("ApiKey");
                            client.DefaultRequestHeaders.Add("ApiKey", currentApiKey);

                            Console.WriteLine("Stored");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.GetBaseException().Message);
                        }
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
                            try
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
                            catch (Exception e)
                            {
                                Console.WriteLine(e.GetBaseException().Message);
                            }
                        }
                    }
                    else if(command.Contains("AddFifty"))
                    {
                        if (currentApiKey == "")
                        {
                            Console.WriteLine("You need to do a User Post or User Set first");
                        }
                        else if (RSAclient.ToXmlString(false) == "")
                        {
                            Console.WriteLine("Client doesn’t yet have the public key");
                        }
                        else
                        {
                            string message = command.Split(" ")[2];
                           

                            if (int.TryParse(message, out int value) == false )
                            {
                                Console.WriteLine("A valid integer must be given!");
                            }
                            else
                            {//encrypt it somehow with just the public key?????
                                //Create local AES with IV and sym key

                                Aes aes = Aes.Create();
                                aes.GenerateIV();
                                aes.GenerateKey();

                                var AES_IV = aes.IV;
                                var SYM_Key = aes.Key;


                                //Integer
                                byte[] byte_m = Encoding.UTF8.GetBytes(message);

                                //false for public only 

                                RSAclient.Encrypt(byte_m, false);

                                string hex_integer = BitConverter.ToString(byte_m);


                                //SymKey

                                RSAclient.Encrypt(SYM_Key, false);

                                //BITCONVERTE gives you dashes
                                string hex_Sym = BitConverter.ToString(SYM_Key);

                                //IV

                                RSAclient.Encrypt(AES_IV, false);

                                string hex_IV = BitConverter.ToString(AES_IV);



                                path = "api/protected/addfifty?encryptedInteger="+ hex_integer + "&encryptedsymkey=" + hex_Sym + "&encryptedIV =" + hex_IV;

                                HttpResponseMessage response = await client.GetAsync(path);

                                //Hexadecimal
                                var response_string = await response.Content.ReadAsStringAsync();
                                try
                                {
                                    if (response_string.Contains("Bad") == false)
                                {
                                    //Trim
                                    response_string = response_string.TrimStart('"').TrimEnd('"');
                                    response_string = response_string.Replace("-", "");

                                    int length = response_string.Length >> 1;
                                    byte[] decrypt_me = new byte[length];

                                    for (int i = 0; i < length; i++)
                                    {
                                        decrypt_me[i] = Byte.Parse(response_string.Substring(i * 2, 2), NumberStyles.HexNumber);
                                    }

                                    string plaintext = "";

                                    //DESCRYPT MESSAGE
                                    using (AesManaged aesAlg = new AesManaged())
                                    {
                                        aesAlg.Key = SYM_Key;
                                        aesAlg.IV = AES_IV;

                                        ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                                        using (MemoryStream msDecrypt = new MemoryStream(decrypt_me))
                                        {
                                            using (CryptoStream csDecrypt =
                                                    new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                                            {
                                                using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                                                {
                                                    plaintext = srDecrypt.ReadToEnd();
                                                }

                                            }
                                        }
                                    }


                                    if (int.TryParse(plaintext, out int irr) == false)
                                    {
                                        Console.WriteLine("An error occurred!");
                                    }
                                    else
                                    {
                                        Console.WriteLine(plaintext);
                                    }
                                }
                                    else
                                    {
                                    Console.WriteLine("An error occurred!");
                                    }
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.GetBaseException().Message);
                                }

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
