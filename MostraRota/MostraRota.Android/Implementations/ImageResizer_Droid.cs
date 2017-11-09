using System;
using MostraRota.Droid.Implementations;
using MostraRota.Interfaces;
using Android.Graphics;
using System.IO;
using System.Reflection;

[assembly: Xamarin.Forms.Dependency(typeof(ImageResizer_Droid))]
namespace MostraRota.Droid.Implementations
{
    public class ImageResizer_Droid : IImageResizer
    {
        public byte[] ResizeImage(byte[] imageData, float width, float height)
        {
            // Load the bitmap
            Bitmap originalImage = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);
            Bitmap resizedImage = Bitmap.CreateScaledBitmap(originalImage, (int)width, (int)height, true);

            using (MemoryStream ms = new MemoryStream())
            {
                resizedImage.Compress(Bitmap.CompressFormat.Jpeg, 100, ms);
                return ms.ToArray();
            }
        }

        // obs.: a imagem a ser carregada deve ser adicionada ao projeto com
        // as seguintes propriedades:
        //      Build action: Embedded Resource
        public byte[] LoadFromResource(String resourceName)
        {
            Assembly asm = this.GetType().GetTypeInfo().Assembly;
            string name = "MostraRota.Android.Resources.drawable." + resourceName;
            using (var stream = asm.GetManifestResourceStream(name))
            {
                if (stream == null)
                    return null;

                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                return buffer;
            }
        }
    }
}