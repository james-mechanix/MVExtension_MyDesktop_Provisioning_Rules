using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Mms_Metaverse;

namespace TestingConsoleForMvExtension
{
    //API key for generic api - f2779ad07e9c2d92d2b0ee12fb2e933f9fdf7d4c
    //API key for Production api - 9ec96aea1c5982d84a83eef8198475998de641e8

    class Program
    {
        private static void Main(string[] args)
        {
            /*
             *Generic API Calls
             */

            //makeAPIGetCall("office");
            printGenericAgentsList();

            /*
             *Production API calls
             */
            //Setup to create Cameron User
            MyDesktopUserRequest data = new MyDesktopUserRequest();
            //getProdAgentsList(data);
            //  TESTINGGETUSERS(data);

            int officeID = 27295; //Office Id required
            string gID = "123"; //Google ID
            string aID = "111111"; //AgentID
            string apiKey = "9ec96aea1c5982d84a83eef8198475998de641e8";
            // DeleteUser(apiKey , gID , aID , officeID); //params are apiKey googleId, agentId and office id
            //CreateUser(data, officeID);

            Console.ReadLine();
        }
        private static void CreateUser(MyDesktopUserRequest data, string offID)
        {
            // barfoot API endpoint - send POST as per specs: https://docs.google.com/document/d/1vWlj3XYS4n8yw3j5KCZ64Z12P6yW-TvivYXxS-Z2TYs/edit# 

            var CurrentTime = DateTimeOffset.UtcNow;
            var CurrentTimeString = CurrentTime.ToString("yyyy-MM-ddTHH:mm:ssZ");
            string baseUrl = "https://api.mydesktop.com.au";
            string resourceUrl = "cgi-bin/barfoot/auth/user.cgi"; //Test
            string DateFormat = "yyyy-MM-ddTHH:mm:ssZ";
            string Action = "CreateUser";
            string apiKey = "9ec96aea1c5982d84a83eef8198475998de641e8";
            string officeID = offID;
            string authString = GetHashSha256($"{CurrentTimeString}{apiKey}{officeID}");


            data.OfficeId = officeID;
            data.AuthenticationString = authString;
            data.Lastname = "Smith";
            data.Commit = 0; //SET to 1 to commit a user
            data.Mobile = "02102657946";
            data.IsSuperUser = false;
            data.Telephone = "02102657946";
            data.Action = Action;
            data.Firstname = "Cameron";
            data.CurrentTime = CurrentTime;
            data.GoogleId = "10576810104834359741";
            data.Email = "Cameron@mechanix.co.nz";

            // build request
            MyDesktopUserResponse response;
            var client = new RestClient(baseUrl);
            var request = new RestRequest(resourceUrl)
            {
                JsonSerializer = new Helper.RestSharpJsonNetSerializer()
                {
                    DateFormat = DateFormat,
                },
                Method = Method.POST
            };
            request.AddJsonBody(data);
            try
            {
                var resp = client.Execute(request);
                response = JsonConvert.DeserializeObject<MyDesktopUserResponse>(resp.Content);

                Console.WriteLine("Creating User: " + data.Firstname + " " + data.Lastname + " googleID: " + data.GoogleId);
                Console.WriteLine(resp.Content);
                Console.WriteLine("\nSuccess (Testing function: User not committed)");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("Create User Failure");

            }
        }
        private static void getProdAgentsList(MyDesktopUserRequest data)
        {
            var CurrentTime = DateTimeOffset.UtcNow;
            var CurrentTimeString = CurrentTime.ToString("yyyy-MM-ddTHH:mm:ssZ");
            string baseUrl = "https://api.mydesktop.com.au";
            // string resourceUrl = "cgi-bin/barfoot/auth/user.cgi"; 
            string resourceUrl = "cgi-bin/barfoot/api.cgi"; //For Testing
            string DateFormat = "yyyy-MM-ddTHH:mm:ssZ";
            string Action = "FetchStaff";
            string apiKey = "9ec96aea1c5982d84a83eef8198475998de641e8";
            string authString = GetHashSha256($"{CurrentTimeString}{apiKey}{Action}");

            data.CurrentTime = CurrentTime;
            data.AuthenticationString = authString;
            data.Action = "FetchStaff";

            var client = new RestClient(baseUrl);
            var request = new RestRequest(resourceUrl)
            {
                JsonSerializer = new Helper.RestSharpJsonNetSerializer()
                {
                    DateFormat = DateFormat,
                },
                Method = Method.POST
            };
            request.AddJsonBody(data);
            try
            {
                var resp = client.Execute(request); //Execute request
                string JSonReturned = FormatJson(resp.Content);
                Console.WriteLine(JSonReturned);
               






            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("response failed there is nothing");
            }
        }






        private static string[] apiConfig(string apiKEY, string act, string officeID)
        {
            //OfficeID in the authentication string is used for Creation/Delete/Update users
            string baseUrl = "https://api.mydesktop.com.au";
            string resourceUrl = "/cgi-bin/barfoot/auth/user.cgi";
            string apiKey = apiKEY;

            string Action = act;

            string officeId = officeID;

            var CurrentTime = DateTimeOffset.UtcNow;
            var CurrentTimeString = CurrentTime.ToString("yyyy-MM-ddTHH:mm:ssZ");

            string authString = GetHashSha256($"{CurrentTimeString}{apiKey}{officeID}");
            string[] config = { apiKey, baseUrl, resourceUrl, Action, authString };

            return config;
        }
        private static void DeleteUser(string api, string gID, string aID, string officeID)
        {
            dynamic[] x = apiConfig(api, "UpdateUser", officeID); // //office ID used for authentication string

            MyDesktopUserRequest data = new MyDesktopUserRequest();
            var CurrentTime = DateTimeOffset.UtcNow;
            string baseUrl = x[1];
            string resourceUrl = x[2];
            string Action = x[3];
            string apiKey = x[0];

            string authString = x[4];

            //Building JSON request for Deleting a user
            data.SecretKey = apiKey;
            data.OfficeId = officeID;
            data.GoogleId = gID;
            data.AgentId = aID;
            data.CurrentTime = CurrentTime;
            data.AuthenticationString = authString;
            data.Action = Action;
            data.Commit = 0; //0 for testing 1 for deletion
            data.IsDeleted = true; // 1 always for delete

            MyDesktopUserResponse response;
            string DateFormat = "yyyy-MM-ddTHH:mm:ssZ";
            var client = new RestClient(baseUrl);
            var request = new RestRequest(resourceUrl)
            {
                JsonSerializer = new Helper.RestSharpJsonNetSerializer()
                {
                    DateFormat = DateFormat,
                },
                Method = Method.POST
            };
            request.AddJsonBody(data);

            try
            {//Execute query on Barfoot API 
                var resp = client.Execute(request);
                response = JsonConvert.DeserializeObject<MyDesktopUserResponse>(resp.Content);


                Console.WriteLine(resp.Content);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("response failed there is nothing");
            }
        }//Untested
        static void printGenericAgentsList()
        {
            foreach (var item in getGenericAgentsList())
            {

                Console.WriteLine("Email: " + isNull(item.email));
                Console.WriteLine("Fax: " + isNull(item.fax));
                Console.WriteLine("Mobile Display: " + isNull(item.mobiledisplay));
                Console.WriteLine("First Name: " + isNull(item.firstname));
                Console.WriteLine("Mobile: " + isNull(item.mobile));
                Console.WriteLine("Last Name: " + isNull(item.lastname));
                Console.WriteLine("Image Url: " + isNull(item.imageurl));
                Console.WriteLine("ID: " + item.id);
                Console.WriteLine("Telephone: " + isNull(item.telephone));
                Console.WriteLine();

            }
        }
        static List<Agent> getGenericAgentsList()
        {
            string[] apiConfig = setGenericApiValues("agents");
            //Function takes param ofive token , apiKeymbaseUrl,apiroot, action.
            string response = makeGenericAPICall(apiConfig[0], apiConfig[1], apiConfig[2], apiConfig[3], apiConfig[4], Method.GET);
            var data = JsonConvert.DeserializeObject<RootObjectAgents>(response).agents; // Returns a list object of all agents
            return data;
        }//Returns all user accounts and information in a List of type Agent
        private static string[] setGenericApiValues(string act)
        {
            string officeToken = "eyJhbGciOiJIUzI1NiJ9.eyJhcGlfa2V5IjoiZjI3NzlhZDA3ZTljMmQ5MmQyYjBlZTEyZmIyZTkzM2Y5ZmRmN2Q0YyIsImFnZW50aWQiOjUwNDkyOSwidHlwZSI6Im9mZmljZSIsImdyb3VwaWQiOjI3Mjk1LCJwYXNzd29yZF9tb2RkYXRlIjoiMjAxNy0wNi0xMiAwNzoyNjoxNiJ9.15vixsCGY7mRHfbrE_QDl_Jthmhu9uRiYAXeUlPdEmE";
            string apiKey = "f2779ad07e9c2d92d2b0ee12fb2e933f9fdf7d4c";
            string baseUrl = "https://integrations.mydesktop.com.au";
            string apiRoot = "/api/v1.2/";
            string action = act;

            string[] config = { officeToken, apiKey, baseUrl, apiRoot, action };
            return config;
        }//generic api Config
        static string isNull(string x)
        { //used for api responses that are null
            if (String.IsNullOrWhiteSpace(x))
            { return "NA"; }
            return x;
        }
        private static string makeGenericAPICall(string officeToken, string apiKey, string baseUrl, string apiVersionRoot, string action, Method method)
        {
            // build request, and attach the token as a basic authentication username with a blank password
            var client = new RestClient(baseUrl)
            { Authenticator = new HttpBasicAuthenticator(officeToken, "") };
            var request = new RestRequest($"{apiVersionRoot}{action}?api_key={apiKey}", method);
            // Send the request to the API
            var response = client.Execute(request);
            // Process/Return the response
            var responseContent = response.Content.ToString();
            if (responseContent == string.Empty)
                throw new Exception("No reponse received. We expect a values to be returned");
            return FormatJson(responseContent);
        }//Used to query the Generic API
        private static string FormatJson(string json) //Used for debugging Json in commandline
        {
            dynamic parsedJson = JsonConvert.DeserializeObject(json);
            return JsonConvert.SerializeObject(parsedJson, Newtonsoft.Json.Formatting.Indented);
        }
        public static string GetHashSha256(string s) // Used for auth string when calling production url
        {
            var crypt = new SHA256Managed();
            var hash = new StringBuilder();
            var crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(s), 0, Encoding.UTF8.GetByteCount(s));
            foreach (var theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }
        //JSON Response deserializaion classes
        public class Agent
        {
            public string email { get; set; }
            public string fax { get; set; }
            public string mobiledisplay { get; set; }
            public string firstname { get; set; }
            public string mobile { get; set; }
            public string lastname { get; set; }
            public string imageurl { get; set; }
            public int id { get; set; }
            public string telephone { get; set; }
        }
        public class RootObjectAgents
        {
            public List<Agent> agents { get; set; }
        }
    }
}
