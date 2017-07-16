using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RestSharp.Serializers;

namespace Mms_Metaverse
{
    public static class Helper
    {
        public static string GetHashSha256(this string s)
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

        public class RestSharpJsonNetSerializer : ISerializer
        {
            private readonly Newtonsoft.Json.JsonSerializer _serializer;

            public RestSharpJsonNetSerializer()
            {
                ContentType = "application/json";
                var serializerSettings = new JsonSerializerSettings();

                //serializerSettings.Converters.Add(new StringEnumConverter());

                _serializer = Newtonsoft.Json.JsonSerializer.Create(serializerSettings);
                _serializer.MissingMemberHandling = MissingMemberHandling.Ignore;
                _serializer.NullValueHandling = NullValueHandling.Include;
                _serializer.DefaultValueHandling = DefaultValueHandling.Include;
                EnumsToString = false;

            }

            public string Serialize(object obj)
            {
                using (var stringWriter = new StringWriter())
                {
                    using (var jsonTextWriter = new JsonTextWriter(stringWriter))
                    {
                        jsonTextWriter.Formatting = Newtonsoft.Json.Formatting.Indented;
                        jsonTextWriter.QuoteChar = '"';

                        _serializer.DateFormatString = DateFormat;

                        _serializer.NullValueHandling = NullValueHandlingType;

                        if (EnumsToString)
                            _serializer.Converters.Add(new StringEnumConverter());

                        _serializer.Serialize(jsonTextWriter, obj);

                        var result = stringWriter.ToString();

                        return result;
                    }
                }
            }

            public T Deserialize<T>(string s)
            {
                return JsonConvert.DeserializeObject<T>(s);
            }

            public NullValueHandling NullValueHandlingType { get; set; }
            public bool EnumsToString { get; set; }
            public string DateFormat { get; set; }
            public string RootElement { get; set; }
            public string Namespace { get; set; }
            public string ContentType { get; set; }
        }
    }


}
