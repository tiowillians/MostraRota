using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MostraRota.Interfaces
{
    public interface IDeviceSpecific
    {
        double CalculaDistancia(double startLat, double startLng, double endLat, double endLng);
        void AtivaGPS();
        void CloseApplication();
    }
}
