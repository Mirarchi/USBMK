using Android.App;
using Android.Runtime;
using Android.Widget;
using System.Diagnostics;
using System.Text;
using USBMK.Interfaces;
using USBMK.Platforms.Android;

namespace USBMK
{
    [Application]
    public class MainApplication : MauiApplication
    {
        
        static bool isok = false;
        public static bool IsOk 
        {  
            get
            {
                return isok;
            }
            set
            {
                isok = value;
                if (value == false)
                {
                    MainActivity.refx = 0;
                    MainActivity.refy = 0;
                    MainActivity.t1 = DateTime.MinValue;
                    MainActivity.t2 = DateTime.MinValue;
                    MainActivity.t3 = DateTime.MinValue;
                    MainActivity.t4 = DateTime.MinValue;
                    MainActivity.touchstart = DateTime.MinValue;
                    MainActivity.performleftclick = false;
                    MainActivity.istwofingers = false;
                    MainActivity.islongpress = false;
                    MainActivity.ispointer1up = false;

                }
            }
        }
        
        
        
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
            Toast t = Toast.MakeText(Android.App.Application.Context, USBMK.Resources.AppResources.RootMessage, ToastLength.Short);
            if (!Root.Acquire())
            {
                t.Show();
            }
            else
            {
                if (!Debugger.IsAttached)
                {
                    USBMK.Platforms.Android.Switch.ClearCapabilities();
                }
                DependencyService.Register<USBMK.Interfaces.IKeyboard, USBMK.Platforms.Android.Keyboard>();
                DependencyService.Register<USBMK.Interfaces.ISwitch, USBMK.Platforms.Android.Switch>();
                DependencyService.Register<USBMK.Interfaces.IMute, USBMK.Platforms.Android.Mute>();
            }
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}
