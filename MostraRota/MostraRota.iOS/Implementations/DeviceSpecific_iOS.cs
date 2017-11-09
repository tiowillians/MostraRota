using System;
using Foundation;
using UIKit;
using Xamarin.Forms;
using MostraRota.iOS.Implementations;
using CoreLocation;
using Xamarin.Forms.Maps;
using MostraRota.Interfaces;
using System.Threading;

[assembly: Dependency(typeof(DeviceSpecific_iOS))]
namespace MostraRota.iOS.Implementations
{
    public class DeviceSpecific_iOS : IDeviceSpecific
    {
        public void CloseApplication()
        {
            Thread.CurrentThread.Abort();
        }

        public void AtivaGPS()
        {
            // a chave "NSLocationWhenInUseUsageDescription" deve, obrigatoriamente, estar em Info.plist
            if (CLLocationManager.Status == CLAuthorizationStatus.Denied)
            {
                if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
                {
                    NSString settingsString = UIApplication.OpenSettingsUrlString;
                    NSUrl url = new NSUrl(settingsString);
                    UIApplication.SharedApplication.OpenUrl(url);
                }
            }
        }

        public double CalculaDistancia(double startLat, double startLng, double endLat, double endLng)
        {
            return Utils.CalculaDistancia(startLat, startLng, endLat, endLng);
        }
    }
}
