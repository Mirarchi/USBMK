using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USBMK.Platforms.Android
{
    internal class GadgetFunction
    {
        public static int Gadgets { get; set; } = 0;
        public string FunctionName { get; }
        public string Protocol { get; }
        public string SubClass { get; }
        public string ReportLen { get; }
        public string ReportDescriptor { get; }
        public string GadgetDevName { get; }
        public GadgetFunction(string functionname,string protocol, string subclass, string reportlen,string reportdescriptor)
        {
            FunctionName = functionname;
            Protocol = protocol;
            SubClass = subclass;
            ReportLen = reportlen;
            ReportDescriptor = reportdescriptor;            
            GadgetDevName = "hidg" + Gadgets++;
        }

        public static void AddFunction(GadgetFunction gf)
        {
            string str = "mkdir -p " + Gadget.GadgetPath + "/functions/" + gf.FunctionName + ";cd " + Gadget.GadgetPath + "/functions/" + gf.FunctionName +
                           ";echo " + gf.Protocol + " > protocol;echo " + gf.SubClass + " > subclass;echo " + gf.ReportLen + " > report_length;echo -ne \"" +
                           gf.ReportDescriptor + "\" > report_desc;ln -s " + Gadget.GadgetPath + "/functions/" + gf.FunctionName + " " + Gadget.ConfigPath + "/" + gf.FunctionName;
            Root.Exec(str);
            Gadgets++;
        }

    }
}
