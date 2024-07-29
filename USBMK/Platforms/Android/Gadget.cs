using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USBMK.Platforms.Android
{
    internal class Gadget
    {
        internal static string ConfigFs { get; } = "/config";
        internal static string GadgetsPath { get; } = ConfigFs + "/usb_gadget";
        internal static string GadgetName { get; } = "keyboard";
        internal static string GadgetPath { get; } = GadgetsPath + "/" + GadgetName;
        internal static string ConfigPath { get; } = GadgetPath + "/configs/c.1";
        internal static string StringPath { get; } = GadgetPath + "/strings/0x409";
    }
}
