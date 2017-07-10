using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.IO;
using System.Net;

namespace TestingConsoleForMvExtension
{
   
    class Program
    {
        
             private static void Main(string[] args)
        {

            //testCallAPIGET("propertycategories");
            testCallAPIPOST("propertycategories");
            //callAgents();
        }

        static void testCallAPIGET(string call)
        {
            string[] apiConfig = setGenericApiValues(call);
            string response = GetMyDesktopUsersForOffice(apiConfig[0], apiConfig[1], apiConfig[2], apiConfig[3], apiConfig[4], Method.GET);
            Console.WriteLine(response);
            Console.ReadLine();
        }
        static void testCallAPIPOST(string call)
        {
            string[] apiConfig = setGenericApiValues(call);

            var client = new RestClient(apiConfig[2])
            { Authenticator = new HttpBasicAuthenticator(apiConfig[0], "") };
            var request = new RestRequest($"{apiConfig[3]}{call}?api_key={apiConfig[1]}", Method.POST);

            string json = "";

            string jsonToSend = JsonHelper.ToJson(json);
            request.AddParameter("application/json; charset=utf-8", jsonToSend, ParameterType.RequestBody);
            request.RequestFormat = DataFormat.Json;



            var response = client.Execute(request);


          
            
            Console.ReadLine();





        }






        static void callAgents()
        {
            string[] apiConfig = setGenericApiValues("agents");
            //Function takes param ofive token , apiKeymbaseUrl,apiroot, action.
            string response = GetMyDesktopUsersForOffice(apiConfig[0], apiConfig[1], apiConfig[2], apiConfig[3], apiConfig[4], Method.GET);
            var data = JsonConvert.DeserializeObject<RootObject>(response).agents; // Returns a list object of all agents
            foreach (var item in data) //iterate through list of agents
            {
                Console.WriteLine("First Name: " + isNull(item.firstname));
                Console.WriteLine("Last Name: " + isNull(item.lastname));
                Console.WriteLine("Email: "+isNull(item.email));
                Console.WriteLine("Mobile: " + isNull(item.mobile));
                Console.WriteLine("ID: " + item.id);
                Console.WriteLine("Fax: "+isNull(item.fax));
                Console.WriteLine("Mobile Display: "+isNull(item.mobiledisplay));
                Console.WriteLine("Image URL: "+isNull(item.imageurl));
                Console.WriteLine("Telephone NO: "+isNull(item.telephone));
                Console.WriteLine();
               
            }
            Console.ReadLine();
        }
        private static string[] setGenericApiValues(string act) 
        {
            string officeToken = "eyJhbGciOiJIUzI1NiJ9.eyJhcGlfa2V5IjoiZjI3NzlhZDA3ZTljMmQ5MmQyYjBlZTEyZmIyZTkzM2Y5ZmRmN2Q0YyIsImFnZW50aWQiOjUwNDkyOSwidHlwZSI6Im9mZmljZSIsImdyb3VwaWQiOjI3Mjk1LCJwYXNzd29yZF9tb2RkYXRlIjoiMjAxNy0wNi0xMiAwNzoyNjoxNiJ9.15vixsCGY7mRHfbrE_QDl_Jthmhu9uRiYAXeUlPdEmE";
            string apiKey = "f2779ad07e9c2d92d2b0ee12fb2e933f9fdf7d4c";
            string baseUrl = "https://integrations.mydesktop.com.au";
            string apiRoot = "/api/v1.2/";
            string action = act;
           
            string[] config = { officeToken, apiKey, baseUrl, apiRoot, action };
            return config;
        }
        static string isNull(string x)
        {
            if (x == String.Empty)
            { return "NA"; }
            return x;
        }
        private static string GetMyDesktopUsersForOffice(string officeToken, string apiKey, string baseUrl, string apiVersionRoot, string action , Method method)
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
        }
        private static string FormatJson(string json)
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
        //JSON Response deserializaion
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
        public class Links
        {
            public object prev { get; set; }
            public object next { get; set; }
        }
        public class RootObject
        {
            public int totalRecords { get; set; }
            public List<Agent> agents { get; set; }
            public Links links { get; set; }
        }
    }
}
