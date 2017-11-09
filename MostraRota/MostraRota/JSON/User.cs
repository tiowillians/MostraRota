using Newtonsoft.Json;

namespace MostraRota.JSON
{
    [JsonObject]
    public class User
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("picture")]
        public string Picture { get; set; }
    }
}
