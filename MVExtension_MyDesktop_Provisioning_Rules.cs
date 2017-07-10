
using System;
using Microsoft.MetadirectoryServices;
using RestSharp;
using RestSharp.Authenticators;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Microsoft.CSharp;

namespace Mms_Metaverse
{
	/// <summary>
	/// Summary description for MVExtensionObject.
	/// </summary>
    public class MVExtensionObject : IMVSynchronization
    {
        string officeToken, apiKey, baseUrl, apiRoot, action;


       
        public MVExtensionObject()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        void IMVSynchronization.Initialize ()
        {
            //
            // TODO: Add initialization logic here
            //
        }

        void IMVSynchronization.Terminate ()
        {
            //
            // TODO: Add termination logic here
            //
        }

        void IMVSynchronization.Provision (MVEntry mventry)
        {
			//
			// TODO: Remove this throw statement if you implement this method
			//
			throw new EntryPointNotImplementedException();
        }	

        bool IMVSynchronization.ShouldDeleteFromMV (CSEntry csentry, MVEntry mventry)
        {
            //
            // TODO: Add MV deletion logic here
            //
            throw new EntryPointNotImplementedException();
        }


        public void setGenericApiValues(string action)
        {
            this.officeToken = "eyJhbGciOiJIUzI1NiJ9.eyJhcGlfa2V5IjoiZjI3NzlhZDA3ZTljMmQ5MmQyYjBlZTEyZmIyZTkzM2Y5ZmRmN2Q0YyIsImFnZW50aWQiOjUwNDkyOSwidHlwZSI6Im9mZmljZSIsImdyb3VwaWQiOjI3Mjk1LCJwYXNzd29yZF9tb2RkYXRlIjoiMjAxNy0wNi0xMiAwNzoyNjoxNiJ9.15vixsCGY7mRHfbrE_QDl_Jthmhu9uRiYAXeUlPdEmE";
            this.apiKey = "f2779ad07e9c2d92d2b0ee12fb2e933f9fdf7d4c";
            this.baseUrl = "https://integrations.mydesktop.com.au";
            this.apiRoot = "/api/v1.2/";
            this.action = action;
        }



        // TODO: Return as a list of users by parsing the JSON string response.
        // Look at "JSON Deserialisation C#" on google.
        private static string GetMyDesktopUsersForOffice(string officeToken, string apiKey, string baseUrl, string apiVersionRoot, string action)
        {
            // build request, and attach the token as a basic authentication username with a blank password
            var client = new RestClient(baseUrl)
            {
                Authenticator = new HttpBasicAuthenticator(officeToken, "")
            };

            var request = new RestRequest($"{apiVersionRoot}{action}?api_key={apiKey}", Method.GET);

            // Send the request to the API
            var response = client.Execute(request);

            // Process/Return the response
            var responseContent = response.Content.ToString();
            if (responseContent == string.Empty)
                throw new Exception("No reponse received. We expect a values to be returned");

            return FormatJson(responseContent);
            //return responseContent;
        }





        private static string FormatJson(string json)
        {
            dynamic parsedJson = JsonConvert.DeserializeObject(json);
            return JsonConvert.SerializeObject(parsedJson, Newtonsoft.Json.Formatting.Indented);
        }




        //Used for Authenticaion when working with production API
        public static string GetHashSha256(string s) 
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
    }
}
