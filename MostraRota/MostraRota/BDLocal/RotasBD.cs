using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MostraRota.BDLocal
{
    [Table("rotas")]
    public class RotasBD
    {
        // identificação da rota
        [PrimaryKey, AutoIncrement, Column("id")]
        public int Id { get; set; }

        // data e horário de início da rota
        [Column("dthr_ini")]
        public DateTime DtHrInicial { get; set; }

        // data e horário de início da rota
        [Column("dthr_fim")]
        public DateTime DtHrFinal { get; set; }

        // distância total percorrida na rota (em metros)
        [Column("distancia")]
        public int Distancia { get; set; }

        // identificação do usuário que fez a rota
        [Column("email_usr")]
        public string EmailUsr { get; set; }

        static public RotasBD GetRota(int id)
        {
            try
            {
                string strQuery = "SELECT * FROM [rotas] where [id] = " + id.ToString();
                List<RotasBD> rotas = App.BDLocal.DBConnection.Query<RotasBD>(strQuery);

                if (rotas.Count == 0)
                    return null;

                return rotas[0];
            }
            catch (Exception)
            {
                return null;
            }
        }

        static public List<RotasBD> GetRotas(string usrEmail)
        {
            try
            {
                string strQuery = "SELECT * FROM [rotas] where [email_usr] = '" + usrEmail + "'";
                List<RotasBD> rotas = App.BDLocal.DBConnection.Query<RotasBD>(strQuery);

                return rotas;
            }
            catch (Exception)
            {
                return null;
            }
        }

        //
        // insere rota e retorna o número da rota inserida
        //
        static public int InsereRota(string emailUsr, DateTime dthrIni,
                                     DateTime dthrFim, int dist)
        {
            try
            {
                // cria nova rota
                RotasBD nova = new RotasBD
                {
                    Id = 0,
                    EmailUsr = emailUsr,
                    DtHrInicial = dthrIni,
                    DtHrFinal = dthrFim,
                    Distancia = dist
                };

                // insere novo registro e obtem o ID gerado pelo BD
                App.BDLocal.DBConnection.Insert(nova);
                return App.BDLocal.GetLastRowId("rotas", "id");
            }
            catch (Exception)
            {
                return -1;
            }
        }

        // apaga uma rota da base de dados
        static public bool ApagaRota(int rotaId)
        {
            try
            {
                // apaga todas as coordenadas da rota
                if (CoordenadasBD.ApagaCoordenadas(rotaId))
                {
                    string query = "DELETE FROM [rotas] WHERE [id] = " + rotaId.ToString();
                    App.BDLocal.DBConnection.Execute(query);

                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
