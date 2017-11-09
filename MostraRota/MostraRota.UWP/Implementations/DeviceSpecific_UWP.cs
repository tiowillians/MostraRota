using MostraRota.Interfaces;
using MostraRota.UWP.Implementations;
using System;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml;
using Xamarin.Forms.Maps;

[assembly: Xamarin.Forms.Dependency(typeof(DeviceSpecific_UWP))]
namespace MostraRota.UWP.Implementations
{
    public class DeviceSpecific_UWP : IDeviceSpecific
    {
        public void CloseApplication()
        {
            Application.Current.Exit();
        }

        public async void AtivaGPS()
        {
            Geolocator locationservice = new Geolocator();
            if (locationservice.LocationStatus == PositionStatus.Disabled)
            {
                var accessStatus = await Geolocator.RequestAccessAsync();
            }
        }

        public double CalculaDistancia(double startLat, double startLng, double endLat, double endLng)
        {
            return Utils.CalculaDistancia(startLat, startLng, endLat, endLng);
        }
    }
}
