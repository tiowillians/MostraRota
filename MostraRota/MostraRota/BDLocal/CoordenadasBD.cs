using MostraRota.CustomControls;
using MostraRota.JSON;
using SQLite;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MostraRota.BDLocal
{
    [Table("coordenadas")]
    public class CoordenadasBD : IComparable
    {
        // identificação do registro
        [PrimaryKey, AutoIncrement, Column("id")]
        public int Id { get; set; }

        // identificação da rota
        [Column("id_rota")]
        public int Rota { get; set; }

        // sequência da coordenada dentro da rota
        [Column("seq")]
        public int Seq { get; set; }

        [Column("latitude")]
        public double Latitude { get; set; }

        [Column("longitude")]
        public double Longitude { get; set; }

        // data e horário
        [Column("datahora")]
        public DateTime DataHora { get; set; }

        static public List<CoordenadasBD> GetCoordenadas(int rotaId)
        {
            try
            {
                string strQuery = "SELECT * FROM [coordenadas] where [id_rota] = " + rotaId.ToString();
                List<CoordenadasBD> coords = App.BDLocal.DBConnection.Query<CoordenadasBD>(strQuery);

                // ordenar coordenadas pela sequência
                coords.Sort();

                return coords;
            }
            catch (Exception)
            {
                return null;
            }
        }

        //
        // insere lista de coordenadas na base de dados
        //
        static public bool InsereCoordenadas(int idRota, List<MyPosition>lista)
        {
            try
            {
                int seq = 0;
                CoordenadasBD nova;
                foreach(MyPosition pos in lista)
                {
                    nova = new CoordenadasBD
                    {
                        Id = 0,
                        Rota = idRota,
                        Seq = ++seq,
                        Latitude = pos.Latitude,
                        Longitude = pos.Longitude,
                        DataHora = pos.Horario
                    };

                    // insere novo registro
                    App.BDLocal.DBConnection.Insert(nova);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //
        // importar lista de coordenadas do Web Service para a base de dados
        //
        static public bool ImportarCoordenadas(int idRota, List<WSCoordenadasJson> lista)
        {
            try
            {
                int seq = 0;
                CoordenadasBD nova;
                foreach (WSCoordenadasJson coord in lista)
                {
                    nova = new CoordenadasBD
                    {
                        Id = 0,
                        Rota = idRota,  // NÃO usar IdRota do objeto 'coord'
                        Seq = ++seq,
                        Latitude = GetDouble(coord.Latitute),
                        Longitude = GetDouble(coord.Longitude),
                        DataHora = coord.DataHora
                    };

                    // insere novo registro
                    App.BDLocal.DBConnection.Insert(nova);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // apaga coordenadas de uma rota
        static public bool ApagaCoordenadas(int rotaId)
        {
            try
            {
                string query = "DELETE FROM [coordenadas] WHERE [id_rota] = " + rotaId.ToString();
                App.BDLocal.DBConnection.Execute(query);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // comparação entre dois objetos. Usados para ordenar lista
        public int CompareTo(object obj)
        {
            CoordenadasBD c = obj as CoordenadasBD;
            return this.Seq - c.Seq;
        }

        // converte string para double, considerando diferentes separadores decimal
        private static double GetDouble(string s)
        {
            double d;

            NumberFormatInfo formatinfo = new NumberFormatInfo();

            formatinfo.NumberDecimalSeparator = ".";

            if (double.TryParse(s, NumberStyles.Float, formatinfo, out d))
                return d;

            formatinfo.NumberDecimalSeparator = ",";

            if (double.TryParse(s, NumberStyles.Float, formatinfo, out d))
                return d;

            return 0;
        }
    }
}
