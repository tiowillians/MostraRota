using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MostraRota.JSON
{
    public class WSRotaResumoJson
    {
        [JsonProperty("NumRota")]
        public int NumRota { get; set; }

        [JsonProperty("DtHrIni")]
        public DateTime DtHrIni { get; set; }
    }
}
