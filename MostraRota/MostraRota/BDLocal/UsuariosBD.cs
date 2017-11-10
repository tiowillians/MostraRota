using MostraRota.JSON;
using SQLite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MostraRota.BDLocal
{
    [Table("usuarios")]
    public class UsuariosBD
    {
        // email de identificação do usuário
        [PrimaryKey, Column("email")]
        public string Email { get; set; }

        // nome do usuário
        [MaxLength(80), Column("nome")]
        public string Nome { get; set; }

        // tipo do login
        [Column("login")]
        public int Login { get; set; }

        // foto do usuário
        [Column("foto")]
        public byte[] Foto { get; set; }

        // URL do Web Service de sincronização
        [Column("ws_url")]
        public string WSUrl{ get; set; }

        // indicador se gravação da rota será automática
        [Column("grava_aut")]
        public int Grava { get; set; }

        // indicador se conexão com Web Service será feita só via Wi-Fi
        [Column("so_wifi")]
        public int WiFi { get; set; }

        static public UsuariosBD GetUsuario(string email)
        {
            try
            {
                string strQuery = "SELECT * FROM [usuarios]";
                if (string.IsNullOrEmpty(email) == false)
                    strQuery += " where [email] = '" + email.Trim() + "'";
                List<UsuariosBD> usuarios = App.BDLocal.DBConnection.Query<UsuariosBD>(strQuery);

                if (usuarios.Count == 0)
                    return null;

                return usuarios[0];
            }
            catch (Exception)
            {
                return null;
            }
        }

        static public List<UsuariosBD> GetUsuarios()
        {
            try
            {
                string strQuery = "SELECT * FROM [usuarios]";
                List<UsuariosBD> usuarios = App.BDLocal.DBConnection.Query<UsuariosBD>(strQuery);
                return usuarios;
            }
            catch (Exception)
            {
                return null;
            }
        }

        static public async Task<UsuariosBD> InsereAtualizaUsuario(User usr, int tipoLogin)
        {
            if (usr == null)
                return null;

            try
            {
                // verifica se usuário já foi cadastrado
                UsuariosBD antigo = GetUsuario(usr.Email);

                // cria novo usuário
                UsuariosBD novo = new UsuariosBD();

                if (antigo == null)
                {
                    novo.Email = usr.Email;
                    novo.WSUrl = string.Empty;
                    novo.Grava = novo.WiFi = 0;
                }
                else
                {
                    novo.Email = antigo.Email;
                    novo.WSUrl = antigo.WSUrl;
                    novo.Grava = antigo.Grava;
                    novo.WiFi = antigo.WiFi;
                }

                novo.Nome = usr.Name;
                novo.Login = tipoLogin;

                // baixa foto do usuário e converte para byte[]
                novo.Foto = await Utils.GetImageByteArrayFromUrl(usr.Picture);

                if (antigo == null)
                    App.BDLocal.DBConnection.Insert(novo);
                else
                    App.BDLocal.DBConnection.Update(novo);

                return novo;
            }
            catch (Exception)
            {
                return null;
            }
        }

        static public void AtualizaConfiguracoes(string email, string url,
                                                 int grava, int wifi)
        {
            try
            {
                // verifica se usuário existe
                UsuariosBD usr = GetUsuario(email);
                if (usr == null)
                    return;

                usr.WSUrl = url;
                usr.Grava = grava;
                usr.WiFi = wifi;

                App.BDLocal.DBConnection.Update(usr);
            }
            catch (Exception)
            {
            }
        }

        // apaga usuário da base de dados
        static public bool ApagaUsuario(string eMail)
        {
            try
            {
                // apaga todas as rotas do usuário
                if (RotasBD.ApagaRotas(eMail) == false)
                    return false;

                // apaga usuário informado, se ele não for o usuário corrente
                if (eMail.CompareTo(App.usrCorrente.Email) != 0)
                {
                    string query = "DELETE FROM [usuarios] WHERE [email] = '" + eMail + "'";
                    App.BDLocal.DBConnection.Execute(query);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
