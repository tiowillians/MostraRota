using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MostraRota.JSON
{
    public class WSRotaResumoJson
    {
        [JsonProperty("Id")]
        public int Id { get; set; }

        [JsonProperty("DtHrIni")]
        public DateTime DtHrIni { get; set; }
    }
}
