using USBMK.Interfaces;
using USBMK.Platforms.Android;
namespace USBMK
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

      

        private void btnKeyboard_Clicked(object sender, EventArgs e)
        {
            
            //if (!entry.IsSoftInputShowing())
            //{
            //    entry.ShowSoftInputAsync(CancellationToken.None);
            //}
            //else
            //{
            //    entry.HideSoftInputAsync(CancellationToken.None);
            //}
            IKeyboard kb = DependencyService.Get<IKeyboard>();
            kb.ShowKeyboard();
        }

        private void Switch_Toggled(object sender, ToggledEventArgs e)
        {
            USBMK.Interfaces.ISwitch sw = DependencyService.Get<USBMK.Interfaces.ISwitch>();
            sw.DoAction(e.Value);
            btnMute.IsEnabled = e.Value;
            if (e.Value)
            {                
                lblOneTap.Text = USBMK.Resources.AppResources.OneTapMessage;
                lblCursorMove.Text = USBMK.Resources.AppResources.CursorMove;
                lblLongTap.Text = USBMK.Resources.AppResources.LongTap;
                lblRightClick.Text = USBMK.Resources.AppResources.RightClick;
                lblScroll.Text = USBMK.Resources.AppResources.Scroll;
            }
            else
            {
                lblOneTap.Text = "";
                lblCursorMove.Text = "";
                lblLongTap.Text = "";
                lblRightClick.Text = "";
                lblScroll.Text = USBMK.Resources.AppResources.ToggleMessage;
            }
        }

        

        private void ContentPage_Loaded(object sender, EventArgs e)
        {
            lblOneTap.Text = "";
            lblCursorMove.Text = "";
            lblLongTap.Text = "";
            lblRightClick.Text = "";
            lblScroll.Text = USBMK.Resources.AppResources.ToggleMessage;
            btnMute.IsEnabled = false;
            if (!Root.HasRoot)
            {
                tglEnableHid.IsEnabled = false;
                btnKeyboard.IsEnabled = false;                
                lblScroll.Text = USBMK.Resources.AppResources.RootMessage; 
            }
            
        }

        private void btnSettings_Clicked(object sender, EventArgs e)
        {
            Shell.Current.GoToAsync(nameof(SettingsPage));
        }

        private void btnMute_Clicked(object sender, EventArgs e)
        {
            IMute mt = DependencyService.Get<IMute>();
            mt.DoAction(btnMute.BackgroundColor == Colors.Transparent);

            if (btnMute.BackgroundColor == Colors.Transparent)
            { 
                btnMute.BackgroundColor = Colors.Red;
            }
            else
            {
                btnMute.BackgroundColor = Colors.Transparent;
            }

        }
    }

}
