using MostraRota.CustomControls;
using MostraRota.UWP.Renderers;
using System.Collections.Generic;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls.Maps;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.UWP;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMap_UWP))]
namespace MostraRota.UWP.Renderers
{
    public class CustomMap_UWP : MapRenderer
    {
        MapControl nativeMap;
        CustomMap formsMap;

        protected override void OnElementChanged(ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                nativeMap = Control as MapControl;
            }

            if (e.NewElement != null)
            {
                formsMap = (CustomMap)e.NewElement;
                nativeMap = Control as MapControl;
                UpdatePolyLine();
            }
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (this.Element == null || this.Control == null)
                return;

            if (e.PropertyName == CustomMap.RouteCoordinatesProperty.PropertyName)
            {
                UpdatePolyLine();
            }
        }

        private async void UpdatePolyLine()
        {
            if (formsMap != null && formsMap.RouteCoordinates.Count > 0)
            {
                List<BasicGeoposition> coordinates = new List<BasicGeoposition>();

                foreach (var position in formsMap.RouteCoordinates)
                {
                    coordinates.Add(new BasicGeoposition() { Latitude = position.Latitude, Longitude = position.Longitude });
                }

                //
                // o desenho do percurso no mapa deve ser feito pelo Thread UI
                //
                Device.BeginInvokeOnMainThread(() =>
                {
                    Geopath path = new Geopath(coordinates);
                    MapPolyline polyline = new MapPolyline
                    {
                        StrokeColor = Windows.UI.Color.FromArgb(128, 255, 0, 0),
                        StrokeThickness = 5,
                        Path = path
                    };
                    nativeMap.MapElements.Add(polyline);
                }
                );
            }
            else
                nativeMap.MapElements.Clear();
        }
    }
}
