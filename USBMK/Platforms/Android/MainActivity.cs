using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using AndroidX.Core.App;
using System.Diagnostics;
using USBMK.Platforms.Android;
using static AndroidX.ConstraintLayout.Widget.ConstraintSet.Constraint;

namespace USBMK
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        public override void OnBackPressed()
        {

            Context ctx = global::Android.App.Application.Context;
            InputMethodManager imm = ctx.GetSystemService(global::Android.Content.Context.InputMethodService) as InputMethodManager;
            imm.HideSoftInputFromWindow(null, HideSoftInputFlags.None);
            //base.OnBackPressed();
        }
        public static float refx = 0;
        public static float refy = 0;
        public static DateTime t1;
        public static DateTime t2;
        public static DateTime t3;
        public static DateTime t4;
        public static DateTime touchstart;
        public static CancellationTokenSource tCancel;
        public static bool performleftclick = false;
        public static bool istwofingers = false;
        public static bool islongpress = false;
        public static bool ispointer1up = false;

        int refpos = -1;
       
        public override bool DispatchTouchEvent(MotionEvent? e)
        {
            //System.Diagnostics.Debug.WriteLine(e.Action.ToString());
            
            if ( MainApplication.IsOk == true && Shell.Current.CurrentPage.GetType() == typeof(MainPage))
            {
                MainPage pg = Shell.Current.CurrentPage as MainPage;
                if (refpos == -1)
                {
                    

                    var androidview = ((Android.Views.View)pg.tglEnableHid.Handler.PlatformView);

                    int[] pos = new int[2];
                    androidview.GetLocationOnScreen(pos);
                    refpos = pos[1] + androidview.Height;
                }
                

                if (e.GetY() > refpos)
                {
                    if (e.Action == MotionEventActions.Down)
                    {
                        touchstart = DateTime.Now;
                        tCancel = new CancellationTokenSource();

                        Task.Run(() =>
                        {
                            while ((DateTime.Now - touchstart).TotalMilliseconds < 500)
                            {
                                Thread.Sleep(100);
                                if (tCancel.IsCancellationRequested)
                                {
                                    return;
                                }
                            }
                            islongpress = !islongpress;


                            if (islongpress)
                            {
                                pg.Dispatcher.Dispatch(() =>
                                {
                                    pg.lblLongTap.Text = USBMK.Resources.AppResources.LongTapBack;
                                    pg.lblLongTap.FontAttributes = FontAttributes.Bold;
                                });

                            }
                            else
                            {
                                pg.Dispatcher.Dispatch(() =>
                                {
                                    pg.lblLongTap.Text = USBMK.Resources.AppResources.LongTap;
                                    pg.lblLongTap.FontAttributes = FontAttributes.None;
                                });
                            }

                            Vibration.Default.Vibrate(50);

                        }, tCancel.Token);

                        refx = e.GetX();
                        refy = e.GetY();
                        t1 = DateTime.Now;
                        performleftclick = true;
                    }
                    string devpath = "";

                    if (Debugger.IsAttached)
                    {
                        devpath = "/storage/emulated/0/" + USBMK.Platforms.Android.Switch.mousefunction.GadgetDevName; 
                    }
                    else
                    {
                        devpath = "/dev/" + USBMK.Platforms.Android.Switch.mousefunction.GadgetDevName;
                    }

                    if (e.Action == MotionEventActions.Move)
                    {
                        if (ispointer1up)
                        {
                            ispointer1up = false;
                            refx = e.GetX();
                            refy = e.GetY();
                        }
                        sbyte deltax = (sbyte)(e.GetX() - refx);
                        sbyte deltay = (sbyte)(e.GetY() - refy);

                        if (Math.Abs(deltax) > 1 || Math.Abs(deltay) > 1)
                        {
                            tCancel?.Cancel();
                            touchstart = DateTime.MinValue;
                        }

                        string deltaxstring = deltax.ToString("X2");
                        string deltaystring = deltay.ToString("X2");

                        if (!islongpress)
                        {
                            if (!istwofingers)
                            {

                                Root.Exec("echo -ne \"\\0\\x" + deltaxstring + "\\x" + deltaystring + "\\0\" > " + devpath);                                

                            }
                            else
                            {                                
                                if (Math.Abs(deltay) > 1)
                                {
                                    if (deltay > 0)
                                    {
                                        Root.Exec("echo -ne \"\\0\\0\\0\\x01\" > " + devpath + ";echo -ne \"\\0\\0\\0\\0\" > " + devpath);
                                    }
                                    else
                                    {
                                        Root.Exec("echo -ne \"\\0\\0\\0\\xFF\" > " + devpath + ";echo -ne \"\\0\\0\\0\\0\" > " + devpath);
                                    }
                                }
                                                               
                            }
                        }
                        else
                        {
                            Root.Exec("echo -ne \"\\x01\\x" + deltaxstring + "\\x" + deltaystring + "\\0\" > " + devpath);                            
                        }
                    }

                    if (e.Action == MotionEventActions.Pointer2Down)
                    {
                        t3 = DateTime.Now;
                        if (!islongpress)
                        {
                            istwofingers = true;
                            tCancel?.Cancel();
                        }
                    }

                    if (e.Action == MotionEventActions.Pointer2Up || e.Action == MotionEventActions.Pointer1Up)
                    {
                        if (e.Action == MotionEventActions.Pointer1Up)
                        {
                            ispointer1up = true;
                        }
                        t4 = DateTime.Now;
                        var interval = t4 - t3;
                        performleftclick = false;
                        if (interval.TotalMilliseconds <= 200 && !islongpress)
                        {
                            Root.Exec("echo -ne \"\\x02\\0\\0\\0\" > " + devpath + ";echo -ne \"\\0\\0\\0\\0\" > " + devpath);
                            
                        }
                    }

                    if (e.Action == MotionEventActions.Up)
                    {
                        tCancel?.Cancel();
                        touchstart = DateTime.MinValue;
                        istwofingers = false;
                        t2 = DateTime.Now;
                        if (performleftclick && (t2 - t1).TotalMilliseconds <= 200)
                        {
                            Root.Exec("echo -ne \"\\x01\\0\\0\\0\" > " + devpath + ";echo -ne \"\\0\\0\\0\\0\" > " + devpath);                            
                            performleftclick = false;
                        }
                    }

                    refx = e.GetX();
                    refy = e.GetY();


                    //((MainPage)Shell.Current.CurrentPage).HandleMouseAction(new() { X = e.GetX(), Y = e.GetY(), Action = e.Action.ToString().ToLower() });
                }

            }
            return base.DispatchTouchEvent(e);
        }

        class KCode
        {            
            public MetaKeyStates Modifiers { get; }
            public List<short> HidCodes { get; } = new();
            public short HIDCode
            { 
                get
                {
                    return HidCodes[0];
                }
            }

            public KCode(MetaKeyStates modifiers, params short[] HIDCodes)
            {
                this.Modifiers = modifiers;
                this.HidCodes = new(HIDCodes);
            }

            
            public KCode(short HIDCode) : this(MetaKeyStates.None,HIDCode)
            {                
            }


        }

        //Dictionary<string, KCode> specials = new() {
        //    {"è",new ( MetaKeyStates.AltOn,0x62,0x5A,0x5B,0x5A) },
        //    {"ò",new ( MetaKeyStates.AltOn,0x62,0x5A,0x5C,0x5A) },
        //    {"ó",new ( MetaKeyStates.AltOn,0x62,0x5A,0x5C,0x5B) },
        //    {"ô",new ( MetaKeyStates.AltOn,0x62,0x5A,0x5C,0x5C) },
        //    {"õ",new ( MetaKeyStates.AltOn,0x62,0x5A,0x5C,0x5D) },
        //    {"ö",new ( MetaKeyStates.AltOn,0x62,0x5A,0x5C,0x5E) },
        //    {"œ",new ( MetaKeyStates.AltOn,0x62,0x59,0x5D,0x5E) },
        //};

        Dictionary<Keycode, KCode> keycodes = new() { 
            { Keycode.A, new (0x04) } ,
            { Keycode.B, new (0x05) } ,
            { Keycode.C, new (0x06) } ,
            { Keycode.D, new (0x07) } ,
            { Keycode.E, new (0x08) } ,
            { Keycode.F, new (0x09) } ,
            { Keycode.G, new (0x0A) } ,
            { Keycode.H, new (0x0B) } ,
            { Keycode.I, new (0x0C) } ,
            { Keycode.J, new (0x0D) } ,
            { Keycode.K, new (0x0E) } ,
            { Keycode.L, new (0x0F) } ,
            { Keycode.M, new (0x10) } ,
            { Keycode.N, new (0x11) } ,
            { Keycode.O, new (0x12) } ,
            { Keycode.P, new (0x13) } ,
            { Keycode.Q, new (0x14) } ,
            { Keycode.R, new (0x15) } ,
            { Keycode.S, new (0x16) } ,
            { Keycode.T, new (0x17) } ,
            { Keycode.U, new (0x18) } ,
            { Keycode.V, new (0x19) } ,
            { Keycode.W, new (0x1A) } ,
            { Keycode.X, new (0x1B) } ,
            { Keycode.Y, new (0x1C) } ,
            { Keycode.Z, new (0x1D) } ,
            { Keycode.Num1, new (0x1E) } ,
            { Keycode.Num2, new (0x1F) } ,
            { Keycode.Num3, new (0x20) } ,
            { Keycode.Num4, new (0x21) } ,
            { Keycode.Num5, new (0x22) } ,
            { Keycode.Num6, new (0x23) } ,
            { Keycode.Num7, new (0x24) } ,
            { Keycode.Num8, new (0x25) } ,
            { Keycode.Num9, new (0x26) } ,
            { Keycode.Num0, new (0x27) } ,
            { Keycode.Enter, new(0x28) } ,
            { Keycode.Escape, new(0x29) } ,
            { Keycode.Del, new(0x2A) } ,
            { Keycode.Tab, new(0x2B) } ,
            { Keycode.Space, new(0x2C) } ,
            { Keycode.Minus, new(0x2D) } ,
            { Keycode.Equals, new(0x2E) } ,
            { Keycode.LeftBracket, new(0x2F) } ,
            { Keycode.RightBracket, new(0x30) } ,
            { Keycode.Backslash, new(0x31) } ,
            //{ Keycode., 0x32 } ,
            { Keycode.Semicolon, new(0x33) } ,
            { Keycode.Apostrophe, new(0x34) } ,
            { Keycode.Grave, new(0x35) } ,
            { Keycode.Comma, new(0x36) } ,
            { Keycode.Period, new(0x37) } ,
            { Keycode.Slash, new(0x38) } ,
            { Keycode.CapsLock, new(0x39) } ,
            { Keycode.F1,new(0x3A)},
            { Keycode.F2,new(0x3B)},
            { Keycode.F3,new(0x3C)},
            { Keycode.F4,new(0x3D)},
            { Keycode.F5,new(0x3E)},
            { Keycode.F6,new(0x3F)},
            { Keycode.F7,new(0x40)},
            { Keycode.F8,new(0x41)},
            { Keycode.F9,new(0x42)},
            { Keycode.F10,new(0x43)},
            { Keycode.F11,new(0x44)},
            { Keycode.F12,new(0x45)},
            { Keycode.Sysrq,new(0x46)},
            { Keycode.ScrollLock,new(0x47)},
            { Keycode.Break,new(0x48)},
            { Keycode.Insert,new(0x49)},
            { Keycode.MoveHome,new(0x4A)},
            { Keycode.PageUp,new(0x4B)},
            { Keycode.ForwardDel,new(0x4C)},
            { Keycode.MoveEnd,new(0x4D)},
            { Keycode.PageDown,new(0x4E)},
            { Keycode.DpadRight,new(0x4F)},
            { Keycode.DpadLeft,new(0x50)},
            { Keycode.DpadDown,new(0x51)},
            { Keycode.DpadUp,new(0x52)},
            { Keycode.NumLock,new(0x53)},
            { Keycode.NumpadDivide,new(0x54)},
            { Keycode.NumpadMultiply,new(0x55)},
            { Keycode.NumpadSubtract,new(0x56)},
            { Keycode.NumpadAdd,new(0x57)},
            { Keycode.NumpadEnter,new(0x58)},
            { Keycode.Numpad1,new(0x59)},
            { Keycode.Numpad2,new(0x5A)},
            { Keycode.Numpad3,new(0x5B)},
            { Keycode.Numpad4,new(0x5C)},
            { Keycode.Numpad5,new(0x5D)},
            { Keycode.Numpad6,new(0x5E)},
            { Keycode.Numpad7,new(0x5F)},
            { Keycode.Numpad8,new(0x60)},
            { Keycode.Numpad9,new(0x61)},
            { Keycode.Numpad0,new(0x62)},
            { Keycode.NumpadDot,new(0x63)},
            //{ Keycode.,0x64},
            //{ Keycode.,0x65},
            { Keycode.Power,new(0x66)},
            { Keycode.NumpadEquals,new(0x67)}, 
            //{ Keycode.,0x68},            
            //{ Keycode.,0x69},            
            //{ Keycode.,0x6A},            
            //{ Keycode.,0x6B},            
            //{ Keycode.,0x6C},            
            //{ Keycode.,0x6D},            
            //{ Keycode.,0x6E},            
            //{ Keycode.,0x6F},            
            //{ Keycode.,0x70},            
            //{ Keycode.,0x71},            
            //{ Keycode.,0x72},            
            //{ Keycode.,0x73},            
            //{ Keycode,0x74},            
            { Keycode.Help,new(0x75)},            
            { Keycode.Menu,new(0x76)},            
            //{ Keycode.,0x77},            
            //{ Keycode.MediaStop,0x78},            
            //{ Keycode.MediaRewind,0x79},            
            //{ Keycode.,0x7A},            
            { Keycode.Cut,new(0x7B)},
            { Keycode.Copy,new(0x7C)},
            { Keycode.Paste,new(0x7D)},
            { Keycode.Search,new(0x7E)},
            { Keycode.VolumeMute,new(0x7F)},
            //{ Keycode.VolumeUp,new(0x80)},
            //{ Keycode.VolumeDown,new(0x81)},
            //{ Keycode.CapsLock,0x82},            
            //{ Keycode.NumLock,0x83},            
            //{ Keycode.ScrollLock,0x84},             
            { Keycode.NumpadComma,new(0x85)},
            //{ Keycode.NumpadEquals,0x86},
            { Keycode.Ro,new(0x87)},
            { Keycode.KatakanaHiragana,new(0x88)},
            { Keycode.Yen,new(0x89)},
            { Keycode.Henkan,new(0x8A)},
            { Keycode.Muhenkan,new(0x8B)},
            //{ Keycode.,0x8C},
            //{ Keycode.,0x8D},
            //{ Keycode.,0x8E},
            //{ Keycode.,0x8F},
            //{ Keycode.LanguageSwitch,0x90},
            //{ Keycode.,0x91},
            //{ Keycode.,0x92},
            //{ Keycode.,0x93},
            { Keycode.ZenkakuHankaku,new(0x94)},
            //{ Keycode.,0x95},
            //{ Keycode.,0x96},
            //{ Keycode.,0x97},
            //{ Keycode.,0x98},
            //{ Keycode.,0x99},
            //{ Keycode.,0x9A},
            //{ Keycode.,0x9B},
            //{ Keycode.,0x9C},
            //{ Keycode.,0x9D},
            //{ Keycode.,0x9E},
            //{ Keycode.,0x9F},
            //{ Keycode.,0xA0},
            //{ Keycode.,0xA1},
            //{ Keycode.,0xA2},
            //{ Keycode.,0xA3},
            //{ Keycode.,0xA4},
            //{ Keycode.,0xB0},
            //{ Keycode.,0xB1},
            //{ Keycode.,0xB2},
            //{ Keycode.,0xB3},
            //{ Keycode.,0xB4},
            //{ Keycode.,0xB5},
            { Keycode.NumpadLeftParen,new(0xB6)},
            { Keycode.NumpadRightParen,new(0xB7)},
            //{ Keycode.,0xB8},
            //{ Keycode.,0xB9},
            //{ Keycode.,0xBA},
            //{ Keycode.,0xBB},
            //{ Keycode.,0xBC},
            //{ Keycode.,0xBD},
            //{ Keycode.,0xBE},
            //{ Keycode.,0xBF},
            //{ Keycode.,0xC0},
            //{ Keycode.,0xC1},
            //{ Keycode.,0xC2},
            //{ Keycode.,0xC3},
            //{ Keycode.,0xC4},
            //{ Keycode.,0xC5},
            //{ Keycode.,0xC6},
            //{ Keycode.,0xC7},
            //{ Keycode.,0xC8},
            //{ Keycode.,0xC9},
            //{ Keycode.,0xCA},
            //{ Keycode.,0xCB},
            //{ Keycode.,0xCC},
            //{ Keycode.,0xCD},
            //{ Keycode.,0xCE},
            //{ Keycode.,0xCF},
            //{ Keycode.,0xD0},
            //{ Keycode.,0xD1},
            //{ Keycode.,0xD2},
            //{ Keycode.,0xD3},
            //{ Keycode.,0xD4},
            //{ Keycode.,0xD5},
            //{ Keycode.,0xD6},
            //{ Keycode.,0xD7},
            //{ Keycode.,0xD8},
            //{ Keycode.,0xD9},
            //{ Keycode.,0xDA},
            //{ Keycode.,0xDB},
            //{ Keycode.,0xDC},
            //{ Keycode.,0xDD},
            { Keycode.CtrlLeft,new(0xE0)},
            { Keycode.ShiftLeft,new(0xE1)},
            { Keycode.AltLeft,new(0xE2)},
            { Keycode.MetaLeft,new(0xE3)},
            { Keycode.CtrlRight,new(0xE4)},
            { Keycode.ShiftRight,new(0xE5)},
            { Keycode.AltRight,new(0xE6)},
            { Keycode.MetaRight,new(0xE7)},
            { Keycode.MediaPlayPause,new(0xE8)},
            { Keycode.MediaStop,new(0xE9)},
            { Keycode.MediaPrevious,new(0xEA)},
            { Keycode.MediaNext,new(0xEB)},
            { Keycode.MediaEject,new(0xEC)},
            //{ Keycode.,0xED},
            //{ Keycode.,0xEE},
            //{ Keycode.,0xEF},
            //{ Keycode.,0xF0},
            //{ Keycode.,0xF1},
            //{ Keycode.,0xF2},
            //{ Keycode.,0xF3},
            //{ Keycode.,0xF4},
            //{ Keycode.,0xF5},
            //{ Keycode.,0xF6},
            //{ Keycode.,0xF7},
            //{ Keycode.,0xF8},
            //{ Keycode.,0xF9},
            //{ Keycode.,0xFA},
            { Keycode.Calculator,new(0xFB)},
            { Keycode.At, new(MetaKeyStates.ShiftOn,0x1F) },
            { Keycode.Pound, new(MetaKeyStates.ShiftOn,0x20) },
            { Keycode.Star, new(MetaKeyStates.ShiftOn,0x25) },



        };


        byte mod = 0;
        public override bool DispatchKeyEvent(KeyEvent? e)
        {

            //System.Diagnostics.Debug.WriteLine(string.Format("{0} -> {1} -> {2}", e.Modifiers.ToString() , e.KeyCode.ToString(), e.Action.ToString()));
            //var app = "x" + specials[e.Characters].HIDCode.ToString("X2");
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
                                
                List<string> str = new();
                KCode kcode = null;
                if (e.Action == KeyEventActions.Down)
                {

                    if (e.KeyCode == Keycode.VolumeUp)
                    {
                        Root.Exec("echo -ne \"\\x01\" > " + devpath + ";echo -ne \"\\0\" > " + devpath);
                        return true;
                    }

                    if (e.KeyCode == Keycode.VolumeDown)
                    {
                        Root.Exec("echo -ne \"\\x02\" > " + devpath + ";echo -ne \"\\0\" > " + devpath);
                        return true;
                    }

                    if (keycodes.ContainsKey(e.KeyCode))
                    {
                        str.Add("\\x" + keycodes[e.KeyCode].HIDCode.ToString("X2") + "\\0\\0\\0\\0\\0");
                        str.Add("\\0\\0\\0\\0\\0\\0");
                        kcode = keycodes[e.KeyCode];
                    }


                    mod = 0;
                    if ((e.Modifiers & MetaKeyStates.CtrlOn)!=0 || (kcode.Modifiers & MetaKeyStates.CtrlOn) != 0)
                    {
                        mod += 0x01;
                    }

                    //    //if ((e.Modifiers & MetaKeyStates.CtrlRightOn) != 0)
                    //    //{
                    //    //    mod = "x010";
                    //    //}

                        if ((e.Modifiers & MetaKeyStates.ShiftOn) != 0 || (kcode.Modifiers & MetaKeyStates.ShiftOn) != 0)
                        {
                            mod += 0x02;
                        }

                    //    //if ((e.Modifiers & MetaKeyStates.ShiftRightOn) != 0)
                    //    //{
                    //    //    mod = "x20";
                    //    //}

                        if ((e.Modifiers & MetaKeyStates.AltOn) != 0 || (kcode.Modifiers & MetaKeyStates.AltOn) != 0)
                        {
                            mod += 0x04;
                        }

                    //    if ((e.Modifiers & MetaKeyStates.AltRightOn) != 0 || (kcode.Modifiers & MetaKeyStates.AltRightOn) != 0)
                    //    {
                    //        mod = "x40";
                    //    }

                        if ((e.Modifiers & MetaKeyStates.MetaOn) != 0 || (kcode.Modifiers & MetaKeyStates.MetaOn) != 0)
                        {
                            mod += 0x08;
                        }

                    //    //if ((e.Modifiers & MetaKeyStates.MetaRightOn) != 0)
                    //    //{
                    //    //    mod = "x80";
                    //    //}



                }

                if (e.Action == KeyEventActions.Up)
                {
                    if (keycodes.ContainsKey(e.KeyCode))
                    {
                        str.Add("\\0\\0\\0\\0\\0\\0");
                        kcode = keycodes[e.KeyCode];
                    }

                    if (e.KeyCode == Keycode.CtrlLeft && mod >= 0x01)
                    {
                        mod -= 0x01;
                    }

                    if (e.KeyCode == Keycode.ShiftLeft && mod >= 0x02)
                    {
                        mod -= 0x02;
                    }

                    if (e.KeyCode == Keycode.AltLeft && mod>=0x04)
                    {
                        mod -= 0x04;
                    }

                    if (e.KeyCode == Keycode.MetaLeft && mod >= 0x08)
                    {
                        mod -= 0x08;
                    }



                }

                if (kcode != null && str.Count > 0)
                {
                    if (Debugger.IsAttached)
                    {
                        devpath = "/storage/emulated/0/" + USBMK.Platforms.Android.Switch.keyboardfunction.GadgetDevName; 
                    }
                    else
                    {
                        devpath = "/dev/" + USBMK.Platforms.Android.Switch.keyboardfunction.GadgetDevName;
                    }

                    for (int i = 0; i < str.Count; i++)
                    {
                        string ex = "echo -ne \"\\x" + mod.ToString("X2") + "\\0" + str[i] + "\" > " + devpath;
                        Root.Exec(ex);
                        //System.Diagnostics.Debug.WriteLine(ex);
                    }

                }

                //if (e.KeyCode == Keycode.Unknown)
                //{
                ////    //    str = "x" + System.Text.Encoding.UTF8.GetBytes(e.Characters)[0].ToString("X2");
                //    if (specials.ContainsKey(e.Characters))
                //    {
                //        kcode = specials[e.Characters];
                //        //string[] arr = new string[] { "\\0", "\\0", "\\0", "\\0", "\\0", "\\0", "\\0", "\\0" };
                //        for (int i = 0; i < kcode.HidCodes.Count; i++)
                //        {
                //            //arr[i] = "\\x" + kcode.HidCodes[i].ToString("X2");
                //            str.Add("\\x" + kcode.HidCodes[i].ToString("X2") + "\\0\\0\\0\\0\\0");
                //        }
                //        //str.Add(string.Join("", arr));

                //    }

                //}

                //if (kcode != null && str.Count >0)
                //{
                //     //"/storage/emulated/0/hidg0"; 

                //    if (Debugger.IsAttached)
                //    {
                //        devpath = "/storage/emulated/0/" + USBMK.Platforms.Android.Switch.keyboardfunction.GadgetDevName; 
                //    }
                //    else
                //    {
                //        devpath = "/dev/" + USBMK.Platforms.Android.Switch.keyboardfunction.GadgetDevName;
                //    }

                //    string mod = "0";



                //    if ((e.Modifiers & MetaKeyStates.CtrlOn)!=0 || (kcode.Modifiers & MetaKeyStates.CtrlOn) != 0)
                //    {
                //        mod = "x01";
                //    }

                //    //if ((e.Modifiers & MetaKeyStates.CtrlRightOn) != 0)
                //    //{
                //    //    mod = "x010";
                //    //}

                //    if ((e.Modifiers & MetaKeyStates.ShiftOn) != 0 || (kcode.Modifiers & MetaKeyStates.ShiftOn) != 0)
                //    {
                //        mod = "x02";
                //    }

                //    //if ((e.Modifiers & MetaKeyStates.ShiftRightOn) != 0)
                //    //{
                //    //    mod = "x20";
                //    //}

                //    if ((e.Modifiers & MetaKeyStates.AltOn) != 0 || (kcode.Modifiers & MetaKeyStates.AltOn) != 0)
                //    {
                //        mod = "x04";
                //    }

                //    if ((e.Modifiers & MetaKeyStates.AltRightOn) != 0 || (kcode.Modifiers & MetaKeyStates.AltRightOn) != 0)
                //    {
                //        mod = "x40";
                //    }

                //    if ((e.Modifiers & MetaKeyStates.MetaOn) != 0 || (kcode.Modifiers & MetaKeyStates.MetaOn) != 0)
                //    {
                //        mod = "x08";
                //    }

                //    //if ((e.Modifiers & MetaKeyStates.MetaRightOn) != 0)
                //    //{
                //    //    mod = "x80";
                //    //}



                //    //Root.Exec("echo -ne \"\\" + mod + "\\0" + str + "\" > " + devpath + ";echo -ne \"\\0\\0\\0\\0\\0\\0\\0\\0\" > " + devpath);                                        

                //    for (int i = 0; i < str.Count; i++)
                //    {
                //        Root.Exec("echo -ne \"\\" + mod + "\\0" + str[i] + "\" > " + devpath + ";echo -ne \"\\" + mod + "\\0\\0\\0\\0\\0\\0\\0\" > " + devpath);
                //    }
                //    Root.Exec("echo -ne \"\\0\\0\\0\\0\\0\\0\\0\\0\" > " + devpath);


                //}


                return true;
               
            }
            return base.DispatchKeyEvent(e);
        }

    }
}
