using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USBMK.Interfaces;

namespace USBMK.Platforms.Android
{
    internal class Mute : IMute
    {
        public void DoAction(bool mute)
        {
            if (MainApplication.IsOk == true && Shell.Current.CurrentPage.GetType() == typeof(MainPage))
            {
                string devpath = "";

                if (Debugger.IsAttached)
                {
                    devpath = "/storage/emulated/0/" + USBMK.Platforms.Android.Switch.volumefunction.GadgetDevName;
                }
                else
                {
                    devpath = "/dev/" + USBMK.Platforms.Android.Switch.volumefunction.GadgetDevName;
                }

                Root.Exec("echo -ne \"\\x04\" > " + devpath + ";echo -ne \"\\0\" > " + devpath);

            }
        }
    }
}
