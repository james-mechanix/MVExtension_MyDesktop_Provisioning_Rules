using System;
using Newtonsoft.Json;

namespace Mms_Metaverse
{
    public class MyDesktopUser : User
    {
        [JsonProperty("OfficeID")]
        public string OfficeId { get; set; }

        [JsonProperty("AgentID")]
        public string AgentId { get; set; }

        [JsonProperty("SecretKey")]
        public string SecretKey { get; set; }

        [JsonProperty("isSuperUser")]
        public bool IsSuperUser { get; set; }

        [JsonProperty("isDeleted")]
        public bool IsDeleted { get; set;}

        [JsonIgnore]
        public string OfficeName { get; set; }
        
    }

    public class MyDesktopUserRequest : MyDesktopUser
    {
        public DateTimeOffset CurrentTime { get; set; }
        public string AuthenticationString { get; set; }
        public string Action { get; set; }
        public int Commit { get; set; }
        

        public void PrepareForMyDesktopRequest(string apiKey, string dateFormat)
        {
            CurrentTime = DateTimeOffset.UtcNow;
            AuthenticationString = $"{CurrentTime.ToString(dateFormat)}{apiKey}{OfficeId}".GetHashSha256();
        }
    }

    public class MyDesktopUserResponse
    {
        public string AgentId { get; set; }
        public int StatusOk { get; set; }
        public string Response { get; set; }



        public string RequestUserName { get; set; }
        public int RequestOfficeId { get; set; }
    }
}


