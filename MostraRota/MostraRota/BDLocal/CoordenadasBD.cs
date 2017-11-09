using MostraRota.CustomControls;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MostraRota.BDLocal
{
    [Table("coordenadas")]
    public class CoordenadasBD
    {
        // identificação do registro
        [PrimaryKey, AutoIncrement, Column("id")]
        public int Id { get; set; }

        // identificação da rota
        [Column("id_rota")]
        public int Rota { get; set; }

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

                return coords;
            }
            catch (Exception)
            {
                return null;
            }
        }

        //
        // insere coordenada
        //
        static public bool InsereCoordenadas(int idRota, List<MyPosition>lista)
        {
            try
            {
                CoordenadasBD nova;
                foreach(MyPosition pos in lista)
                {
                    nova = new CoordenadasBD();
                    nova.Id = 0;
                    nova.Rota = idRota;
                    nova.Latitude = pos.Latitude;
                    nova.Longitude = pos.Longitude;
                    nova.DataHora = pos.Horario;

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
    }
}
