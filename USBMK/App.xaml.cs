using System.Text.Json;

namespace USBMK
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            
            if (!Globals.Settings.Exists())
            {
                Globals.Settings.RestoreDefaults();
                
            }
            else
            {
                
                Globals.Settings =  Settings.Read();
            }

            //using(StreamWriter sw = new(FileSystem.Current.AppDataDirectory + "/known.lst"))
            //{
            //    sw.WriteLine("0x25a7,0xfa67,Areson,Areson Technology Corp 2.4G Receiver,0");
            //}

            MainPage = new AppShell();
        }
    }
}
