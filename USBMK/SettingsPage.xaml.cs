using System.Text.Json;
using static USBMK.SettingsPage;

namespace USBMK;

public partial class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		InitializeComponent();
	}

    public class Known
    {
        public string ID { get; }
        public string VID { get; }
        public string PID { get; }
        public string Manufacturer { get; }
        public string Description { get; }
        public string Serial { get; }

        public Known(string vid, string pid, string manufacturer, string description, string serial) : this(vid,pid,manufacturer,description,serial,description)
        {

        }

        public Known(string vid, string pid, string manufacturer, string description, string serial,string id)
        {
            VID = vid;
            PID = pid;
            Manufacturer = manufacturer;
            Description = description;
            Serial = serial;
            ID = id;
        }
    }
    List<Known> lst = new() 
    { 
    new("0x25a7","0xfa67","Areson","Areson Technology Corp 2.4G Receiver","0"),
    new(Globals.Settings.VendorID, Globals.Settings.ProductID,Globals.Settings.Manufacturer,Globals.Settings.Description,Globals.Settings.Serial,USBMK.Resources.AppResources.Custom)
    };
    private void ContentPage_Loaded(object sender, EventArgs e)
    {
		//txtVendorID.Text = Globals.Settings.VendorID;
		//txtProductID.Text = Globals.Settings.ProductID;
        txtManufacturer.Text = Globals.Settings.Manufacturer;
  //      txtDescription.Text = Globals.Settings.Description;
        txtSerial.Text = Globals.Settings.Serial;

        

        //if(File.Exists(FileSystem.Current.AppDataDirectory + "/known.lst"))
        //{
        //    using (StreamReader sr = new(FileSystem.Current.AppDataDirectory + "/known.lst"))
        //    {
        //        while (!sr.EndOfStream)
        //        {
        //            string[] str = sr.ReadLine().Split(",");
        //            lst.Add(new(str[0], str[1], str[2], str[3], str[4], str[3]));
        //        }
        //    }
        //}

        
        
        cmbKnown.ItemsSource = lst;
        cmbKnown.ItemDisplayBinding = new Binding(nameof(Known.ID));

        Known match = lst.Where(x => x.VID == Globals.Settings.VendorID && x.PID == Globals.Settings.ProductID).FirstOrDefault();

        if (match != null)
        {
            cmbKnown.SelectedItem = match;
        }
        else
        {
            cmbKnown.SelectedItem = lst.LastOrDefault();
        }
               



    }

    private void btnSave_Clicked(object sender, EventArgs e)
    {
        
        Globals.Settings.VendorID = txtVendorID.Text;
        Globals.Settings.ProductID = txtProductID.Text;
        Globals.Settings.Manufacturer = txtManufacturer.Text;
        Globals.Settings.Description = txtDescription.Text;
        Globals.Settings.Serial = txtSerial.Text;
        Globals.Settings.Write();
        Shell.Current.SendBackButtonPressed();
    }

    private async void btnRestore_Clicked(object sender, EventArgs e)
    {
        bool res = await DisplayAlert(USBMK.Resources.AppResources.Warning, USBMK.Resources.AppResources.RestoreMessage, USBMK.Resources.AppResources.Yes, USBMK.Resources.AppResources.No);
        if (res)
        {            
            cmbKnown.SelectedItem = lst.LastOrDefault();
            txtVendorID.Text = USBMK.Resources.AppResources.VID;
            txtProductID.Text = USBMK.Resources.AppResources.PID;
            txtManufacturer.Text = USBMK.Resources.AppResources.Manufacturer;
            txtDescription.Text = USBMK.Resources.AppResources.Desc;
            txtSerial.Text = USBMK.Resources.AppResources.Serial;
        }
    }

    private void cmbKnown_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (cmbKnown.SelectedIndex != -1) 
        { 
            Known k = (Known)cmbKnown.SelectedItem;
            txtVendorID.Text = k.VID;
            txtProductID.Text = k.PID;
            txtDescription.Text= k.Description;
            txtManufacturer.Text  = k.Manufacturer;
            txtSerial.Text= k.Serial;
            txtVendorID.IsEnabled = cmbKnown.SelectedItem == lst.Last() ? true : false;
            txtProductID.IsEnabled = cmbKnown.SelectedItem == lst.Last() ? true : false;            
        }
    }
}