using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace USBMK
{
    public class Settings
    {
        private static string fname= FileSystem.Current.AppDataDirectory + "/settings.json";

        

        public string VendorID { get; set; }
        public string ProductID { get; set; }
        public string Description { get; set; }
        public string Manufacturer { get; set; }
        public string Serial { get; set; }

       
        public void RestoreDefaults()
        {
            Globals.Settings.VendorID = USBMK.Resources.AppResources.VID;
            Globals.Settings.ProductID = USBMK.Resources.AppResources.PID;
            Globals.Settings.Manufacturer = USBMK.Resources.AppResources.Manufacturer;
            Globals.Settings.Description = USBMK.Resources.AppResources.Desc;
            Globals.Settings.Serial = USBMK.Resources.AppResources.Serial;
            Write();
        }

        public bool Exists()
        {
            return File.Exists(fname);
        }

        public static Settings Read()
        {
            var json = System.Text.Encoding.UTF8.GetString(File.ReadAllBytes(fname));
            return JsonSerializer.Deserialize<Settings>(json);
        }

        public void Write()
        {
            File.WriteAllBytes(fname, System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Serialize(this)));
        }

    }
}
