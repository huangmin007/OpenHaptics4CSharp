using OH4CSharp.HD;
using OH4CSharp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Example_HDStatus
{
    class Program
    {
        static HDErrorInfo error;
        static void Main(string[] args)
        {
            uint hHD = HDAPI.hdInitDevice(HDAPI.HD_DEFAULT_DEVICE);
            error = HDAPI.hdGetError();
            if(error.CheckedError())
            {
                Console.WriteLine("Device Initialize Failed..");
                return;
            }

            //更新校准
            //HDAPI.hdUpdateCalibration(HDCalibrationStyles.HD_CALIBRATION_INKWELL);

            HDSchedulerCallback pCallback = AnchoredSpringForceHandler;

            IntPtr deviceHHD = new IntPtr(hHD);
            ulong pHandler = HDAPI.hdScheduleAsynchronous(pCallback, deviceHHD, HDSchedulerPriority.HD_MAX_SCHEDULER_PRIORITY);

            //启用力输出功能
            HDAPI.hdEnable(HDEDParameters.HD_FORCE_OUTPUT);
            double maxStiffness = 0;
            //查询设备能够处理的最大闭环控制刚度。使用超过这个限制的值可能会导致设备嗡嗡作响。
            HDAPI.hdGetDoublev(HDGetParameters.HD_NOMINAL_MAX_STIFFNESS, ref maxStiffness);

            HDAPI.hdStartScheduler();
            error = HDAPI.hdGetError();
            if(error.CheckedError())
            {
                Console.WriteLine("启动调度程序失败.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("按下 Button 1 开始，按上下键设置 force 值");
            while(true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if(key.Key == ConsoleKey.DownArrow)
                {
                    gSpringStiffness -= 0.05;
                    if (gSpringStiffness <= 0) gSpringStiffness = 0.00;

                    Console.WriteLine("gSpringStiffness:{0}", gSpringStiffness);
                }
                else if(key.Key == ConsoleKey.UpArrow)
                {
                    gSpringStiffness += 0.05;
                    if (gSpringStiffness >= maxStiffness) gSpringStiffness = maxStiffness;

                    Console.WriteLine("gSpringStiffness:{0}", gSpringStiffness);
                }
            }

            HDAPI.hdStopScheduler();
            HDAPI.hdUnschedule(pHandler);
            HDAPI.hdDisableDevice(hHD);
        }

        static bool renderForce = false;
        static double gSpringStiffness = 0.25;
        static double[] anchor = new double[3];

        static HDCallbackCode AnchoredSpringForceHandler(IntPtr pUserData)
        {
            uint hHD = (uint)pUserData;
            
            double[] force = new double[3] { 0.0, 0.0, 0.0 };

            HDAPI.hdBeginFrame(hHD);

            double[] position = new double[3];
            HDAPI.hdGetDoublev(HDGetParameters.HD_CURRENT_POSITION, position);

            int currButtons = 0;
            int lastButtons = 0;
            HDAPI.hdGetIntegerv(HDGetParameters.HD_LAST_BUTTONS, ref lastButtons);
            HDAPI.hdGetIntegerv(HDGetParameters.HD_CURRENT_BUTTONS, ref currButtons);

            //按下 Button 1
            if ((currButtons & (int)HDButtonMasks.HD_DEVICE_BUTTON_1) != 0 &&
                (lastButtons & (int)HDButtonMasks.HD_DEVICE_BUTTON_1) == 0)
            {
                anchor = position;
                renderForce = true;

                Console.WriteLine("gSpringStiffness:{0}", gSpringStiffness);
                //Console.WriteLine("Button Down:{0}  {1}  {2}", position[0], position[1], position[2]);
            }
            else if ((currButtons & (int)HDButtonMasks.HD_DEVICE_BUTTON_1) == 0 &&
                (lastButtons & (int)HDButtonMasks.HD_DEVICE_BUTTON_1) != 0) 
            {
                renderForce = false;
                //向设备发送零力，否则它将继续呈现最后发送的力
                HDAPI.hdSetDoublev(HDSetParameters.HD_CURRENT_FORCE, force);
            }

            if(renderForce)
            {
                //计算弹簧力为 F = k * (anchor - position)，这将吸引设备位置朝向锚点位置
                Vector3D.Subtrace(ref force, anchor, position);
                Vector3D.ScaleInPlace(ref force, gSpringStiffness);

                HDAPI.hdSetDoublev(HDSetParameters.HD_CURRENT_FORCE, force);
            }

            HDAPI.hdEndFrame(hHD);

            return HDCallbackCode.HD_CALLBACK_CONTINUE;
        }
    }
}
