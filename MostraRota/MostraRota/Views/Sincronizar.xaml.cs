using MostraRota.BDLocal;
using MostraRota.Interfaces;
using MostraRota.JSON;
using MostraRota.ViewModels;
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
        private SincronizarViewModel viewModel;

        public Sincronizar()
        {
            InitializeComponent();
            viewModel = new SincronizarViewModel();
            this.BindingContext = viewModel;
        }

        public async void OnSincronizar_Clicked(object sender, EventArgs e)
        {
            if (viewModel.IsBusy)
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
            viewModel.IsBusy = true;

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
                    int novaRotaId;
                    foreach (WSRotaResumoJson resumo in listResumo)
                    {
                        if (RotaExisteNoBDLocal(rotasLocal, resumo) == false)
                        {
                            // obter dados completo da rota via Web Service
                            request = "Rota?email=" + App.usrCorrente.Email + "&rota=" + resumo.NumRota.ToString();
                            resposta = await WSMostraRota.GetStringAsync(request);

                            // verificar se Web Service retornou dados válidos
                            if ((resposta != null) && (resposta.ToLower().CompareTo("null") != 0))
                            {
                                // desserializar dados da rota
                                rota = JsonConvert.DeserializeObject<WSRotasJson>(resposta);

                                // não sincronizar rotas que não tenham pelo menos duas coordenadas
                                if ((rota.Coordenadas == null) ||
                                    (rota.Coordenadas.Count < 2))
                                        continue;

                                // ordenar coordenadas pela sequência
                                rota.Coordenadas.Sort();

                                // inserir rota na base de dados local
                                novaRotaId = RotasBD.InsereRota(App.usrCorrente.Email, rota.DtHrIni,
                                                                rota.DtHrFim, rota.Distancia);

                                if (novaRotaId > 0)
                                {

                                    // insere lista de coordenadas da rota na base de dados local
                                    CoordenadasBD.ImportarCoordenadas(novaRotaId, rota.Coordenadas);

                                    ++recebidas;
                                }
                                else
                                    ++rErros;
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
                    rota = new WSRotasJson
                    {
                        NumRota = rotaBD.Id,
                        EmailUsuario = App.usrCorrente.Email,
                        DtHrIni = rotaBD.DtHrInicial,
                        DtHrFim = rotaBD.DtHrFinal,
                        Distancia = rotaBD.Distancia
                    };

                    //obtem coordenadas da rota
                    coords = CoordenadasBD.GetCoordenadas(rotaBD.Id);

                    // não sincronizar rotas que não tenham pelo menos duas coordenadas
                    if ((coords == null) || (coords.Count < 2))
                        continue;

                    // monta lista de coordenadas
                    listaCoord = new List<WSCoordenadasJson>();
                    foreach (CoordenadasBD c in coords)
                    {
                        cJson = new WSCoordenadasJson
                        {
                            EmailUsr = App.usrCorrente.Email,
                            IdRota = rotaBD.Id,
                            Seq = c.Seq,
                            DataHora = c.DataHora,
                            Latitute = c.Latitude.ToString(),
                            Longitude = c.Longitude.ToString()
                        };

                        listaCoord.Add(cJson);
                    }

                    rota.Coordenadas = listaCoord;

                    resposta = await WSMostraRota.UpdateData("Rota", rota, true);
                    if ((resposta == null) || (resposta.ToLower().CompareTo("true") != 0))
                        ++tErros;
                    else
                        ++transmidas;
                }
            }

            // fim do processamento em segundo plano
            viewModel.IsBusy = false;

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
                // compara rotas pela data/hora de início
                if (rota.DtHrInicial.ToString("G").CompareTo(resumoRota.DtHrIni.ToString("G")) == 0)
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
                if (rota.DtHrIni.ToString("G").CompareTo(rotaBD.DtHrInicial.ToString("G")) == 0)
                    return true;
            }

            return false;
        }
    }
}
