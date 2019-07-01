using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OH4CSharp.HD;
using System.Runtime.InteropServices;

namespace OH4CSharp.HL
{
    #region HL Error / Error Codes
    /// <summary>
    /// HL错误结构数据
    /// </summary>
    public struct HLError
    {
        /// <summary>
        /// errorCode 字符指针
        /// <para>注意：指针值是枚举字符串值，而不是枚举值</para>
        /// <see cref="HLErrorCodes"/>
        /// </summary>
        public IntPtr errorCode;
        /// <summary>
        /// <see cref="HDAPI.HDErrorInfo"/>
        /// </summary>
        public HDErrorInfo errorInfo;

        public String GetErrorCodeString()
        {
            return Marshal.PtrToStringAnsi(errorCode);
        }

        public static string GetErrorCodeString(HLError error)
        {
            return error.GetErrorCodeString();
        }
    }

    /// <summary>
    /// Table B-3: hlGetError
    /// </summary>
    public enum HLErrorCodes
    {
        /// <summary>
        /// 设备错误
        /// </summary>
        HL_DEVICE_ERROR,

        /// <summary>
        /// 无效的枚举值/参数
        /// </summary>
        HL_INVALID_ENUM,

        /// <summary>
        /// 无效的操作。可能的错误，设备句柄不存在，或设备/渲染上下文不存在
        /// </summary>
        HL_INVALID_OPERATION,

        /// <summary>
        /// 无效的值，或是超出范围的值
        /// </summary>
        HL_INVALID_VALUE,

        /// <summary>
        /// 无错误
        /// </summary>
        HL_NO_ERROR,

        /// <summary>
        /// 内存溢出
        /// </summary>
        HL_OUT_OF_MEMORY,

        /// <summary>
        /// 堆栈溢出
        /// </summary>
        HL_STACK_OVERFLOW,

        /// <summary>
        /// 堆栈下溢出
        /// </summary>
        HL_STACK_UNDERFLOW,

        /// <summary>
        /// 无效的许可
        /// </summary>
        HL_INVALID_LICENSE,
    }

    #endregion

    //===============================hlEnable, hlDisable, hlIsEnabled
    #region Capability Parameters
    /// <summary>
    /// Table B-9: hlEnable, hlDisable, hlIsEnabled
    /// </summary>
    public enum HLCapabilityParameters
    {
        /// <summary>
        /// 如果启用此功能，则此功能可作为深度缓冲区形状单遍呈现的优化。
        /// <para>该特性根据代理的位置和运动自适应调整视图的回读像素尺寸。这一功能与触觉相机视图不兼容。这在默认情况下是禁用的。</para>
        /// </summary>
        HL_ADAPTIVE_VIEWPORT,

        /// <summary>
        /// 通过基于代理的运动修改形状查看转换来增强形状呈现。
        /// <para>这允许用户感受主相机视图中被遮挡的深度缓冲特性。触觉相机视图还可以通过调整查看量和视图端口来进行优化，这样图形管道只处理接近代理的几何图形。这在默认情况下是禁用的。</para>
        /// </summary>
        HL_HAPTIC_CAMERA_VIEW,

        /// <summary>
        /// 如果启用，请使用形状几何自动更新代理位置。
        /// <para>计算出的代理位置对于每个帧指定的所有形状都是有效的。如果禁用，形状将被忽略，代理位置将由客户机程序设置。这是默认启用的。</para>
        /// </summary>
        HL_PROXY_RESOLUTION,

        /// <summary>
        /// 如果启用，将OpenGL模型视图矩阵应用于所有用于触觉呈现的几何图形，并忽略当前HL_MODELVIEW设置。
        /// <para>在没有有效的OpenGL呈现上下文查询的HLAPI程序中，应该禁用此功能。它是默认启用的。</para>
        /// </summary>
        HL_USE_GL_MODELVIEW,
    }
    #endregion

    #region HL hlGetBooleanv/hlGetDoublev/hlGetIntegerv/GetString Parameters
    /// <summary>
    /// Table B-1: hlGetBooleanv, hlGetIntegerv, hlGetDoublev
    /// </summary>
    public enum HLGetParameters
    {
        /// <summary>
        /// 一个布尔值，表示触觉设备上的第一个按钮
        /// </summary>
        HL_BUTTON1_STATE,

        /// <summary>
        /// 一个布尔值，表示触觉设备上的第二个按钮
        /// </summary>
        HL_BUTTON2_STATE,

        /// <summary>
        /// 一个布尔值，表示触觉装置上的安全开关(如果存在)。true值表示开关被抑制。
        /// </summary>
        HL_SAFETY_STATE,

        HL_INKWELL_STATE,

        HL_DEPTH_OF_PENETRATION,

        HL_DEVICE_FORCE,

        HL_DEVICE_POSITION,

        HL_DEVICE_ROTATION,

        HL_DEVICE_TORQUE,

        HL_DEVICE_TRANSFORM,

        HL_EVENT_MOTION_ANGULAR_TOLERANCE,

        HL_EVENT_MOTION_LINEAR_TOLERANCE,

        HL_GOLDEN_POSITION,

        HL_GOLDEN_RADIUS,

        HL_MODELVIEW,

        HL_PROXY_IS_TOUCHING,

        HL_PROXY_POSITION,

        HL_PROXY_ROTATION,

        HL_TOUCHABLE_FACE,

        HL_PROXY_TOUCH_NORMAL,

        HL_PROXY_TRANSFORM,

        HL_TOUCHWORKSPACE_MATRIX,

        HL_VIEWTOUCH,


    }

    /// <summary>
    /// Table B-4: hlGetString
    /// </summary>
    public enum HLGetStringParameters
    {
        /// <summary>
        /// 返回负责实现haptic renderer的公司名称。
        /// </summary>
        HL_VENDOR,

        /// <summary>
        /// 以形式的字符串形式返回触觉呈现库的版本号:major_number.minor_number.build_number
        /// </summary>
        HL_VERSION,
    }

    /// <summary>
    /// Table B-2: hlCacheGetBooleanv, hlCacheGetDoublev
    /// </summary>
    public enum HLCacheGetParameters
    {
        HL_BUTTON1_STATE,
        HL_BUTTON2_STATE,
        HL_SAFETY_STATE,
        HL_INKWELL_STATE,
        HL_DEVICE_FORCE,
        HL_DEVICE_POSITION,
        HL_DEVICE_ROTATION,
        HL_DEVICE_TORQUE,
        HL_DEVICE_TRANSFORM,
        HL_PROXY_IS_TOUCHING,
        HL_PROXY_POSITION,
        HL_PROXY_ROTATION,
        HL_PROXY_TOUCH_NORMAL,
        HL_PROXY_TRANSFORM,
    }
    #endregion

    /// <summary>
    /// Table B-5: hlHinti, hlHintb
    /// </summary>
    public enum HLHintParameters
    {
        /// <summary>
        /// 一个布尔值，指示后面的形状是否应视为动态变化的曲面。
        /// <para>true值告诉触觉呈现引擎进行额外的处理，以确保代理不会落入动态变化的表面。</para>
        /// <para>false值告诉渲染引擎对不会改变的表面优化触觉渲染。</para>
        /// </summary>
        HL_SHAPE_DYNAMIC_SURFACE_CHANGE,

        /// <summary>
        /// 一个整数值，向触觉渲染引擎表示下一个反馈缓冲区形状中大约有多少个顶点，以便它可以保留正确的内存量
        /// </summary>
        HL_SHAPE_FEEDBACK_BUFFER_VERTICES,

        /// <summary>
        /// 一个布尔值，指示后面的形状是否应该在当前帧之外持久。
        /// <para>true值告诉触觉渲染器，形状在下一帧中不需要重新指定，但是应该自动包含它。这对于背景对象特别有用，即场景中很少变化的静态对象，因为这些对象只需要指定一次并留在场景中，而不是让场景为每个帧重新计算对象。若要清除持久性形状，请在随后的任何帧中将其设置为非持久性。</para>
        /// <para>注意:这是v2.0中支持最少的实验特性。在下一个版本中，它很可能被弃用、删除或替换为替代API命令。</para>
        /// </summary>
        HL_SHAPE_PERSISTENCE,
    }

    /// <summary>
    ///  Table B-6: hlBegin Shape
    /// </summary>
    public enum HLBeginShapeParams
    {
        HL_SHAPE_CALLBACK,
        HL_SHAPE_DEPTH_BUFFER,
        HL_SHAPE_FEEDBACK_BUFFER,
    }

    /// <summary>
    /// Table B-8: hlLocalFeature types
    /// </summary>
    public enum HLLocalFeatureTypes
    {
        /// <summary>
        /// 局部特征是线段，线段的起点和终点分别由向量v1和v2给出
        /// </summary>
        HL_LOCAL_FEATURE_LINE,

        /// <summary>
        /// 局部特征是一个平面它的法线是由向量v1给出的并且通过点v1。
        /// </summary>
        HL_LOCAL_FEATURE_PLANE,

        /// <summary>
        /// 局部特征是由向量v给出位置的单点。
        /// </summary>
        HL_LOCAL_FEATURE_POINT,
    }

    /// <summary>
    ///  Table B-7: hlGetShapeBooleanv, hlGetShapeDoublev
    /// </summary>
    public enum HLGetShapeParams
    {
        /// <summary>
        /// 单一的布尔值。True表示代理当前与形状保持联系。
        /// </summary>
        HL_PROXY_IS_TOUCHING,

        /// <summary>
        /// 向量 double[3] 示该形状对最后一帧期间发送给触觉装置的总反作用力的贡献。如果代理未与最后一帧形状接触，则此向量将为零。
        /// </summary>
        HL_REACTION_FORCE,
    }

    /// <summary>
    /// Table B-12: hlGetMaterialfv - face values
    /// </summary>
    public enum HLGetMaterialfvFaces
    {
        /// <summary>
        /// 查询形状正面的材质属性和所有约束条件。
        /// </summary>
        HL_FRONT,
        /// <summary>
        /// 查询形状背面的材质属性和所有约束条件。
        /// </summary>
        HL_BACK,
    }

    /// <summary>
    ///  Table B-13: hlGetMaterialfv - pname values
    /// </summary>
    public enum HLGetMaterialfvParams
    {
        /// <summary>
        /// 控制着表面的硬度。参数必须是0到1之间的值，其中1表示触觉设备能够呈现的最坚硬的表面，0表示可以呈现的最兼容的表面。
        /// </summary>
        HL_STIFFNESS,

        /// <summary>
        /// 阻尼降低了表面的弹性。参数必须在0和1之间，其中0表示无阻尼，即高弹性表面，1表示可能的最大阻尼水平。
        /// </summary>
        HL_DAMPING,

        /// <summary>
        /// 静摩擦控制着一个表面对切向运动的阻力，当代理位置不变时，即从一个完全停止沿表面滑动的难度。参数值0是完全无摩擦的表面，值1是触觉装置能够呈现的最大静摩擦量。
        /// </summary>
        HL_STATIC_FRICTION,

        /// <summary>
        /// 动摩擦控制着一个表面对切向运动的阻力，当代理位置发生变化时，也就是说，一旦已经移动，沿着表面滑动有多难。参数值0是完全无摩擦的表面，值1是触觉装置能够呈现的最大动摩擦力。
        /// </summary>
        HL_DYNAMIC_FRICTION,

        /// <summary>
        /// Popthrough控制用户必须对形状施加的力的大小，然后设备才会将形状弹出到另一边。参数值越大，所需的力就越大。参数必须在0和1之间，其中值0禁用popthrough。
        /// </summary>
        HL_POPTHROUGH,
    }

    /// <summary>
    ///  Table B-10: hlMaterialf - face values
    /// </summary>
    public enum HLMaterialfFaces
    {
        /// <summary>
        /// 只将材质属性应用于形状的正面和所有约束面。
        /// </summary>
        HL_FRONT,

        /// <summary>
        /// 只将材质属性应用于形状的背面和所有约束面。
        /// </summary>
        HL_BACK,

        /// <summary>
        /// 将材质属性应用于正面和背面。
        /// </summary>
        HL_FRONT_AND_BACK,

        /// <summary>
        /// Popthrough控制用户必须对形状施加的力的大小，然后设备才会将形状弹出到另一边。参数值越大，所需的力就越大。参数值o禁用popthrough。
        /// </summary>
        HL_POPTHROUGH,
    }

    /// <summary>
    /// Table B-11: hlMaterialf - pname values
    /// </summary>
    public enum HLMaterialfParams
    {
        /// <summary>
        /// 控制着表面的硬度。参数必须是0到1之间的值，其中1表示触觉设备能够呈现的最坚硬的表面，0表示可以呈现的最兼容的表面。
        /// </summary>
        HL_STIFFNESS,

        /// <summary>
        /// 阻尼降低了表面的弹性。参数必须在0和1之间，其中0表示无阻尼，即高弹性表面，1表示可能的最大阻尼水平
        /// </summary>
        HL_DAMPING,

        /// <summary>
        /// 静摩擦控制着一个表面对切向运动的阻力，当代理位置不变时，即从一个完全停止沿表面滑动的难度。参数值0是完全无摩擦的表面，值1是触觉装置能够呈现的最大静摩擦量。
        /// </summary>
        HL_STATIC_FRICTION,

        /// <summary>
        /// 动摩擦控制着一个表面对切向运动的阻力，当代理位置发生变化时，也就是说，一旦已经移动，沿着表面滑动有多难。参数值0是完全无摩擦的表面，值1是触觉装置能够呈现的最大动摩擦力。
        /// </summary>
        HL_DYNAMIC_FRICTION,
    }

    /// <summary>
    /// Table B-14: hlTouchableFace - mode values
    /// </summary>
    public enum HLTouchableFaceParams
    {
        /// <summary>
        /// 只有正面是可触摸
        /// </summary>
        HL_FRONT,
        /// <summary>
        /// 只有背面可以触摸。
        /// </summary>
        HL_BACK,

        /// <summary>
        /// 所有的正面和背面都是可触摸的。
        /// </summary>
        HL_FRONT_AND_BACK
    }

    /// <summary>
    /// Table B-15: hlTouchModel
    /// </summary>
    public enum HLTouchModelParams
    {
        HL_CONTACT,
        HL_CONSTRAINT,
    }

    /// <summary>
    /// Table B-16: hlTouchModelIf
    /// </summary>
    public enum HLTouchModelIfParams
    {
        HL_FRONT,
    }

    /// <summary>
    /// Table B-17: hlStartEffect - effect types
    /// </summary>
    public enum HLStartEffectTypes
    {
        HL_EFFECT_CONSTANT,

        HL_EFFECT_SPRING,

        HL_EFFECT_VISCOUS,

        HL_EFFECT_FRICTION,

        HL_EFFECT_CALLBACK,
    }

    /// <summary>
    /// Table B-18: hlTriggerEffect
    /// </summary>
    public enum HLTriggerEffectTypes
    {
        HL_EFFECT_CONSTANT,

        HL_EFFECT_SPRING,

        HL_EFFECT_VISCOUS,

        HL_EFFECT_FRICTION,

        HL_EFFECT_CALLBACK,
    }

    /// <summary>
    /// Table B-19: hlEffectd, hlEffecti, hlEffectdv, hlEffectiv
    /// </summary>
    public enum HLEffectParams
    {
        /// <summary>
        /// 适用于弹簧、摩擦和粘滞效应类型。更高的增益将导致这些效应产生更大的力量。
        /// </summary>
        HL_EFFECT_PROPERTY_GAIN,

        /// <summary>
        /// 使用的常数,弹簧、摩擦和粘滞效应类型。表示这些影响所产生的最大力的上限。
        /// </summary>
        HL_EFFECT_PROPERTY_MAGNITUDE,

        /// <summary>
        /// 保留给将来的效果类型和回调效果使用。
        /// </summary>
        HL_EFFECT_PROPERTY_FREQUENCY,

        /// <summary>
        /// 用于所有效果，开始时调用hlTriggerEffect()。当持续时间过去时，该效果将自动终止。持续时间以毫秒为单位指定。
        /// </summary>
        HL_EFFECT_PROPERTY_DURATION,

        /// <summary>
        /// 用于弹簧效应。表示弹簧的锚点位置。
        /// </summary>
        HL_EFFECT_PROPERTY_POSITION,

        /// <summary>
        /// 用于常数效应。表示恒力矢量的方向。
        /// </summary>
        HL_EFFECT_PROPERTY_DIRECTION,
    }

    /// <summary>
    /// Table B-26: hlMatrix - mode values
    /// </summary>
    public enum HLMatrixModeParams
    {
        /// <summary>
        /// 所有矩阵操作命令都将针对从视图坐标到触摸坐标的转换。
        /// </summary>
        HL_VIEWTOUCH,

        /// <summary>
        /// 所有矩阵操作命令都将目标从触觉坐标转换到触觉设备的本地坐标。
        /// </summary>
        HL_TOUCHWORKSPACE,

        /// <summary>
        /// 所有矩阵操作命令都将针对从模型坐标到视图坐标的转换。
        /// <para>只有当禁用hl_use_gl_modelview时，才应用hl_modelview，否则将使用OpenGL modelview矩阵。</para>
        /// </summary>
        HL_MODELVIEW,
    }

    /// <summary>
    ///  Table B-27: hlPushAttrib, hlPopAttrib
    /// </summary>
    [Flags]
    public enum HLPushPopAttrib
    {
        HL_HINT_BIT = 0x01,
        HL_MATERIAL_BIT = 0x02,
        HL_TOUCH_BIT = 0x04,
        HL_TRANSFORM_BIT = 0x08,
        HL_EFFECT_BIT = 0x10,
        HL_EVENTS_BIT = 0x20,
    }

    /// <summary>
    /// Table B-21: hlCallback
    /// </summary>
    public enum HLCallbackTypes
    {
        HL_SHAPE_INTERSECT_LS,
        HL_SHAPE_CLOSEST_FEATURES,
    }

    /// <summary>
    /// Table B-22: hlAddEventCallback, hlRemoveEventCallBack - event values
    /// </summary>
    public enum HLCallbackEvents
    {
        HL_EVENT_MOTION,
        HL_EVENT_1BUTTONDOWN,
        HL_EVENT_1BUTTONUP,
        HL_EVENT_2BUTTONDOWN,
        HL_EVENT_2BUTTONUP,
        HL_EVENT_SAFETYDOWN,
        HL_EVENT_SAFETYUP,
        HL_EVENT_INKWELLDOWN,
        HL_EVENT_INKWELL_UP,
        HL_EVENT_TOUCH,
    }

    /// <summary>
    /// Table B-23: hlAddEventCallback, hlRemoveEventCallback - thread values
    /// </summary>
    public enum HLCallbackThreads
    {
        HL_CLIENT_THREAD,
        HL_COLLISION_THREAD,
    }

    /// <summary>
    /// Table B-24: hlEventd - pname values
    /// </summary>
    public enum HLEventdParams
    {
        HL_EVENT_MOTION_LINEAR_TOLERANCE,
        HL_EVENT_MOTION_ANGULAR_TOLERANCE,
    }
}
