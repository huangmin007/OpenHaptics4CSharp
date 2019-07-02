using OH4CSharp.HD;
using OH4CSharp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Example_HDSphere
{
    class Program
    {
        static void Main(string[] args)
        {
            uint hHD = HDAPI.hdInitDevice(null);

            HDAPI.hdEnable(HDEDParameters.HD_FORCE_OUTPUT);
            HDAPI.hdStartScheduler();

            HDErrorInfo error = HDAPI.hdGetError();
            if(error.CheckedError())
            {
                Console.WriteLine("Start Scheduler Failed.");
                Console.ReadKey();
                return;
            }

            ulong pHandler = HDAPI.hdScheduleAsynchronous(FrictionlessSphereCallback, IntPtr.Zero, HDSchedulerPriority.HD_DEFAULT_SCHEDULER_PRIORITY);

            while(true)
            {
                if(HDAPI.hdWaitForCompletion(pHandler, HDWaitCode.HD_WAIT_CHECK_STATUS) == 0x00)
                {
                    Console.WriteLine("主回调器退出。");
                    Console.ReadKey();
                    return;
                }
            }

            HDAPI.hdStopScheduler();
            HDAPI.hdUnschedule(pHandler);
            HDAPI.hdDisableDevice(hHD);
        }

        //球体半径
        const double radius = 40.0;
        //球的刚度，即k值。刚度越大，表面越硬。
        const double stiffness = 0.25;
        //球体位置
        static readonly Vector3D spherePosition = new Vector3D();

        static HDCallbackCode FrictionlessSphereCallback(IntPtr pUserData)
        {
            uint hHD = HDAPI.hdGetCurrentDevice();
            HDAPI.hdBeginFrame(hHD);

            //获取坐标位置
            Vector3D position;
            HDAPI.hdGetDoublev(HDGetParameters.HD_CURRENT_POSITION, out position);
            //Console.WriteLine("{0}  {1}  {2}", position.X, position.Y, position.Z);

            //计算设备和球体中心之间的距离。
            Vector3D res = position - spherePosition;
            double distance = Vector3D.Magnitude(ref res);

            //如果用户在球体内，即用户到球体中心的距离小于球体半径，
            //则用户正在穿透球体，应命令一个力将用户推向表面。
            if (distance < radius)
            {
                //计算穿透距离。
                double penetrationDistance = radius - distance;

                //在力的方向上创建一个单位矢量，它总是从球体的中心通过用户的位置向外。
                Vector3D forceDirection = (position - spherePosition) / distance;

                //使用 F = k * x 创建一个远离中心的力向量
                //球体与穿透距离成正比，并受物体刚度的制约。 
                double k = stiffness;
                Vector3D x = penetrationDistance * forceDirection;
                Vector3D f = k * x;

                HDAPI.hdSetDoublev(HDSetParameters.HD_CURRENT_FORCE, ref f);
            }

            HDAPI.hdEndFrame(hHD);
            HDErrorInfo error = HDAPI.hdGetError();
            if(error.CheckedError())
            {
                Console.WriteLine("主调度程序回调期间出错.");
                if(error.IsSchedulerError())
                {
                    return HDCallbackCode.HD_CALLBACK_DONE;
                }
            }

            return HDCallbackCode.HD_CALLBACK_CONTINUE;
        }
    }
}

