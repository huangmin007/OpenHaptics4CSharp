using OH4CSharp.HD;
using OH4CSharp.HL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Example_CannedForceEffect
{
    class Program
    {
        
        static void Main(string[] args)
        {
            uint hHD = HDAPI.hdInitDevice(null);
            Console.WriteLine(HLAPI.hlGetString(HLGetStringParameters.HL_VENDOR));

            HDAPI.hdMakeCurrentDevice(hHD);

            IntPtr hHLRC = HLAPI.hlCreateContext(hHD);
            HLAPI.hlMakeCurrent(hHLRC);
            //禁用GL
            HLAPI.hlDisable(HLCapabilityParameters.HL_USE_GL_MODELVIEW);

            uint spring = HLAPI.hlGenEffects(1);
            Console.WriteLine("spring:{0}", spring);
            //添加回调处理按扭按下
            HLAPI.hlAddEventCallback(HLCallbackEvents.HL_EVENT_1BUTTONDOWN, HLAPI.HL_OBJECT_ANY, HLCallbackThreads.HL_CLIENT_THREAD, ButtonCB, new IntPtr(spring));
            HLAPI.hlAddEventCallback(HLCallbackEvents.HL_EVENT_1BUTTONUP, HLAPI.HL_OBJECT_ANY, HLCallbackThreads.HL_CLIENT_THREAD, ButtonCB, new IntPtr(spring));
            HLAPI.hlAddEventCallback(HLCallbackEvents.HL_EVENT_2BUTTONDOWN, HLAPI.HL_OBJECT_ANY, HLCallbackThreads.HL_CLIENT_THREAD, ButtonCB, new IntPtr(0));

            uint friction = HLAPI.hlGenEffects(1);

            HLAPI.hlBeginFrame();
            HLAPI.hlEffectd(HLEffectParams.HL_EFFECT_PROPERTY_GAIN, 0.2);
            HLAPI.hlEffectd(HLEffectParams.HL_EFFECT_PROPERTY_MAGNITUDE, 0.5);
            HLAPI.hlStartEffect(HLStartEffectTypes.HL_EFFECT_FRICTION, friction);
            HLAPI.hlEndFrame();

            HLError error;
            while(true)
            {
                HLAPI.hlBeginFrame();
                //轮询事件。请注意，客户端线程事件回调是从这里的一个框架内分派的，因此我们可以安全地直接启动/停止事件回调的效果
                HLAPI.hlCheckEvents();
                HLAPI.hlEndFrame();

                error = HLAPI.hlGetError();
                if(error.CheckedError())
                {
                    Console.WriteLine("HL Error:{0}", error.GetErrorCodeString());
                }
            }

            //Stop the friction effect.
            HLAPI.hlBeginFrame();
            HLAPI.hlStopEffect(friction);
            HLAPI.hlEndFrame();

            HLAPI.hlDeleteEffects(friction, 1);
            HLAPI.hlDeleteEffects(spring, 1);

            HLAPI.hlDeleteContext(hHLRC);
            HDAPI.hdDisableDevice(hHD);

            Console.ReadKey();
        }

        static void ButtonCB(String evt, uint obj, String thread, IntPtr cache, IntPtr pUserData)
        {
            //Console.WriteLine(Marshal.PtrToStringAnsi(evt));
            Console.WriteLine("{0}  {1}  {2}", obj, (uint)pUserData, evt);
            Console.WriteLine("{0}  {1}", evt, thread);

            uint spring = (uint)pUserData;
            HLCallbackEvents cb_event = (HLCallbackEvents)Enum.Parse(typeof(HLCallbackEvents), evt);
            HLCallbackThreads cb_thread = (HLCallbackThreads)Enum.Parse(typeof(HLCallbackThreads), thread);

            if (cb_event == HLCallbackEvents.HL_EVENT_1BUTTONDOWN)
            {
                double[] anchor = new double[3];
                HLAPI.hlCacheGetDoublev(cache, HLCacheGetParameters.HL_PROXY_POSITION, anchor);

                HLAPI.hlEffectd(HLEffectParams.HL_EFFECT_PROPERTY_GAIN, 0.8);
                HLAPI.hlEffectd(HLEffectParams.HL_EFFECT_PROPERTY_MAGNITUDE, 1.0);
                HLAPI.hlEffectdv(HLEffectParams.HL_EFFECT_PROPERTY_POSITION, anchor);
                HLAPI.hlStartEffect(HLStartEffectTypes.HL_EFFECT_SPRING, spring);
            }
            else if(cb_event == HLCallbackEvents.HL_EVENT_1BUTTONUP)
            {
                HLAPI.hlStopEffect(spring);
            }
            else if(cb_event == HLCallbackEvents.HL_EVENT_2BUTTONDOWN)
            {
                double[] direction = new double[3]{ 0.0, 0.0, 1.0};
                double duration = 100; //持续 100ms

                //通过在短时间内指挥具有方向和大小的力来触发脉冲。
                HLAPI.hlEffectd(HLEffectParams.HL_EFFECT_PROPERTY_DURATION, duration);
                HLAPI.hlEffectd(HLEffectParams.HL_EFFECT_PROPERTY_MAGNITUDE, 0.8);
                HLAPI.hlEffectdv(HLEffectParams.HL_EFFECT_PROPERTY_DIRECTION, direction);
                HLAPI.hlTriggerEffect(HLTriggerEffectTypes.HL_EFFECT_CONSTANT);
            }
        }

    }
}
