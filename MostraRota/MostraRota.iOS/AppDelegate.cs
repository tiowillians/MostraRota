
using Foundation;
using UIKit;
using Xamarin;

namespace MostraRota.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			global::Xamarin.Forms.Forms.Init();
            FormsMaps.Init();
            global::Xamarin.Auth.Presenters.XamarinIOS.AuthenticationConfiguration.Init();
            LoadApplication(new App());

			return base.FinishedLaunching(app, options);
		}
	}
}
