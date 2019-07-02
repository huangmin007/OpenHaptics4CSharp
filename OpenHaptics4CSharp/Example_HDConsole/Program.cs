#define 直接获取坐标位置输出
//#define 使用调度器同步输出位置

using OH4CSharp.HD;
using OH4CSharp.Utilities;
using System;
using System.Threading;

namespace OH4CSharp2HDConsole
{
    class Program
    {
        static uint hHD;
        static HDErrorInfo error;

        static int supportedCalibrationStyles;
        static HDSchedulerCallback PositionSynchronous;

        static void Main(string[] args)
        {
            hHD = HDAPI.hdInitDevice(null);
            error = HDAPI.hdGetError();

            if(error.CheckedError())
            {
                Console.WriteLine("Init Error.");
                Console.ReadKey();
                return;
            }

            //获取支持的校准样式，因为一些设备可能支持多种类型的校准
            HDAPI.hdGetIntegerv(HDGetParameters.HD_CALIBRATION_STYLE, ref supportedCalibrationStyles);
            Console.WriteLine("supportedCalibrationStyles:{0} {1}", supportedCalibrationStyles, 
                Enum.GetName(typeof(HDCalibrationStyles), supportedCalibrationStyles));

            //使用墨水池校准
            HDAPI.hdUpdateCalibration(HDCalibrationStyles.HD_CALIBRATION_INKWELL);
            //启动 Servo Loop
            HDAPI.hdStartScheduler();

#if 直接获取坐标位置输出
            double[] pPosition = new double[3];
            int buttons = 0;
            while(true)
            {
                if(HDAPI.hdCheckCalibration() == HDCalibrationCodes.HD_CALIBRATION_NEEDS_UPDATE)
                   HDAPI.hdUpdateCalibration(HDCalibrationStyles.HD_CALIBRATION_INKWELL);

                HDAPI.hdBeginFrame(hHD);
                HDAPI.hdGetDoublev(HDGetParameters.HD_CURRENT_POSITION, pPosition);
                HDAPI.hdGetIntegerv(HDGetParameters.HD_CURRENT_BUTTONS, ref buttons);
                HDAPI.hdEndFrame(hHD);

                Console.WriteLine("Button Status:Btn1:{0}  Btn2:{1}  Btn3:{2}  Btn4:{3}", 
                    buttons & (int)HDButtonMasks.HD_DEVICE_BUTTON_1,
                    buttons & (int)HDButtonMasks.HD_DEVICE_BUTTON_2,
                    buttons & (int)HDButtonMasks.HD_DEVICE_BUTTON_3,
                    buttons & (int)HDButtonMasks.HD_DEVICE_BUTTON_4);
                Console.WriteLine("Position: X:{0}  Y:{1}   Z:{2}", pPosition[0], pPosition[1], pPosition[2]);

                Thread.Sleep(100);
            }
#endif

#if 使用调度器同步输出位置
            PositionSynchronous = GetSyncPos;
            Vector3D v3 = new Vector3D();
            v3.HHD = hHD;

            IntPtr pPosition = OHUtils.StructToIntPtr(v3);
            while (true)
            {
                HDAPI.hdScheduleSynchronous(PositionSynchronous, pPosition, HDSchedulerPriority.HD_DEFAULT_SCHEDULER_PRIORITY);
                v3 = OHUtils.IntPtrToStruct<Vector3D>(pPosition);
                Console.WriteLine("Position: X:{0}  Y:{1}   Z:{2}", v3.X, v3.Y, v3.Z);

                //Sleep一下，不然打印的数据太多，看不清，实际项目是不需要的
                //Thread.Sleep(100);
            }
#endif
            //停止 Servo Loop
            HDAPI.hdStopScheduler();
            //禁用设备
            HDAPI.hdDisableDevice(hHD);
            Console.ReadKey();
            return;
        }

        static HDCallbackCode GetSyncPos(IntPtr pUserData)
        {
            Vector3D v3 = OHUtils.IntPtrToStruct<Vector3D>(pUserData);

            HDAPI.hdBeginFrame(v3.HHD);
            //将获取的坐标位置放入结构数据前三个double字节中
            HDAPI.hdGetDoublev(HDGetParameters.HD_CURRENT_POSITION, pUserData);
            HDAPI.hdEndFrame(v3.HHD);

            return HDCallbackCode.HD_CALLBACK_DONE;
        }
    }
}
