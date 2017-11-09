using MostraRota.BDLocal;
using MostraRota.Interfaces;
using MostraRota.JSON;
using MostraRota.WebServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MostraRota.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Sincronizar : ContentPage
    {
        public Sincronizar()
        {
            InitializeComponent();
        }

        public async void OnSincronizar_Clicked(object sender, EventArgs e)
        {
            if (IsBusy)
                return;

            // verificar se o e-mail do usuário existe
            if (string.IsNullOrEmpty(App.usrCorrente.WSUrl))
            {
                DependencyService.Get<IMessage>().ShortAlert("Web Service ainda não foi configurado.");
                return;
            }

            // verificar se o Web Service já foi configurado
            if (string.IsNullOrEmpty(App.usrCorrente.WSUrl))
            {
                DependencyService.Get<IMessage>().ShortAlert("Web Service ainda não foi configurado.");
                return;
            }

            // verificar se tem sinal de Wi-Fi
            if (App.usrCorrente.WiFi != 0)
            {
                if (App.ExisteWiFi() == false)
                {
                    DependencyService.Get<IMessage>().ShortAlert("Sem sinal de Wi-Fi.");
                    return;
                }
            }

            // totalizadores de rotas
            int transmidas = 0;
            int recebidas = 0;
            int tErros = 0;
            int rErros = 0;

            // indica processamento em segundo plano
            IsBusy = true;

            // lista de rotas cadastradas localmente
            List<RotasBD> rotasLocal = RotasBD.GetRotas(App.usrCorrente.Email);

            // ======================================================
            // primeira parte: receber do servidor lista de rotas
            // ======================================================

            // lista das rotas cadastradas no Web Service
            List<WSRotaResumoJson> listResumo = null;

            // requisitar lista de rotas para o Web Service
            string request = "Rotas/" + App.usrCorrente.Email;
            string resposta = await WSMostraRota.GetStringAsync(request);

            // verificar se Web Service retornou dados válidos
            WSRotasJson rota;
            if ((resposta != null) && (resposta.ToLower().CompareTo("null") != 0))
            {
                // converter string retornada pelo WebService em uma lista de rotas
                listResumo = JsonConvert.DeserializeObject<List<WSRotaResumoJson>>(resposta);
                if (listResumo.Count > 0)
                {
                    // para cada rota, verificar se ela já está cadastrada na base de dados local
                    foreach (WSRotaResumoJson resumo in listResumo)
                    {
                        if (RotaExisteNoBDLocal(rotasLocal, resumo) == false)
                        {
                            // obter dados completo da rota via Web Service
                            request = "Rota?email=" + App.usrCorrente.Email + "&rota=" + resumo.Id.ToString();
                            resposta = await WSMostraRota.GetStringAsync(request);

                            // verificar se Web Service retornou dados válidos
                            if ((resposta != null) && (resposta.ToLower().CompareTo("null") != 0))
                            {
                                // desserializar dados da rota
                                rota = JsonConvert.DeserializeObject<WSRotasJson>(resposta);

                                // inserir rota na base de dados local
                                RotasBD.InsereRota(App.usrCorrente.Email, rota.DtHrIni,
                                                   rota.DtHrFim, rota.Distancia);

                                ++recebidas;
                            }
                            else
                                ++rErros;
                        }
                    }
                }
            }

            // ======================================================
            // segunda parte: enviar para o servidor
            // ======================================================
            List<CoordenadasBD> coords;
            List<WSCoordenadasJson> listaCoord;
            WSCoordenadasJson cJson;
            foreach (RotasBD rotaBD in rotasLocal)
            {
                if (RotaExisteNoWS(listResumo, rotaBD) == false)
                {
                    rota = new WSRotasJson();
                    rota.Id = rotaBD.Id;
                    rota.EmailUsuario = App.usrCorrente.Email;
                    rota.DtHrIni = rotaBD.DtHrInicial;
                    rota.DtHrFim = rotaBD.DtHrFinal;
                    rota.Distancia = rotaBD.Distancia;

                    //obtem coordenadas da rota
                    coords = CoordenadasBD.GetCoordenadas(rotaBD.Id);

                    // monta lista de coordenadas
                    listaCoord = new List<WSCoordenadasJson>();
                    foreach (CoordenadasBD c in coords)
                    {
                        cJson = new WSCoordenadasJson();
                        cJson.Id = 0;
                        cJson.DataHora = c.DataHora;
                        cJson.EmailUsr = App.usrCorrente.Email;
                        cJson.Latitute = (float)c.Latitude;
                        cJson.Longitude = (float)c.Longitude;
                        cJson.IdRota = rotaBD.Id;

                        listaCoord.Add(cJson);
                    }

                    rota.Coordenadas = listaCoord;

                    resposta = await WSMostraRota.UpdateData("Rota", rota, true);
                    if (resposta == null)
                        ++tErros;
                    else
                        ++transmidas;
                }
            }

            // fim do processamento em segundo plano
            IsBusy = false;

            // mostra mensagem mostrando totais de rotas sincronizadas
            string msg = "Rotas recebidas: " + recebidas.ToString();
            msg += "\nRotas transmitidas: " + transmidas.ToString();
            msg += "\n\nErros recepção: " + rErros.ToString();
            msg += "\nErros transmissão: " + tErros.ToString();

            await DisplayAlert("Sincronização de Rotas", msg, "OK");
        }

        private bool RotaExisteNoBDLocal(List<RotasBD> rotasLocal, WSRotaResumoJson resumoRota)
        {
            if (rotasLocal == null)
                return false;

            foreach (RotasBD rota in rotasLocal)
            {
                if (rota.DtHrInicial == resumoRota.DtHrIni)
                    return true;
            }

            return false;
        }

        private bool RotaExisteNoWS(List<WSRotaResumoJson> listResumo, RotasBD rotaBD)
        {
            if (listResumo == null)
                return false;

            foreach (WSRotaResumoJson rota in listResumo)
            {
                if (rota.DtHrIni == rotaBD.DtHrInicial)
                    return true;
            }

            return false;
        }
    }
}
