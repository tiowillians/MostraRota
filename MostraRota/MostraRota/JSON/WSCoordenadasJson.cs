using Newtonsoft.Json;
using System;

namespace MostraRota.JSON
{
    public class WSCoordenadasJson : IComparable
    {
        [JsonProperty("EmailUsr")]
        public string EmailUsr { get; set; }

        [JsonProperty("NumRota")]
        public int IdRota { get; set; }

        [JsonProperty("Seq")]
        public int Seq { get; set; }

        [JsonProperty("Latitude")]
        public string Latitute { get; set; }

        [JsonProperty("Longitude")]
        public string Longitude { get; set; }

        [JsonProperty("DataHora")]
        public DateTime DataHora { get; set; }

        // comparação entre objetos. Usado para ordenação
        public int CompareTo(object obj)
        {
            // ordenar por sequência
            WSCoordenadasJson c = obj as WSCoordenadasJson;
            return this.Seq - c.Seq;
        }
    }
}
