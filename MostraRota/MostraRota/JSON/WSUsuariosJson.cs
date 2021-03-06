﻿using MostraRota.WebServices;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace MostraRota.JSON
{
    public class WSUsuariosJson
    {
        [JsonProperty("Email")]
        public string Email { get; set; }

        [JsonProperty("Nome")]
        public string Nome { get; set; }

        [JsonProperty("Login")]
        public int Login { get; set; }

        public static async Task<WSUsuariosJson> BuscarDadosUsuario()
        {
            if (App.usrCorrente == null)
                return null;

            // busca dados do usuário corrente no servidor
            string request = "GetUser/" + App.usrCorrente.Email;
            string json = await WSMostraRota.GetStringAsync(request);

            // verificar se Web Service retornou dados válidos
            if ((json == null) || json.ToLower().CompareTo("null") == 0)
                return null;

            WSUsuariosJson usr = JsonConvert.DeserializeObject<WSUsuariosJson>(json);
            return usr;
        }

        public static async Task<bool> UpdateData()
        {
            if (App.usrCorrente == null)
                return false;

            WSUsuariosJson usr = new WSUsuariosJson
            {
                Email = App.usrCorrente.Email,
                Nome = App.usrCorrente.Nome,
                Login = App.usrCorrente.Login
            };

            WSUsuariosJson antigo = await BuscarDadosUsuario();

            string resposta = null;
            if (antigo != null)
            {
                // envia dados para servidor, via WebService (PUT ou POST),
                // se dados do usuário tiver sido alterado
                if ((antigo.Nome.CompareTo(App.usrCorrente.Nome) != 0) ||
                    (antigo.Login != App.usrCorrente.Login))
                    resposta = await WSMostraRota.UpdateData("User", usr, false);
                else
                    return true;
            }
            else
                resposta = await WSMostraRota.UpdateData("User", usr, true);

            // verifica resposta do servidor
            if (string.IsNullOrEmpty(resposta))
                return false;
            else
                return (resposta.ToLower().CompareTo("true") == 0);
        }
    }
}
