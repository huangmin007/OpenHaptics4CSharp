using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace OH2CSharp.HL
{
    /// <summary>
    /// HLAPI是为高级触觉场景渲染而设计的。它针对的是高级OpenGL开发人员，熟悉触觉编程，但希望能快速轻松地将触觉添加到现有的图形应用程序中。
    /// <para>HLAPI构建在HDAPI之上，以牺牲灵活性为代价，提供比HDAPI更高级别的触觉控制，HLAPI主要是为那些精通OpenGL编程的人设计的。</para>
    /// <para>例如：HLAPI程序员不必担心设计力方程等低级问题，处理线程安全并实现高效的数据结构进行触觉渲染。
    /// HLAPI遵循传统OpenGLAPI中的图形技术。向对象添加触觉是一个相当简单的过程，类似于模型，用于以图形方式表示对象，触觉特性，如刚度和摩擦力，同样被抽象为材料。
    /// HLAPI还提供事件处理，以便于集成到应用程序中。</para>
    /// <para>如：模拟摩擦效果、重力效果、粘性流体效果、球体等</para>
    /// </summary>
    public partial class HLAPI
    {
        static HLAPI()
        {
            string path = Environment.GetEnvironmentVariable("OH_SDK_BASE");
            Console.WriteLine("OH_SDK_BASE:{0}", path);
            Console.WriteLine("Directory:{0}", Environment.CurrentDirectory);
        }

        #region DLL Path/IntPtr/EnumToIntPtr
        /// <summary>
        /// hd.dll 路径，后面在改
        /// </summary>
        public const string DLL_PATH = @"D:\OpenHaptics\Developer\3.5.0\lib\x64\Release\hl.dll";

        /// <summary>
        /// hl.dll 指针
        /// <para>退出时应该释放应该指针，WINAPI.FreeLibrary(DLL_IntPtr)</para>
        /// </summary>
        public static readonly IntPtr DLL_IntPtr = WINAPI.LoadLibrary(DLL_PATH);

        /// <summary>
        /// 获取 HL C++ 常量/枚举对象的指针
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        private static IntPtr DLLEnumToIntPtr<T>(T value)
        {
            //IntPtr DLLIntPtr = WINAPI.LoadLibrary(DLL_PATH);
            //if (DLLIntPtr == IntPtr.Zero) return IntPtr.Zero;
            if (DLL_IntPtr == IntPtr.Zero) return IntPtr.Zero;

            String name = Enum.GetName(typeof(T), value);

            IntPtr ptr = WINAPI.GetProcAddress(DLL_IntPtr, name);
            if (ptr == IntPtr.Zero) return IntPtr.Zero;

            return Marshal.ReadIntPtr(ptr); //返回 C++ 
        }
        #endregion


        //====================================================STATE MAINTENANCE AND ACCESSORS
        #region hlCreateContext/hlDeleteContext/hlMakeCurrent/hlEnable/hlDisable/...
        /// <summary>
        /// 创建触觉反馈渲染的上下文(haptic rendering context)
        /// </summary>
        /// <param name="hHD">设备句柄</param>
        /// <returns>返回下下文名柄</returns>
        [DllImport(DLL_PATH)]
        public static extern IntPtr hlCreateContext(UInt32 hHD);

        /// <summary>
        /// 删除触觉反馈渲染的上下文(haptic rendering context)
        /// </summary>
        /// <param name="hHLRC"></param>
        [DllImport(DLL_PATH)]
        public static extern void hlDeleteContext(IntPtr hHLRC);

        /// <summary>
        /// 呈现当前触觉上下文。当前呈现上下文是所有呈现和状态命令的目标。所有触觉渲染命令将被发送到当前上下文的设备，直到具有不同设备的上下文成为当前的。
        /// <para>创建呈现上下文后在程序启动时调用，或在程序执行期间调用，以切换呈现上下文，以便呈现给多个触觉设备。</para>
        /// <see cref="HLAPI.hlCreateContext"/>
        /// <see cref="HLAPI.hlDeleteContext"/>
        /// </summary>
        /// <param name="hHLRC">hlCreateContext()返回的触觉呈现上下文句柄。</param>
        [DllImport(DLL_PATH)]
        public static extern void hlMakeCurrent(IntPtr hHLRC);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hHD"></param>
        /// <returns></returns>
        [DllImport(DLL_PATH)]
        public static extern IntPtr hlContextDevice(UInt32 hHD);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [DllImport(DLL_PATH)]
        public static extern IntPtr hlGetCurrentContext();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [DllImport(DLL_PATH)]
        public static extern UInt32 hlGetCurrentDevice();

        /// <summary>
        /// 启用或禁用当前呈现上下文的功能。
        /// <para>启用或禁用指定的功能</para>
        /// <see cref="HLAPI.hlDisable"/>
        /// </summary>
        /// <param name="cap"></param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM">如果cap不是列出的值之一</exception>
        public static void hlEnable(HLCapabilityParameters cap){    _hlEnable(DLLEnumToIntPtr(cap));    }
        [DllImport(DLL_PATH, EntryPoint = "hlEnable")]
        private static extern void _hlEnable(IntPtr cap);

        /// <summary>
        /// 启用或禁用当前呈现上下文的功能。
        /// <para>启用或禁用指定的功能</para>
        /// <see cref="HLAPI.hlIsEnable"/>
        /// </summary>
        /// <param name="cap"></param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM">如果cap不是列出的值之一</exception>
        public static void hlDisable(HLCapabilityParameters cap){   _hlDisable(DLLEnumToIntPtr(cap));}
        [DllImport(DLL_PATH, EntryPoint = "hlDisable")]
        private static extern void _hlDisable(IntPtr cap);


        /// <summary>
        /// 检查功能是否启用或禁用。
        /// <para>用于查询是否启用了特定的功能特性。功能包括代理分辨率、触觉相机视图、自适应视图和gl模型视图。</para>
        /// </summary>
        /// <param name="cap"></param>
        /// <returns></returns>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM">如果cap不是列出的值之一</exception>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION">如果没有触觉渲染是当前的</exception>
        public static bool hlIsEnabled(HLCapabilityParameters cap){ return _hlIsEnabled(DLLEnumToIntPtr(cap)) == 0x01;  }
        [DllImport(DLL_PATH, EntryPoint = "hlIsEnabled")]
        private static extern byte _hlIsEnabled(IntPtr cap);


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [DllImport(DLL_PATH)]
        public static extern HLError hlGetError();


        /// <summary>
        /// 返回描述触觉呈现器实现的字符串。
        /// </summary>
        /// <param name="pname">要描述的触觉呈现器实现属性</param>
        /// <returns>描述触觉呈现器实现的静态字符串</returns>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM">如果pname不是列出的值之一</exception>
        public static String hlGetString(HLGetStringParameters pname){  return Marshal.PtrToStringAnsi(_hlGetString(DLLEnumToIntPtr(pname)));   }
        [DllImport(DLL_PATH, EntryPoint = "hlGetString")]
        public static extern IntPtr _hlGetString(IntPtr pname);

        #endregion


    }

    /// <summary>
    /// Win API
    /// </summary>
    public class WINAPI
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll")]
        public static extern bool FreeLibrary(IntPtr hModule);
    }

}
