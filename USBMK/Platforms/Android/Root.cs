using Android.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USBMK.Platforms.Android
{
    public class Root
    {
        private static Java.Lang.Process RootProcess;
        private static Java.IO.DataOutputStream os;
        private static Java.IO.DataInputStream osRes;

        public static bool HasRoot { get; private set; }

        public static void Exec(string cmd)
        {
            os.WriteBytes(cmd + "\n");
            os.Flush();
        }

        public static bool Acquire()
        {
            try
            {
                HasRoot = false;
                RootProcess = Java.Lang.Runtime.GetRuntime().Exec(new string[] { "su" });
                os = new Java.IO.DataOutputStream(RootProcess.OutputStream);
                osRes = new(RootProcess.InputStream);

                
                if (null != os && null != osRes)
                {
                    Exec("id");                    
                    string currUid = osRes.ReadLine();
                    if (null == currUid)
                    {
                        Console.WriteLine("Can't get root access or denied by user");                        

                    }
                    else if (true == currUid.Contains("uid=0"))
                    {
                        Console.WriteLine("Root access granted");
                        HasRoot = true;
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Root access rejected: " + currUid);                        
                    }

                }              


            }
            catch (Java.Lang.Exception e)
            {

                Console.WriteLine("Root access rejected [" + e.Class.Name + "] : " + e.Message);
                
            }
            return false;
        }
    }
}
