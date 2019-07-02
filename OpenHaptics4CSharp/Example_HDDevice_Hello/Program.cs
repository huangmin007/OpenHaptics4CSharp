using OH4CSharp.HD;
using OH4CSharp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Example_HDDevice_Hello
{
    class Program
    {
        static HDErrorInfo error;
        static Boolean InForce = true;

        static void Main(string[] args)
        {
            uint hHD = HDAPI.hdInitDevice("Default Device");
            error = HDAPI.hdGetError();
            if(error.CheckedError())
            {
                Console.WriteLine("Device Initialize Failed..");
                Console.ReadKey();
                return;
            }

            //HDSchedulerCallback pCallback = GravityWellCallback;
            ulong gravityWell = HDAPI.hdScheduleAsynchronous(GravityWellCallback, IntPtr.Zero, HDSchedulerPriority.HD_MAX_SCHEDULER_PRIORITY);

            HDAPI.hdEnable(HDEDParameters.HD_FORCE_OUTPUT);
            HDAPI.hdStartScheduler();

            error = HDAPI.hdGetError();
            if(error.CheckedError())
            {
                Console.WriteLine("Start Scheduler Failed..");
                Console.ReadKey();
                return;
            }

            while(true)
            {
                ConsoleKeyInfo key = Console.ReadKey();
                if (key.Key == ConsoleKey.DownArrow)
                {
                    InForce = true;
                    Console.WriteLine("In Force:{0}", true);
                }
                else if (key.Key == ConsoleKey.UpArrow)
                {
                    InForce = false;
                    Console.WriteLine("Out Force:{0}", false);
                }

                if (HDAPI.hdWaitForCompletion(gravityWell, HDWaitCode.HD_WAIT_CHECK_STATUS) == 0x00)
                {
                    Console.WriteLine("Ready quit..");
                    Console.ReadKey();
                    break;
                }
            }

            HDAPI.hdStopScheduler();
            HDAPI.hdUnschedule(gravityWell);
            HDAPI.hdDisableDevice(hHD);
        }

        static Vector3D wellPos = new Vector3D();

        static HDCallbackCode GravityWellCallback(IntPtr pUserData)
        {
            const double kStiffness = 0.175;            //(N/mm)
            const double kGravityWellInfluence = 50;    //Box Size:50x50x50(mm)

            Vector3D position = new Vector3D();
            Vector3D force = new Vector3D();
            Vector3D positionTwell = new Vector3D();

            uint hHD = HDAPI.hdGetCurrentDevice();
            //触觉技术框架开始。(一般来说，所有与状态相关的触觉调用都应该在一个框架内进行。)
            HDAPI.hdBeginFrame(hHD);
            //获取设备的当前位置
            HDAPI.hdGetDoublev(HDGetParameters.HD_CURRENT_POSITION, out position);
            //Console.WriteLine("Vector3D:{0}  {1}  {2}", position.X, position.Y, position.Z);
            force.ResetZero();

            // positionTwell = wellPos - position
            //创建一个从设备位置到重力井中心的矢量
            Vector3D.Subtrace(ref positionTwell, ref wellPos, ref position);

            //如果装置位置在重力井中心一定距离内，则向重力井中心施加弹簧力。
            //力的计算不同于传统的重力体，因为装置离中心越近，井所施加的力就越小;
            //该装置的行为就像弹簧连接在它自己和井的中心。
            if (InForce && Vector3D.Magnitude(ref positionTwell) < kGravityWellInfluence)
            {
                // F = k * x 
                //F:力的单位为牛顿(N)
                //k: 井的刚度(N / mm)
                //x: 从设备端点位置到井中心的向量。
                Vector3D.Scale(ref force, ref positionTwell, kStiffness);
            }

            if (!InForce && Vector3D.Magnitude(ref positionTwell) >= kGravityWellInfluence)
            {
                Vector3D.Scale(ref force, ref positionTwell, kStiffness);
            }


            //把力传送到设备上
            HDAPI.hdSetDoublev(HDSetParameters.HD_CURRENT_FORCE, ref force);
            //End haptics frame.
            HDAPI.hdEndFrame(hHD);

            //check error
            HDErrorInfo error = HDAPI.hdGetError();
            if(error.CheckedError())
            {
                Console.WriteLine("渲染重力时检测到错误");
                if(error.IsSchedulerError())
                {
                    Console.WriteLine("调度器错误。");
                    return HDCallbackCode.HD_CALLBACK_DONE;
                }
            }

            return HDCallbackCode.HD_CALLBACK_CONTINUE;
        }
    }
}
