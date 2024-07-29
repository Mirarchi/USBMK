using Android.App;
using Android.Content;
using Android.Views;
using Android.Views.InputMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USBMK.Interfaces;


namespace USBMK.Platforms.Android
{
    public class Keyboard : IKeyboard
    {
        public void HideKeyboard()
        {
            Context ctx = global::Android.App.Application.Context;
            InputMethodManager imm = ctx.GetSystemService(global::Android.Content.Context.InputMethodService) as InputMethodManager;
            imm.HideSoftInputFromWindow(null, HideSoftInputFlags.None);
        }

        public void ShowKeyboard()
        {
            Context ctx = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity.BaseContext; //global::Android.App.Application.Context;
            InputMethodManager imm = ctx.GetSystemService(global::Android.Content.Context.InputMethodService) as InputMethodManager;
            Activity act = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
            global::Android.Views.View v = act.Window.DecorView.FindViewById(global::Android.Resource.Id.Content);
            v.RequestFocus();
            imm.ToggleSoftInput(ShowFlags.Forced, HideSoftInputFlags.None);

            

            //imm.ShowSoftInput(v, ShowFlags.Implicit);
        }
    }
}
