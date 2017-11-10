using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MostraRota.JSON
{
    public class WSRotasJson
    {
        [JsonProperty("EmailUsuario")]
        public string EmailUsuario { get; set; }

        [JsonProperty("NumRota")]
        public int NumRota { get; set; }

        [JsonProperty("DtHrIni")]
        public DateTime DtHrIni { get; set; }

        [JsonProperty("DtHrFim")]
        public DateTime DtHrFim { get; set; }

        [JsonProperty("Distancia")]
        public int Distancia { get; set; }

        [JsonProperty("Coordenadas")]
        public List<WSCoordenadasJson> Coordenadas { get; set; }
    }
}
