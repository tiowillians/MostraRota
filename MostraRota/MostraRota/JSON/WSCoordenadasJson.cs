using Newtonsoft.Json;
using System;

namespace MostraRota.JSON
{
    public class WSCoordenadasJson
    {
        [JsonProperty("Id")]
        public int Id { get; set; }

        [JsonProperty("EmailUsr")]
        public string EmailUsr { get; set; }

        [JsonProperty("IdRota")]
        public int IdRota { get; set; }

        [JsonProperty("Latitude")]
        public float Latitute { get; set; }

        [JsonProperty("Longitude")]
        public float Longitude { get; set; }

        [JsonProperty("DataHora")]
        public DateTime DataHora { get; set; }
    }
}
