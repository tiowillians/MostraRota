using MostraRota.BDLocal;
using MostraRota.CustomControls;
using MostraRota.Interfaces;
using MostraRota.JSON;
using MostraRota.ViewModels;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace MostraRota.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PaginaInicial : ContentPage
    {
        private PaginaInicialViewModel viewModel;
        private bool ColetandoDadosGPS { get; set; }

        MyPosition ultPosicao;

        private DateTime dthrInicial;
        private DateTime dthrFinal;

        public PaginaInicial()
        {
            InitializeComponent();
            viewModel = new PaginaInicialViewModel();
            this.BindingContext = viewModel;

            ColetandoDadosGPS = false;
            ultPosicao = null;
        }

        private void BtnIniciarParar_Clicked(object sender, EventArgs e)
        {
            if (ColetandoDadosGPS)
            {
                // pega data/hora de encerramento da rota
                dthrFinal = DateTime.Now;

                ColetandoDadosGPS = false;
                viewModel.IniciarParar = "Iniciar";

                Task.Factory.StartNew(StopListening);

                // mostra popup para usuário gravar ou não a rota
                if (App.usrCorrente.Grava == 1)
                {
                    GravaRota();
                }
                else
                {
                    viewModel.Pergunta = "Deseja gravar a rota?";
                    viewModel.MostraPopup = true;
                    viewModel.InformaAlteracao("Pergunta");
                    viewModel.InformaAlteracao("MostraPopup");
                }
            }
            else
            {
                ColetandoDadosGPS = true;
                viewModel.IniciarParar = "Parar";
                viewModel.DistTotal = 0.0;

                mapVisualizacao.RouteCoordinates = new List<MyPosition>();
                Task.Factory.StartNew(StartListening);

                // pega data/hora de início da rota
                dthrInicial = DateTime.Now;
            }

            viewModel.InformaAlteracao("IniciarParar");
            viewModel.InformaAlteracao("DistTotal");
        }

        //
        // Inicia "escuta" do GPS
        //
        // GeoLocator Plugin Documentation:
        // https://jamesmontemagno.github.io/GeolocatorPlugin/LocationChanges.html
        //
        private async Task StartListening()
        {
            // verifica se Geolocalizador já está escutando o GPS
            if (CrossGeolocator.Current.IsListening)
                return;

            // ativa GPS
            DependencyService.Get<IDeviceSpecific>().AtivaGPS();

            // inicia escuta do GPS
            await CrossGeolocator.Current.StartListeningAsync(TimeSpan.FromSeconds(2), 2);

            // adiciona métodos para tratamento dos eventos de mudança de posição e
            // erro na obtenção das coordenadas do GPS
            CrossGeolocator.Current.PositionChanged += PositionChanged;
            CrossGeolocator.Current.PositionError += PositionError;
        }

        //
        // Evento: posição corrente do usuário foi alterada
        //
        private void PositionChanged(object sender, PositionEventArgs e)
        {
            var position = e.Position;

            // verifica se posição realmente mudou
            if (ultPosicao != null)
            {
                if ((ultPosicao.Latitude == position.Latitude) &&
                    (ultPosicao.Longitude == position.Longitude))
                    return;

            }

            MyPosition pos = new MyPosition(position.Latitude, position.Longitude, DateTime.Now);

            // cria uma nova lista a partir da lista já existente e adiciona novo elemento "pos"
            var list = new List<MyPosition>(mapVisualizacao.RouteCoordinates)
            {
                pos
            };
            mapVisualizacao.RouteCoordinates = list;

            mapVisualizacao.MoveToRegion(
                    MapSpan.FromCenterAndRadius(
                            new Xamarin.Forms.Maps.Position(pos.Latitude, pos.Longitude),
                            Distance.FromMeters(250.0)
                    )
            );

            // calcula distância entre as coordenadas
            if (list.Count > 1)
            {
                viewModel.DistTotal +=
                    DependencyService.Get<IDeviceSpecific>().CalculaDistancia(pos.Latitude, pos.Longitude,
                                                                              ultPosicao.Latitude, ultPosicao.Longitude);
                viewModel.InformaAlteracao("DistTotal");
            }

            ultPosicao = pos;
        }

        //
        // Evento: erro na obtençao da posição corrente do usuário via GPS
        //
        private void PositionError(object sender, PositionErrorEventArgs e)
        {
            // Colocar aqui evento para tratamento de erros
        }

        //
        // Encerra "escuta" do GPS
        //
        private async Task StopListening()
        {
            // verifica se geolocalizador está escutando o GPS
            if (!CrossGeolocator.Current.IsListening)
                return;

            // para de escutar o GPS
            await CrossGeolocator.Current.StopListeningAsync();

            // retira métodos para tratamento dos eventos de mudança de posição e
            // erro na obtenção das coordenadas do GPS
            CrossGeolocator.Current.PositionChanged -= PositionChanged;
            CrossGeolocator.Current.PositionError -= PositionError;
        }

        private void BtnSim_Clicked(object sender, EventArgs e)
        {
            viewModel.MostraPopup = false;
            viewModel.InformaAlteracao("MostraPopup");

            GravaRota();
        }

        private void BtnNao_Clicked(object sender, EventArgs e)
        {
            viewModel.MostraPopup = false;
            viewModel.InformaAlteracao("MostraPopup");
        }

        // grava rotas na base de dados local
        private void GravaRota()
        {
            // insere novo registro na base de dados local
            int idRota = RotasBD.InsereRota(App.usrCorrente.Email, dthrInicial,
                                            dthrFinal, (int)viewModel.DistTotal);

            // salva todas as coordenadas da lista
            CoordenadasBD.InsereCoordenadas(idRota, mapVisualizacao.RouteCoordinates);
    }
}
}