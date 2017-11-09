using Android.App;
using Android.Widget;
using MostraRota.Interfaces;
using MostraRota.Droid.Implementations;

[assembly: Xamarin.Forms.Dependency(typeof(Message_Droid))]
namespace MostraRota.Droid.Implementations
{
    public class Message_Droid : IMessage
    {
        public void LongAlert(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Long).Show();
        }

        public void ShortAlert(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Short).Show();
        }
    }
}