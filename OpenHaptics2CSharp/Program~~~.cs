using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using HDDLLInterfaces;
using System.Threading;


namespace TestOHDll
{
    class Program
    {
        static UInt32 hHDA;
        static UInt32 hHDB;

        static HDErrorInfo error;
        static HDCalibrationStyles calibrationStyle;

        static HDSchedulerCallback pSchedulerCallback = SchedulerCallbackHandler;
        static HDSchedulerCallback pDevicePositionCallBack = DevicePositionCallBackHandler;
        static HDSchedulerCallback pSpringForceCallBack = SpringForceCallBackHandler;

        static void Main(string[] args)
        {
            //test
            //Console.WriteLine("Size:{0}", Marshal.SizeOf(typeof(HDErrorInfo)));
            //Console.ReadKey();
            //return;

            hHDA = HDDLL.hdInitDevice("Left Device");

            error = HDDLL.hdGetError();
            if(HDErrorInfo.CheckedError(error))
            {
                Console.WriteLine("Failed to initialize left device.");
                Console.ReadKey();
            }
#if false
            hHDB = HDDLL.hdInitDevice("Right Device");
            error = HDDLL.hdGetError();
            if (HDErrorInfo.CheckedError(error))
            {
                Console.WriteLine("Failed to initialize right device.");
                Console.ReadKey();
            }
#endif
            //获取设备类型
            IntPtr str = HDDLL.hdGetString(HDIdentification.HD_DEVICE_MODEL_TYPE);

            Console.WriteLine("HHD:{0}", hHDA);
            Console.WriteLine("Device Model:{0}", Marshal.PtrToStringAnsi(str));

#region 校准
            //1.获取支持的校准样式
            //2.选择校准样式。
            //译：一些设备可能支持多种类型的校准。在这种情况下，更喜欢自动校准超过墨水瓶校准，更喜欢墨水瓶校准超过复位编码器。
            int supportedCalibrationStyles = 0;
            int[] styles = new int[1];
            //IntPtr styles = new IntPtr(supportedCalibrationStyles);
            HDDLL.hdGetIntegerv(HDGetParameterOptions.HD_CALIBRATION_STYLE, styles);
            supportedCalibrationStyles = styles[0];
            //Marshal.FreeHGlobal(styles);

            Console.WriteLine("supportedCalibrationStyles:{0}  {1}", supportedCalibrationStyles, Enum.GetName(typeof(HDCalibrationStyles), supportedCalibrationStyles));

            if(((HDCalibrationStyles)supportedCalibrationStyles & HDCalibrationStyles.HD_CALIBRATION_ENCODER_RESET) > 0)
                calibrationStyle = HDCalibrationStyles.HD_CALIBRATION_ENCODER_RESET;

            if (((HDCalibrationStyles)supportedCalibrationStyles & HDCalibrationStyles.HD_CALIBRATION_INKWELL) > 0)
                calibrationStyle = HDCalibrationStyles.HD_CALIBRATION_INKWELL;

            if (((HDCalibrationStyles)supportedCalibrationStyles & HDCalibrationStyles.HD_CALIBRATION_AUTO) > 0)
                calibrationStyle = HDCalibrationStyles.HD_CALIBRATION_AUTO;

            //一些触觉设备只支持通过硬件重置手动编码器校准。
            if (calibrationStyle == HDCalibrationStyles.HD_CALIBRATION_ENCODER_RESET)
            {
                Console.WriteLine("请准备手动校准");
                Console.ReadKey();

                HDDLL.hdUpdateCalibration(calibrationStyle);
                if(HDDLL.hdCheckCalibration() == HDCalibrationReturns.HD_CALIBRATION_OK)
                {
                    Console.WriteLine("校准完成");
                }
                error = HDDLL.hdGetError();
                if(HDErrorInfo.CheckedError(error))
                {
                    Console.WriteLine("编码器复位失败");
                    Console.ReadKey();
                    return;
                }
            }
            #endregion

            ulong h1 = HDDLL.hdScheduleAsynchronous(pSpringForceCallBack, new IntPtr(hHDA), HDSchedulerPriority.HD_MAX_SCHEDULER_PRIORITY);
            //开启force输出
            HDDLL.hdEnable(HDEDParameters.HD_FORCE_OUTPUT);
            double gMaxStiffness = 0.0;
            HDDLL.hdGetDoublev(HDGetParameterOptions.HD_NOMINAL_MAX_STIFFNESS, ref gMaxStiffness);
            Console.WriteLine("最大硬度/抗阻性：{0}", gMaxStiffness);


            HDDLL.hdStartScheduler();
            error = HDDLL.hdGetError();
            if(HDErrorInfo.CheckedError(error))
            {
                Console.WriteLine("Start scheduler failed.");
                Console.ReadKey();
                return;
            }

            //当框架被放置到设备墨水瓶中时，一些触觉设备被校准，并调用更新校准。
            //这种形式的校准总是在servoloop开始运行之后执行。
            if(calibrationStyle == HDCalibrationStyles.HD_CALIBRATION_INKWELL)
            {
                Console.WriteLine("Style::{0}", calibrationStyle);
                if(GetCalibrationStatus(hHDA) == HDCalibrationReturns.HD_CALIBRATION_NEEDS_MANUAL_INPUT)
                {
                    Console.WriteLine("请将仪器放入墨水瓶中校准A");
                    Console.ReadKey();
                }
            }
#if false
            //if(calibrationStyleB == HDCalibrationStyles.HD_CALIBRATION_INKWELL)
            //{
            if (GetCalibrationStatus(hHDB) == HDCalibrationReturns.HD_CALIBRATION_NEEDS_MANUAL_INPUT)
                {
                    Console.WriteLine("请将仪器放入墨水瓶中校准B");
                    Console.ReadKey();
                }
            //}
#endif
            //这里应该使用线程来获取设备坐标，一台设备一个线程，这里我简写了，只为测试
            //Position position = new Position();
            //IntPtr pos = HDDLLUtils.StructToIntPtr<Position>(position);
            //OR
            //double[] position = new double[3];
            //IntPtr pos = HDDLLUtils.DoubleArrToIntPtr(position);
            //OR
            Vector3D positionA = new Vector3D
            {
                HHD = hHDA,
            };
            IntPtr posA = HDDLLUtils.StructToIntPtr<Vector3D>(positionA);
#if false
            Position positionB = new Position
            {
                HHD = hHDB,
            };
            IntPtr posB = HDDLLUtils.StructToIntPtr<Position>(positionB);
#endif
           
            while (true)
            {
                //HDDLL.hdScheduleSynchronous(pDevicePositionCallBack, posA, HDSchedulerPriority.HD_DEFAULT_SCHEDULER_PRIORITY);

                //结构类型
                //position = HDDLLUtils.IntPtrToStruct<Position>(pos);
                //Console.WriteLine("Position: x:{0} y:{1} z:{2}", position.X, position.Y, position.Z);
                //OR数组类型
                //position = HDDLLUtils.IntPtrToDoubleArr(pos, 3);
                //Console.WriteLine("Position: x:{0} y:{1} z:{2}", position[0], position[1], position[2]);
                //OR
                //positionA = HDDLLUtils.IntPtrToStruct<Vector3D>(posA);
                //Console.WriteLine("Position: x:{0:f4} y:{1:f4} z:{2:f4}", positionA.X, positionA.Y, positionA.Z);
#if false
                //HDDLL.hdScheduleSynchronous(pDevicePositionCallBack, posB, HDSchedulerPriority.HD_DEFAULT_SCHEDULER_PRIORITY);
                //positionB = HDDLLUtils.IntPtrToStruct<Position>(posB);
                Console.WriteLine("Position: x:{0:f4} y:{1:f4} z:{2:f4}      {3:f4} {4:f4} {5:f4}", positionA.X, positionA.Y, positionA.Z, positionB.X, positionB.Y, positionB.Z);
#endif   
                if(!HDDLL.hdWaitForCompletion(h1, HDWaitCode.HD_WAIT_CHECK_STATUS))
                {
                    Console.WriteLine("主调度程序回调已退出");
                }
                Thread.Sleep(500);
            }

            HDDLL.hdStopScheduler();
            HDDLL.hdUnschedule(h1);
            HDDLL.hdDisableDevice(hHDA);

            Console.ReadKey();
        }



        static HDCalibrationReturns GetCalibrationStatus(UInt32 hHD)
        {
            CalibrationStatus cs = new CalibrationStatus
            {
                HHD = hHD,
            };
            IntPtr status = HDDLLUtils.StructToIntPtr<CalibrationStatus>(cs);

            HDDLL.hdScheduleSynchronous(pSchedulerCallback, status, HDSchedulerPriority.HD_DEFAULT_SCHEDULER_PRIORITY);
            cs = HDDLLUtils.IntPtrToStruct<CalibrationStatus>(status);

            return (HDCalibrationReturns)((int)cs.Status);
        }

        static HDCallbackCode SchedulerCallbackHandler(IntPtr pUserData)
        {
            CalibrationStatus cs = HDDLLUtils.IntPtrToStruct<CalibrationStatus>(pUserData);

            HDDLL.hdBeginFrame(cs.HHD);
            cs.Status = (UInt32)HDDLL.hdCheckCalibration();
            HDDLL.hdEndFrame(cs.HHD);

            return HDCallbackCode.HD_CALLBACK_DONE;
        }

        static HDCallbackCode DevicePositionCallBackHandler(IntPtr pUserData)
        {
            Vector3D pos = HDDLLUtils.IntPtrToStruct<Vector3D>(pUserData);
            double[] dou = new double[3] { 0.0, 0.0, 0.0};

            HDDLL.hdBeginFrame(pos.HHD);
            HDDLL.hdGetDoublev(HDGetParameterOptions.HD_CURRENT_POSITION, pUserData);
            HDDLL.hdEndFrame(pos.HHD);

            //HDDLL.hdBeginFrame(HDDLL.hdGetCurrentDevice());
            //HDDLL.hdGetDoublev(HDCartesianSpace.HD_CURRENT_POSITION, pUserData);
            //HDDLL.hdEndFrame(HDDLL.hdGetCurrentDevice());

            return HDCallbackCode.HD_CALLBACK_DONE;
        }

        static Vector3D anchor;
        static double gSpringStiffness = 0.25;
        static Boolean bRenderForce = false;
        
        static HDCallbackCode SpringForceCallBackHandler(IntPtr pUserData)
        {
            UInt32 hHD = (UInt32)pUserData;
            Vector3D position = new Vector3D();
            Vector3D force = new Vector3D();

            IntPtr posPtr = HDDLLUtils.StructToIntPtr<Vector3D>(position);
            IntPtr forPtr = HDDLLUtils.StructToIntPtr<Vector3D>(force);
            
            int nCurrentButtons = -1, nLastButtons = -1;

            HDDLL.hdBeginFrame(hHD);
            HDDLL.hdGetDoublev(HDGetParameterOptions.HD_CURRENT_POSITION, posPtr);

            HDDLL.hdGetIntegerv(HDGetParameterOptions.HD_CURRENT_BUTTONS, ref nCurrentButtons);
            HDDLL.hdGetIntegerv(HDGetParameterOptions.HD_LAST_BUTTONS, ref nLastButtons);

            position = HDDLLUtils.IntPtrToStruct<Vector3D>(posPtr);
            

            if ((nCurrentButtons & (int)(HDButtonMasks.HD_DEVICE_BUTTON_1)) != 0 &&
                (nLastButtons & (int)HDButtonMasks.HD_DEVICE_BUTTON_1) == 0)
            {
                anchor = position;
                bRenderForce = true;
            }
            else if((nCurrentButtons & (int)(HDButtonMasks.HD_DEVICE_BUTTON_1)) == 0 &&
                (nLastButtons & (int)HDButtonMasks.HD_DEVICE_BUTTON_1) != 0)
            {
                bRenderForce = false;

                HDDLL.hdSetDoublev(HDSetParameterOptions.HD_CURRENT_FORCE, forPtr);
                force = HDDLLUtils.IntPtrToStruct<Vector3D>(forPtr);
            }

            //Console.WriteLine("bRenderForce::{0}", bRenderForce);
            if(bRenderForce)
            {
                //force = Vector3D.Subtrace(anchor, position);    //OR
                Vector3D.Subtrace(ref force, ref anchor, ref position);
                Vector3D.ScaleInPlace(ref force, gSpringStiffness);

                forPtr = HDDLLUtils.StructToIntPtr<Vector3D>(force);
                HDDLL.hdSetDoublev(HDSetParameterOptions.HD_CURRENT_FORCE, forPtr);

                Console.WriteLine("force:{0}  {1}  {2}", force.X, force.Y, force.Z);
                Console.WriteLine("position:{0} {1} {2}", position.X, position.Y, position.Z);
            }

            //error handler 
            error = HDDLL.hdGetError();
            if(HDErrorInfo.CheckedError(error))
            {
                
            }


            HDDLL.hdEndFrame(hHD);

            return HDCallbackCode.HD_CALLBACK_CONTINUE;
        }
    }
}
