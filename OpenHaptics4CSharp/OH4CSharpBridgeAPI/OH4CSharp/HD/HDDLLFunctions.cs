using OH4CSharp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace OH4CSharp.HD
{
    /// <summary>
    /// HDAPI是触觉的低级别的基础层。适合已经熟悉触觉的开发人员。
    /// <para>是设备通信数据与上层软件的第一层底封装(程序接口)</para>
    /// </summary>
    public partial class HDAPI
    {

        static HDAPI()
        {
            //Console.WriteLine("Static HDAPI Constructor.");
        }

        #region Const Variables
        /// <summary>
        /// hd.dll 路径，后面在改
        /// </summary>
        public const string DLL_PATH = @"hd.dll";
        //public const string DLL_PATH = @"D:\OpenHaptics\Developer\3.5.0\lib\x64\Release\hd.dll";

        /// <summary>
        /// 默认设备
        /// </summary>
        public const String HD_DEFAULT_DEVICE = null;

        /// <summary>
        /// 无效设备句柄
        /// </summary>
        public const uint HD_INVALID_HANDLE = 0xFFFFFFFF;
        #endregion

        //======================================================DEVICE ROUTINES 
        #region hdGetError/hdGetErrorString/hdGetString
        /// <summary>
        /// 以相反的顺序返回错误信息(即最近的错误优先)。如果堆栈上没有错误，则返回带有代码 HD_SUCCESS 的 HDErrorInfo 结构对象 。
        /// 返回按从最近的顺序错误。每个调用从错误堆栈中检索并删除一个错误。如果没有错误存在，这个函数返回一个HDErrorInfo.HD_SUCCESS
        /// 成功是它的代码。HDErrorInfo包含定义文件中的错误代码、错误发生时处于活动状态的设备句柄以及设备的原始内部错误代码。内部代码可用于从设备供应商获得额外的支持。
        /// <para>插入代码时，偶尔检查错误</para>
        /// <see cref="HDAPI.hdGetErrorString"/>
        /// <see cref="HDErrorInfo"/>
        /// </summary>
        /// <returns>返回错误信息</returns>
        [DllImport(DLL_PATH)]
        public static extern HDErrorInfo hdGetError();

        /// <summary>
        /// 返回有关错误代码的信息。
        /// <para>获取有关错误代码的有用信息。返回字符串是静态的，不应该使用Free或Delete或任何类似的函数进行释放。</para>
        /// <see cref="HDAPI.hdGetError"/>
        /// <see cref="HDErrorInfo.CheckedError"/>
        /// </summary>
        /// <param name="errorCode">HDErrorInfo 的属性 errorCode</param>
        /// <returns>返回错误代码解释的可读字符串指针</returns>
        [DllImport(DLL_PATH)]
        public static extern IntPtr hdGetErrorString(uint errorCode);

        /// <summary>
        /// 获取关联参数名称的字符串值。
        /// <para>获取有关设备属性(如设备模型类型)的可读字符串信息。返回字符串是静态的，不应该使用Free或Delete或任何类似的函数进行释放。</para>
        /// </summary>
        /// <see cref="HDGetStringParameters"/>
        /// <param name="hdenum"></param>
        /// <returns>请求的字符串与参数名关联</returns>
        /// <exception cref="HDErrorCodes.HD_INVALID_ENUM">pname不支持hdGetString()</exception>
        /// <exception cref="HDErrorCodes.HD_INVALID_INPUT_TYPE">pname为不支持字符串作为输入类型</exception>
        [DllImport(DLL_PATH)]
        public static extern IntPtr hdGetString(HDGetStringParameters pname);
        #endregion


        #region hdIninDevice/hdDisableDevice/hdMakeCurrentDevice/hdGetCurrentDevice
        /// <summary>
        /// 初始化设备，如果成功，将返回设备句柄并将设备设置为当前设备。
        /// <para>通常应该发出的第一个触觉命令。</para>
        /// <para>Initializes the device. If successful, this returns a handle to the device and sets the device as the current device. </para>
        /// <see cref="HDAPI.hdDisableDevice"/>
        /// <see cref="HDAPI.hdMakeCurrentDevice"/>
        /// <see cref="HDAPI.hdStartScheduler"/>
        /// </summary>
        /// <exception cref="HDErrorCodes.HD_DEVICE_ALREADY_INITIATED">设备名称已经初始化</exception>
        /// <exception cref="HDErrorCodes.HD_DEVICE_FAULT">如果无法启动设备，例如：hdDeviceName无效</exception>
        /// <param name="hdDeviceName">设备的名称，在控制面板"Geomagic Touch Setup"中找到的名称。
        /// 如果 HD_DEFAULT_DEVICE 作为 hdDeviceName 传入，hdInitiDevice()将初始化它找到的第一个设备。</param>
        /// <returns>返回 HHD 设备句柄，在C++中 HHD 是 unsigned int 类型</returns>
        [DllImport(DLL_PATH)]
        public static extern uint hdInitDevice(String hdDeviceName);

        /// <summary>
        /// 禁用一个设备，之后不应使用设备句柄。使用设备进行清理时调用。通常是在停止调度程序并取消所有调度回调后的最后一次调用。
        /// <para>Disables a device. The handle should not be used afterward.</para>
        /// <see cref="HDAPI.hdInitDevice"/>
        /// <see cref="HDAPI.hdStopScheduler"/>
        /// </summary>
        /// <param name="hHD">初始化设备的设备句柄。</param>
        /// <example>Example:
        /// <code>
        ///     hdStopScheduler();
        ///     hdUnschedule(scheduleCallbackHandle);
        ///     hdDisableDevice(hdGetCurrentDevice());
        /// </code>
        /// </example>
        [DllImport(DLL_PATH)]
        public static extern void hdDisableDevice(uint hHD);

        /// <summary>
        /// 使设备为当前设备。所有后续特定于设备的操作，如获取和设置状态或查询设备信息，都将在此设备上执行，直到另一个设备变为当前状态。
        /// <para>主要用于多设备应用中的设备间切换。</para>
        /// <para>Makes the device current. All subsequent device-specific actions such as getting and setting
        /// state or querying device information will be performed on this device until another is made current.</para>
        /// <see cref="HDAPI.hdInitDevice"/>
        /// </summary>
        /// <param name="hHD">已初始化设备的设备句柄。</param>
        /// <exception cref="HD_INVALID_HANDLE">如果hHD为不引用启动的设备</exception>
        [DllImport(DLL_PATH)]
        public static extern void hdMakeCurrentDevice(uint hHD);

        /// <summary>
        /// 获取当前设备的句柄。主要用于多设备应用程序中，以跟踪哪个设备是当前的，或用于需要设备句柄的调用。
        /// <para>Gets the handle of the current device.</para>
        /// <see cref="HDAPI.hdMakeCurrentDevice"/>
        /// </summary>
        /// <returns>当前设备的句柄。</returns>
        /// <exception cref="HD_INVALID_HANDLE">没有当前设备，例如：还没有启动任何设备。</exception>
        [DllImport(DLL_PATH)]
        public static extern uint hdGetCurrentDevice();
        #endregion


        /********************************************************************************************************
         * 注意：hdGet/hdSet函数在 C++ 原型函数是 hdGetxxx/hdSetxxx(unsigned int pname, basetype *params)
         * pname：是参数名称，有些参数是可读写，有些只是可读，可写的参数较少
         * basetype：表示基本数据类型，如果unsigned char, int, unsigned int, long , float, double ... 
         * params：是输入/输出指针数据
         * 
         * 对应：C#
         * 将 pname 进行分类，可自行扩展 hdGet 函数；
         *      例：public static extern void hdGetIntegerv(uint prop, IntPtr/int[]/ref int value);
         * prop：只要是无符号int类型值即可，已封装为 HDSetParameters 数据类型
         * value：是指针类型值，C#在安全编译的模式下有IntPtr、基本数据类型数组(数组可理解为指针)、和 ref 基本数据类型
         * 引用 ref 基本数据类型只能返回一个值，数组可以返回多组基本数据，IntPtr可以返回复杂数据(例如结构类型)
         * 
         ********************************************************************************************************/
        #region hdBeginFrame/hdEndFrame
        /// <summary>
        /// 开始一个 haptics 帧，这是一个代码块，其中的设备状态被保证是一致的。
        /// 更新当前/上次信息的状态，所有与状态相关的信息，例如设置状态，都应该在一个 haptics 帧内完成。
        /// <para>通常每个设备每个调度程序的第一个触觉调用。
        /// 例如，如果管理两个触觉设备，那么在进行特定于设备的调用之前，应该为每个设备调用hdBeginFrame()。
        /// 除非禁用HD_ONE_FRAME_LIMIT，否则每个设备每个调度程序只允许一个帧。
        /// 但是，相同设备的帧可以嵌套在调度器中。此函数自动使所提供的设备处于当前状态。</para>
        /// <see cref="HDAPI.hdMakeCurrentDevice"/>
        /// <see cref="HDAPI.hdDisable"/>
        /// <see cref="HDAPI.hdIsEnabled"/>
        /// <see cref="HDAPI.hdDisable"/>
        /// </summary>
        /// <param name="hHD">已初始化设备的设备句柄</param>
        /// <exception cref="HDErrorCodes.HD_ILLEGAL_BEGIN">如果当前调度程序中的当前设备的帧已经完成。</exception>
        [DllImport(DLL_PATH)]
        public static extern void hdBeginFrame(uint hHD);

        /// <summary>
        /// 结束了一个 haptics 帧。将力和其他状态写入设备，hdBeginFrame()和hdEndFrame()应该总是在同一个调度程序中配对。
        /// <para>通常每个设备,每个调度程序的最后一次调用。</para>
        /// <para>Ends a haptics frame. Causes forces and other states to be written to the device. An
        /// hdBeginFrame() and hdEndFrame() should always be paired within the same scheduler tick.</para>
        /// <see cref="HDAPI.hdBeginFrame"/>
        /// <see cref="HDAPI.hdMakeCurrentDevice"/>
        /// </summary>
        /// <param name="hHD">已初始化设备的设备句柄</param>
        /// <exception cref="HDErrorCodes.HD_ILLEGAL_END">如果没有正确地调用与相同设备句柄的hdBeginFrame()配对。</exception>
        [DllImport(DLL_PATH)]
        public static extern void hdEndFrame(uint hHD);
        #endregion


        #region hdEnabled/hdDisable/hdIsEnabled
        /// <summary>
        /// 启用功能
        /// <para>功能通常与安全机制相关，大多数都是默认打开的。</para>
        /// <see cref="HDEDParameters"/>
        /// </summary>
        /// <param name="cap"></param> 
        /// <exception cref="HDErrorCodes.HD_INVALID_ENUM">如果上限不支持启用/禁用，则为HD_INVALID_ENUM。</exception>
        [DllImport(DLL_PATH)]
        public static extern void hdEnable(HDEDParameters cap);

        /// <summary>
        /// 禁用功能
        /// <para>功能通常与安全机制相关，禁用安全功能时要格外小心。</para>
        /// <see cref="HDEDParameters"/>
        /// </summary>
        /// <param name="cap"></param>
        /// <exception cref="HDErrorCodes.HD_INVALID_ENUM">如果上限不支持启用/禁用，则为HD_INVALID_ENUM。</exception>
        [DllImport(DLL_PATH)]
        public static extern void hdDisable(HDEDParameters cap);

        /// <summary>
        /// 检查是否启用了某个功能。
        /// <para>功能通常与安全机制相关，大多数都是默认打开的。</para>
        /// <see cref="HDEDParameters"/>
        /// </summary>
        /// <param name="cap">功能属性，参考 HDEDCapabilities</param>
        /// <returns>C++返回的是字节数据</returns>
        /// <exception cref="HDErrorCodes.HD_INVALID_ENUM">如果上限不支持启用/禁用，则为HD_INVALID_ENUM。</exception>
        [DllImport(DLL_PATH)]
        public static extern byte hdIsEnabled(HDEDParameters cap);
        #endregion


        #region hdGet (Parameter Values)
        /// <summary>
        /// 跟据参数名称/参数编码/参数ID返回对应的值
        /// </summary>
        /// <param name="pname">参数名称/参数编码/参数ID</param>
        /// <param name="value">输出/返回值</param>
        [DllImport(DLL_PATH)]
        public static extern void hdGetBooleanv(HDGetParameters pname, IntPtr value);
        /// <summary>
        /// 跟据参数名称/参数编码/参数ID返回对应的值
        /// </summary>
        /// <param name="pname">参数名称/参数编码/参数ID</param>
        /// <param name="value">输出/返回值</param>
        [DllImport(DLL_PATH)]
        public static extern void hdGetBooleanv(HDGetParameters pname, byte[] value);
        /// <summary>
        /// 跟据参数名称/参数编码/参数ID返回对应的值
        /// </summary>
        /// <param name="pname">参数名称/参数编码/参数ID</param>
        /// <param name="value">输出/返回值</param>
        [DllImport(DLL_PATH)]
        public static extern void hdGetBooleanv(HDGetParameters pname, ref byte value);

        /// <summary>
        /// 跟据参数名称/参数编码/参数ID返回对应的值
        /// </summary>
        /// <param name="pname">参数名称/参数编码/参数ID</param>
        /// <param name="value">输出/返回值</param>
        [DllImport(DLL_PATH)]
        public static extern void hdGetIntegerv(HDGetParameters hdProp, IntPtr value);
        /// <summary>
        /// 跟据参数名称/参数编码/参数ID返回对应的值
        /// </summary>
        /// <param name="pname">参数名称/参数编码/参数ID</param>
        /// <param name="value">输出/返回值</param>
        [DllImport(DLL_PATH)]
        public static extern void hdGetIntegerv(HDGetParameters hdProp, int[] value);
        /// <summary>
        /// 跟据参数名称/参数编码/参数ID返回对应的值
        /// </summary>
        /// <param name="pname">参数名称/参数编码/参数ID</param>
        /// <param name="value">输出/返回值</param>
        [DllImport(DLL_PATH)]
        public static extern void hdGetIntegerv(HDGetParameters hdProp, ref int value);

        /// <summary>
        /// 跟据参数名称/参数编码/参数ID返回对应的值
        /// </summary>
        /// <param name="pname">参数名称/参数编码/参数ID</param>
        /// <param name="value">输出/返回值</param>
        [DllImport(DLL_PATH)]
        public static extern void hdGetFloatv(HDGetParameters pname, IntPtr value);
        /// <summary>
        /// 跟据参数名称/参数编码/参数ID返回对应的值
        /// </summary>
        /// <param name="pname">参数名称/参数编码/参数ID</param>
        /// <param name="value">输出/返回值</param>
        [DllImport(DLL_PATH)]
        public static extern void hdGetFloatv(HDGetParameters pname, float[] vlaue);
        /// <summary>
        /// 跟据参数名称/参数编码/参数ID返回对应的值
        /// </summary>
        /// <param name="pname">参数名称/参数编码/参数ID</param>
        /// <param name="value">输出/返回值</param>
        [DllImport(DLL_PATH)]
        public static extern void hdGetFloatv(HDGetParameters pname, ref float vlaue);

        /// <summary>
        /// 跟据参数名称/参数编码/参数ID返回对应的值
        /// </summary>
        /// <param name="pname">参数名称/参数编码/参数ID</param>
        /// <param name="value">输出/返回值</param>
        [DllImport(DLL_PATH)]
        public static extern void hdGetDoublev(HDGetParameters pname, IntPtr value);
        /// <summary>
        /// 跟据参数名称/参数编码/参数ID返回对应的值
        /// </summary>
        /// <param name="pname">参数名称/参数编码/参数ID</param>
        /// <param name="value">输出/返回值</param>
        [DllImport(DLL_PATH)]
        public static extern void hdGetDoublev(HDGetParameters pname, ref Vector3D value);
        /// <summary>
        /// 跟据参数名称/参数编码/参数ID返回对应的值
        /// </summary>
        /// <param name="pname">参数名称/参数编码/参数ID</param>
        /// <param name="value">输出/返回值</param>
        [DllImport(DLL_PATH)]
        public static extern void hdGetDoublev(HDGetParameters pname, double[] value);
        /// <summary>
        /// 跟据参数名称/参数编码/参数ID返回对应的值
        /// </summary>
        /// <param name="pname">参数名称/参数编码/参数ID</param>
        /// <param name="value">输出/返回值</param>
        [DllImport(DLL_PATH)]
        public static extern void hdGetDoublev(HDGetParameters pname, ref double value);

        /// <summary>
        /// 跟据参数名称/参数编码/参数ID返回对应的值
        /// </summary>
        /// <param name="pname">参数名称/参数编码/参数ID</param>
        /// <param name="value">输出/返回值</param>
        [DllImport(DLL_PATH)]
        public static extern void hdGetLongv(HDGetParameters pname, IntPtr value);
        /// <summary>
        /// 跟据参数名称/参数编码/参数ID返回对应的值
        /// </summary>
        /// <param name="pname">参数名称/参数编码/参数ID</param>
        /// <param name="value">输出/返回值</param>
        [DllImport(DLL_PATH)]
        public static extern void hdGetLongv(HDGetParameters pname, long[] value);
        /// <summary>
        /// 跟据参数名称/参数编码/参数ID返回对应的值
        /// </summary>
        /// <param name="pname">参数名称/参数编码/参数ID</param>
        /// <param name="value">输出/返回值</param>
        [DllImport(DLL_PATH)]
        public static extern void hdGetLongv(HDGetParameters pname, ref long value);

        #endregion


        #region hdSet (Parameter Values)
        /// <summary>
        /// 设置参数名称/参数编码/参数ID对应的值
        /// </summary>
        /// <param name="pname">参数名称/参数编码/参数ID</param>
        /// <param name="value">输入/设置的值</param>
        [DllImport(DLL_PATH)]
        public static extern void hdSetBooleanv(HDSetParameters pname, IntPtr value);
        /// <summary>
        /// 设置参数名称/参数编码/参数ID对应的值
        /// </summary>
        /// <param name="pname">参数名称/参数编码/参数ID</param>
        /// <param name="value">输入/设置的值</param>
        [DllImport(DLL_PATH)]
        public static extern void hdSetBooleanv(HDSetParameters pname, byte[] value);
        /// <summary>
        /// 设置参数名称/参数编码/参数ID对应的值
        /// </summary>
        /// <param name="pname">参数名称/参数编码/参数ID</param>
        /// <param name="value">输入/设置的值</param>
        [DllImport(DLL_PATH)]
        public static extern void hdSetBooleanv(HDSetParameters pname, ref byte value);

        /// <summary>
        /// 设置参数名称/参数编码/参数ID对应的值
        /// </summary>
        /// <param name="pname">参数名称/参数编码/参数ID</param>
        /// <param name="value">输入/设置的值</param>
        [DllImport(DLL_PATH)]
        public static extern void hdSetIntegerv(HDSetParameters pname, IntPtr value);
        /// <summary>
        /// 设置参数名称/参数编码/参数ID对应的值
        /// </summary>
        /// <param name="pname">参数名称/参数编码/参数ID</param>
        /// <param name="value">输入/设置的值</param>
        [DllImport(DLL_PATH)]
        public static extern void hdSetIntegerv(HDSetParameters pname, int[] value);
        /// <summary>
        /// 设置参数名称/参数编码/参数ID对应的值
        /// </summary>
        /// <param name="pname">参数名称/参数编码/参数ID</param>
        /// <param name="value">输入/设置的值</param>
        [DllImport(DLL_PATH)]
        public static extern void hdSetIntegerv(HDSetParameters pname, ref int value);

        /// <summary>
        /// 设置参数名称/参数编码/参数ID对应的值
        /// </summary>
        /// <param name="pname">参数名称/参数编码/参数ID</param>
        /// <param name="value">输入/设置的值</param>
        [DllImport(DLL_PATH)]
        public static extern void hdSetFloatv(HDSetParameters pname, IntPtr value);
        /// <summary>
        /// 设置参数名称/参数编码/参数ID对应的值
        /// </summary>
        /// <param name="pname">参数名称/参数编码/参数ID</param>
        /// <param name="value">输入/设置的值</param>
        [DllImport(DLL_PATH)]
        public static extern void hdSetFloatv(HDSetParameters pname, float[] value);
        /// <summary>
        /// 设置参数名称/参数编码/参数ID对应的值
        /// </summary>
        /// <param name="pname">参数名称/参数编码/参数ID</param>
        /// <param name="value">输入/设置的值</param>
        [DllImport(DLL_PATH)]
        public static extern void hdSetFloatv(HDSetParameters pname, ref float value);

        /// <summary>
        /// 设置参数名称/参数编码/参数ID对应的值
        /// </summary>
        /// <param name="pname">参数名称/参数编码/参数ID</param>
        /// <param name="value">输入/设置的值</param>
        [DllImport(DLL_PATH)]
        public static extern void hdSetDoublev(HDSetParameters pname, IntPtr value);
        /// <summary>
        /// 设置参数名称/参数编码/参数ID对应的值
        /// </summary>
        /// <param name="pname">参数名称/参数编码/参数ID</param>
        /// <param name="value">输入/设置的值</param>
        [DllImport(DLL_PATH)]
        public static extern void hdSetDoublev(HDSetParameters pname, ref Vector3D value);
        /// <summary>
        /// 设置参数名称/参数编码/参数ID对应的值
        /// </summary>
        /// <param name="pname">参数名称/参数编码/参数ID</param>
        /// <param name="value">输入/设置的值</param>
        [DllImport(DLL_PATH)]
        public static extern void hdSetDoublev(HDSetParameters pname, double[] value);
        /// <summary>
        /// 设置参数名称/参数编码/参数ID对应的值
        /// </summary>
        /// <param name="pname">参数名称/参数编码/参数ID</param>
        /// <param name="value">输入/设置的值</param>
        [DllImport(DLL_PATH)]
        public static extern void hdSetDoublev(HDSetParameters pname, ref double value);

        /// <summary>
        /// 设置参数名称/参数编码/参数ID对应的值
        /// </summary>
        /// <param name="pname">参数名称/参数编码/参数ID</param>
        /// <param name="value">输入/设置的值</param>
        [DllImport(DLL_PATH)]
        public static extern void hdSetLongv(HDSetParameters pname, IntPtr value);
        /// <summary>
        /// 设置参数名称/参数编码/参数ID对应的值
        /// </summary>
        /// <param name="pname">参数名称/参数编码/参数ID</param>
        /// <param name="value">输入/设置的值</param>
        [DllImport(DLL_PATH)]
        public static extern void hdSetLongv(HDSetParameters pname, long[] value);
        /// <summary>
        /// 设置参数名称/参数编码/参数ID对应的值
        /// </summary>
        /// <param name="pname">参数名称/参数编码/参数ID</param>
        /// <param name="value">输入/设置的值</param>
        [DllImport(DLL_PATH)]
        public static extern void hdSetLongv(HDSetParameters pname, ref long value);
        #endregion


        //======================================================CALIBRATION ROUTINES
        #region hdCheckCalibration/hdUpdateCalibration
        /// <summary>
        /// 检查校准状态。
        /// <para>对于支持自动校准的设备，请间歇调用，以便设备继续更新其校准。</para>
        /// <see cref="HDAPI.hdUpdateCalibration"/>
        /// </summary>
        /// <returns>返回 HDCalibrationCodes 类型数据</returns>
        /// <exception cref="HDErrorCodes.HD_DEVICE_FAULT">如果无法从设备中获得校准信息</exception>
        [DllImport(DLL_PATH)]
        public static extern HDCalibrationCodes hdCheckCalibration();

        /// <summary>
        /// [API文档上没有注释]
        /// </summary>
        /// <returns></returns>
        [DllImport(DLL_PATH)]
        public static extern uint hdCheckCalibrationStyle();

        /// <summary>
        /// [API文档上没有注释]
        /// </summary>
        /// <param name="style"></param>
        [DllImport(DLL_PATH)]
        public static extern void hdUpdateCalibrationMessage(HDCalibrationStyles style);

        /// <summary>
        /// 更新设备校准样式
        /// <para>当hdCheckCalibration()返回时校准设备 HD_CALIBRATION_NEEDS_UPDATE。</para>
        /// <see cref="HDAPI.hdCheckCalibration"/>
        /// </summary>
        /// <param name="style">校准样式</param>
        /// <exception cref="HDErrorCodes.HD_DEVICE_FAULT">如果无法在设备上执行校准类型</exception>
        [DllImport(DLL_PATH)]
        public static extern void hdUpdateCalibration(HDCalibrationStyles style);
        #endregion


        //======================================================SCHEDULER ROUTINES
        #region Scheduler Synchronous/Asynchronous/WaitForCompletion/Start/Stop ...
        /// <summary>
        /// 在伺服循环(servo loop)中调度程序要执行的操作，并等待其完成。无论优先级如何，从伺服循环线程提交的调度程序回调都会立即运行。
        /// <para>通常用作伺服循环线程与应用程序中的其他线程之间的同步机制。例如，如果主应用程序线程需要访问位置或按钮状态，它可以通过同步调度程序调用来访问。
        /// 可用于从伺服循环中同步复制状态或同步执行伺服循环中的操作。 </para>
        /// <see cref="HDSchedulerCallback"/>
        /// <see cref="HDAPI.hdStartScheduler"/>
        /// </summary>
        /// <param name="pCallback">回调函数</param>
        /// <param name="pUserData">函数要使用的数据</param>
        /// <param name="nPriority">调度优先级，它决定排序的顺序；当多个回调被调度时运行回调(更高的优先级意味着先运行)。</param>
        /// <exception cref="HDErrorCodes.HD_SCHEDULER_FULL">如果调度器已达到一次可支持的调度器操作数的上限。</exception>
        /// <exception cref="HDErrorCodes.HD_INVALID_PRIORITY">超出调度范围，参考：HDSchedulerPriority </exception>
        [DllImport(DLL_PATH)]
        public static extern void hdScheduleSynchronous(HDSchedulerCallback pCallback, IntPtr pUserData, HDSchedulerPriority nPriority);

        /// <summary>
        /// 在伺服循环(servo loop)中调度调度程序要执行的操作，而不等待其完成。无论优先级如何，从伺服循环线程提交的调度程序回调都会立即运行。
        /// <para>通常用于调度回调，运行伺服回路的每一个滴答。例如，可以在异步回调中运行动态模拟，并在该模拟中设置力。 </para>
        /// <see cref="HDSchedulerCallback"/>
        /// <see cref="HDAPI.hdStartScheduler"/>
        /// </summary>
        /// <param name="pCallback">回调函数</param>
        /// <param name="pUserData">函数要使用的数据</param>
        /// <param name="nPriority">调度优先级，它决定排序的顺序；当多个回调被调度时运行回调(更高的优先级意味着先运行)。</param>
        /// <returns>返回句柄用于取消调度或阻塞等待完成，可使用 hdUnschedule 取消， hdWaitForCompletion 获取调度状态。</returns>
        /// <exception cref="HDErrorCodes.HD_SCHEDULER_FULL">如果调度器已达到一次可支持的调度器操作数的上限。</exception>
        /// <exception cref="HDErrorCodes.HD_INVALID_PRIORITY">超出调度范围，参考：HDSchedulerPriority </exception>
        [DllImport(DLL_PATH)]
        public static extern ulong hdScheduleAsynchronous(HDSchedulerCallback pCallback, IntPtr pUserData, HDSchedulerPriority nPriority);

        /// <summary>
        /// 通过从调度程序中删除关联的回调来取消调度操作。
        /// <para>用于停止活动的异步操作。例如，如果应用程序线程创建了一个异步操作，该操作返回 HDCallbackCode.HD_CALLBACK_CONTINUE，则应用程序可以调用hUnschedule（）以强制终止回调。</para>
        /// <see cref="HDAPI.hdStopScheduler"/>
        /// <see cref="HDAPI.hdScheduleAsynchronous"/>
        /// <see cref="HDAPI.hdScheduleSynchronous"/>
        /// </summary>
        /// <param name="hHandler">计划的活动操作句柄，通过 hdscheduleAsynchronous() 或 hdscheduleAsynchronous() 获取</param>
        /// <exception cref="HDErrorCodes.HD_INVALID_OPERATION">如果与句柄关联的调度程序操作已经终止</exception>
        [DllImport(DLL_PATH)]
        public static extern void hdUnschedule(ulong hHandler);

        /// <summary>
        /// 检查回调是否仍计划执行。这可以用作非阻塞测试，也可以阻塞并等待回调完成。
        /// <para>可以在异步操作上使用，以检查状态或提交以等待操作完成</para>
        /// <para>如果将 HD_WAIT_CHECK_STATUS 传递给param，如果调度程序操作仍然处于活动状态，则返回true</para>
        /// <para>如果将 HD_WAIT_INFINITE 传递给param，如果等待成功，该函数将返回true，如果等待失败，则返回false</para>
        /// <see cref="HDAPI.hdUnschedule"/>
        /// <see cref="HDAPI.hdScheduleAsynchronous"/>
        /// <see cref=" HDAPI.hdScheduleSynchronous"/>
        /// </summary>
        /// <param name="pHandler">活动异步操作的句柄</param>
        /// <param name="param"></param>
        /// <returns>返回是否仍计划回调</returns>
        [DllImport(DLL_PATH)]
        public static extern byte hdWaitForCompletion(ulong pHandler, HDWaitCode param);
        //[DllImport(DLL_PATH)]
        //public static extern bool hdWaitForCompletion(ulong pHandler, HDWaitCode param);

        /// <summary>
        /// 开始调度器。调度程序管理要在伺服回路(servo loop)线程中执行的回调。
        /// <para>通常在添加了所有设备初始化例程和所有异步调度器回调之后调用此函数。
        /// 只有一个调度程序，因此无论使用多少设备，都需要启动一次，回调函数的执行从调度程序启动时开始，此时还启用了force。</para>
        /// <see cref="HDAPI.hdStopScheduler"/>
        /// </summary>
        /// <exception cref="HDErrorCodes.HD_TIMER_ERROR">如果伺服回路线程无法初始化或伺服回路无法启动。</exception>
        [DllImport(DLL_PATH)]
        public static extern void hdStartScheduler();

        /// <summary>
        /// 停止调度器
        /// <para>通常将此称为设备清理和关闭的第一步。</para>
        /// <see cref="HDAPI.hdUnschedule"/>
        /// <see cref="HDAPI.hdDisableDevice"/>
        /// </summary>
        /// <exception cref="HDErrorCodes.HD_TIMER_ERROR">如果伺服回路线程无法初始化</exception>
        [DllImport(DLL_PATH)]
        public static extern void hdStopScheduler();

        /// <summary>
        /// 获取自伺服回路(servo loop)滴答启动以来所经过的时间，这对于测量伺服回路的dutycycle是很有用的。
        /// <para>用于检查操作花费了多长时间。</para>
        /// </summary>
        /// <returns>返回从伺服回路开始计时的秒数</returns>
        [DllImport(DLL_PATH)]
        public static extern double hdGetSchedulerTimeStamp();

        /// <summary>
        /// 设置调度程序每秒计时回调的次数。
        /// <para>通常用于控制力渲染的保真度。大多数触觉应用程序运行在1000赫兹。</para>
        /// <para>PCI和EPP支持500、1000和2000赫兹。Firewire支持500、1000、1600赫兹，以及基于以下表达式的一些增量:floor(8000/N + 0.5)。</para>
        /// <para>需要注意的是，降低速度会导致不稳定和踢腿。提高伺服回路速度可以产生更硬的表面和更好的触觉响应，但留给调度程序的操作完成的时间更少。
        /// 在设置调度器速率时，检查hdGetSchedulerTimeStamp()和 HD_INSTANTANEOUS_UPDATE_RATE，以验证伺服回路能够维护请求的速率。</para>
        /// <para>有些设备可能支持各种设置。某些设备可能需要使用最新固件才能使用此功能。</para>
        /// <see cref="HDAPI.hdGetSchedulerTimeStamp"/>
        /// </summary>
        /// <param name="nRate">调度程序以多少赫兹速率运行</param>
        /// <exception cref="HDErrorCodes.HD_INVALID_VALUE">如果无法设置指定的nRate</exception>
        [DllImport(DLL_PATH)]
        public static extern void hdSetSchedulerRate(ulong nRate);
        #endregion


        //======================================================HDAPI DEPLOYMENT
        #region hdScaleGimbalAngles/hdDeploymentLicense
        /// <summary>
        /// [API文档里面没有写]万向坐标系缩放比例
        /// <para>Gimbal Scaling Routine</para>
        /// </summary>
        /// <param name="scaleX"></param>
        /// <param name="scaleY"></param>
        /// <param name="nT">nT 为一个16长度的数组</param>
        [DllImport(DLL_PATH)]
        public static extern void hdScaleGimbalAngles(double scaleX, double scaleY, double[] nT);

        [DllImport(DLL_PATH)]
        public static extern void hdScaleGimbalAngles(double scaleX, double scaleY, IntPtr nT);


        /// <summary>
        /// 激活颁发给应用程序开发人员的部署许可证。一旦验证了部署许可证，它将一直处于活动状态，直到应用程序退出为止。在应用程序会话中多次调用此函数不是错误。
        /// </summary>
        /// <param name="vendorName">向其颁发许可证的组织的名称</param>
        /// <param name="applicationName">许可证所绑定的应用程序的名称</param>
        /// <param name="password">验证部署许可证的许可证字符串</param>
        /// <returns></returns>
        /// <exception cref="HDErrorCodes.HD_DEVICE_FAULT"></exception>
        [DllImport(DLL_PATH, EntryPoint = "hdDeploymentLicense", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte hdDeploymentLicense(string vendorName, string applicationName, string password);
        #endregion

        
    }
}
