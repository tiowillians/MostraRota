using MostraRota.Interfaces;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace MostraRota
{
    public class Utils
    {
        public static async Task<byte[]> GetImageByteArrayFromUrl(string url)
        {
            Stream imgStream = await GetStreamAsync(url);
            return ConvertToByteArray(imgStream);
        }

        public static async Task<Stream> GetStreamAsync(string url)
        {
            using (var client = new HttpClient())
            {
                var stream = await client.GetStreamAsync(url);
                return stream;
            }
        }

        public static byte[] ConvertToByteArray(Stream input)
        {
            if (input == null)
                return null;

            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }

        public static object ResizeImage(byte[] image, float width, float height)
        {
            if (image == null)
                return null;

            byte[] resizedImage =
                DependencyService.Get<IImageResizer>().ResizeImage(image, width, height);

            return ConvertToImage(resizedImage);
        }

        public static object ConvertToImage(byte[] img)
        {
            if (img != null)
            {
                Image image = new Image();
                image.Source = ImageSource.FromStream(() => new MemoryStream(img));

                return image.Source;
            }

            return null;
        }

        // calcula distância, em metros, entre duas coordenadas geográficas
        public static double CalculaDistancia(double startLat, double startLng, double endLat, double endLng)
        {
            // ângulo em radianos ao longo do grande círculo
            double radians = Math.Acos(Math.Sin(DegreesToRadians(startLat)) * Math.Sin(DegreesToRadians(endLat)) +
                                       Math.Cos(DegreesToRadians(startLat)) * Math.Cos(DegreesToRadians(endLat)) *
                                       Math.Cos(DegreesToRadians(startLng - endLng)));

            // Multiplica pelo raio da Terra para obter a distância atual
            Distance dist = Distance.FromKilometers(6371 * radians);
            return dist.Meters;
        }

        private static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }
    }
}
