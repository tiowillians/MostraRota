using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MostraRota.WebServices
{
    public class WSMostraRota
    {
        public static async Task<string> GetStringAsync(string request)
        {
            if (string.IsNullOrEmpty(App.usrCorrente.WSUrl))
                return null;

            try
            {
                using (var client = new HttpClient())
                {
                    string url = App.usrCorrente.WSUrl;
                    if (url.EndsWith("/") == false)
                        url += "/";
                    url += request;
                    var strDados = await client.GetStringAsync(url);
                    return strDados;
                }
            }
            catch(Exception e)
            {
                return null;
            }
        }

        public static async Task<string> UpdateData(string servico, object payload, bool inserting)
        {
            if (string.IsNullOrEmpty(App.usrCorrente.WSUrl))
                return null;

            // Serialize o conteúdo do objeto em uma string JSON
            string stringPayload;
            if (payload == null)
                stringPayload = string.Empty;
            else
                stringPayload = await Task.Run(() => JsonConvert.SerializeObject(payload));

            // Empacota a JSON dentro de um StringContent que pode ser usado pela classe HttpClient
            StringContent httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

            try
            {
                using (var httpClient = new HttpClient())
                {
                    // Faz a requisição e espera a resposta
                    string url = App.usrCorrente.WSUrl;
                    if (url.EndsWith("/") == false)
                        url += "/";
                    url += servico;
                    HttpResponseMessage httpResponse;
                    if (inserting)
                        httpResponse = await httpClient.PostAsync(url, httpContent);
                    else
                        httpResponse = await httpClient.PutAsync(url, httpContent);

                    // Se o campo Content da resposta existir, devemos lê-lo
                    string resposta = string.Empty;
                    if (httpResponse.Content != null)
                    {
                        // verifica se a operação foi bem sucedida
                        if (httpResponse.ReasonPhrase.ToLower().CompareTo("ok") != 0)
                            return null;

                        resposta = await httpResponse.Content.ReadAsStringAsync();
                    }

                    return resposta;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
