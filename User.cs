using Newtonsoft.Json;

namespace Mms_Metaverse
{
    public class User
    {
        [JsonProperty("GoogleID")]
        public string GoogleId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Telephone { get; set; }

        [JsonIgnore]
        public int BranchId { get; set; }
    }
}
