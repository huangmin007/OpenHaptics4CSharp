using OH4CSharp.HD;
using OH4CSharp.HL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Example_HLDeployment
{
    
    class Program
    {
        static void Main(string[] args)
        {
            uint hHD = HDAPI.hdInitDevice(null);
            HDErrorInfo error = HDAPI.hdGetError();
            if(error.CheckedError())
            {
                Console.WriteLine("Initialize Device Failed.");
                Console.ReadKey();
                return;
            }

            HDAPI.hdMakeCurrentDevice(hHD);
            IntPtr hHLRC = HLAPI.hlCreateContext(hHD);
            HLAPI.hlMakeCurrent(hHLRC);

            HLAPI.hlDisable(HLCapabilityParameters.HL_USE_GL_MODELVIEW);
            uint spring = HLAPI.hlGenEffects(1);
            IntPtr ptr = new IntPtr(spring);

            HLAPI.hlAddEventCallback(HLCallbackEvents.HL_EVENT_1BUTTONDOWN, HLAPI.HL_OBJECT_ANY, HLCallbackThreads.HL_CLIENT_THREAD, ButtonCB, ptr);
            HLAPI.hlAddEventCallback(HLCallbackEvents.HL_EVENT_1BUTTONUP, HLAPI.HL_OBJECT_ANY, HLCallbackThreads.HL_CLIENT_THREAD, ButtonCB, ptr);

            while(true)
            {
                HLAPI.hlBeginFrame();
                //轮询事件。请注意，客户端线程事件回调是从这里的一个框架内分派的，因此我们可以安全地直接启动/停止事件回调的效果。
                HLAPI.hlCheckEvents();
                HLAPI.hlEndFrame();
                
            }

            HLAPI.hlDeleteEffects(spring, 1);
            HLAPI.hlDeleteContext(hHLRC);
            HDAPI.hdDisableDevice(hHD);
        }

        static void ButtonCB(String evt, uint obj, String thread, IntPtr cache, IntPtr pUserData)
        {
            uint spring = (uint)pUserData;
            HLCallbackEvents cEvent = (HLCallbackEvents)Enum.Parse(typeof(HLCallbackEvents), evt);

            if(cEvent == HLCallbackEvents.HL_EVENT_1BUTTONDOWN)
            {
                double[] anchor = new double[3];
                HLAPI.hlCacheGetDoublev(cache, HLCacheGetParameters.HL_PROXY_POSITION, anchor);

                HLAPI.hlEffectd(HLEffectParams.HL_EFFECT_PROPERTY_GAIN, 0.8);
                HLAPI.hlEffectd(HLEffectParams.HL_EFFECT_PROPERTY_MAGNITUDE, 1.0);
                HLAPI.hlEffectdv(HLEffectParams.HL_EFFECT_PROPERTY_POSITION, anchor);
                HLAPI.hlStartEffect(HLStartEffectTypes.HL_EFFECT_SPRING, spring);     //弹力
                //HLAPI.hlStartEffect(HLStartEffectTypes.HL_EFFECT_FRICTION, friction);     //摩擦力
                //HLAPI.hlStartEffect(HLStartEffectTypes.HL_EFFECT_VISCOUS, viscous);     //粘滞效果
            }
            else if (cEvent == HLCallbackEvents.HL_EVENT_1BUTTONUP)
            {
                HLAPI.hlStopEffect(spring);
            }
        }
    }
}
