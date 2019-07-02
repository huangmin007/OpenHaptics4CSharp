using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace OH4CSharp.HD
{

    #region HDErrorInfo/HDErrorCodes
    /// <summary>
    /// 设备错误信息/或执行信息（也可理解为函数调用返回的信息，一不定完全理解为"错误信息"）
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct HDErrorInfo
    {
        /// <summary>
        /// 错误代码
        /// </summary>
        uint ErrorCode;
        //HDErrorCodes errorCode;

        /// <summary>
        /// 内部代码可用于从设备供应商获得额外的支持
        /// </summary>
        int InternalErrorCode;

        /// <summary>
        /// 错误发生时处于活动状态的设备句柄
        /// </summary>
        uint HHD;

        /// <summary>
        /// 错误检查，返回 true 表示有错误
        /// </summary>
        /// <returns></returns>
        public bool CheckedError()
        {
            return ErrorCode != 0x0000;
        }

        /// <summary>
        /// 检查是否发生错误
        /// <para>通常使用 HDDLL.hdGetError() 调用这个函数来检查错误堆栈是否包含错误。</para>
        /// <see cref="HDAPI.hdGetError"/>
        /// <see cref="HDAPI.hdGetErrorString"/>
        /// </summary>
        /// <param name="errorInfo">要检查的错误信息</param>
        /// <returns>返回true表示错误, false表示没有错误</returns>
        public static bool CheckedError(HDErrorInfo errorInfo)
        {
            return errorInfo.ErrorCode != 0x0000;
        }
    }

    /// <summary>
    /// Table A-15: Device Error Codes
    /// </summary>
    public enum HDErrorCodes
    {
        /// <summary>
        /// 无错误
        /// </summary>
        HD_SUCCESS = 0x0000,
        /// <summary>
        /// 要求以 Enum 数据类型作为其输入之一的函数指定的功能参数无效
        /// </summary>
        HD_INVALID_ENUM = 0x0100,
        /// <summary>
        /// Function error type
        /// </summary>
        HD_INVALID_VALUE = 0x0101,
        /// <summary>
        /// Function error type
        /// </summary>
        HD_INVALID_OPERATION = 0x0102,
        /// <summary>
        /// Function error type
        /// </summary>
        HD_INVALID_INPUT_TYPE = 0x0103,
        /// <summary>
        /// Function error type
        /// </summary>
        HD_BAD_HANDLE = 0x0104,

        /// <summary>
        /// Force error type
        /// </summary>
        HD_WARM_MOTORS = 0x0200,
        /// <summary>
        /// Force error type
        /// </summary>
        HD_EXCEEDED_MAX_FORCE = 0x0201,
        /// <summary>
        /// Force error type
        /// </summary>
        HD_EXCEEDED_MAX_FORCE_IMPULSE = 0x0202,
        /// <summary>
        /// Force error type
        /// </summary>
        HD_EXCEEDED_MAX_VELOCITY = 0x0203,
        /// <summary>
        /// Force error type
        /// </summary>
        HD_FORCE_ERROR = 0x0204,

        /// <summary>
        /// 设备故障，检查设备连接是否正确并已激活。
        /// <para>Device fault. Check that the device is connected properly and activated.</para>
        /// </summary>
        HD_DEVICE_FAULT = 0x0300,

        /// <summary>
        /// 设备名称已经初始化。
        /// <para>The device name was already initialized.</para>
        /// </summary>
        HD_DEVICE_ALREADY_INITIATED = 0x0301,

        /// <summary>
        /// Device error type
        /// </summary>
        HD_COMM_ERROR = 0x0302,
        /// <summary>
        /// Device error type
        /// </summary>
        HD_COMM_CONFIG_ERROR = 0x0303,
        /// <summary>
        /// Device error type
        /// </summary>
        HD_TIMER_ERROR = 0x0304,

        /// <summary>
        /// hdBeginFrame()用于已在设备帧中调用的设备，或在单个伺服回路调用多次。
        /// </summary>
        HD_ILLEGAL_BEGIN = 0x0400,
        /// <summary>
        /// 为设备调用hdEndFrame()之前没有为该设备调用匹配的hdBeginFrame()。
        /// </summary>
        HD_ILLEGAL_END = 0x0401,
        /// <summary>
        /// Haptic rendering context
        /// </summary>
        HD_FRAME_ERROR = 0x0402,

        /// <summary>
        /// Scheduler error
        /// </summary>
        HD_INVALID_PRIORITY = 0x0500,
        /// <summary>
        /// Scheduler error
        /// </summary>
        HD_SCHEDULER_FULL = 0x0501,

        /// <summary>
        /// Licensing error
        /// </summary>
        HD_INVALID_LICENSE = 0x0600,

    }
    #endregion


    #region hdEnable, hdDisable Parameters
    /// <summary>
    /// Table A-10: hdEnable, hdDisable Parameters
    /// <seealso cref="HDAPI.hdEnable(HDEDParameters)">hdEnabled</seealso>
    /// <seealso cref="HDAPI.hdDisable(HDEDParameters)">hdDisable</seealso>
    /// <seealso cref="HDAPI.hdIsEnabled(HDEDParameters)">hdIsEnabled</seealso>
    /// </summary>
    public enum HDEDParameters
    {
        /// <summary>
        /// 启用或禁用设备的力输出，所有电机都打开或关闭。
        /// <para>Enables or disables force output for the device. All motors are turned on or off.</para>
        /// </summary>
        HD_FORCE_OUTPUT = 0x4000,

        /// <summary>
        /// 启用或禁用设备的最大力夹紧。如果启用，这将钳位整体力输出量达到可达到的最大值。 
        /// <para>Enables or disables max force clamping for the device. If enabled, this will clamp the overall force output magnitude to the maximum achievable.</para>
        /// </summary>
        HD_MAX_FORCE_CLAMPING = 0x4001,

        /// <summary>
        /// 启用或禁用强制慢加速，即当调度器打开时，设备是否强制慢加速。
        /// <para>Enables or disables force ramping, i.e. whether the device ramps up forces when the scheduler is turned on.</para>
        /// </summary>
        HD_FORCE_RAMPING = 0x4002,

        /// <summary>
        /// 启用或禁用软件最大力检查，如果向设备发出的力大小超过最大力限制，则返回错误。
        /// 这主要是为了禁用强制反冲，禁用此功能的风险自负。
        /// <para>
        /// Enables or disables the software maximum force check, 
        /// which returns an error if the force magnitude commanded to the device exceeds the maximum force limit.
        /// This is primarily to disable force kicking. Disable this at your own risk.
        /// </para>
        /// </summary>
        HD_SOFTWARE_FORCE_LIMIT = 0x4003,

        /// <summary>
        /// 启用或禁用软件最大速度检查，如果向设备发出的速度大小超过最大速度限制，则返回错误。
        /// 这主要是为了禁用强制反冲，禁用此功能的风险自负。
        /// <para>Enables or disables the software maximum velocity check, which returns an error
        /// if the velocity magnitude commanded to the device exceeds the maximum velocity limit.
        /// This is primarily to disable force kicking. Disable this at your own risk.</para>
        /// </summary>
        HD_SOFTWARE_VELOCITY_LIMIT = 0x2606,

        /// <summary>
        /// 启用或禁用软件的最大力脉冲检查，以防止力脉冲和方向的变化超过脉冲极限的变化。
        /// 这主要是为了防止设备反撞，禁用此功能的风险自负。
        /// <para>Enables or disables the software maximum force impulse check, which prevents
        /// changes in force impulse and direction that exceed the change of impulse limit. This
        /// is primarily to prevent device kicking. Disable this at your own risk.</para>
        /// </summary>
        HD_SOFTWARE_FORCE_IMPULSE_LIMIT = 0x2607,

        /// <summary>
        /// 启用或禁用一帧限制检查，该检查将应用程序限制为每个调度器选中一个触觉帧，
        /// 只有在开发人员运行自己的调度器时才应禁用此功能。
        /// <para>Enables or disables the one frame limit check, 
        /// which restricts the application to one haptics frame per scheduler tick This should only 
        /// be disabled if the developer is running his own scheduler.</para>
        /// </summary>
        HD_ONE_FRAME_LIMIT = 0x4004,

        /// <summary>
        /// 启用或禁用设备上用户指定的LED状态灯设置。
        /// <para>Enables or disables user specified setting of the LED status light on the device.</para>
        /// </summary>
        HD_USER_STATUS_LIGHT = 0x2900,
    }
    #endregion


    #region hdGet Parameter Values
    /// <summary>
    /// Table A-1: hdGet Parameters
    /// <para>Table A-2: Get Forces Parameters</para>
    /// <para>Table A-3: Get Identification Parameters</para>
    /// <para>Table A-4: Get Last Values Parameters</para>
    /// <para>Table A-5: Get Safety Parameters</para>
    /// <para>Table A-6: Get Scheduler Update Codes Parameters</para>
    /// <para>[参数数量]可理解为字节数量</para>
    /// <see cref="HDAPI.hdGetBaseType(HDGetParameters pname, IntPtr value)"/>
    /// </summary>
    public enum HDGetParameters
    {
        #region Get Raw Parameters
        /// <summary>
        /// [Get Raw Parameters]获取按钮状态，可以通过按位 And 检索单个按钮值 HD_DEVICE_BUTTON_N (N = 1、2、3或4)。
        /// <para>参数数量：1</para>
        /// <para>允许类型：int</para>
        /// <para>计量单位：(无)</para>
        /// </summary>
        HD_CURRENT_BUTTONS = 0x2000,

        /// <summary>
        /// [Get Raw Parameters]查看安全开关(如果存在)是否处于活动状态。例如，如果触控笔的导电部分被握着，Geomagic Touch X安全开关是正确的。
        /// <para>参数数量：1</para>
        /// <para>允许类型：bool</para>
        /// <para>计量单位：(无)</para>
        /// </summary>
        HD_CURRENT_SAFETY_SWITCH = 0x2001,

        /// <summary>
        /// [Get Raw Parameters]如果墨水瓶开关存在，检查它是否处于活动状态。
        /// <para>参数数量：1</para>
        /// <para>允许类型：bool</para>
        /// <para>计量单位：(无)</para>
        /// </summary>
        HD_CURRENT_INKWELL_SWITCH = 0x2002,

        /// <summary>
        /// [Get Raw Parameters]获取原始编码器值。
        /// <para>参数数量：6</para>
        /// <para>允许类型：long</para>
        /// <para>计量单位：(无)</para>
        /// </summary>
        HD_CURRENT_ENCODER_VALUES = 0x2010,

        /// <summary>
        /// [Get Raw Parameters]获取当前规范化缩放编码器值(仅限Windows)。
        /// <para>参数数量：1</para>
        /// <para>允许类型：float,double</para>
        /// <para>计量单位：(无)</para>
        /// </summary>
        HD_CURRENT_PINCH_VALUE = 0x2011,

        /// <summary>
        /// [Get Raw Parameters]获取最后一个规范化缩放编码器值(仅限Windows)。
        /// <para>参数数量：1</para>
        /// <para>允许类型：float,double</para>
        /// <para>计量单位：(无)</para>
        /// </summary>
        HD_LAST_PINCH_VALUE = 0x2012,

        /// <summary>
        /// 取设备的关节角度。这些是关节角度，用于计算电枢相对于设备基架的运动学。For Touch devices: Turet Left +, Thigh Up +, Shin Up +
        /// <para>参数数量：3</para>
        /// <para>允许类型：float,double</para>
        /// <para>计量单位：rad</para>
        /// </summary>
        HD_CURRENT_JOINT_ANGLES = 0x2100,

        /// <summary>
        /// 从设备万向坐标系中获取角度。From Neutral position Right is +, Up is -, CW is +
        /// <para>参数数量：3</para>
        /// <para>允许类型：float,double</para>
        /// <para>计量单位：rad</para>
        /// </summary>
        HD_CURRENT_GIMBAL_ANGLES = 0x2150,

        /// <summary>
        /// 获取LED状态灯的用户设置。
        /// <para>参数数量：1</para>
        /// <para>允许类型：int</para>
        /// <para>计量单位：(无)</para>
        /// </summary>
        HD_USER_STATUS_LIGHT = 0x2900,
        #endregion


        #region Get Cartesian Space Value Parameters
        /// <summary>
        /// [Get Cartesian Space Value Parameters]
        /// 获取设备面对设备基座的当前位置。右边是+ x，上面是+y，朝向用户是+z。
        /// <para>参数数量：3</para>
        /// <para>允许类型：float,double</para>
        /// <para>计量单位：mm</para>
        /// </summary>
        HD_CURRENT_POSITION = 0x2050,

        /// <summary>
        /// [Get Cartesian Space Value Parameters]
        /// 获取设备的当前速度，注意：这个值被平滑以减少高频抖动。
        /// <para>参数数量：3</para>
        /// <para>允许类型：float,double</para>
        /// <para>计量单位：mm/s</para>
        /// </summary>
        HD_CURRENT_VELOCITY = 0x2051,

        /// <summary>
        /// [Get Cartesian Space Value Parameters]
        /// 获取设备endeffector的列主转换。
        /// <para>Get the column-major transform of the device endeffector.</para>
        /// <para>参数数量：16</para>
        /// <para>允许类型：float,double</para>
        /// <para>计量单位：(无)</para>
        /// </summary>
        HD_CURRENT_TRANSFORM = 0x2052,

        /// <summary>
        /// [Get Cartesian Space Value Parameters]
        /// 从设备万向坐标系中获取角速度。
        /// <para>参数数量：3</para>
        /// <para>允许类型：float,double</para>
        /// <para>计量单位：rad/s</para>
        /// </summary>
        HD_CURRENT_ANGULAR_VELOCITY = 0x2053,

        /// <summary>
        /// [Get Cartesian Space Value Parameters]API文档上未查到
        /// <para>参数数量：</para>
        /// <para>允许类型：</para>
        /// <para>计量单位：</para>
        /// </summary>
        HD_CURRENT_JACOBIAN = 0x2054,
        #endregion


        #region Get Forces Parameters
        /// <summary>
        /// [Get Forces Parameters]
        /// 获取当前力，即用户在调用它的帧期间向设备发出的力。如果在帧中尚未命令任何力，则返回0。
        /// <para>参数数量：3</para>
        /// <para>允许类型：float,double</para>
        /// <para>计量单位：N</para>
        /// </summary>
        HD_CURRENT_FORCE = 0x2700,

        /// <summary>
        /// [Get Forces Parameters]获取当前扭矩，即用户在调用该转矩的帧期间向设备发出的扭矩。如果在帧中尚未命令扭矩，则返回0。
        /// <para>参数数量：3</para>
        /// <para>允许类型：float,double</para>
        /// <para>计量单位：mNm</para>
        /// </summary>
        HD_CURRENT_TORQUE = 0x2701,

        /// <summary>
        /// [Get Forces Parameters]获取当前电机DAC，即用户在调用该DAC的帧期间向设备发出的电机值。如果在帧中还没有命令DAC，则返回0。
        /// <para>参数数量：3 or 6</para>
        /// <para>允许类型：long</para>
        /// <para>计量单位：包含范围 "[-32767,32766]"</para>
        /// </summary>
        HD_CURRENT_MOTOR_DAC_VALUES = 0x2750,

        /// <summary>
        /// [Get Forces Parameters]API文档上未查到信息
        /// <para>参数数量：</para>
        /// <para>允许类型：</para>
        /// <para>计量单位：</para>
        /// </summary>
        HD_JOINT_ANGLE_REFERENCES = 0x2702,

        /// <summary>
        /// [Get Forces Parameters]获取当前关节扭矩，即用户在调用该扭矩的帧中对设备前3个关节所发出的扭矩。如果在帧中还没有控制关节扭矩，则返回0。
        /// <para>参数数量：3</para>
        /// <para>允许类型：float,double</para>
        /// <para>计量单位：mNm</para>
        /// </summary>
        HD_CURRENT_JOINT_TORQUE = 0x2703,

        /// <summary>
        /// [Get Forces Parameters]获取当前帧扭矩，即用户在调用帧时向设备发出的帧扭矩。如果帧中还没有控制帧扭矩，则返回0。
        /// <para>参数数量：3</para>
        /// <para>允许类型：float,double</para>
        /// <para>计量单位：mNm</para>
        /// </summary>
        HD_CURRENT_GIMBAL_TORQUE = 0x2704,
        #endregion


        #region Get Last Parameters
        /// <summary>
        /// [Get Last Parameters]获取最后一帧的按钮状态。单个按钮值可以通过 按位And 使用HD_DEVICE_BUTTON_N来检索(N = 1、2、3、4)。
        /// <para>参数数量：1</para>
        /// <para>允许类型：int</para>
        /// <para>计量单位：(无)</para>
        /// </summary>
        HD_LAST_BUTTONS = 0x2200,

        /// <summary>
        /// [Get Last Parameters]获取最后一帧的安全开关激活状态。
        /// <para>参数数量：1</para>
        /// <para>允许类型：bool</para>
        /// <para>计量单位：(无)</para>
        /// </summary>
        HD_LAST_SAFETY_SWITCH = 0x2201,

        /// <summary>
        /// [Get Last Parameters]获取最后一帧的墨水瓶开关激活状态。
        /// <para>参数数量：1</para>
        /// <para>允许类型：bool</para>
        /// <para>计量单位：(无)</para>
        /// </summary>
        HD_LAST_INKWELL_SWITCH = 0x2202,

        /// <summary>
        /// [Get Last Parameters]获取上一帧的原始编码器值。
        /// <para>参数数量：6</para>
        /// <para>允许类型：long</para>
        /// <para>计量单位：(无)</para>
        /// </summary>
        HD_LAST_ENCODER_VALUES = 0x2210,

        /// <summary>
        /// [Get Last Parameters]获取设备面对设备基座的最后一个位置。右边是+x，上面是+y，面向用户是+z。
        /// <para>参数数量：3</para>
        /// <para>允许类型：float,double</para>
        /// <para>计量单位：mm</para>
        /// </summary>
        HD_LAST_POSITION = 0x2250,

        /// <summary>
        /// [Get Last Parameters]获取设备的最后速度。注意:这个值是平滑的，以减少高频抖动。
        /// <para>参数数量：3</para>
        /// <para>允许类型：float,double</para>
        /// <para>计量单位：mm/s</para>
        /// </summary>
        HD_LAST_VELOCITY = 0x2251,

        /// <summary>
        /// [Get Last Parameters]获取设备端执行器的最后一行主变换。
        /// <para>参数数量：16</para>
        /// <para>允许类型：float,double</para>
        /// <para>计量单位：(无)</para>
        /// </summary>
        HD_LAST_TRANSFORM = 0x2252,

        /// <summary>
        /// [Get Last Parameters]获取设备框架的最后一个笛卡尔角速度。
        /// <para>参数数量：3</para>
        /// <para>允许类型：float,double</para>
        /// <para>计量单位：rad</para>
        /// </summary>
        HD_LAST_ANGULAR_VELOCITY = 0x2253,

        /// <summary>
        /// [Get Last Parameters]API文档上未查到信息
        /// <para>参数数量：</para>
        /// <para>允许类型：</para>
        /// <para>计量单位：</para>
        /// </summary>
        HD_LAST_JACOBIAN = 0x2254,

        /// <summary>
        /// [Get Last Parameters]获取设备的最后一个关节角度。这些关节角用于计算电枢相对于该装置的基础框架的运动学。For Touch devices:Turet Left +, Thigh Up +, Shin Up +
        /// <para>参数数量：3</para>
        /// <para>允许类型：float,double</para>
        /// <para>计量单位：rd</para>
        /// </summary>
        HD_LAST_JOINT_ANGLES = 0x2300,

        /// <summary>
        /// 获取设备框架的最后一个角度。From Neutral position Right is +, Up is -, CW is +
        /// <para>参数数量：3</para>
        /// <para>允许类型：float,double</para>
        /// <para>计量单位：rad/s</para>
        /// </summary>
        HD_LAST_GIMBAL_ANGLES = 0x2350,
        #endregion


        #region Get Identification Parameters
        /// <summary>
        /// [Get Identification Parameters]
        /// 获取设备的最大工作空间尺寸，即设备的最大机械极限，为(minX, minY, minZ, maxX, maxY, maxZ)。
        /// <para>参数数量：6</para>
        /// <para>允许类型：float,double</para>
        /// <para>计量单位：mm</para>
        /// </summary>
        HD_MAX_WORKSPACE_DIMENSIONS = 0x2550,

        /// <summary>
        /// [Get Identification Parameters]
        /// 获取设备的可用工作区维度，即设备的实际限制，如(minX、minY、minZ、maxX、maxY、maxZ)。
        /// <para>参数数量：6</para>
        /// <para>允许类型：float,double</para>
        /// <para>计量单位：mm</para>
        /// </summary>
        HD_USABLE_WORKSPACE_DIMENSIONS = 0x2551,

        /// <summary>
        /// [Get Identification Parameters]
        /// 从工作台上取设备的机械偏移量为Y。/从工作台顶部获取Y中设备endeffector的机械偏移。
        /// <para>参数数量：1</para>
        /// <para>允许类型：float</para>
        /// <para>计量单位：mm</para>
        /// </summary>
        HD_TABLETOP_OFFSET = 0x2552,

        /// <summary>
        /// [Get Identification Parameters]获取输入自由度的数目。
        /// （即，完全指定触摸设备的末端效应器位置所需的独立位置变量数量）3DOF输入仅表示XYZ变速感测。6DOF表示3个平移和3个旋转。
        /// <para>参数数量：1</para>
        /// <para>允许类型：int</para>
        /// <para>计量单位：(无)</para>
        /// </summary>
        HD_INPUT_DOF = 0x2553,

        /// <summary>
        /// [Get Identification Parameters]
        /// 获取输出自由度的个数，即独立驱动变量的个数。对于触控设备，3DOF表示XYZ线性力输出，而6DOF表示XYZ线性力和万向节的滚转、俯仰、偏航、扭矩。
        /// <para>参数数量：1</para>
        /// <para>允许类型：int</para>
        /// <para>计量单位：(无)</para>
        /// </summary>
        HD_OUTPUT_DOF = 0x2554,

        /// <summary>
        /// [Get Identification Parameters]
        /// 仪器所支持的校正方式。可以是以下hd_calibration_auto的一个或多个位，HD_CALIBRATION_ENCODER_RESET或HD_CALIBRATION_INKWELL
        /// <para>参数数量：1</para>
        /// <para>允许类型：int</para>
        /// <para>计量单位：(无)</para>
        /// </summary>
        HD_CALIBRATION_STYLE = 0x2555,
        #endregion


        #region Get Scheduler Update Codes Parameters
        /// <summary>
        /// [Get Scheduler Update Codes Parameters]
        /// 获取设备的平均更新速率，即调度程序每秒执行的更新数。
        /// <para>参数数量：1</para>
        /// <para>允许类型：int</para>
        /// <para>计量单位：Hz</para>
        /// </summary>
        HD_UPDATE_RATE = 0x2600,

        /// <summary>
        /// [Get Scheduler Update Codes Parameters]
        /// 获取设备的瞬时更新速率，即I/T，其中T为上次更新后的时间，单位为秒。
        /// <para>参数数量：1</para>
        /// <para>允许类型：int</para>
        /// <para>计量单位：Hz</para>
        /// </summary>
        HD_INSTANTANEOUS_UPDATE_RATE = 0x2601,
        #endregion


        #region Get Safety Parameters
        /// <summary>
        /// [Get Safety Parameters]
        /// 获取设备建议的最大闭环刚度，即F=kx输出中使用的弹簧常数，其中k为弹簧常数，x为弹簧长度。
        /// <para>参数数量：1</para>
        /// <para>允许类型：float,double</para>
        /// <para>计量单位：0 or 1</para>
        /// </summary>
        HD_NOMINAL_MAX_STIFFNESS = 0x2602,

        /// <summary>
        /// [Get Safety Parameters]
        /// 获取设备建议的最大阻尼水平，即F=kx-dv中使用的阻尼常数，其中d为阻尼常数，v为设备速度。
        /// <para>参数数量：1</para>
        /// <para>允许类型：float,double</para>
        /// <para>计量单位：0 or 1</para>
        /// </summary>
        HD_NOMINAL_MAX_DAMPING = 0x2609,

        /// <summary>
        /// [Get Safety Parameters]
        /// 获取额定最大力，即电机处于室温（最佳）时设备能够承受的力。
        /// <para>参数数量：1</para>
        /// <para>允许类型：float,double</para>
        /// <para>计量单位：N</para>
        /// </summary>
        HD_NOMINAL_MAX_FORCE = 0x2603,

        /// <summary>
        /// [Get Safety Parameters]
        /// 获取名义最大连续力，即装置在一段时间内所能承受的力。
        /// <para>参数数量：1</para>
        /// <para>允许类型：float,double</para>
        /// <para>计量单位：N</para>
        /// </summary>
        HD_NOMINAL_MAX_CONTINUOUS_FORCE = 0x2604,

        /// <summary>
        /// [Get Safety Parameters]
        /// 获取电机温度，这是所有电机的预测温度。
        /// <para>参数数量：3 or 6</para>
        /// <para>允许类型：float,double</para>
        /// <para>计量单位：0(coldest)  to 1(warmest)</para>
        /// </summary>
        HD_MOTOR_TEMPERATURE = 0x2605,

        /// <summary>
        /// [Get Safety Parameters]
        /// 获取软件最大速度限制。这不会取代设备的硬件限制
        /// <para>参数数量：1</para>
        /// <para>允许类型：float,double</para>
        /// <para>计量单位：mm/s</para>
        /// </summary>
        HD_SOFTWARE_VELOCITY_LIMIT = 0x2606,

        /// <summary>
        /// [Get Safety Parameters]
        /// 得到软件的最大推力脉冲限制，这并不会取代设备的硬件限制。
        /// <para>参数数量：1</para>
        /// <para>允许类型：float</para>
        /// <para>计量单位：N</para>
        /// </summary>
        HD_SOFTWARE_FORCE_IMPULSE_LIMIT = 0x2607,

        /// <summary>
        /// [Get Safety Parameters]
        /// 获取强制爬坡率，这是设备在调度程序启动或出错后强制爬坡的速率。
        /// <para>参数数量：1</para>
        /// <para>允许类型：float,double</para>
        /// <para>计量单位：N/s</para>
        /// </summary>
        HD_FORCE_RAMPING_RATE = 0x2608,
        #endregion


        #region Get Nominal Max Torque Parameters
        /// <summary>
        /// [API上没查到]获取名义上最大的扭矩的强度
        /// </summary>
        HD_NOMINAL_MAX_TORQUE_STIFFNESS = 0x2620,
        /// <summary>
        /// [API上没查到]获取名义上最大的扭矩的阻尼
        /// </summary>
        HD_NOMINAL_MAX_TORQUE_DAMPING = 0x2621,
        /// <summary>
        /// [API上没查到]获取名义上最大的扭矩的力
        /// </summary>
        HD_NOMINAL_MAX_TORQUE_FORCE = 0x2622,
        /// <summary>
        /// /// <summary>
        /// [API上没查到]获取名义上最大的扭矩的连续力
        /// </summary>
        HD_NOMINAL_MAX_TORQUE_CONTINUOUS_FORCE = 0x2623,
        #endregion
    }

    /// <summary>
    /// Table A-3: Get Identification Parameters
    /// <seealso cref="HDAPI.hdGetString(HDGetStringParameters)"/>
    /// </summary>
    public enum HDGetStringParameters
    {
        /// <summary>
        /// 获取 HDAPI 软件版本
        /// </summary>
        HD_VERSION = 0x2500,

        /// <summary>
        /// 获取设备模型类型的可读字符串
        /// </summary>
        HD_DEVICE_MODEL_TYPE = 0x2501,

        /// <summary>
        /// 获取驱动程序版本的可读字符串
        /// </summary>
        HD_DEVICE_DRIVER_VERSION = 0x2502,

        /// <summary>
        /// 获取设备供应商的可读字符串
        /// </summary>
        HD_DEVICE_VENDOR = 0x2503,

        /// <summary>
        /// 获取设备序列号的可读字符串
        /// </summary>
        HD_DEVICE_SERIAL_NUMBER = 0x2504,
    }
    #endregion


    #region hdSet Parameter Values
    /// <summary>
    /// Table A-7: Set Parameters
    /// <para>Table A-8: Set Forces Parameters</para>
    /// <para>Table A-9: Set Safety Parameters</para>
    /// <para>[参数数量]可理解为字节数量</para>
    /// <see cref="HDAPI.hdSetBaseType(HDSetParameters pname, IntPtr value)"/>
    /// </summary>
    public enum HDSetParameters
    {
        /// <summary>
        /// [Set LED Parameters]允许用户设置LED状态灯
        /// <para>参数数量：1</para>
        /// <para>允许类型：int</para>
        /// <para>计量单位：(无)</para>
        /// </summary>
        HD_USER_STATUS_LIGHT = 0x2900,

        /// <summary>
        /// [Set Safety Parameters]设置软件的最大速度限制，这并不会取代设备的硬件限制。
        /// <para>参数数量：1</para>
        /// <para>允许类型：float,double</para>
        /// <para>计量单位：mm/s</para>
        /// </summary>
        HD_SOFTWARE_VELOCITY_LIMIT = 0x2606,

        /// <summary>
        /// [Set Safety Parameters]设置软件最大力脉冲限值，即通过计算当前力和最后一个命令力之间的差而计算出的大小和方向的最大变化。
        /// <para>参数数量：1</para>
        /// <para>允许类型：float,double</para>
        /// <para>计量单位：N/ms</para>
        /// </summary>
        HD_SOFTWARE_FORCE_IMPULSE_LIMIT = 0x2607,

        /// <summary>
        /// [Set Safety Parameters]设置强制爬坡率，这是在调度程序启动或出错后设备强制爬坡的速率。
        /// <para>参数数量：1</para>
        /// <para>允许类型：float,double</para>
        /// <para>计量单位：N/s</para>
        /// </summary>
        HD_FORCE_RAMPING_RATE = 0x2608,

        /// <summary>
        /// [Set Forces Parameters]将当前力设置为笛卡尔坐标向量。这是向设备发送力的主要方法，设置当前力会导致在
        /// 的末尾执行该力。
        /// <para>参数数量：3</para>
        /// <para>允许类型：N</para>
        /// <para>计量单位：Hz</para>
        /// </summary>
        HD_CURRENT_FORCE = 0x2700,

        /// <summary>
        /// [Set Forces Parameters]设置当前扭矩笛卡尔坐标矢量，对于6DOF设备，这是向设备发送扭矩的主要方法。设置当前扭矩会导致该扭矩被控制在帧的末端。
        /// <para>参数数量：3</para>
        /// <para>允许类型：Nm</para>
        /// <para>计量单位：Hz</para>
        /// </summary>
        HD_CURRENT_TORQUE = 0x2701,

        /// <summary>
        /// [Set Forces Parameters]设置当前扭矩，为触摸设备前三个关节的关节协调向量。设置当前扭矩会导致在机架末端命令该扭矩。
        /// <para>参数数量：3</para>
        /// <para>允许类型：nNm</para>
        /// <para>计量单位：(无)</para>
        /// </summary>
        HD_CURRENT_JOINT_TORQUE = 0x2703,

        /// <summary>
        /// [Set Forces Parameters]将当前帧扭矩设置为触摸6DOF装置的三个帧接头的关节协调向量。设置当前的平衡架扭矩会导致在机架末端命令该扭矩。
        /// <para>参数数量：3</para>
        /// <para>允许类型：nNm</para>
        /// <para>计量单位：(无)</para>
        /// </summary>
        HD_CURRENT_GIMBAL_TORQUE = 0x2704,

        /// <summary>
        /// [Set Forces Parameters]设置当前电机的DAC值。这是控制电机转矩计数的主要方法。这种方法目前不能与笛卡儿力或扭矩命令结合使用。
        /// <para>参数数量：3 or 6</para>
        /// <para>允许类型：包含范围 "[-32767,32766]"</para>
        /// <para>计量单位：(无)</para>
        /// </summary>
        HD_CURRENT_MOTOR_DAC_VALUES = 0x2750,
    }
    #endregion


    #region Calibration Return Codes & Styles
    /// <summary>
    /// Table A-11: Calibration Return Codes
    /// </summary>
    public enum HDCalibrationCodes
    {
        /// <summary>
        /// 校准是准确的
        /// </summary>
        HD_CALIBRATION_OK = 0x5000,

        /// <summary>
        /// 校准需要更新，调用hdUpdateCalibration()来更新设备的校准。
        /// </summary>
        HD_CALIBRATION_NEEDS_UPDATE = 0x5001,

        /// <summary>
        /// 校准需要手动输入，例如让用户将设备置于重置位置或墨水瓶中。
        /// 一旦用户这样做，进一步的调用应该不再返回校准需要手动输入。
        /// </summary>
        HD_CALIBRATION_NEEDS_MANUAL_INPUT = 0x5002,
    }

    /// <summary>
    /// Table A-12: Calibration Styles
    /// </summary>
    [Flags]
    public enum HDCalibrationStyles
    {
        /// <summary>
        /// 需要将设备置于复位位置进行校准。
        /// </summary>
        HD_CALIBRATION_ENCODER_RESET = 0x01,     //(1 << 0)

        /// <summary>
        /// 当电枢移动时，该装置将收集新的校准信息。
        /// </summary>
        HD_CALIBRATION_AUTO = 0x02,     //(1 << 1)

        /// <summary>
        /// 墨水池校准。在进行校准之前，该设备需要放入夹具，即墨水瓶中。
        /// </summary>
        HD_CALIBRATION_INKWELL = 0x04,     //(1 << 2)
    }
    #endregion


    #region Schedule Callback Codes/Priority/WaitCodes
    /// <summary>
    /// 调度函数回调码。确定操作是否已完成并将不调度，或是否将继续运行。
    /// <para>Return code for callbacks, determines whether the operation is finished and will unscheduled, or whether it will continue running.</para>
    /// </summary>
    public enum HDCallbackCode
    {
        /// <summary>
        /// 回调完成
        /// </summary>
        HD_CALLBACK_DONE = 0,

        /// <summary>
        /// 回调继续
        /// </summary>
        HD_CALLBACK_CONTINUE = 1,
    }

    /// <summary>
    /// 调用的优先级范围
    /// <para>Scheduler Priority Ranges/Codes</para>
    /// </summary>
    public enum HDSchedulerPriority
    {
        /// <summary>
        /// 调度程序回调的最大优先级。
        /// <para>设置具有此优先级的回调意味着，回调将首先运行与其他回调相关的每个调度程序滴答。</para>
        /// </summary>
        HD_MAX_SCHEDULER_PRIORITY = 0xFFFF,

        /// <summary>
        /// 调度程序回调的最小优先级。
        /// <para>设置具有此优先级的回调意味着回调将在与其他回调相关的每个调度程序滴答的最后一次运行。</para>
        /// </summary>
        HD_MIN_SCHEDULER_PRIORITY = 0x0000,

        /// <summary>
        /// 默认调度优先级。
        /// <para>将此设置用于回调，这些回调不关心在调度器滴答过程中何时执行。这个值介于最小优先级和最大优先级之间。</para>
        /// </summary>
        HD_DEFAULT_SCHEDULER_PRIORITY = 0x7FFF, //((HD_MAX_SCHEDULER_PRIORITY + HD_MIN_SCHEDULER_PRIORITY) / 2);
    }

    /// <summary>
    /// 等待调度码。hdWaitForCompletion()参数
    /// </summary>
    public enum HDWaitCode
    {
        /// <summary>
        /// 给定句柄的调度程序操作是否仍然调度
        /// </summary>
        HD_WAIT_CHECK_STATUS = 0,

        /// <summary>
        /// 等待调度程序操作完成
        /// </summary>
        HD_WAIT_INFINITE = 1,
    }
    #endregion

    /// <summary>
    /// Button Masks
    /// </summary>
    [Flags]
    public enum HDButtonMasks
    {
        HD_DEVICE_BUTTON_1 = (1 << 0),
        HD_DEVICE_BUTTON_2 = (1 << 1),
        HD_DEVICE_BUTTON_3 = (1 << 2),
        HD_DEVICE_BUTTON_4 = (1 << 3),
    }
}
