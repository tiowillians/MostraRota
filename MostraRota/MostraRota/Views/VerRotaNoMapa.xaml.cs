using MostraRota.CustomControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace MostraRota.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class VerRotaNoMapa : ContentPage
	{
        public VerRotaNoMapa(List<MyPosition> coordenadas)
        {
            InitializeComponent();

            // calcula retângulo envolvente da rota
            // valores mínimo e máximo da latitude: -90 .. 90
            // valores mínimo e máximo da longitude: -180 .. 180
            double minLat = 91;
            double maxLat = -91;
            double minLng = 181;
            double maxLng = -181;
            foreach (MyPosition pos in coordenadas)
            {
                if (pos.Latitude > maxLat)
                    maxLat = pos.Latitude;
                if (pos.Latitude < minLat)
                    minLat = pos.Latitude;

                if (pos.Longitude > maxLng)
                    maxLng = pos.Longitude;
                if (pos.Longitude < minLng)
                    minLng = pos.Longitude;
            }

            // coordenada do centro do retângulo envolvente
            Position centro = new Position((minLat + maxLat) / 2, (minLng + maxLng) / 2);

            // altura, em metros, do retângulo envolvente
            double altura = Utils.CalculaDistancia(minLat, minLng, maxLat, minLng);

            // largura, em metros, do retângulo envolvente
            double largura = Utils.CalculaDistancia(minLat, minLng, minLat, maxLng);

            double raio = (altura > largura ? altura : largura) * 0.6;

            // centraliza e mostra rota no pama
            mapVisualizacao.MoveToRegion(
                    MapSpan.FromCenterAndRadius(centro, Distance.FromMeters(raio)));

            // espera 2 segundos para mostrar a rota
            Device.StartTimer(TimeSpan.FromSeconds(2), () =>
            {
                mapVisualizacao.RouteCoordinates = new List<MyPosition>(coordenadas);

                // elimina temporizador
                return false;
            });
        }
    }
}
