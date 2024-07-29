using Android.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Android.Provider.ContactsContract.CommonDataKinds;

namespace USBMK.Platforms.Android
{
    internal class Switch : USBMK.Interfaces.ISwitch
    {       
        static string hidkeyboard = "hid.keyboard";
        static string hidmouse = "hid.mouse";
        static string hidvolume = "hid.consumer";
        
        //Simple keyboard report descriptor
        static string keyboard_report_descriptor =  "\\x05\\x01" +    /* USAGE_PAGE (Generic Desktop)	        */
                                                    "\\x09\\x06" +    /* USAGE (Keyboard)                       */
                                                    "\\xa1\\x01" +    /* COLLECTION (Application)               */
                                                    "\\x05\\x07" +    /*   USAGE_PAGE (Keyboard)                */
                                                    "\\x19\\xe0" +    /*   USAGE_MINIMUM (Keyboard LeftControl) */
                                                    "\\x29\\xe7" +    /*   USAGE_MAXIMUM (Keyboard Right GUI)   */
                                                    "\\x15\\x00" +    /*   LOGICAL_MINIMUM (0)                  */
                                                    "\\x25\\x01" +    /*   LOGICAL_MAXIMUM (1)                  */
                                                    "\\x75\\x01" +    /*   REPORT_SIZE (1)                      */
                                                    "\\x95\\x08" +    /*   REPORT_COUNT (8)                     */
                                                    "\\x81\\x02" +    /*   INPUT (Data,Var,Abs)                 */
                                                    "\\x95\\x01" +    /*   REPORT_COUNT (1)                     */
                                                    "\\x75\\x08" +    /*   REPORT_SIZE (8)                      */
                                                    "\\x81\\x03" +    /*   INPUT (Cnst,Var,Abs)                 */
                                                    "\\x95\\x05" +    /*   REPORT_COUNT (5)                     */
                                                    "\\x75\\x01" +    /*   REPORT_SIZE (1)                      */
                                                    "\\x05\\x08" +    /*   USAGE_PAGE (LEDs)                    */
                                                    "\\x19\\x01" +    /*   USAGE_MINIMUM (Num Lock)             */
                                                    "\\x29\\x05" +    /*   USAGE_MAXIMUM (Kana)                 */
                                                    "\\x91\\x02" +    /*   OUTPUT (Data,Var,Abs)                */
                                                    "\\x95\\x01" +    /*   REPORT_COUNT (1)                     */
                                                    "\\x75\\x03" +    /*   REPORT_SIZE (3)                      */
                                                    "\\x91\\x03" +    /*   OUTPUT (Cnst,Var,Abs)                */
                                                    "\\x95\\x06" +    /*   REPORT_COUNT (6)                     */
                                                    "\\x75\\x08" +    /*   REPORT_SIZE (8)                      */
                                                    "\\x15\\x00" +    /*   LOGICAL_MINIMUM (0)                  */
                                                    "\\x25\\x65" +    /*   LOGICAL_MAXIMUM (101)                */
                                                    "\\x05\\x07" +    /*   USAGE_PAGE (Keyboard)                */
                                                    "\\x19\\x00" +    /*   USAGE_MINIMUM (Reserved)             */
                                                    "\\x29\\x65" +    /*   USAGE_MAXIMUM (Keyboard Application) */
                                                    "\\x81\\x00" +    /*   INPUT (Data,Ary,Abs)                 */
                                                    "\\xc0"; 		  /* END_COLLECTION                         */


        public static GadgetFunction keyboardfunction = new(hidkeyboard, "1", "1", "8", keyboard_report_descriptor);

        //Simple mouse report descriptor
        static string mouse_report_descriptor = "\\x05\\x01" +  // Usage Page (Generic Desktop)
                                                "\\x09\\x02" +  // Usage (Mouse)
                                                "\\xa1\\x01" +  // Collection (Application)
                                                "\\x09\\x01" +  // Usage (Pointer)
                                                "\\xa1\\x00" +  // Collection (Physical)
                                                "\\x05\\x09" +  // Usage Page (Buttons)
                                                "\\x19\\x01" +  // Usage Minimum (Button 1)
                                                "\\x29\\x05" +  // Usage Maximum (Button 5)
                                                "\\x15\\x00" +  // Logical Minimum (0)
                                                "\\x25\\x01" +  // Logical Maximum (1)
                                                "\\x95\\x05" +  // Report Count (5 buttons)
                                                "\\x75\\x01" +  // Report Size (1 bit)
                                                "\\x81\\x02" +  // Input (Data, Variable, Absolute)
                                                "\\x95\\x01" +  // Report Count (1)
                                                "\\x75\\x03" +  // Report Size (3 bits - padding)
                                                "\\x81\\x01" +  // Input (Constant)
                                                "\\x05\\x01" +  // Usage Page (Generic Desktop)
                                                "\\x09\\x30" +  // Usage (X)
                                                "\\x09\\x31" +  // Usage (Y)
                                                "\\x09\\x38" +  // Usage (Wheel)
                                                "\\x15\\x81" +  // Logical Minimum (-127)
                                                "\\x25\\x7F" +  // Logical Maximum (127)
                                                "\\x75\\x08" +  // Report Size (8 bits)
                                                "\\x95\\x03" +  // Report Count (3 - X, Y, and Wheel)
                                                "\\x81\\x06" +  // Input (Data, Variable, Relative)
                                                "\\xc0" +       // End Collection
                                                "\\xc0";        // End Collection

        public static GadgetFunction mousefunction = new(hidmouse, "2", "1", "4", mouse_report_descriptor);

        //Simple custom report descriptor that uses the vol up and down only

        // Consumer Control Report Descriptor
        static string vol_report_descriptor =   "\\x05\\x0C" + // Usage Page (Consumer Devices)
                                                "\\x09\\x01" + // Usage (Consumer Control)
                                                "\\xA1\\x01" + // Collection (Application)
                                                "\\x15\\x00" + // Logical Minimum (0)
                                                "\\x25\\x01" + // Logical Maximum (1)
                                                "\\x75\\x01" + // Report Size (1)
                                                "\\x95\\x03" + // Report Count (3)
                                                "\\x09\\xE9" + // Usage (Volume Up)
                                                "\\x09\\xEA" + // Usage (Volume Down)
                                                "\\x09\\xE2" + // Usage (Mute)
                                                "\\x81\\x02" + // Input (Data,Var,Abs)
                                                "\\x95\\x05" + // Report Count (5) - Padding
                                                "\\x75\\x01" + // Report Size (1)
                                                "\\x81\\x03" + // Input (Cnst,Var,Abs)
                                                "\\xC0";       // End Collection

        public static GadgetFunction volumefunction = new(hidvolume, "0", "0", "1", vol_report_descriptor);

        private void CreateGadget()
        {
            //Creates the gadget in the correct directories and set the correct properties

            //string str =    "mkdir -p " + configpath + ";mkdir -p " + stringpath + ";mkdir -p " + gadgetpath + 
            //                "/functions/" + hidkeyboard + ";cd " + gadgetpath + "/functions/" + hidkeyboard + 
            //                ";echo 1 > protocol;echo 1 > subclass;echo 8 > report_length;echo -ne \"" + 
            //                keyboard_report_descriptor + "\" > report_desc;mkdir -p " + gadgetpath + "/functions/" + 
            //                hidmouse + ";cd " + gadgetpath + "/functions/" + hidmouse + 
            //                ";echo 2 > protocol;echo 1 > subclass;echo 4 > report_length;echo -ne \"" + 
            //                mouse_report_descriptor + "\" > report_desc;cd " + gadgetpath + ";echo " + Globals.Settings.VendorID + 
            //                " > idVendor;echo " + Globals.Settings.ProductID + " > idProduct;cd " + stringpath + 
            //                ";echo \"" + Globals.Settings.Manufacturer + "\" > manufacturer;echo \"" + Globals.Settings.Description + 
            //                "\" > product;echo \"" + Globals.Settings.Serial + "\" > serialnumber;cd " + configpath + 
            //                ";echo 100 > MaxPower;mkdir -p strings/0x409;echo \"Configuration\" > strings/0x409/configuration;ln -s " +
            //                gadgetpath + "/functions/hid.keyboard " + configpath + "/hid.keyboard;ln -s " + gadgetpath + 
            //                "/functions/hid.mouse " + configpath + "/hid.mouse";


            string str = "mkdir -p " + Gadget.ConfigPath + ";mkdir -p " + Gadget.StringPath + ";cd " + Gadget.GadgetPath + ";echo " + Globals.Settings.VendorID +
                            " > idVendor;echo " + Globals.Settings.ProductID + " > idProduct;cd " + Gadget.StringPath +
                            ";echo \"" + Globals.Settings.Manufacturer + "\" > manufacturer;echo \"" + Globals.Settings.Description +
                            "\" > product;echo \"" + Globals.Settings.Serial + "\" > serialnumber;cd " + Gadget.ConfigPath +
                            ";echo 100 > MaxPower;mkdir -p strings/0x409;echo \"Configuration\" > strings/0x409/configuration";


            Root.Exec(str);
            GadgetFunction.Gadgets = 0;

            GadgetFunction.AddFunction(keyboardfunction);
            GadgetFunction.AddFunction(mousefunction);
            GadgetFunction.AddFunction(volumefunction);

        }

       

        public static void ClearCapabilities()
        {
            //Clears the usb capabilities of device (when debugging via usb, the connection will be lost!)
            string str = "find " + Gadget.GadgetsPath + "  -name UDC -type f -exec sh -c 'echo \"\" >  \"$@\"' _ {} \\;";
            Root.Exec(str);
            GadgetFunction.Gadgets = 0;
        }

        private void WriteHIDCapability()
        {
            //Writes the keyboard gadget to the usb capabilities of device
            string str = "getprop sys.usb.controller > " + Gadget.GadgetPath + "/UDC";
            Root.Exec(str);            
        }


        public void DoAction(bool value)
        {
            string str = "";
            if (value)
            {
                CreateGadget();                

                ClearCapabilities();
                
                WriteHIDCapability();

                //str = "for dir in /config/usb_gadget/*/; do echo GADGET_PATH=$dir; cd $dir/configs/; echo CONFIG_PATH=\"$dir/configs/`ls -1 | head -1`/\"; cd $dir; if [ \"$?\" -ne \"0\" ]; then echo \"Error - not able to change dir to $dir... exit\"; exit 1; fi; echo UDC=$(cat UDC); find ./configs/ -type l -exec sh -c 'echo FUNCTIONS_ACTIVE=$(basename $(readlink \"$@\"))' _ {} \\;; for f in `ls -1 ./functions/`; do echo FUNCTIONS=$f; done; cd ./strings/0x409/; for vars in *; do echo ${vars}=$(cat $vars); done; echo \"=============\"; done; \n";
                //MainApplication.os.WriteBytes(str);
                //MainApplication.os.Flush();

                //while (MainApplication.istr.Available() != 0)
                //{
                //    MainApplication.istr.ReadLine();
                //}

                //Thread.Sleep(10000);
                MainApplication.IsOk = true;

                
            }
            else
            {
                ClearCapabilities();
                MainApplication.IsOk = false;
            }


        }
    }
}
