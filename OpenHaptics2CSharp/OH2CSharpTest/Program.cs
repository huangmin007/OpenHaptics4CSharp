using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OH2CSharp.HD;
using OH2CSharp.HL;
using System.Runtime.InteropServices;

namespace OH2CSharpTest
{
    
    class Program
    {
        static UInt32 hHD;

        //static readonly IntPtr HL_VENDOR = Marshal.ReadIntPtr(HLAPI.GetProcAddress(HLAPI.LoadLibrary(HLAPI.DLL_PATH), "HL_VENDOR"));

        //public const string HL_VERSION = "HL_VERSION";//0xED4CC7 0xD4E42F

        static HLError error;
        static void Main(string[] args)
        {
            #region
            /*
            hHD = HDAPI.hdInitDevice(null);
            Console.WriteLine("Test..{0}", HDAPI.hdGetString(HDGetStringParameters.HD_DEVICE_SERIAL_NUMBER));
            Console.WriteLine("Test..{0}", Marshal.PtrToStringAnsi(HDAPI.hdGetString(HDGetStringParameters.HD_DEVICE_MODEL_TYPE)));

            IntPtr hHLRC = HLAPI.hlContextDevice(hHD);
            HLAPI.hlEnable(HLCapabilityParameters.HL_PROXY_RESOLUTION);
            error = HLAPI.hlGetError();
            Console.WriteLine("ErrorCode:{0}", error.GetErrorCodeStr());
            
            String ver = "HL_VENDOR";
            IntPtr ps = HLAPI.GetVariables(ver);    // Marshal.StringToHGlobalAnsi(ver);

            IntPtr ptr = HLAPI.hlGetString(ref ps);
            if(ptr == IntPtr.Zero)
                Console.WriteLine("Zero IntPtr..");

            Console.WriteLine("BOOl::{0}", ptr);
            Console.WriteLine(Marshal.PtrToStringAnsi(ptr));

            error = HLAPI.hlGetError();
            Console.WriteLine("ErrorCode:{0}", error.GetErrorCodeStr());

            HDAPI.hdDisableDevice(hHD);
            */
            #endregion

            //IntPtr ptr = HLAPI.GetVariables("HL_VENDOR");
            //IntPtr p2 = Marshal.ReadIntPtr(ptr);        //这才是我要的常数
            //Console.WriteLine(Marshal.PtrToStringAnsi(HL_VENDOR));


            //IntPtr tr = HLAPI._hlGetString(p2);
            //IntPtr tr = HLAPI._hlGetString(HL_VENDOR);
            //Console.WriteLine(Marshal.PtrToStringAnsi(tr));


            Console.WriteLine(HLAPI.hlGetString(HLGetStringParameters.HL_VERSION));

            Console.ReadKey(); 
        }
    }
}
