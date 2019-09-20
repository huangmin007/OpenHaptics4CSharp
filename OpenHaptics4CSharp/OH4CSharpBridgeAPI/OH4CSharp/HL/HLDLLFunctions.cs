using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace OH4CSharp.HL
{
    /// <summary>
    /// HL Callback Process
    /// </summary>
    public delegate void HLCallbackProc();
    /*
    /// <summary>
    /// 事件回调处理代理函数
    /// </summary>
    /// <param name="evt">DLLEnumToIntPtr<HLCallbackEvents>(event)</param>
    /// <param name="obj"></param>
    /// <param name="thread">DLLEnumToIntPtr<HLCallbackThreads>(pthread)</param>
    /// <param name="cache"></param>
    /// <param name="pUserData"></param>
    public delegate void HLEventProc(IntPtr evt, uint obj, IntPtr thread, IntPtr cache, IntPtr pUserData);
    */

    /// <summary>
    /// 事件回调处理代理函数
    /// </summary>
    /// <param name="callbackEvent">HLCallbackEvents</param>
    /// <param name="obj"></param>
    /// <param name="callbackThread">HLCallbackThreads</param>
    /// <param name="cache"></param>
    /// <param name="pUserData"></param>
    public delegate void HLEventProc(String callbackEvent, uint obj, String callbackThread, IntPtr cache, IntPtr pUserData);


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
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            WINAPI.FreeLibrary(DLL_IntPtr);
            Console.WriteLine("AppDomain.CurrentDomain.ProcessExit.");
        }

        #region DLL Path/IntPtr/EnumToIntPtr
        /// <summary>
        /// hd.dll 路径，后面在改
        /// </summary>
        public const string DLL_PATH = @"hl.dll";

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

            return Marshal.ReadIntPtr(ptr); //返回 C++ 指针地址
        }
        #endregion

        /// <summary>
        /// Shape/Effect Id for Events
        /// </summary>
        public const int HL_OBJECT_ANY = 0x00;

        //====================================================STATE MAINTENANCE AND ACCESSORS
        #region hlCreateContext/hlDeleteContext/hlMakeCurrent/hlEnable/hlDisable/...
        /// <summary>
        /// 创建触觉反馈渲染的上下文(haptic rendering context)
        /// </summary>
        /// <param name="hHD">设备句柄</param>
        /// <returns>返回下下文名柄</returns>
        [DllImport(DLL_PATH, EntryPoint = "hlCreateContext")]
        public static extern IntPtr hlCreateContext(uint hHD);

        /// <summary>
        /// 删除触觉反馈渲染的上下文(haptic rendering context)
        /// </summary>
        /// <param name="hHLRC"></param>
        [DllImport(DLL_PATH, EntryPoint = "hlDeleteContext")]
        public static extern void hlDeleteContext(IntPtr hHLRC);

        /// <summary>
        /// 呈现当前触觉上下文。当前呈现上下文是所有呈现和状态命令的目标。所有触觉渲染命令将被发送到当前上下文的设备，直到具有不同设备的上下文成为当前的。
        /// <para>创建呈现上下文后在程序启动时调用，或在程序执行期间调用，以切换呈现上下文，以便呈现给多个触觉设备。</para>
        /// <see cref="HLAPI.hlCreateContext"/>
        /// <see cref="HLAPI.hlDeleteContext"/>
        /// </summary>
        /// <param name="hHLRC">hlCreateContext()返回的触觉呈现上下文句柄。</param>
        [DllImport(DLL_PATH, EntryPoint = "hlMakeCurrent")]
        public static extern void hlMakeCurrent(IntPtr hHLRC);

        /// <summary>
        /// 设置上下文设备
        /// </summary>
        /// <param name="hHD">设备句柄</param>
        /// <returns>返回上下文 HHLRC </returns>
        [DllImport(DLL_PATH, EntryPoint = "hlContextDevice")]
        public static extern IntPtr hlContextDevice(uint hHD);

        /// <summary>
        /// 获取当前上下文
        /// </summary>
        /// <returns>返回上下文 HHLRC </returns>
        [DllImport(DLL_PATH, EntryPoint = "hlGetCurrentContext")]
        public static extern IntPtr hlGetCurrentContext();

        /// <summary>
        /// 获取当前设备
        /// </summary>
        /// <returns>返回设备句柄</returns>
        [DllImport(DLL_PATH, EntryPoint = "hlGetCurrentDevice")]
        public static extern uint hlGetCurrentDevice();

        /// <summary>
        /// 启用或禁用当前呈现上下文的功能。
        /// <para>启用或禁用指定的功能</para>
        /// <see cref="HLAPI.hlDisable"/>
        /// </summary>
        /// <param name="cap"></param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM">如果cap不是列出的值之一</exception>
        public static void hlEnable(HLCapabilityParameters cap) { _hlEnable(DLLEnumToIntPtr(cap)); }
        [DllImport(DLL_PATH, EntryPoint = "hlEnable")]
        private static extern void _hlEnable(IntPtr cap);

        /// <summary>
        /// 启用或禁用当前呈现上下文的功能。
        /// <para>启用或禁用指定的功能</para>
        /// <see cref="HLAPI.hlIsEnable"/>
        /// </summary>
        /// <param name="cap"></param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM">如果cap不是列出的值之一</exception>
        public static void hlDisable(HLCapabilityParameters cap) { _hlDisable(DLLEnumToIntPtr(cap)); }
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
        public static bool hlIsEnabled(HLCapabilityParameters cap) { return _hlIsEnabled(DLLEnumToIntPtr(cap)) == 0x01; }
        [DllImport(DLL_PATH, EntryPoint = "hlIsEnabled")]
        private static extern byte _hlIsEnabled(IntPtr cap);

        /// <summary>
        /// 获取错误信息
        /// <seealso cref="HDAPI.HDGetError"/>
        /// </summary>
        /// <returns></returns>
        [DllImport(DLL_PATH, EntryPoint = "hlGetError")]
        public static extern HLError hlGetError();

        /// <summary>
        /// 返回描述触觉呈现器实现的字符串。
        /// </summary>
        /// <param name="pname">要描述的触觉呈现器实现属性</param>
        /// <returns>描述触觉呈现器实现的静态字符串</returns>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM">如果pname不是列出的值之一</exception>
        public static String hlGetString(HLGetStringParameters pname) { return Marshal.PtrToStringAnsi(_hlGetString(DLLEnumToIntPtr(pname))); }
        [DllImport(DLL_PATH, EntryPoint = "hlGetString")]
        public static extern IntPtr _hlGetString(IntPtr pname);

        /// <summary>
        /// 设置参数，允许触觉呈现器选择性地执行选定的优化。
        /// <para>用于允许控制触觉呈现器使用的优化。</para>
        /// <see cref="HLAPI.hlBeginShape"/>
        /// <see cref="HLAPI.hlPushAttrib"/>
        /// <see cref="HLAPI.hlPopAttrib"/>
        /// </summary>
        /// <param name="target"></param>
        /// <param name="value"></param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM">如果target不是列出的值之一</exception>
        /// <exception cref="HLErrorCodes.HL_INVALID_VALUE">如果值超出范围</exception>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION">如果当前没有触觉呈现上下文</exception>
        public static void hlHint(HLHintParameters target, int value) { _hlHinti(DLLEnumToIntPtr(target), value); }
        [DllImport(DLL_PATH, EntryPoint = "hlHinti")]
        private static extern void _hlHinti(IntPtr target, int value);

        /// <summary>
        /// 设置参数，允许触觉呈现器选择性地执行选定的优化。
        /// <para>用于允许控制触觉呈现器使用的优化。</para>
        /// <see cref="HLAPI.hlBeginShape"/>
        /// <see cref="HLAPI.hlPushAttrib"/>
        /// <see cref="HLAPI.hlPopAttrib"/>
        /// </summary>
        /// <param name="target"></param>
        /// <param name="value"></param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM">如果target不是列出的值之一</exception>
        /// <exception cref="HLErrorCodes.HL_INVALID_VALUE">如果值超出范围</exception>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION">如果当前没有触觉呈现上下文</exception>
        public static void hlHint(HLHintParameters target, bool value) { _hlHintb(DLLEnumToIntPtr(target), value ? (byte)0x01 : (byte)0x00); }
        [DllImport(DLL_PATH, EntryPoint = "hlHintb")]
        private static extern void _hlHintb(IntPtr target, byte value);

        /// <summary>
        /// 允许查询触觉渲染器的不同状态值
        /// <para>查询触觉渲染器的状态</para>
        /// <see cref="HLAPI.hlIsEnabled"/>
        /// <see cref="HLAPI.hlCacheGetBooleanv"/>
        /// <see cref="HLAPI.hlCacheGetDoublev"/>
        /// </summary>
        /// <param name="pname">要查询的参数(状态值)的名称</param>
        /// <param name="value">返回要查询的参数值的地址</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM">如果pname不是列出的值之一</exception>
        /// <exception cref="HLErrorCodes.HD_INVALID_HANDLE">设备句柄无效</exception>
        public static void hlGetBooleanv(HLGetParameters pname, IntPtr value) { _hlGetBooleanv(DLLEnumToIntPtr(pname), value); }
        [DllImport(DLL_PATH, EntryPoint = "hlGetBooleanv")]
        private static extern void _hlGetBooleanv(IntPtr pname, IntPtr value);

        /// <summary>
        /// 允许查询触觉渲染器的不同状态值
        /// <para>查询触觉渲染器的状态</para>
        /// <see cref="HLAPI.hlIsEnabled"/>
        /// <see cref="HLAPI.hlCacheGetBooleanv"/>
        /// <see cref="HLAPI.hlCacheGetDoublev"/>
        /// </summary>
        /// <param name="pname">要查询的参数(状态值)的名称</param>
        /// <param name="value">返回要查询的参数值的地址</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM">如果pname不是列出的值之一</exception>
        /// <exception cref="HLErrorCodes.HD_INVALID_HANDLE">设备句柄无效</exception>
        public static void hlGetBooleanv(HLGetParameters pname, byte[] value) { _hlGetBooleanv(DLLEnumToIntPtr(pname), value); }
        [DllImport(DLL_PATH, EntryPoint = "hlGetBooleanv")]
        private static extern void _hlGetBooleanv(IntPtr pname, byte[] value);

        /// <summary>
        /// 允许查询触觉渲染器的不同状态值
        /// <para>查询触觉渲染器的状态</para>
        /// <see cref="HLAPI.hlIsEnabled"/>
        /// <see cref="HLAPI.hlCacheGetBooleanv"/>
        /// <see cref="HLAPI.hlCacheGetDoublev"/>
        /// </summary>
        /// <param name="pname">要查询的参数(状态值)的名称</param>
        /// <param name="value">返回要查询的参数值的地址</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM">如果pname不是列出的值之一</exception>
        /// <exception cref="HLErrorCodes.HD_INVALID_HANDLE">设备句柄无效</exception>
        public static void hlGetBooleanv(HLGetParameters pname, ref byte value) { _hlGetBooleanv(DLLEnumToIntPtr(pname), ref value); }
        [DllImport(DLL_PATH, EntryPoint = "hlGetBooleanv")]
        private static extern void _hlGetBooleanv(IntPtr pname, ref byte value);

        /// <summary>
        /// 允许查询触觉渲染器的不同状态值
        /// <para>查询触觉渲染器的状态</para>
        /// <see cref="HLAPI.hlIsEnabled"/>
        /// <see cref="HLAPI.hlCacheGetBooleanv"/>
        /// <see cref="HLAPI.hlCacheGetDoublev"/>
        /// </summary>
        /// <param name="pname">要查询的参数(状态值)的名称</param>
        /// <param name="value">返回要查询的参数值的地址</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM">如果pname不是列出的值之一</exception>
        /// <exception cref="HLErrorCodes.HD_INVALID_HANDLE">设备句柄无效</exception>
        public static void hlGetDoublev(HLGetParameters pname, IntPtr value) { _hlGetDoublev(DLLEnumToIntPtr(pname), value); }
        [DllImport(DLL_PATH, EntryPoint = "hlGetDoublev")]
        private static extern void _hlGetDoublev(IntPtr pname, IntPtr value);

        /// <summary>
        /// 允许查询触觉渲染器的不同状态值
        /// <para>查询触觉渲染器的状态</para>
        /// <see cref="HLAPI.hlIsEnabled"/>
        /// <see cref="HLAPI.hlCacheGetBooleanv"/>
        /// <see cref="HLAPI.hlCacheGetDoublev"/>
        /// </summary>
        /// <param name="pname">要查询的参数(状态值)的名称</param>
        /// <param name="value">返回要查询的参数值的地址</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM">如果pname不是列出的值之一</exception>
        /// <exception cref="HLErrorCodes.HD_INVALID_HANDLE">设备句柄无效</exception>
        public static void hlGetDoublev(HLGetParameters pname, double[] value) { _hlGetDoublev(DLLEnumToIntPtr(pname), value); }
        [DllImport(DLL_PATH, EntryPoint = "hlGetDoublev")]
        private static extern void _hlGetDoublev(IntPtr pname, double[] value);

        /// <summary>
        /// 允许查询触觉渲染器的不同状态值
        /// <para>查询触觉渲染器的状态</para>
        /// <see cref="HLAPI.hlIsEnabled"/>
        /// <see cref="HLAPI.hlCacheGetBooleanv"/>
        /// <see cref="HLAPI.hlCacheGetDoublev"/>
        /// </summary>
        /// <param name="pname">要查询的参数(状态值)的名称</param>
        /// <param name="value">返回要查询的参数值的地址</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM">如果pname不是列出的值之一</exception>
        /// <exception cref="HLErrorCodes.HD_INVALID_HANDLE">设备句柄无效</exception>
        public static void hlGetDoublev(HLGetParameters pname, ref double value) { _hlGetDoublev(DLLEnumToIntPtr(pname), ref value); }
        [DllImport(DLL_PATH, EntryPoint = "hlGetDoublev")]
        private static extern void _hlGetDoublev(IntPtr pname, ref double value);

        /// <summary>
        /// 允许查询触觉渲染器的不同状态值
        /// <para>查询触觉渲染器的状态</para>
        /// <see cref="HLAPI.hlIsEnabled"/>
        /// <see cref="HLAPI.hlCacheGetBooleanv"/>
        /// <see cref="HLAPI.hlCacheGetDoublev"/>
        /// </summary>
        /// <param name="pname">要查询的参数(状态值)的名称</param>
        /// <param name="value">返回要查询的参数值的地址</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM">如果pname不是列出的值之一</exception>
        /// <exception cref="HLErrorCodes.HD_INVALID_HANDLE">设备句柄无效</exception>
        public static void hlGetIntegerv(HLGetParameters pname, IntPtr value) { _hlGetIntegerv(DLLEnumToIntPtr(pname), value); }
        [DllImport(DLL_PATH, EntryPoint = "hlGetIntegerv")]
        private static extern void _hlGetIntegerv(IntPtr pname, IntPtr value);

        /// <summary>
        /// 允许查询触觉渲染器的不同状态值
        /// <para>查询触觉渲染器的状态</para>
        /// <see cref="HLAPI.hlIsEnabled"/>
        /// <see cref="HLAPI.hlCacheGetBooleanv"/>
        /// <see cref="HLAPI.hlCacheGetDoublev"/>
        /// </summary>
        /// <param name="pname">要查询的参数(状态值)的名称</param>
        /// <param name="value">返回要查询的参数值的地址</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM">如果pname不是列出的值之一</exception>
        /// <exception cref="HLErrorCodes.HD_INVALID_HANDLE">设备句柄无效</exception>
        public static void hlGetIntegerv(HLGetParameters pname, int[] value) { _hlGetIntegerv(DLLEnumToIntPtr(pname), value); }
        [DllImport(DLL_PATH, EntryPoint = "hlGetIntegerv")]
        private static extern void _hlGetIntegerv(IntPtr pname, int[] value);

        /// <summary>
        /// 允许查询触觉渲染器的不同状态值
        /// <para>查询触觉渲染器的状态</para>
        /// <see cref="HLAPI.hlIsEnabled"/>
        /// <see cref="HLAPI.hlCacheGetBooleanv"/>
        /// <see cref="HLAPI.hlCacheGetDoublev"/>
        /// </summary>
        /// <param name="pname">要查询的参数(状态值)的名称</param>
        /// <param name="value">返回要查询的参数值的地址</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM">如果pname不是列出的值之一</exception>
        /// <exception cref="HLErrorCodes.HD_INVALID_HANDLE">设备句柄无效</exception>
        public static void hlGetIntegerv(HLGetParameters pname, ref int value) { _hlGetIntegerv(DLLEnumToIntPtr(pname), ref value); }
        [DllImport(DLL_PATH, EntryPoint = "hlGetIntegerv")]
        private static extern void _hlGetIntegerv(IntPtr pname, ref int value);

        /// <summary>
        /// hlBeginFrame()更新触觉设备的当前状态，并清除当前呈现的触觉原语集，为呈现新的或更新的原语集做准备。所有触觉原始渲染函数(即形状和效果)都必须在开始/结束帧对中完成。
        /// <para>hlBeginFrame()还更新触觉渲染引擎使用的世界坐标参考系。默认情况下，hlBeginFrame()从OpenGL®中采样当前GL_MODELVIEW_MATRIX，为整个触觉框架提供一个世界坐标空间。</para>
        /// <para>客户端或冲突线程中通过hlGet*()或hlCacheGet*()查询的所有位置、向量和转换都将转换到该世界坐标空间。通常，GL_MODELVIEW_MATRIX只包含呈现传递开始时的视图转换。</para>
        /// </summary>
        [DllImport(DLL_PATH, EntryPoint = "hlBeginFrame")]
        public static extern void hlBeginFrame();

        /// <summary>
        /// hlEndFrame()将自最后一个hlBeginFrame()以来指定的一组触觉原语(即形状、效果)刷新到触觉设备。所有触觉原始渲染函数必须在开始/结束帧对中完成。
        /// </summary>
        [DllImport(DLL_PATH, EntryPoint = "hlEndFrame")]
        public static extern void hlEndFrame();

        [DllImport(DLL_PATH, EntryPoint = "hlBegin")]
        public static extern void hlBegin(IntPtr mode);

        [DllImport(DLL_PATH, EntryPoint = "hlEnd")]
        public static extern void hlEnd();

        [DllImport(DLL_PATH, EntryPoint = "hlFrontFace")]
        public static extern void hlFrontFace(IntPtr face);
        #endregion


        //====================================================CACHED STATE ACCESSORS
        #region hlCacheGetBooleanv/hlCacheGetDoublev
        /// <summary>
        /// 这些函数允许从触觉呈现器状态的缓存版本查询不同的状态值。缓存的呈现器状态被传递到事件和效果回调函数中，并包含与回调函数使用相关的呈现器状态。
        /// <para>由于触觉渲染器的多线程实现，缓存非常重要。在事件发生的时间和调用回调的时间之间，状态可能会发生变化。</para>
        /// <see cref="HLAPI.hlGetBooleanv"/>
        /// <see cref="HLAPI.hlGetDoublev"/>
        /// <see cref="HLAPI.hlGetIntegerv"/>
        /// <see cref="HLAPI.hlAddEventCallback"/>
        /// </summary>
        /// <param name="cache">查询状态缓存参数</param>
        /// <param name="pname">要查询的参数(状态值)的名称</param>
        /// <param name="value">返回正在查询的参数值的地址</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM">如果param不是列出的值之一</exception>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION">如果没有活动的触觉呈现上下文</exception>
        public static void hlCacheGetBooleanv(IntPtr cache, HLCacheGetParameters pname, IntPtr value) { _hlCacheGetBooleanv(cache, DLLEnumToIntPtr(pname), value); }
        [DllImport(DLL_PATH, EntryPoint = "hlCacheGetBooleanv")]
        private static extern void _hlCacheGetBooleanv(IntPtr cache, IntPtr pname, IntPtr value);

        /// <summary>
        /// 这些函数允许从触觉呈现器状态的缓存版本查询不同的状态值。缓存的呈现器状态被传递到事件和效果回调函数中，并包含与回调函数使用相关的呈现器状态。
        /// <para>由于触觉渲染器的多线程实现，缓存非常重要。在事件发生的时间和调用回调的时间之间，状态可能会发生变化。</para>
        /// <see cref="HLAPI.hlGetBooleanv"/>
        /// <see cref="HLAPI.hlGetDoublev"/>
        /// <see cref="HLAPI.hlGetIntegerv"/>
        /// <see cref="HLAPI.hlAddEventCallback"/>
        /// </summary>
        /// <param name="cache">查询状态缓存参数</param>
        /// <param name="pname">要查询的参数(状态值)的名称</param>
        /// <param name="value">返回正在查询的参数值的地址</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM">如果param不是列出的值之一</exception>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION">如果没有活动的触觉呈现上下文</exception>
        public static void hlCacheGetBooleanv(IntPtr cache, HLCacheGetParameters pname, byte[] value) { _hlCacheGetBooleanv(cache, DLLEnumToIntPtr(pname), value); }
        [DllImport(DLL_PATH, EntryPoint = "hlCacheGetBooleanv")]
        private static extern void _hlCacheGetBooleanv(IntPtr cache, IntPtr pname, byte[] value);

        /// <summary>
        /// 这些函数允许从触觉呈现器状态的缓存版本查询不同的状态值。缓存的呈现器状态被传递到事件和效果回调函数中，并包含与回调函数使用相关的呈现器状态。
        /// <para>由于触觉渲染器的多线程实现，缓存非常重要。在事件发生的时间和调用回调的时间之间，状态可能会发生变化。</para>
        /// <see cref="HLAPI.hlGetBooleanv"/>
        /// <see cref="HLAPI.hlGetDoublev"/>
        /// <see cref="HLAPI.hlGetIntegerv"/>
        /// <see cref="HLAPI.hlAddEventCallback"/>
        /// </summary>
        /// <param name="cache">查询状态缓存参数</param>
        /// <param name="pname">要查询的参数(状态值)的名称</param>
        /// <param name="value">返回正在查询的参数值的地址</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM">如果param不是列出的值之一</exception>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION">如果没有活动的触觉呈现上下文</exception>
        public static void hlCacheGetBooleanv(IntPtr cache, HLCacheGetParameters pname, ref byte value) { _hlCacheGetBooleanv(cache, DLLEnumToIntPtr(pname), ref value); }
        [DllImport(DLL_PATH, EntryPoint = "hlCacheGetBooleanv")]
        private static extern void _hlCacheGetBooleanv(IntPtr cache, IntPtr pname, ref byte value);


        /// <summary>
        /// 这些函数允许从触觉呈现器状态的缓存版本查询不同的状态值。缓存的呈现器状态被传递到事件和效果回调函数中，并包含与回调函数使用相关的呈现器状态。
        /// <para>由于触觉渲染器的多线程实现，缓存非常重要。在事件发生的时间和调用回调的时间之间，状态可能会发生变化。</para>
        /// <see cref="HLAPI.hlGetBooleanv"/>
        /// <see cref="HLAPI.hlGetDoublev"/>
        /// <see cref="HLAPI.hlGetIntegerv"/>
        /// <see cref="HLAPI.hlAddEventCallback"/>
        /// </summary>
        /// <param name="cache">查询状态缓存参数</param>
        /// <param name="pname">要查询的参数(状态值)的名称</param>
        /// <param name="value">返回正在查询的参数值的地址</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM">如果param不是列出的值之一</exception>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION">如果没有活动的触觉呈现上下文</exception>
        public static void hlCacheGetDoublev(IntPtr cache, HLCacheGetParameters pname, IntPtr value) { _hlCacheGetDoublev(cache, DLLEnumToIntPtr(pname), value); }
        [DllImport(DLL_PATH, EntryPoint = "hlCacheGetDoublev")]
        private static extern void _hlCacheGetDoublev(IntPtr cache, IntPtr pname, IntPtr value);

        /// <summary>
        /// 这些函数允许从触觉呈现器状态的缓存版本查询不同的状态值。缓存的呈现器状态被传递到事件和效果回调函数中，并包含与回调函数使用相关的呈现器状态。
        /// <para>由于触觉渲染器的多线程实现，缓存非常重要。在事件发生的时间和调用回调的时间之间，状态可能会发生变化。</para>
        /// <see cref="HLAPI.hlGetBooleanv"/>
        /// <see cref="HLAPI.hlGetDoublev"/>
        /// <see cref="HLAPI.hlGetIntegerv"/>
        /// <see cref="HLAPI.hlAddEventCallback"/>
        /// </summary>
        /// <param name="cache">查询状态缓存参数</param>
        /// <param name="pname">要查询的参数(状态值)的名称</param>
        /// <param name="value">返回正在查询的参数值的地址</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM">如果param不是列出的值之一</exception>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION">如果没有活动的触觉呈现上下文</exception>
        public static void hlCacheGetDoublev(IntPtr cache, HLCacheGetParameters pname, double[] value) { _hlCacheGetDoublev(cache, DLLEnumToIntPtr(pname), value); }
        [DllImport(DLL_PATH, EntryPoint = "hlCacheGetDoublev")]
        private static extern void _hlCacheGetDoublev(IntPtr cache, IntPtr pname, double[] value);

        /// <summary>
        /// 这些函数允许从触觉呈现器状态的缓存版本查询不同的状态值。缓存的呈现器状态被传递到事件和效果回调函数中，并包含与回调函数使用相关的呈现器状态。
        /// <para>由于触觉渲染器的多线程实现，缓存非常重要。在事件发生的时间和调用回调的时间之间，状态可能会发生变化。</para>
        /// <see cref="HLAPI.hlGetBooleanv"/>
        /// <see cref="HLAPI.hlGetDoublev"/>
        /// <see cref="HLAPI.hlGetIntegerv"/>
        /// <see cref="HLAPI.hlAddEventCallback"/>
        /// </summary>
        /// <param name="cache">查询状态缓存参数</param>
        /// <param name="pname">要查询的参数(状态值)的名称</param>
        /// <param name="value">返回正在查询的参数值的地址</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM">如果param不是列出的值之一</exception>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION">如果没有活动的触觉呈现上下文</exception>
        public static void hlCacheGetDoublev(IntPtr cache, HLCacheGetParameters pname, ref double value) { _hlCacheGetDoublev(cache, DLLEnumToIntPtr(pname), ref value); }
        [DllImport(DLL_PATH, EntryPoint = "hlCacheGetDoublev")]
        private static extern void _hlCacheGetDoublev(IntPtr cache, IntPtr pname, ref double value);

        #endregion


        //====================================================SHAPES
        #region Shapes
        /// <summary>
        /// 指示后续几何命令将作为所指示形状的一部分发送到触觉呈现程序，直到调用hlEndShape()。
        /// 几何图元只有在hlBeginShape()/hlEndShape()块中指定时才可以发送给触觉渲染器。
        /// 对几何图元进行分组很重要，原因有二:
        /// <para>1.事件回调将报告所触及的几何图元的形状id</para>
        /// <para>2.通过观察基于形状id的帧与帧之间的差异，可以对动态移动的几何图元进行正确的触觉呈现。</para>
        /// <para>在向触觉渲染器指定几何形状和回调函数之前使用。</para>
        /// <see cref="HLAPI.hlEndShape"/>
        /// <see cref="HLAPI.hlHint"/>
        /// <see cref="HLAPI.hlHint"/>
        /// <see cref="HLAPI.hlCallback"/>
        /// </summary>
        /// <param name="type">要指定的形状的类型</param>
        /// <param name="shape">指定从先前调用 hlGenShapes() 返回的形状的id</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM"></exception>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION"></exception>
        public static void hlBeginShape(HLBeginShapeParams type, uint shape) { _hlBeginShape(DLLEnumToIntPtr(type), shape); }
        [DllImport(DLL_PATH, EntryPoint = "hlBeginShape")]
        private static extern void _hlBeginShape(IntPtr type, uint shape);

        /// <summary>
        /// 完成最后一次调用hlBeginShape()指定的形状。几何、材料、变换和形状的其他状态被捕获并发送到触觉渲染器。
        /// <para>调用hlBeginShape()之后</para>
        /// <see cref="HLAPI.hlBeginShape"/>
        /// </summary>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION">如果不在hlBeginFrame()/hlEndFrame()或hlBeginShape()/ hlEndShape() 代码块中</exception>
        [DllImport(DLL_PATH, EntryPoint = "hlEndShape")]
        public static extern void hlEndShape();

        /// <summary>
        /// hl Call Shape
        /// </summary>
        /// <param name="shape"></param>
        [DllImport(DLL_PATH, EntryPoint = "hlCallShape")]
        public static extern void hlCallShape(uint shape);

        /// <summary>
        /// 释放hlGenShapes创建的惟一标识符。
        /// <para>删除范围[shape, shape+range-1]中的所有连续形状标识符。</para>
        /// <see cref="HLAPI.hlGenShapes"/>
        /// <see cref="HLAPI.hlBeginShape"/>
        /// <see cref="HLAPI.hlIsShape"/>
        /// </summary>
        /// <param name="shape">要删除的第一个形状的ID</param>
        /// <param name="range">要生成的连续惟一标识符的数目</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_VALUE">如果要释放的任何标识符以前都不是由glGenShapes()分配的</exception>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION">如果没有活动的触觉呈现上下文</exception>
        [DllImport(DLL_PATH, EntryPoint = "hlDeleteShapes")]
        public static extern void hlDeleteShapes(uint shape, int range);

        /// <summary>
        /// 确定标识符是否是有效的形状标识符。
        /// <para></para>
        /// <see cref="HLAPI.hlGenShapes"/>
        /// <see cref="HLAPI.hlBeginShape"/>
        /// <see cref="HLAPI.hlIsShape"/>
        /// </summary>
        /// <param name="shape"></param>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION"></exception>
        /// <returns>如果形状标识符是有效的分配值，则返回 TRUE（0x01）。</returns>
        [DllImport(DLL_PATH, EntryPoint = "hlIsShape")]
        public static extern byte hlIsShape(uint shape);

        /// <summary>
        /// 对于形状，生成可与hlBeginShape()一起使用的唯一标识符。
        /// <para>在调用hlBeginShape()为新形状创建唯一标识符之前。</para>
        /// <see cref="HLAPI.hlBeginShape"/>
        /// <see cref="HLAPI.hlDeleteShapes"/>
        /// <see cref="HLAPI.hlIsShape"/>
        /// </summary>
        /// <param name="range"></param>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION"></exception>
        /// <returns>返回可以用作形状标识符的唯一整数。如果范围大于1，则返回值表示一系列范围连续惟一标识符中的第一个。</returns>
        [DllImport(DLL_PATH, EntryPoint = "hlGenShapes")]
        public static extern uint hlGenShapes(int range);

        /// <summary>
        /// 为触觉渲染器指定局部特征几何体
        /// <para></para>
        /// <see cref="HLAPI.hlBeginShape, hlEndShape"/>
        /// <see cref="HLAPI.hlEndShape"/>
        /// </summary>
        /// <param name="geom">容器中的局部特性作为参数传入回调形状最近的特性回调函数。</param>
        /// <param name="type">要创建的本地特性的类型</param>
        /// <param name="v">一个向量，给定在局部坐标的形状，其中定义了局部特征</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM"></exception>
        public static void hlLocalFeature1fv(IntPtr geom, HLLocalFeatureTypes type, IntPtr v) { _hlLocalFeature1fv(geom, DLLEnumToIntPtr(type), v); }
        [DllImport(DLL_PATH, EntryPoint = "hlLocalFeature1fv")]
        private static extern void _hlLocalFeature1fv(IntPtr geom, IntPtr type, IntPtr v);

        /// <summary>
        /// 为触觉渲染器指定局部特征几何体
        /// <para></para>
        /// <see cref="HLAPI.hlBeginShape, hlEndShape"/>
        /// <see cref="HLAPI.hlEndShape"/>
        /// </summary>
        /// <param name="geom">容器中的局部特性作为参数传入回调形状最近的特性回调函数。</param>
        /// <param name="type">要创建的本地特性的类型</param>
        /// <param name="v">一个向量，给定在局部坐标的形状，其中定义了局部特征</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM"></exception>
        public static void hlLocalFeature1fv(IntPtr geom, HLLocalFeatureTypes type, float[] v) { _hlLocalFeature1fv(geom, DLLEnumToIntPtr(type), v); }
        [DllImport(DLL_PATH, EntryPoint = "hlLocalFeature1fv")]
        private static extern void _hlLocalFeature1fv(IntPtr geom, IntPtr type, float[] v);

        /// <summary>
        /// 为触觉渲染器指定局部特征几何体
        /// <para></para>
        /// <see cref="HLAPI.hlBeginShape, hlEndShape"/>
        /// <see cref="HLAPI.hlEndShape"/>
        /// </summary>
        /// <param name="geom">容器中的局部特性作为参数传入回调形状最近的特性回调函数。</param>
        /// <param name="type">要创建的本地特性的类型</param>
        /// <param name="v">一个向量，给定在局部坐标的形状，其中定义了局部特征</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM"></exception>
        public static void hlLocalFeature1fv(IntPtr geom, HLLocalFeatureTypes type, ref float v) { _hlLocalFeature1fv(geom, DLLEnumToIntPtr(type), ref v); }
        [DllImport(DLL_PATH, EntryPoint = "hlLocalFeature1fv")]
        private static extern void _hlLocalFeature1fv(IntPtr geom, IntPtr type, ref float v);

        /// <summary>
        /// 为触觉渲染器指定局部特征几何体
        /// <para></para>
        /// <see cref="HLAPI.hlBeginShape, hlEndShape"/>
        /// <see cref="HLAPI.hlEndShape"/>
        /// </summary>
        /// <param name="geom">容器中的局部特性作为参数传入回调形状最近的特性回调函数。</param>
        /// <param name="type">要创建的本地特性的类型</param>
        /// <param name="v">一个向量，给定在局部坐标的形状，其中定义了局部特征</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM"></exception>
        public static void hlLocalFeature1dv(IntPtr geom, HLLocalFeatureTypes type, IntPtr v) { _hlLocalFeature1dv(geom, DLLEnumToIntPtr(type), v); }
        [DllImport(DLL_PATH, EntryPoint = "hlLocalFeature1dv")]
        private static extern void _hlLocalFeature1dv(IntPtr geom, IntPtr type, IntPtr v);

        /// <summary>
        /// 为触觉渲染器指定局部特征几何体
        /// <para></para>
        /// <see cref="HLAPI.hlBeginShape, hlEndShape"/>
        /// <see cref="HLAPI.hlEndShape"/>
        /// </summary>
        /// <param name="geom">容器中的局部特性作为参数传入回调形状最近的特性回调函数。</param>
        /// <param name="type">要创建的本地特性的类型</param>
        /// <param name="v">一个向量，给定在局部坐标的形状，其中定义了局部特征</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM"></exception>
        public static void hlLocalFeature1dv(IntPtr geom, HLLocalFeatureTypes type, double[] v) { _hlLocalFeature1dv(geom, DLLEnumToIntPtr(type), v); }
        [DllImport(DLL_PATH, EntryPoint = "hlLocalFeature1dv")]
        private static extern void _hlLocalFeature1dv(IntPtr geom, IntPtr type, double[] v);

        /// <summary>
        /// 为触觉渲染器指定局部特征几何体
        /// <para></para>
        /// <see cref="HLAPI.hlBeginShape, hlEndShape"/>
        /// <see cref="HLAPI.hlEndShape"/>
        /// </summary>
        /// <param name="geom">容器中的局部特性作为参数传入回调形状最近的特性回调函数。</param>
        /// <param name="type">要创建的本地特性的类型</param>
        /// <param name="v">一个向量，给定在局部坐标的形状，其中定义了局部特征</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM"></exception>  
        public static void hlLocalFeature1dv(IntPtr geom, HLLocalFeatureTypes type, ref double v) { _hlLocalFeature1dv(geom, DLLEnumToIntPtr(type), ref v); }
        [DllImport(DLL_PATH, EntryPoint = "hlLocalFeature1dv")]
        private static extern void _hlLocalFeature1dv(IntPtr geom, IntPtr type, ref double v);

        /// <summary>
        /// 为触觉渲染器指定局部特征几何体
        /// <para></para>
        /// <see cref="HLAPI.hlBeginShape, hlEndShape"/>
        /// <see cref="HLAPI.hlEndShape"/>
        /// </summary>
        /// <param name="geom">容器中的局部特性作为参数传入回调形状最近的特性回调函数。</param>
        /// <param name="type">要创建的本地特性的类型</param>
        /// <param name="v1">向量</param>
        /// <param name="v2">向量</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM"></exception>
        public static void hlLocalFeature2fv(IntPtr geom, HLLocalFeatureTypes type, IntPtr v1, IntPtr v2) { _hlLocalFeature2fv(geom, DLLEnumToIntPtr(type), v1, v2); }
        [DllImport(DLL_PATH, EntryPoint = "hlLocalFeature2fv")]
        private static extern void _hlLocalFeature2fv(IntPtr geom, IntPtr type, IntPtr v1, IntPtr v2);

        /// <summary>
        /// 为触觉渲染器指定局部特征几何体
        /// <para></para>
        /// <see cref="HLAPI.hlBeginShape, hlEndShape"/>
        /// <see cref="HLAPI.hlEndShape"/>
        /// </summary>
        /// <param name="geom">容器中的局部特性作为参数传入回调形状最近的特性回调函数。</param>
        /// <param name="type">要创建的本地特性的类型</param>
        /// <param name="v1">向量</param>
        /// <param name="v2">向量</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM"></exception>
        public static void hlLocalFeature2fv(IntPtr geom, HLLocalFeatureTypes type, float[] v1, float[] v2) { _hlLocalFeature2fv(geom, DLLEnumToIntPtr(type), v1, v2); }
        [DllImport(DLL_PATH, EntryPoint = "hlLocalFeature2fv")]
        private static extern void _hlLocalFeature2fv(IntPtr geom, IntPtr type, float[] v1, float[] v2);

        /// <summary>
        /// 为触觉渲染器指定局部特征几何体
        /// <para></para>
        /// <see cref="HLAPI.hlBeginShape, hlEndShape"/>
        /// <see cref="HLAPI.hlEndShape"/>
        /// </summary>
        /// <param name="geom">容器中的局部特性作为参数传入回调形状最近的特性回调函数。</param>
        /// <param name="type">要创建的本地特性的类型</param>
        /// <param name="v1">向量</param>
        /// <param name="v2">向量</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM"></exception>
        public static void hlLocalFeature2fv(IntPtr geom, HLLocalFeatureTypes type, ref float v1, ref float v2) { _hlLocalFeature2fv(geom, DLLEnumToIntPtr(type), ref v1, ref v2); }
        [DllImport(DLL_PATH, EntryPoint = "hlLocalFeature2fv")]
        private static extern void _hlLocalFeature2fv(IntPtr geom, IntPtr type, ref float v1, ref float v2);

        /// <summary>
        /// 为触觉渲染器指定局部特征几何体
        /// <para></para>
        /// <see cref="HLAPI.hlBeginShape, hlEndShape"/>
        /// <see cref="HLAPI.hlEndShape"/>
        /// </summary>
        /// <param name="geom">容器中的局部特性作为参数传入回调形状最近的特性回调函数。</param>
        /// <param name="type">要创建的本地特性的类型</param>
        /// <param name="v1">向量</param>
        /// <param name="v2">向量</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM"></exception>
        public static void hlLocalFeature2dv(IntPtr geom, HLLocalFeatureTypes type, IntPtr v1, IntPtr v2) { _hlLocalFeature2dv(geom, DLLEnumToIntPtr(type), v1, v2); }
        [DllImport(DLL_PATH, EntryPoint = "hlLocalFeature2dv")]
        private static extern void _hlLocalFeature2dv(IntPtr geom, IntPtr type, IntPtr v1, IntPtr v2);

        /// <summary>
        /// 为触觉渲染器指定局部特征几何体
        /// <para></para>
        /// <see cref="HLAPI.hlBeginShape, hlEndShape"/>
        /// <see cref="HLAPI.hlEndShape"/>
        /// </summary>
        /// <param name="geom">容器中的局部特性作为参数传入回调形状最近的特性回调函数。</param>
        /// <param name="type">要创建的本地特性的类型</param>
        /// <param name="v1">向量</param>
        /// <param name="v2">向量</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM"></exception>
        public static void hlLocalFeature2dv(IntPtr geom, HLLocalFeatureTypes type, double[] v1, double[] v2) { _hlLocalFeature2dv(geom, DLLEnumToIntPtr(type), v1, v2); }
        [DllImport(DLL_PATH, EntryPoint = "hlLocalFeature2dv")]
        private static extern void _hlLocalFeature2dv(IntPtr geom, IntPtr type, double[] v1, double[] v2);

        /// <summary>
        /// 为触觉渲染器指定局部特征几何体
        /// <para></para>
        /// <see cref="HLAPI.hlBeginShape, hlEndShape"/>
        /// <see cref="HLAPI.hlEndShape"/>
        /// </summary>
        /// <param name="geom">容器中的局部特性作为参数传入回调形状最近的特性回调函数。</param>
        /// <param name="type">要创建的本地特性的类型</param>
        /// <param name="v1">向量</param>
        /// <param name="v2">向量</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM"></exception>
        public static void hlLocalFeature2dv(IntPtr geom, HLLocalFeatureTypes type, ref double v1, ref double v2) { _hlLocalFeature2dv(geom, DLLEnumToIntPtr(type), ref v1, ref v2); }
        [DllImport(DLL_PATH, EntryPoint = "hlLocalFeature2dv")]
        private static extern void _hlLocalFeature2dv(IntPtr geom, IntPtr type, ref double v1, ref double v2);

        /// <summary>
        /// 允许查询特定形状的状态
        /// <para>使用标识符shapeid从触觉呈现引擎查询形状的状态。此形状必须在最后一帧中呈现。</para>
        /// <see cref="HLAPI.hlBeginShape"/>
        /// <see cref="HLAPI.hlEndShape"/>
        /// <see cref="HLAPI.hlGetBooleanv"/>
        /// <see cref="HLAPI.hlGetDoublev"/>
        /// <see cref="HLAPI.hlGetIntegerv"/>
        /// </summary>
        /// <param name="shapeId">要查询的形状标识符</param>
        /// <param name="pname">要查询的参数(状态值)的名称</param>
        /// <param name="value">返回正在查询的参数值的地址。</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM"></exception>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION"></exception>
        public static void hlGetShapeBooleanv(uint shapeId, HLGetShapeParams pname, IntPtr value) { _hlGetShapeBooleanv(shapeId, DLLEnumToIntPtr(pname), value); }
        [DllImport(DLL_PATH, EntryPoint = "hlGetShapeBooleanv")]
        private static extern void _hlGetShapeBooleanv(uint shapeId, IntPtr pname, IntPtr value);

        /// <summary>
        /// 允许查询特定形状的状态
        /// <para>使用标识符shapeid从触觉呈现引擎查询形状的状态。此形状必须在最后一帧中呈现。</para>
        /// <see cref="HLAPI.hlBeginShape"/>
        /// <see cref="HLAPI.hlEndShape"/>
        /// <see cref="HLAPI.hlGetBooleanv"/>
        /// <see cref="HLAPI.hlGetDoublev"/>
        /// <see cref="HLAPI.hlGetIntegerv"/>
        /// </summary>
        /// <param name="shapeId">要查询的形状标识符</param>
        /// <param name="pname">要查询的参数(状态值)的名称</param>
        /// <param name="value">返回正在查询的参数值的地址。</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM"></exception>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION"></exception>
        public static void hlGetShapeBooleanv(uint shapeId, HLGetShapeParams pname, byte[] value) { _hlGetShapeBooleanv(shapeId, DLLEnumToIntPtr(pname), value); }
        [DllImport(DLL_PATH, EntryPoint = "hlGetShapeBooleanv")]
        private static extern void _hlGetShapeBooleanv(uint shapeId, IntPtr pname, byte[] value);

        /// <summary>
        /// 允许查询特定形状的状态
        /// <para>使用标识符shapeid从触觉呈现引擎查询形状的状态。此形状必须在最后一帧中呈现。</para>
        /// <see cref="HLAPI.hlBeginShape"/>
        /// <see cref="HLAPI.hlEndShape"/>
        /// <see cref="HLAPI.hlGetBooleanv"/>
        /// <see cref="HLAPI.hlGetDoublev"/>
        /// <see cref="HLAPI.hlGetIntegerv"/>
        /// </summary>
        /// <param name="shapeId">要查询的形状标识符</param>
        /// <param name="pname">要查询的参数(状态值)的名称</param>
        /// <param name="value">返回正在查询的参数值的地址</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM"></exception>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION"></exception>
        public static void hlGetShapeBooleanv(uint shapeId, HLGetShapeParams pname, ref byte value) { _hlGetShapeBooleanv(shapeId, DLLEnumToIntPtr(pname), ref value); }
        [DllImport(DLL_PATH, EntryPoint = "hlGetShapeBooleanv")]
        private static extern void _hlGetShapeBooleanv(uint shapeId, IntPtr pname, ref byte value);

        /// <summary>
        /// 允许查询特定形状的状态
        /// <para>使用标识符shapeid从触觉呈现引擎查询形状的状态。此形状必须在最后一帧中呈现。</para>
        /// <see cref="HLAPI.hlBeginShape"/>
        /// <see cref="HLAPI.hlEndShape"/>
        /// <see cref="HLAPI.hlGetBooleanv"/>
        /// <see cref="HLAPI.hlGetDoublev"/>
        /// <see cref="HLAPI.hlGetIntegerv"/>
        /// </summary>
        /// <param name="shapeId">要查询的形状标识符</param>
        /// <param name="pname">要查询的参数(状态值)的名称</param>
        /// <param name="value">返回正在查询的参数值的地址</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM"></exception>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION"></exception>
        public static void hlGetShapeDoublev(uint shapeId, HLGetShapeParams pname, IntPtr value) { _hlGetShapeDoublev(shapeId, DLLEnumToIntPtr(pname), value); }
        [DllImport(DLL_PATH, EntryPoint = "hlGetShapeDoublev")]
        private static extern void _hlGetShapeDoublev(uint shapeId, IntPtr pname, IntPtr value);

        /// <summary>
        /// 允许查询特定形状的状态
        /// <para>使用标识符shapeid从触觉呈现引擎查询形状的状态。此形状必须在最后一帧中呈现。</para>
        /// <see cref="HLAPI.hlBeginShape"/>
        /// <see cref="HLAPI.hlEndShape"/>
        /// <see cref="HLAPI.hlGetBooleanv"/>
        /// <see cref="HLAPI.hlGetDoublev"/>
        /// <see cref="HLAPI.hlGetIntegerv"/>
        /// </summary>
        /// <param name="shapeId">要查询的形状标识符</param>
        /// <param name="pname">要查询的参数(状态值)的名称</param>
        /// <param name="value">返回正在查询的参数值的地址</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM"></exception>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION"></exception>
        public static void hlGetShapeDoublev(uint shapeId, HLGetShapeParams pname, double[] value) { _hlGetShapeDoublev(shapeId, DLLEnumToIntPtr(pname), value); }
        [DllImport(DLL_PATH, EntryPoint = "hlGetShapeDoublev")]
        private static extern void _hlGetShapeDoublev(uint shapeId, IntPtr pname, double[] value);

        /// <summary>
        /// 允许查询特定形状的状态
        /// <para>使用标识符shapeid从触觉呈现引擎查询形状的状态。此形状必须在最后一帧中呈现。</para>
        /// <see cref="HLAPI.hlBeginShape"/>
        /// <see cref="HLAPI.hlEndShape"/>
        /// <see cref="HLAPI.hlGetBooleanv"/>
        /// <see cref="HLAPI.hlGetDoublev"/>
        /// <see cref="HLAPI.hlGetIntegerv"/>
        /// </summary>
        /// <param name="shapeId">要查询的形状标识符</param>
        /// <param name="pname">要查询的参数(状态值)的名称</param>
        /// <param name="value">返回正在查询的参数值的地址</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM"></exception>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION"></exception>
        public static void hlGetShapeDoublev(uint shapeId, HLGetShapeParams pname, ref double value) { _hlGetShapeDoublev(shapeId, DLLEnumToIntPtr(pname), ref value); }
        [DllImport(DLL_PATH, EntryPoint = "hlGetShapeDoublev")]
        private static extern void _hlGetShapeDoublev(uint shapeId, IntPtr pname, ref double value);

        #endregion


        //====================================================MATERIAL AND SURFACE PROPERTIES
        #region Material and surface properties
        /// <summary>
        /// Gets the current haptic material properties for shapes.
        /// <para>用于查找用于呈现形状的材料属性。</para>
        /// <see cref="HLAPI.hlMaterialf"/>
        /// <see cref="HLAPI.hlPushAttrib"/>
        /// <see cref="HLAPI.hlPopAttrib"/>
        /// </summary>
        /// <param name="face">Face(s) to apply this material to.</param>
        /// <param name="pname">Material property to set.</param>
        /// <param name="value">New value for material property.</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM"></exception>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION"></exception>
        public static void hlGetMaterialfv(HLGetMaterialfvFaces face, HLGetMaterialfvParams pname, IntPtr value) { _hlGetMaterialfv(DLLEnumToIntPtr(face), DLLEnumToIntPtr(pname), value); }
        [DllImport(DLL_PATH, EntryPoint = "hlGetMaterialfv")]
        private static extern void _hlGetMaterialfv(IntPtr face, IntPtr pname, IntPtr value);

        /// <summary>
        /// Gets the current haptic material properties for shapes.
        /// <para>用于查找用于呈现形状的材料属性。</para>
        /// <see cref="HLAPI.hlMaterialf"/>
        /// <see cref="HLAPI.hlPushAttrib"/>
        /// <see cref="HLAPI.hlPopAttrib"/>
        /// </summary>
        /// <param name="face">Face(s) to apply this material to.</param>
        /// <param name="pname">Material property to set.</param>
        /// <param name="value">New value for material property.</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM"></exception>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION"></exception>
        public static void hlGetMaterialfv(HLGetMaterialfvFaces face, HLGetMaterialfvParams pname, float[] value) { _hlGetMaterialfv(DLLEnumToIntPtr(face), DLLEnumToIntPtr(pname), value); }
        [DllImport(DLL_PATH, EntryPoint = "hlGetMaterialfv")]
        private static extern void _hlGetMaterialfv(IntPtr face, IntPtr pname, float[] value);

        /// <summary>
        /// Gets the current haptic material properties for shapes.
        /// <para>用于查找用于呈现形状的材料属性。</para>
        /// <see cref="HLAPI.hlMaterialf"/>
        /// <see cref="HLAPI.hlPushAttrib"/>
        /// <see cref="HLAPI.hlPopAttrib"/>
        /// </summary>
        /// <param name="face">Face(s) to apply this material to.</param>
        /// <param name="pname">Material property to set.</param>
        /// <param name="value">New value for material property.</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM"></exception>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION"></exception>
        public static void hlGetMaterialfv(HLGetMaterialfvFaces face, HLGetMaterialfvParams pname, ref float value) { _hlGetMaterialfv(DLLEnumToIntPtr(face), DLLEnumToIntPtr(pname), ref value); }
        [DllImport(DLL_PATH, EntryPoint = "hlGetMaterialfv")]
        private static extern void _hlGetMaterialfv(IntPtr face, IntPtr pname, ref float value);

        /// <summary>
        /// 设置形状的触觉材料属性。可以独立设置接触形状的正面和背面三角形的材料属性。只有正面材料适用于约束
        /// <para>在定义形状以设置其材质之前使用。</para>
        /// <see cref="HLAPI.hlGetMaterialfv,"/>
        /// <see cref="HLAPI.hlPushAttrib"/>
        /// <see cref="HLAPI.hlPopAttrib"/>
        /// </summary>
        /// <param name="face">Face(s) to apply this material to.</param>
        /// <param name="pname">Material property to set. </param>
        /// <param name="value">New value for material property</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM"></exception>
        /// <exception cref="HLErrorCodes.HL_INVALID_VALUE"></exception>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION"></exception>
        public static void hlMaterialf(HLMaterialfFaces face, HLMaterialfParams pname, float value) { _hlMaterialf(DLLEnumToIntPtr(face), DLLEnumToIntPtr(pname), value); }
        [DllImport(DLL_PATH, EntryPoint = "hlMaterialf")]
        private static extern void _hlMaterialf(IntPtr face, IntPtr pname, float value);

        /// <summary>
        /// 该触觉装置可触摸形状的面集
        /// <para>形状可以在一侧或两侧触摸。使用这个函数来设置哪些边是可触摸的。反馈缓冲区和深度缓冲区形状的前端由OpenGL中设置的顶点的缠绕顺序来定义;也就是说，三角形被认为是正面朝上的 OpenGL也将被HL视为正面。</para>
        /// <para>当使用 HL_CONSTRAINT 触摸模型时，所有的形状都是可从两边触摸的，独立于可触摸的面。</para>
        /// <see cref="HLAPI.hlTouchModel"/>
        /// </summary>
        /// <param name="mode">哪些面是可以触摸的</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM"></exception>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION"></exception>
        public static void hlTouchableFace(HLTouchableFaceParams mode) { _hlTouchableFace(DLLEnumToIntPtr(mode)); }
        [DllImport(DLL_PATH, EntryPoint = "hlTouchableFace")]
        private static extern void _hlTouchableFace(IntPtr mode);

        /// <summary>
        /// 将触摸模型设置为将形状指定为接触形状或约束
        /// <para>在指定形状之前使用，以设置是否为抗穿透的接触形状或是否为将触觉装置强制置于形状表面的约束。</para>
        /// <see cref="HLAPI.hlTouchModelf"/>
        /// <see cref="HLAPI.hlPushAttrib"/>
        /// <see cref="HLAPI.hlPopAttrib"/>
        /// </summary>
        /// <param name="mode">接触或约束模型</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM"></exception>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION"></exception>
        public static void hlTouchModel(HLTouchModelParams mode) { _hlTouchModel(DLLEnumToIntPtr(mode)); }
        [DllImport(DLL_PATH, EntryPoint = "hlTouchModel")]
        private static extern void _hlTouchModel(IntPtr mode);

        /// <summary>
        /// 设置触摸模型的属性。
        /// <para>在指定形状之前使用，以设置触摸模型为该形状使用的参数。</para>
        /// <see cref="HLAPI.hlTouchModel"/>
        /// <see cref="HLAPI.hlPushAttrib"/>
        /// <see cref="HLAPI.hlPopAttrib"/>
        /// </summary>
        /// <param name="pname">触摸要修改的模型参数。</param>
        /// <param name="value">新的参数值</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM"></exception>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION"></exception>
        public static void hlTouchModelf(HLTouchModelIfParams pname, float value) { _hlTouchModelf(DLLEnumToIntPtr(pname), value); }
        [DllImport(DLL_PATH, EntryPoint = "hlTouchModelf")]
        private static extern void _hlTouchModelf(IntPtr pname, float value);

        #endregion


        //====================================================FORCE EFFECTS
        #region FORCE EFFECTS
        /// <summary>
        /// 释放hlGenEffects()创建的 effect 惟一标识符。
        /// <para>Deletes all consecutive effect identifiers starting in the range [effect, effect+range-1].</para>
        /// <see cref="HLAPI.hlGenEffects"/>
        /// <see cref="HLAPI.hlStartEffect"/>
        /// <see cref="HLAPI.hlStopEffect"/>
        /// <see cref="HLAPI.hlIsEffect"/>
        /// </summary>
        /// <param name="effect">效果的标识符</param>
        /// <param name="range">标识符的数目</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_VALUE"></exception>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION"></exception>
        [DllImport(DLL_PATH, EntryPoint = "hlDeleteEffects")]
        public static extern void hlDeleteEffects(uint effect, int range);

        /// <summary>
        /// 设置当前效果属性的值
        /// <para>设置effect属性的值，该属性将应用于下一次调用hlStartEffect()或hlTriggerEffect()时生成的效果。</para>
        /// <see cref="HLAPI.hl*Effect*()"/>
        /// </summary>
        /// <param name="pname">要设置的参数的名称</param>
        /// <param name="value">属性的新值</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM"></exception>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION">如果不在hlBeginFrame()-hlEndFrame()块中，或者在hlBeginShape()-hlEndShape()块中。</exception>
        public static void hlEffectd(HLEffectParams pname, double value) { _hlEffectd(DLLEnumToIntPtr(pname), value); }
        [DllImport(DLL_PATH, EntryPoint = "hlEffectd")]
        private static extern void _hlEffectd(IntPtr pname, double value);

        /// <summary>
        /// 设置当前效果属性的值
        /// <para>设置effect属性的值，该属性将应用于下一次调用hlStartEffect()或hlTriggerEffect()时生成的效果。</para>
        /// <see cref="HLAPI.hl*Effect*()"/>
        /// </summary>
        /// <param name="pname">要设置的参数的名称</param>
        /// <param name="value">属性的新值</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM"></exception>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION">如果不在hlBeginFrame()-hlEndFrame()块中，或者在hlBeginShape()-hlEndShape()块中。</exception>
        public static void hlEffecti(HLEffectParams pname, int value) { _hlEffecti(DLLEnumToIntPtr(pname), value); }
        [DllImport(DLL_PATH, EntryPoint = "hlEffecti")]
        private static extern void _hlEffecti(IntPtr pname, int value);

        /// <summary>
        /// 设置当前效果属性的值
        /// <para>设置effect属性的值，该属性将应用于下一次调用hlStartEffect()或hlTriggerEffect()时生成的效果。</para>
        /// <see cref="HLAPI.hl*Effect*()"/>
        /// </summary>
        /// <param name="pname">要设置的参数的名称</param>
        /// <param name="value">属性的新值</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM"></exception>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION">如果不在hlBeginFrame()-hlEndFrame()块中，或者在hlBeginShape()-hlEndShape()块中。</exception>
        public static void hlEffectdv(HLEffectParams pname, IntPtr value) { _hlEffectdv(DLLEnumToIntPtr(pname), value); }
        [DllImport(DLL_PATH, EntryPoint = "hlEffectdv")]
        private static extern void _hlEffectdv(IntPtr pname, IntPtr value);

        /// <summary>
        /// 设置当前效果属性的值
        /// <para>设置effect属性的值，该属性将应用于下一次调用hlStartEffect()或hlTriggerEffect()时生成的效果。</para>
        /// <see cref="HLAPI.hl*Effect*()"/>
        /// </summary>
        /// <param name="pname">要设置的参数的名称</param>
        /// <param name="value">属性的新值</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM"></exception>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION">如果不在hlBeginFrame()-hlEndFrame()块中，或者在hlBeginShape()-hlEndShape()块中。</exception>
        public static void hlEffectdv(HLEffectParams pname, double[] value) { _hlEffectdv(DLLEnumToIntPtr(pname), value); }
        [DllImport(DLL_PATH, EntryPoint = "hlEffectdv")]
        private static extern void _hlEffectdv(IntPtr pname, double[] value);

        /// <summary>
        /// 设置当前效果属性的值
        /// <para>设置effect属性的值，该属性将应用于下一次调用hlStartEffect()或hlTriggerEffect()时生成的效果。</para>
        /// <see cref="HLAPI.hl*Effect*()"/>
        /// </summary>
        /// <param name="pname">要设置的参数的名称</param>
        /// <param name="value">属性的新值</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM"></exception>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION">如果不在hlBeginFrame()-hlEndFrame()块中，或者在hlBeginShape()-hlEndShape()块中。</exception>
        public static void hlEffectdv(HLEffectParams pname, ref double value) { _hlEffectdv(DLLEnumToIntPtr(pname), ref value); }
        [DllImport(DLL_PATH, EntryPoint = "hlEffectdv")]
        private static extern void _hlEffectdv(IntPtr pname, ref double value);

        /// <summary>
        /// 设置当前效果属性的值
        /// <para>设置effect属性的值，该属性将应用于下一次调用hlStartEffect()或hlTriggerEffect()时生成的效果。</para>
        /// <see cref="HLAPI.hl*Effect*()"/>
        /// </summary>
        /// <param name="pname">要设置的参数的名称</param>
        /// <param name="value">属性的新值</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM"></exception>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION">如果不在hlBeginFrame()-hlEndFrame()块中，或者在hlBeginShape()-hlEndShape()块中。</exception>
        public static void hlEffectiv(HLEffectParams pname, IntPtr value) { _hlEffectiv(DLLEnumToIntPtr(pname), value); }
        [DllImport(DLL_PATH, EntryPoint = "hlEffectiv")]
        private static extern void _hlEffectiv(IntPtr pname, IntPtr value);

        /// <summary>
        /// 设置当前效果属性的值
        /// <para>设置effect属性的值，该属性将应用于下一次调用hlStartEffect()或hlTriggerEffect()时生成的效果。</para>
        /// <see cref="HLAPI.hl*Effect*()"/>
        /// </summary>
        /// <param name="pname">要设置的参数的名称</param>
        /// <param name="value">属性的新值</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM"></exception>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION">如果不在hlBeginFrame()-hlEndFrame()块中，或者在hlBeginShape()-hlEndShape()块中。</exception>
        public static void hlEffectiv(HLEffectParams pname, int[] value) { _hlEffectiv(DLLEnumToIntPtr(pname), value); }
        [DllImport(DLL_PATH, EntryPoint = "hlEffectiv")]
        private static extern void _hlEffectiv(IntPtr pname, int[] value);

        /// <summary>
        /// 设置当前效果属性的值
        /// <para>设置effect属性的值，该属性将应用于下一次调用hlStartEffect()或hlTriggerEffect()时生成的效果。</para>
        /// <see cref="HLAPI.hl*Effect*()"/>
        /// </summary>
        /// <param name="pname">要设置的参数的名称</param>
        /// <param name="value">属性的新值</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM"></exception>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION">如果不在hlBeginFrame()-hlEndFrame()块中，或者在hlBeginShape()-hlEndShape()块中。</exception>
        public static void hlEffectiv(HLEffectParams pname, ref int value) { _hlEffectiv(DLLEnumToIntPtr(pname), ref value); }
        [DllImport(DLL_PATH, EntryPoint = "hlEffectiv")]
        private static extern void _hlEffectiv(IntPtr pname, ref int value);

        /// <summary>
        /// For effect, deallocates unique identifiers created by hlGenEffects().
        /// <para>Deletes all consecutive effect identifiers starting in the range [effect, effect+range-1].</para>
        /// <see cref="HLAPI.hlGenEffects"/>
        /// <see cref="HLAPI.hlStartEffect"/>
        /// <see cref="HLAPI.hlStopEffect"/>
        /// <see cref="HLAPI.hlIsEffect"/>
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        /// <exception cref="HLErrorCodes.HL_INVALID_VALUE"></exception>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION"></exception>
        [DllImport(DLL_PATH, EntryPoint = "hlGenEffects")]
        public static extern uint hlGenEffects(int range);

        /// <summary>
        /// Gets the current value of an effect property.
        /// <para></para>
        /// <see cref="HLAPI.hlEffectd"/>
        /// <see cref="HLAPI.hlEffecti"/>
        /// <see cref="HLAPI.hlEffectdv"/>
        /// <see cref="HLAPI.hlEffectiv"/>
        /// </summary>
        /// <param name="effect"></param>
        /// <param name="pname"></param>
        /// <param name="value"></param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM"></exception>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION"></exception>
        public static void hlGetEffectdv(uint effect, HLEffectParams pname, IntPtr value) { _hlGetEffectdv(effect, DLLEnumToIntPtr(pname), value); }
        [DllImport(DLL_PATH, EntryPoint = "hlGetEffectdv")]
        private static extern void _hlGetEffectdv(uint effect, IntPtr pname, IntPtr value);

        /// <summary>
        /// Gets the current value of an effect property.
        /// <para></para>
        /// <see cref="HLAPI.hlEffectd"/>
        /// <see cref="HLAPI.hlEffecti"/>
        /// <see cref="HLAPI.hlEffectdv"/>
        /// <see cref="HLAPI.hlEffectiv"/>
        /// </summary>
        /// <param name="effect"></param>
        /// <param name="pname"></param>
        /// <param name="value"></param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM"></exception>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION"></exception>
        public static void hlGetEffectdv(uint effect, HLEffectParams pname, double[] value) { _hlGetEffectdv(effect, DLLEnumToIntPtr(pname), value); }
        [DllImport(DLL_PATH, EntryPoint = "hlGetEffectdv")]
        private static extern void _hlGetEffectdv(uint effect, IntPtr pname, double[] value);

        /// <summary>
        /// Gets the current value of an effect property.
        /// <para></para>
        /// <see cref="HLAPI.hlEffectd"/>
        /// <see cref="HLAPI.hlEffecti"/>
        /// <see cref="HLAPI.hlEffectdv"/>
        /// <see cref="HLAPI.hlEffectiv"/>
        /// </summary>
        /// <param name="effect"></param>
        /// <param name="pname"></param>
        /// <param name="value"></param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM"></exception>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION"></exception>
        public static void hlGetEffectdv(uint effect, HLEffectParams pname, ref double value) { _hlGetEffectdv(effect, DLLEnumToIntPtr(pname), ref value); }
        [DllImport(DLL_PATH, EntryPoint = "hlGetEffectdv")]
        private static extern void _hlGetEffectdv(uint effect, IntPtr pname, ref double value);

        /// <summary>
        /// Gets the current value of an effect property.
        /// <para></para>
        /// <see cref="HLAPI.hlEffectd"/>
        /// <see cref="HLAPI.hlEffecti"/>
        /// <see cref="HLAPI.hlEffectdv"/>
        /// <see cref="HLAPI.hlEffectiv"/>
        /// </summary>
        /// <param name="effect"></param>
        /// <param name="pname"></param>
        /// <param name="value"></param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM"></exception>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION"></exception>
        public static void hlGetEffectiv(uint effect, HLEffectParams pname, IntPtr value) { _hlGetEffectiv(effect, DLLEnumToIntPtr(pname), value); }
        [DllImport(DLL_PATH, EntryPoint = "hlGetEffectiv")]
        private static extern void _hlGetEffectiv(uint effect, IntPtr pname, IntPtr value);

        /// <summary>
        /// Gets the current value of an effect property.
        /// <para></para>
        /// <see cref="HLAPI.hlEffectd"/>
        /// <see cref="HLAPI.hlEffecti"/>
        /// <see cref="HLAPI.hlEffectdv"/>
        /// <see cref="HLAPI.hlEffectiv"/>
        /// </summary>
        /// <param name="effect"></param>
        /// <param name="pname"></param>
        /// <param name="value"></param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM"></exception>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION"></exception>
        public static void hlGetEffectiv(uint effect, HLEffectParams pname, int[] value) { _hlGetEffectiv(effect, DLLEnumToIntPtr(pname), value); }
        [DllImport(DLL_PATH, EntryPoint = "hlGetEffectiv")]
        private static extern void _hlGetEffectiv(uint effect, IntPtr pname, int[] value);

        /// <summary>
        /// Gets the current value of an effect property.
        /// <para></para>
        /// <see cref="HLAPI.hlEffectd"/>
        /// <see cref="HLAPI.hlEffecti"/>
        /// <see cref="HLAPI.hlEffectdv"/>
        /// <see cref="HLAPI.hlEffectiv"/>
        /// </summary>
        /// <param name="effect"></param>
        /// <param name="pname"></param>
        /// <param name="value"></param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM"></exception>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION"></exception>
        public static void hlGetEffectiv(uint effect, HLEffectParams pname, ref int value) { _hlGetEffectiv(effect, DLLEnumToIntPtr(pname), ref value); }
        [DllImport(DLL_PATH, EntryPoint = "hlGetEffectiv")]
        private static extern void _hlGetEffectiv(uint effect, IntPtr pname, ref int value);

        /// <summary>
        /// Gets the current value of an effect property.
        /// <para></para>
        /// <see cref="HLAPI.hlEffectd"/>
        /// <see cref="HLAPI.hlEffecti"/>
        /// <see cref="HLAPI.hlEffectdv"/>
        /// <see cref="HLAPI.hlEffectiv"/>
        /// </summary>
        /// <param name="effect"></param>
        /// <param name="pname"></param>
        /// <param name="value"></param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM"></exception>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION"></exception>
        public static void hlGetEffectbv(uint effect, HLEffectParams pname, IntPtr value) { _hlGetEffectbv(effect, DLLEnumToIntPtr(pname), value); }
        [DllImport(DLL_PATH, EntryPoint = "hlGetEffectbv")]
        private static extern void _hlGetEffectbv(uint effect, IntPtr pname, IntPtr value);

        /// <summary>
        /// Gets the current value of an effect property.
        /// <para></para>
        /// <see cref="HLAPI.hlEffectd"/>
        /// <see cref="HLAPI.hlEffecti"/>
        /// <see cref="HLAPI.hlEffectdv"/>
        /// <see cref="HLAPI.hlEffectiv"/>
        /// </summary>
        /// <param name="effect"></param>
        /// <param name="pname"></param>
        /// <param name="value"></param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM"></exception>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION"></exception>
        public static void hlGetEffectbv(uint effect, HLEffectParams pname, byte[] value) { _hlGetEffectbv(effect, DLLEnumToIntPtr(pname), value); }
        [DllImport(DLL_PATH, EntryPoint = "hlGetEffectbv")]
        private static extern void _hlGetEffectbv(uint effect, IntPtr pname, byte[] value);

        /// <summary>
        /// Gets the current value of an effect property.
        /// <para></para>
        /// <see cref="HLAPI.hlEffectd"/>
        /// <see cref="HLAPI.hlEffecti"/>
        /// <see cref="HLAPI.hlEffectdv"/>
        /// <see cref="HLAPI.hlEffectiv"/>
        /// </summary>
        /// <param name="effect"></param>
        /// <param name="pname"></param>
        /// <param name="value"></param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM"></exception>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION"></exception>
        public static void hlGetEffectbv(uint effect, HLEffectParams pname, ref byte value) { _hlGetEffectbv(effect, DLLEnumToIntPtr(pname), ref value); }
        [DllImport(DLL_PATH, EntryPoint = "hlGetEffectbv")]
        private static extern void _hlGetEffectbv(uint effect, IntPtr pname, ref byte value);

        /// <summary>
        /// 确定标识符是否是有效的效果标识符。
        /// <para></para>
        /// <see cref="HLAPI.hlGenEffects"/>
        /// </summary>
        /// <param name="effect">要检查的第一效果标识符。</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION"></exception>
        /// <returns>如果effect是先前由hlGenEffects()返回的有效标识符，则返回true。</returns>
        [DllImport(DLL_PATH, EntryPoint = "hlIsEffect")]
        public static extern byte hlIsEffect(uint effect);

        /// <summary>
        /// 启动一个效果，该效果将继续运行，直到调用hlStopEffect()结束。
        /// <para>当使用hlStartEffect()时，会忽略duration属性。</para>
        /// <see cref="HLAPI.hlStopEffect"/>
        /// <see cref="HLAPI.hlTriggerEffect"/>
        /// <see cref="HLAPI.hlUpdateEffect"/>
        /// </summary>
        /// <param name="type">要启动的效果类型</param>
        /// <param name="effect">要启动的效果的标识符，由调用hlGenEffects()生成。</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM"></exception>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION"></exception>
        public static void hlStartEffect(HLStartEffectTypes type, uint effect) { _hlStartEffect(DLLEnumToIntPtr(type), effect); }
        [DllImport(DLL_PATH, EntryPoint = "hlStartEffect")]
        private static extern void _hlStartEffect(IntPtr type, uint effect);

        /// <summary>
        /// 停止使用hlStartEffect()启动的效果。
        /// <para>一旦调用hlStartEffect()启动了一个效果，它将继续运行，直到调用hlStopEffect()。</para>
        /// <see cref="HLAPI.hlStartEffect"/>
        /// </summary>
        /// <param name="effect">要启动的效果的标识符，由调用hlGenEffects()生成。</param>
        [DllImport(DLL_PATH, EntryPoint = "hlStopEffect")]
        public static extern void hlStopEffect(uint effect);

        /// <summary>
        /// 启动将持续运行指定持续时间的效果。
        /// <para>触发一个效果将导致它在当前效果持续时间指定的时间内继续运行。与以hlStartEffect()开始的效果不同，以hlTriggerEffect()开始的效果不需要通过调用hlStopEffect()来终止。</para>
        /// <para>当调用hlTriggerEffect()的时间超过效果持续时间后，效果将自动终止。效果持续时间将是最后一次调用hlEffectd()时使用HL_EFFECT_PROPERTY_DURATION指定的值。</para>
        /// <see cref="HLAPI.hlStartEffect"/>
        /// <see cref="HLAPI.hlStopEffect"/>
        /// </summary>
        /// <param name="type"></param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM"></exception>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION"></exception>
        public static void hlTriggerEffect(HLTriggerEffectTypes type) { _hlTriggerEffect(DLLEnumToIntPtr(type)); }
        [DllImport(DLL_PATH, EntryPoint = "hlTriggerEffect")]
        private static extern void _hlTriggerEffect(IntPtr type);

        /// <summary>
        /// 用当前效果状态更新给定的活动效果。hlUpdateEffect()用于更改当前活动的效果的参数(已由hlStartEffect()启动)。
        /// <para>例如，用户可能想要在每次按下按钮时增加弹簧效果的力度;hlUpdateEffect()允许更改效果，而不需要用户停止效果、指定新参数并重新启动它。使用当前状态下的效果属性更新效果，就像通过hlStartEffect()指定效果一样。</para>
        /// <see cref="HLAPI.hlStartEffect"/>
        /// <see cref="HLAPI.hlStopEffect"/>
        /// </summary>
        /// <param name="effect">要更新的效果的id</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_VALUE"></exception>
        [DllImport(DLL_PATH, EntryPoint = "hlUpdateEffect")]
        public static extern void hlUpdateEffect(uint effect);

        #endregion


        //====================================================PROXY
        #region PROXY
        /// <summary>
        /// Proxy
        /// </summary>
        /// <param name="pname"></param>
        /// <param name="value"></param>
        public static void hlProxy(HLProxyParams pname, IntPtr value) { _hlProxydv(DLLEnumToIntPtr(pname), value); }
        [DllImport(DLL_PATH, EntryPoint = "hlProxydv")]
        private static extern void _hlProxydv(IntPtr pname, IntPtr value);
        /// <summary>
        /// Proxy
        /// </summary>
        /// <param name="pname"></param>
        /// <param name="value"></param>
        public static void hlProxy(HLProxyParams pname, double[] value) { _hlProxydv(DLLEnumToIntPtr(pname), value); }
        [DllImport(DLL_PATH, EntryPoint = "hlProxydv")]
        private static extern void _hlProxydv(IntPtr pname, double[] value);
        /// <summary>
        /// Proxy
        /// </summary>
        /// <param name="pname"></param>
        /// <param name="value"></param>
        public static void hlProxy(HLProxyParams pname, ref double[] value) { _hlProxydv(DLLEnumToIntPtr(pname), ref value); }
        [DllImport(DLL_PATH, EntryPoint = "hlProxydv")]
        private static extern void _hlProxydv(IntPtr pname, ref double[] value);
        /// <summary>
        /// Proxy
        /// </summary>
        /// <param name="pname"></param>
        /// <param name="value"></param>
        public static void hlProxy(HLProxyParams pname, ref double value) { _hlProxydv(DLLEnumToIntPtr(pname), ref value); }
        [DllImport(DLL_PATH, EntryPoint = "hlProxydv")]
        private static extern void _hlProxydv(IntPtr pname, ref double value);

        /// <summary>
        /// Proxy
        /// </summary>
        /// <param name="pname"></param>
        /// <param name="value"></param>
        public static void hlProxy(HLProxyParams pname, float value) { _hlProxyf(DLLEnumToIntPtr(pname), value); }
        [DllImport(DLL_PATH, EntryPoint = "hlProxyf")]
        private static extern void _hlProxyf(IntPtr pname, float value);
        #endregion


        //====================================================TRANSFORMS
        #region TRANSFORMS
        /// <summary>
        /// 将当前矩阵堆栈顶部的当前矩阵替换为标识矩阵。
        /// <para>清除当前矩阵的顶部并将其替换为标识矩阵</para>
        /// <para>此命令根据当前矩阵模式应用于TouchWorkspace、ViewTouch矩阵或模型视图。</para>
        /// <para>1 0 0 0, 0 1 0 0, 0 0 1 0, 0 0 0 1</para>
        /// </summary>
        [DllImport(DLL_PATH, EntryPoint = "hlLoadIdentity")]
        public static extern void hlLoadIdentity();

        /// <summary>
        /// 将当前矩阵堆栈顶部的当前矩阵与指定的4x4矩阵相乘。
        /// <para>用栈顶的乘积替换当前矩阵栈顶，由m中的值构造矩阵如下:</para>
        /// <para>m0 m4 m8 m12, m1 m5 m9 m13, m2 m6 m10 m14, m3 m7 m11 m15</para>
        /// </summary>
        /// <param name="m">由16个浮点数或双精度值组成的数组，表示4x4变换矩阵。</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION"></exception>
        [DllImport(DLL_PATH, EntryPoint = "hlLoadMatrixd")]
        public static extern void hlLoadMatrix(IntPtr m);

        /// <summary>
        /// 将当前矩阵堆栈顶部的当前矩阵与指定的4x4矩阵相乘。
        /// <para>用栈顶的乘积替换当前矩阵栈顶，由m中的值构造矩阵如下:</para>
        /// <para>m0 m4 m8 m12, m1 m5 m9 m13, m2 m6 m10 m14, m3 m7 m11 m15</para>
        /// </summary>
        /// <param name="m">由16个浮点数或双精度值组成的数组，表示4x4变换矩阵。</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION"></exception>
        [DllImport(DLL_PATH, EntryPoint = "hlLoadMatrixd")]
        public static extern void hlLoadMatrix(double[] m);

        /*
        /// <summary>
        /// 将当前矩阵堆栈顶部的当前矩阵与指定的4x4矩阵相乘。
        /// <para>用栈顶的乘积替换当前矩阵栈顶，由m中的值构造矩阵如下:</para>
        /// <para>m0 m4 m8 m12, m1 m5 m9 m13, m2 m6 m10 m14, m3 m7 m11 m15</para>
        /// </summary>
        /// <param name="m">由16个浮点数或双精度值组成的数组，表示4x4变换矩阵。</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION"></exception>
        [DllImport(DLL_PATH, EntryPoint = "hlLoadMatrixf")]
        public static extern void hlLoadMatrixf(IntPtr m);
        */

        /// <summary>
        /// 将当前矩阵堆栈顶部的当前矩阵与指定的4x4矩阵相乘。
        /// <para>用栈顶的乘积替换当前矩阵栈顶，由m中的值构造矩阵如下:</para>
        /// <para>m0 m4 m8 m12, m1 m5 m9 m13, m2 m6 m10 m14, m3 m7 m11 m15</para>
        /// </summary>
        /// <param name="m">由16个浮点数或双精度值组成的数组，表示4x4变换矩阵。</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION"></exception>
        [DllImport(DLL_PATH, EntryPoint = "hlLoadMatrixf")]
        public static extern void hlLoadMatrix(float[] m);

        /// <summary>
        /// 设置哪个矩阵堆栈是将来调用矩阵操作命令的目标。
        /// </summary>
        /// <param name="mode">要对哪个矩阵堆栈应用矩阵操作命令</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM"></exception>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION"></exception>
        public static void hlMatrixMode(HLMatrixModeParams mode) { _hlMatrixMode(DLLEnumToIntPtr(mode)); }
        [DllImport(DLL_PATH, EntryPoint = "hlMatrixMode")]
        private static extern void _hlMatrixMode(IntPtr mode);

        [DllImport(DLL_PATH, EntryPoint = "hlMultMatrixf")]
        public static extern void hlMultMatrixf(IntPtr m);

        [DllImport(DLL_PATH, EntryPoint = "hlMultMatrixf")]
        public static extern void hlMultMatrixf(float[] m);

        [DllImport(DLL_PATH, EntryPoint = "hlMultMatrixf")]
        public static extern void hlMultMatrixf(ref float[] m);

        [DllImport(DLL_PATH, EntryPoint = "hlMultMatrixf")]
        public static extern void hlMultMatrixf(ref float m);

        [DllImport(DLL_PATH, EntryPoint = "hlMultMatrixd")]
        public static extern void hlMultMatrixd(IntPtr m);

        [DllImport(DLL_PATH, EntryPoint = "hlMultMatrixd")]
        public static extern void hlMultMatrixd(double[] m);

        [DllImport(DLL_PATH, EntryPoint = "hlMultMatrixd")]
        public static extern void hlMultMatrixd(ref double[] m);

        [DllImport(DLL_PATH, EntryPoint = "hlMultMatrixd")]
        public static extern void hlMultMatrixd(ref double m);

        /// <summary>
        /// 设置触觉视图卷，该视图卷决定如何将触觉设备的工作区映射到图形视图卷。
        /// <para></para>
        /// <see cref="HLAPI.hlWorkspace"/>
        /// <see cref="HLAPI.hlMatrixMode"/>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="bottom"></param>
        /// <param name="top"></param>
        /// <param name="zNear"></param>
        /// <param name="zFar"></param>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION"></exception>
        [DllImport(DLL_PATH, EntryPoint = "hlOrtho")]
        public static extern void hlOrtho(double left, double right, double bottom, double top, double zNear, double zFar);

        /// <summary>
        /// 将一组属性推到当前属性矩阵堆栈的顶部。
        /// <para></para>
        /// <see cref="HLAPI.hlPushMatrix"/>
        /// <see cref="HLAPI.hlPopMatrix"/>
        /// </summary>
        /// <param name="mask"></param>
        /// <exception cref="HLErrorCodes.HL_INVALID_VALUE"></exception>
        [DllImport(DLL_PATH, EntryPoint = "hlPushAttrib")]
        public static extern void hlPushAttrib(HLPushPopAttrib mask);

        /// <summary>
        /// 移除当前顶部属性的堆栈。
        /// <para></para>
        /// <see cref="HLAPI.hlPushMatrix"/>
        /// <see cref="HLAPI.hlPopMatrix"/>
        /// </summary>
        [DllImport(DLL_PATH, EntryPoint = "hlPopAttrib")]
        public static extern void hlPopAttrib();

        /// <summary>
        /// 将一个新矩阵推到当前矩阵堆栈的顶部
        /// </summary>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION"></exception>
        /// <exception cref="HLErrorCodes.HL_STACK_OVERFLOW"></exception>
        /// <exception cref="HLErrorCodes.HL_STACK_UNDERFLOW"></exception>
        [DllImport(DLL_PATH, EntryPoint = "hlPushMatrix")]
        public static extern void hlPushMatrix();

        /// <summary>
        /// 移除当前矩阵堆栈的顶部
        /// </summary>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION"></exception>
        /// <exception cref="HLErrorCodes.HL_STACK_OVERFLOW"></exception>
        /// <exception cref="HLErrorCodes.HL_STACK_UNDERFLOW"></exception>
        [DllImport(DLL_PATH, EntryPoint = "hlPopMatrix")]
        public static extern void hlPopMatrix();

        /// <summary>
        /// 将当前矩阵堆栈顶部的当前矩阵乘以4x4旋转矩阵。
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION"></exception>
        [DllImport(DLL_PATH, EntryPoint = "hlRotatef")]
        public static extern void hlRotate(float angle, float x, float y, float z);
        /// <summary>
        /// 将当前矩阵堆栈顶部的当前矩阵乘以4x4旋转矩阵。
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION"></exception>
        [DllImport(DLL_PATH, EntryPoint = "hlRotated")]
        public static extern void hlRotate(double angle, double x, double y, double z);

        /// <summary>
        /// 将当前矩阵堆栈顶部的当前矩阵乘以4x4比例矩阵。
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION"></exception>
        [DllImport(DLL_PATH, EntryPoint = "hlScalef")]
        public static extern void hlScale(float x, float y, float z);
        /// <summary>
        /// 将当前矩阵堆栈顶部的当前矩阵乘以4x4比例矩阵。
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION"></exception>
        [DllImport(DLL_PATH, EntryPoint = "hlScaled")]
        public static extern void hlScale(double x, double y, double z);

        /// <summary>
        /// Multiplies the current matrix on the top of the current matrix stack by a 4x4 translation matrix.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION"></exception>
        [DllImport(DLL_PATH, EntryPoint = "hlTranslatef")]
        public static extern void hlTranslate(float x, float y, float z);
        /// <summary>
        /// Multiplies the current matrix on the top of the current matrix stack by a 4x4 translation matrix.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION"></exception>
        [DllImport(DLL_PATH, EntryPoint = "hlTranslated")]
        public static extern void hlTranslate(double x, double y, double z);

        /// <summary>
        /// 定义用于在图形坐标系和触觉设备的物理单元之间进行映射的触觉设备的工作区范围。
        /// <para></para>
        /// <see cref="HLAPI.hlOrtho"/>
        /// <see cref="HLAPI.hlMatrixMode"/>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="bottom"></param>
        /// <param name="back"></param>
        /// <param name="right"></param>
        /// <param name="top"></param>
        /// <param name="front"></param>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION"></exception>
        [DllImport(DLL_PATH, EntryPoint = "hlWorkspace")]
        public static extern void hlWorkspace(double left, double bottom, double back, double right, double top, double front);
        #endregion


        //====================================================CALLBACKS
        #region CALLBACKS
        /// <summary>
        /// 设置用户回调函数。
        /// <para>用于为自定义形状和自定义效果类型设置回调函数。</para>
        /// <see cref="HLAPI.hlBeginShape"/>
        /// </summary>
        /// <param name="type">回调类型</param>
        /// <param name="fn">回调函数</param>
        /// <param name="pUserData">指向将传递给回调函数的数据指针。</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM"></exception>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION"></exception>
        public static void hlCallback(HLCallbackTypes type, HLCallbackProc fn, IntPtr pUserData) { _hlCallback(DLLEnumToIntPtr(type), fn, pUserData); }
        [DllImport(DLL_PATH, EntryPoint = "hlCallback")]
        private static extern void _hlCallback(IntPtr type, HLCallbackProc fn, IntPtr pUserData);

        /// <summary>
        /// 将用户定义的事件处理函数添加到事件的回调函数列表中。
        /// <para>事件回调用于通知程序在触觉呈现程序中发生的事件，例如被触摸的对象。</para>
        /// <see cref="HLAPI.hlRemoveEventCallback"/>
        /// <see cref="HLAPI.hlEventd"/>
        /// <see cref="HLAPI.hlCheckEvents"/>
        /// </summary>
        /// <param name="evt">要订阅的事件</param>
        /// <param name="shapeId">标识符的形状。回调只会在具有此标识符的形状上发生事件时调用，除非将此参数设置为 HL_OBJECT_ANY，在这种情况下，回调将独立于任何对象调用。</param>
        /// <param name="thread">要调用回调函数的线程</param>
        /// <param name="fn">回调函数</param>
        /// <param name="pUserData">回调函数的数据指针</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM"></exception>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION"></exception>
        public static void hlAddEventCallback(HLCallbackEvents evt, uint shapeId, HLCallbackThreads thread, HLEventProc fn, IntPtr pUserData)
        {
            _hlAddEventCallback(DLLEnumToIntPtr(evt), shapeId, DLLEnumToIntPtr(thread), fn, pUserData);
        }
        [DllImport(DLL_PATH, EntryPoint = "hlAddEventCallback")]
        private static extern void _hlAddEventCallback(IntPtr evt, uint shapeId, IntPtr thread, HLEventProc fn, IntPtr pUserData);

        /// <summary>
        /// 为订阅的所有事件调用回调函数，以及自上次调用hlCheckEvents()以来发生的所有事件调用回调函数。
        /// <para>hlCheckEvents()可以在开始/结束帧对之外调用。它可以代替开始/结束帧来更新设备和事件状态。例如，如果用户感觉是一个静态场景，并且只需要定期更新设备状态信息，那么他可以放弃调用hlBegin()/EndFrame()，而是定期调用hlCheckEvents()。</para>
        /// <see cref="HLAPI.hlAddEventCallback"/>
        /// <see cref="HLAPI.hlRemoveEventCallback"/>
        /// <see cref="HLAPI.hlEventd"/>
        /// </summary>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION"></exception>
        [DllImport(DLL_PATH, EntryPoint = "hlCheckEvents")]
        public static extern void hlCheckEvents();

        /// <summary>
        /// 设置影响如何以及何时调用事件回调的参数。
        /// <para>用于程序启动时根据特定应用程序调整触觉呈现程序的事件管理。</para>
        /// <see cref="HLAPI.hlAddEventCallback"/>
        /// <see cref="HLAPI.hlPushAttrib"/>
        /// <see cref="HLAPI.hlPopAttrib"/>
        /// </summary>
        /// <param name="pname">要设置的事件参数的名称</param>
        /// <param name="value">要设置的参数的值</param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM"></exception>
        /// <exception cref="HLErrorCodes.HL_INVALID_VALUE"></exception>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION"></exception>
        public static void hlEventd(HLEventdParams pname, double value) { _hlEventd(DLLEnumToIntPtr(pname), value); }
        [DllImport(DLL_PATH, EntryPoint = "hlEventd")]
        private static extern void _hlEventd(IntPtr pname, double value);

        /// <summary>
        /// 从事件的回调函数列表中移除现有用户定义的事件处理函数。
        /// <para></para>
        /// <see cref="HLAPI.hlAddEventCallback"/>
        /// <see cref="HLAPI.hlCheckEvents"/>
        /// <see cref="HLAPI.hlEventd"/>
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="shapeId"></param>
        /// <param name="thread"></param>
        /// <param name="fn"></param>
        /// <exception cref="HLErrorCodes.HL_INVALID_ENUM"></exception>
        /// <exception cref="HLErrorCodes.HL_INVALID_OPERATION"></exception>
        public static void hlRemoveEventCallback(HLCallbackEvents evt, uint shapeId, HLCallbackThreads thread, HLEventProc fn)
        {
            _hlRemoveEventCallback(DLLEnumToIntPtr(evt), shapeId, DLLEnumToIntPtr(thread), fn);
        }
        [DllImport(DLL_PATH, EntryPoint = "hlRemoveEventCallback")]
        private static extern void _hlRemoveEventCallback(IntPtr evt, uint shapeId, IntPtr thread, HLEventProc fn);

        #endregion


        //====================================================CALIBRATION
        #region Calibration
        /// <summary>
        /// 使触觉装置重新校准
        /// </summary>
        [DllImport(DLL_PATH, EntryPoint = "hlUpdateCalibration")]
        public static extern void hlUpdateCalibration();

        #endregion


        //====================================================HLAPI DEPLOYMENT
        #region HLAPI DEPLOYMENT
        /// <summary>
        /// 激活颁发给应用程序开发人员的部署许可证。一旦验证了部署许可证，它将一直处于活动状态，直到应用程序退出为止。在应用程序会话中多次调用此函数不是错误。
        /// </summary>
        /// <param name="vendorName"></param>
        /// <param name="applicationName"></param>
        /// <param name="password"></param>
        /// <returns>返回 bool 值</returns>
        [DllImport(DLL_PATH, EntryPoint = "hlDeploymentLicense")]
        public static extern byte hlDeploymentLicense(String vendorName, String applicationName, String password);
        #endregion
    }

    /// <summary>
    /// Win API
    /// </summary>
    public class WINAPI
    {
        /// <summary>
        /// 加载库/模块
        /// </summary>
        /// <param name="lpFileName"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LoadLibrary(string lpFileName);

        /// <summary>
        /// 获取库/模块指定的 proceName
        /// </summary>
        /// <param name="hModule"></param>
        /// <param name="procName"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        /// <summary>
        /// 释放库/模块
        /// </summary>
        /// <param name="hModule"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern bool FreeLibrary(IntPtr hModule);
    }

}
