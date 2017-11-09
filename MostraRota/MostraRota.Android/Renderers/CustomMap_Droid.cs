using Android.App;
using Xamarin.Forms;
using MostraRota.CustomControls;
using MostraRota.Droid.Renderers;
using Xamarin.Forms.Maps.Android;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMap_Droid))]
namespace MostraRota.Droid.Renderers
{
    public class CustomMap_Droid : MapRenderer, IOnMapReadyCallback
    {
        GoogleMap map;

        protected override void OnElementChanged(ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                // Unsubscribe
            }

            if (e.NewElement != null)
            {
                ((MapView)Control).GetMapAsync(this);
            }
        }

        protected override void OnMapReady(GoogleMap googleMap)
        {
            googleMap.UiSettings.ZoomControlsEnabled = Map.HasZoomEnabled;
            googleMap.UiSettings.ZoomGesturesEnabled = Map.HasZoomEnabled;
            googleMap.UiSettings.ScrollGesturesEnabled = Map.HasScrollEnabled;
            googleMap.MyLocationEnabled = Map.IsShowingUser;

            map = googleMap;
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == "RouteCoordinates" || e.PropertyName == "VisibleRegion")
            {
                CustomMap cMap = (CustomMap)this.Element;
                if (cMap.RouteCoordinates != null)
                {
                    var polylineOptions = new PolylineOptions();
                    polylineOptions.InvokeColor(0x66FF0000);

                    // limpa percurso
                    ((Activity)Forms.Context).RunOnUiThread(() => map.Clear());

                    // adiciona posições do percurso já percorrido
                    foreach (var position in cMap.RouteCoordinates)
                        polylineOptions.Add(new LatLng(position.Latitude, position.Longitude));

                    // desenha percurso no mapa
                    ((Activity)Forms.Context).RunOnUiThread(() => map.AddPolyline(polylineOptions));
                }
            }
        }
    }
}