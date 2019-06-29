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

        

        public const string HL_VERSION = "HL_VERSION";//0xED4CC7 0xD4E42F

        static HLError error;
        static void Main(string[] args)
        {
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
            Console.ReadKey();

            

        }
    }
}
