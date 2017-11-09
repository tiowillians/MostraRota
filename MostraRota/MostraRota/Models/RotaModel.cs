using MostraRota.CustomControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MostraRota.Models
{
    public class RotaModel
    {
        public int RotaId { get; set; }
        public string DtHrIni { get; set; }
        public string DtHrFim { get; set; }
        public string Distancia { get; set; }
        public string Pontos { get; set; }
        public List<MyPosition> Coordenadas { get; set; }

        public RotaModel()
        {
            RotaId = 0;
            DtHrIni = DtHrFim = string.Empty;
            Distancia = Pontos = string.Empty;
            Coordenadas = null;
        }

        public RotaModel(int rotaId, DateTime dtIni, DateTime dtFim,
                         int dist, List<MyPosition> list)
        {
            RotaId = rotaId;
            DtHrIni = dtIni.ToString("G");
            DtHrFim = dtFim.ToString("G");
            Distancia = dist.ToString() + " m";
            Coordenadas = new List<MyPosition>(list);

            if (list == null)
                Pontos = "0 pontos";
            else
                Pontos = list.Count.ToString() + " pontos";
        }
    }
}
