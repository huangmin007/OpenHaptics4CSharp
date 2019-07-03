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

        /// <summary>
        /// 检查错误，如果有错误信息将返回 true
        /// </summary>
        /// <returns></returns>
        public bool CheckedError()
        {
            return Marshal.PtrToStringAnsi(errorCode) != "HL_NO_ERROR";
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

        /// <summary>
        /// 单布尔值，表示触觉装置上的墨孔开关(如果存在)。true值表示开关被抑制。
        /// </summary>
        HL_INKWELL_STATE,

        /// <summary>
        /// 双精度值表示设备在世界坐标下的穿透深度。这将返回代理所触及的当前对象的穿透深度。
        /// </summary>
        HL_DEPTH_OF_PENETRATION,

        /// <summary>
        /// Vector 3 of doubles. 表示最后一个力，以世界坐标系发送给触觉装置。
        /// </summary>
        HL_DEVICE_FORCE,

        /// <summary>
        /// Vector 3 of doubles. 表示触觉装置在世界坐标中的位置。
        /// </summary>
        HL_DEVICE_POSITION,

        /// <summary>
        /// Vector 4 of doubles. 表示一个四元数，它指定触觉装置在世界坐标中的旋转。
        /// </summary>
        HL_DEVICE_ROTATION,

        /// <summary>
        /// Vector 3 of doubles. 代表最后一个扭矩，以世界坐标表示，发送到触觉装置。
        /// </summary>
        HL_DEVICE_TORQUE,

        /// <summary>
        /// Vector 16 of doubles. 表示4x4变换矩阵，按列主序表示触觉装置相对于世界坐标的变换。
        /// </summary>
        HL_DEVICE_TRANSFORM,

        /// <summary>
        /// 双精度值，以弧度表示代理在触发运动事件之前必须移动的最小旋转。
        /// </summary>
        HL_EVENT_MOTION_ANGULAR_TOLERANCE,

        /// <summary>
        /// 双精度值，表示在设备工作区坐标中，代理在触发运动事件之前必须移动的最小距离。
        /// </summary>
        HL_EVENT_MOTION_LINEAR_TOLERANCE,

        /// <summary>
        /// Vector 3 of doubles. 在世界坐标中表示金色球体的中心位置。
        /// <para>金色球体是代理的包围体，它为特定的HL帧指定了允许代理移动的空间。客户端负责在这个边界卷中提供几何图形。通常，当启用触觉相机视图时，这个边界卷用于调整图形视图卷的大小。</para>
        /// <para>adaptive viewport优化还引用了这个金色球体来确定需要读取多少深度缓冲区。此外，在实现回调形状或执行包围卷筛选时，golden sphere非常有用。球体的半径可以使用 HL_GOLDEN_RADIUS 查询。</para>
        /// </summary>
        HL_GOLDEN_POSITION,

        /// <summary>
        /// 一个双精度值，以世界坐标表示金球半径的双精度值。
        /// <para>金色球体是代理的包围体，它为特定的HL帧指定了允许代理移动的空间。客户端负责在这个边界卷中提供几何图形。通常，当启用触觉相机视图时，这个边界卷用于调整图形视图卷的大小。</para>
        /// <para>adaptive viewport优化还引用了这个金色球体来确定需要读取多少深度缓冲区。此外，在实现回调形状或执行包围卷筛选时，golden sphere非常有用。球体的中心可以使用HL_GOLDEN_POSITION查询。</para>
        /// </summary>
        HL_GOLDEN_RADIUS,

        /// <summary>
        /// Vector 16 of doubles. 16个双精度数组，按列主顺序表示4x4转换矩阵，指定从模型坐标到视图坐标的转换。
        /// <para>这个矩阵只在不使用OpenGL modelview(当HL_USE_GL_MODELVIEW被禁用时)时应用。当使用OpenGL modelview时，您应该使用OpenGL函数来查询modelview矩阵。</para>
        /// </summary>
        HL_MODELVIEW,

        /// <summary>
        /// 当代理与一个或多个形状接触时，将单个布尔值设置为true。
        /// </summary>
        HL_PROXY_IS_TOUCHING,

        /// <summary>
        /// Vector 3 of doubls. 表示代理在世界坐标中的位置。
        /// </summary>
        HL_PROXY_POSITION,

        /// <summary>
        /// Vector 4 of doubles. 表示一个四元数，该四元数指定代理在世界坐标中的旋转。
        /// </summary>
        HL_PROXY_ROTATION,

        /// <summary>
        /// 获取模型的可触摸面，它表示可以感觉到模型的哪一侧。返回值列表见“表B-10: hlMaterialf - face values”。
        /// </summary>
        HL_TOUCHABLE_FACE,

        /// <summary>
        /// Vector 3 of doubles. 表示与代理相接触的一组形状在接触点处的表面法线，只有 HL_PROXY_IS_TOUCHING 为 true 时才有效
        /// </summary>
        HL_PROXY_TOUCH_NORMAL,

        /// <summary>
        /// Vector 16 of doubles. 表示一个4x4变换矩阵，按列主顺序排列，指定代理相对于世界坐标的变换。
        /// </summary>
        HL_PROXY_TRANSFORM,

        /// <summary>
        /// 16个双精度数组，按列主顺序表示4x4转换矩阵，指定从触摸坐标到工作空间坐标的转换。
        /// </summary>
        HL_TOUCHWORKSPACE_MATRIX,

        /// <summary>
        /// 16个双精度数组，以列主顺序表示4x4转换矩阵，指定从视图坐标到触摸坐标的转换。
        /// </summary>
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
        /// <summary>
        /// 单个布尔值，表示触觉设备上的第一个按钮。true的值表示按钮被按下。
        /// </summary>
        HL_BUTTON1_STATE,

        /// <summary>
        /// 单个布尔值，表示触觉设备上的第二个按钮。true的值表示按钮被按下。
        /// </summary>
        HL_BUTTON2_STATE,

        /// <summary>
        /// 单布尔值，表示触觉装置上的安全开关(如果存在)。true值表示开关被抑制。
        /// </summary>
        HL_SAFETY_STATE,

        /// <summary>
        /// 单布尔值，表示触觉装置上的墨孔开关(如果存在)。true值表示开关被抑制。
        /// </summary>
        HL_INKWELL_STATE,

        /// <summary>
        /// Vector 3 of doubles. 表示最后一个力，以世界坐标系发送给触觉装置。
        /// </summary>
        HL_DEVICE_FORCE,

        /// <summary>
        /// Vector 3 of doubles. 表示触觉装置在世界坐标中的位置。
        /// </summary>
        HL_DEVICE_POSITION,

        /// <summary>
        /// Vector 4 of doubles. 表示一个四元数，它指定触觉装置在世界坐标中的旋转。
        /// </summary>
        HL_DEVICE_ROTATION,

        /// <summary>
        /// Vector 3 of doubles. 代表最后一个扭矩，以世界坐标表示，发送到触觉装置。
        /// </summary>
        HL_DEVICE_TORQUE,

        /// <summary>
        /// Vector 16 of doubles. 表示4x4变换矩阵，按列主序表示触觉装置相对于世界坐标的变换。
        /// </summary>
        HL_DEVICE_TRANSFORM,

        /// <summary>
        /// 单个布尔值，设置为 true 时代理与一个或多个形状接触
        /// </summary>
        HL_PROXY_IS_TOUCHING,

        /// <summary>
        /// Vector 3 of doubles. 表示代理在世界坐标中的位置。
        /// </summary>
        HL_PROXY_POSITION,

        /// <summary>
        /// Vector 4 of doubles. 表示一个四元数，该四元数指定代理在世界坐标中的旋转。
        /// </summary>
        HL_PROXY_ROTATION,

        /// <summary>
        /// Vector 3 of doubles. 表示与代理相接触的一组形状在接触点处的表面法线，只有 HL_PROXY_IS_TOUCHING 为 true 时才有效
        /// </summary>
        HL_PROXY_TOUCH_NORMAL,

        /// <summary>
        /// Vector 16 of doubles. 表示一个4x4变换矩阵，按列主顺序排列，指定代理相对于世界坐标的变换。
        /// </summary>
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
        /// <summary>
        /// 代理位置不允许通过几何基元（三角形）。代理可能会从表面移动，但仍将保留在其一侧。
        /// </summary>
        HL_CONTACT,

        /// <summary>
        /// 代理位置被约束为精确地保持在几何基本体的曲面上，并且只有当设备位置和代理之间的距离大于捕捉距离时，才能将其移出曲面。
        /// </summary>
        HL_CONSTRAINT,
    }

    /// <summary>
    /// Table B-16: hlTouchModelIf
    /// </summary>
    public enum HLTouchModelIfParams
    {
        /// <summary>
        /// 代理位置和曲面之间的距离，必须超过该距离才能解除约束。
        /// <para>参数应该是一个浮点值，表示工作区坐标中以毫米为单位的距离。默认值为 FLT_MAX 以始终处于活动状态。</para>
        /// </summary>
        HL_FRONT,
    }

    /// <summary>
    /// Table B-17: hlStartEffect - effect types
    /// </summary>
    public enum HLStartEffectTypes
    {
        /// <summary>
        /// 向触觉装置发送的总力中添加一个恒定的力矢量。
        /// <para>effect属性 HL_EFFECT_PROPERTY_DIRECTION 指定力向量的方向。</para>
        /// <para>效果属性 HL_EFFECT_PROPERTY_MAGNITUDE 指定力向量的大小。</para>
        /// </summary>
        HL_EFFECT_CONSTANT,

        /// <summary>
        /// 将弹簧力添加到发送到触觉设备的总力中。
        /// <para>弹簧力将触觉装置拉向效应位置，并与增益和效应位置与装置位置之间的距离成正比。</para>
        /// <para>具体来说，弹簧力是使用表达式 f=k(p-x) 计算的，其中f是弹簧力，p是作用位置，x是当前的触觉装置位置，k是增益。</para>
        /// <para>效果位置由属性 HL_EFFECT_PROPERTY_POSITION 指定。增益由属性 HL_EFFECT_PROPERTY_GAIN 指定。效应力的大小限制在效果属性 HL_EFFECT_MAGNITUDE 的值上。</para>
        /// </summary>
        HL_EFFECT_SPRING,

        /// <summary>
        /// 将粘性力添加到发送到触觉设备的总力中。
        /// <para>粘性力以触觉装置的电流速度为基础，通过计算来抵抗触觉装置的运动。</para>
        /// <para>具体来说，力是用表达式 f=-kv 计算的，其中f是弹簧力，v是速度，k是增益。</para>
        /// <para>增益由属性 HL_EFFECT_PROPERTY_GAIN 指定。效应力的大小限制在效果属性 HL_EFFECT_MAGNITUDE 的值上。</para>
        /// </summary>
        HL_EFFECT_VISCOUS,

        /// <summary>
        /// 将摩擦力添加到发送到触觉设备的总力中。
        /// <para>与通过 hlMaterial()调用指定的摩擦不同，这是在接触对象和REE空间时的摩擦。</para>
        /// <para>摩擦力的增益由属性 HL_EFFECT_PROPERTY_GAIN 指定。效应力的大小限制在效果属性 HL_EFFECT_MAGNITUDE 的值上。</para>
        /// </summary>
        HL_EFFECT_FRICTION,

        /// <summary>
        /// 允许用户通过使用 hlCallback() s设置效果回调来创建自定义效果。
        /// </summary>
        HL_EFFECT_CALLBACK,
    }

    /// <summary>
    /// Table B-18: hlTriggerEffect
    /// </summary>
    public enum HLTriggerEffectTypes
    {
        /// <summary>
        /// 向触觉装置发送的总力中添加一个恒定的力矢量。
        /// <para>effect属性 HL_EFFECT_PROPERTY_DIRECTION 指定力向量的方向。</para>
        /// <para>效果属性 HL_EFFECT_PROPERTY_MAGNITUDE 指定力向量的大小。</para>
        /// </summary>
        HL_EFFECT_CONSTANT,

        /// <summary>
        /// 将弹簧力添加到发送到触觉设备的总力中。
        /// <para>弹簧力将触觉装置拉向效应位置，并与增益和效应位置与装置位置之间的距离成正比。</para>
        /// <para>具体来说，弹簧力是使用表达式 f=k(p-x) 计算的，其中f是弹簧力，p是作用位置，x是当前的触觉装置位置，k是增益。</para>
        /// <para>效果位置由属性 HL_EFFECT_PROPERTY_POSITION 指定。增益由属性 HL_EFFECT_PROPERTY_GAIN 指定。效应力的大小限制在效果属性 HL_EFFECT_MAGNITUDE 的值上。</para>
        /// </summary>
        HL_EFFECT_SPRING,

        /// <summary>
        /// 将粘性力添加到发送到触觉设备的总力中。
        /// <para>粘性力以触觉装置的电流速度为基础，通过计算来抵抗触觉装置的运动。</para>
        /// <para>具体来说，力是用表达式 f=-kv 计算的，其中f是弹簧力，v是速度，k是增益。</para>
        /// <para>增益由属性 HL_EFFECT_PROPERTY_GAIN 指定。效应力的大小限制在效果属性 HL_EFFECT_MAGNITUDE 的值上。</para>
        /// </summary>
        HL_EFFECT_VISCOUS,

        /// <summary>
        /// 将摩擦力添加到发送到触觉设备的总力中。
        /// <para>与通过 hlMaterial()调用指定的摩擦不同，这是在接触对象和REE空间时的摩擦。</para>
        /// <para>摩擦力的增益由属性 HL_EFFECT_PROPERTY_GAIN 指定。效应力的大小限制在效果属性 HL_EFFECT_MAGNITUDE 的值上。</para>
        /// </summary>
        HL_EFFECT_FRICTION,

        /// <summary>
        /// 允许用户通过使用 hlCallback() s设置效果回调来创建自定义效果。
        /// </summary>
        HL_EFFECT_CALLBACK,
    }

    /// <summary>
    /// Table B-19: hlEffectd, hlEffecti, hlEffectdv, hlEffectiv
    /// </summary>
    public enum HLEffectParams
    {
        /// <summary>
        /// 适用于弹簧、摩擦和粘滞效果类型。更高的增益将导致这些效应产生更大的力量。
        /// </summary>
        HL_EFFECT_PROPERTY_GAIN,

        /// <summary>
        /// 使用的常数、弹簧、摩擦和粘滞效应类型。表示这些影响所产生的最大力的上限。
        /// </summary>
        HL_EFFECT_PROPERTY_MAGNITUDE,

        /// <summary>
        /// 保留给将来的效果类型和回调效果使用。
        /// </summary>
        HL_EFFECT_PROPERTY_FREQUENCY,

        /// <summary>
        /// 用于所有效果，开始时调用 hlTriggerEffect()。当持续时间过去时，该效果将自动终止。持续时间以毫秒为单位指定。
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
    /// Table B-25: hlProxydv
    /// </summary>
    public enum HLProxyParams
    {
        /// <summary>
        /// 在世界坐标中设置代理的位置。如果启用代理解析，此调用将没有效果。
        /// </summary>
        HL_PROXY_POSITION,
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
        /// <summary>
        /// 影响 hlInt() 命令设置的所有属性，如 HL_SHAPE_FEEDBACK_BUFFER。
        /// </summary>
        HL_HINT_BIT = 0x01,
        /// <summary>
        /// 影响 hlMaterial() 命令指定的所有属性，如 HL_STIFFNESS、HL_DYNAMIC_FRICTION。
        /// </summary>
        HL_MATERIAL_BIT = 0x02,
        /// <summary>
        /// 影响 hlTouch() 命令指定的所有属性，如 HL_SNAP_DISTANCE、HL_TOUCH_MODEL。
        /// </summary>
        HL_TOUCH_BIT = 0x04,
        /// <summary>
        /// 影响所有转换和相机属性，如 HL_HAPTIC_CAMERA_VIEW、HL_ADAPTIVE_VIEWPORT。
        /// </summary>
        HL_TRANSFORM_BIT = 0x08,
        /// <summary>
        /// 影响 hlEffect() 命令指定的所有属性，如 HL_EFFECT_PROPERTY_GAIN、HL_EFFECT_PROPERTY_DURATION。
        /// </summary>
        HL_EFFECT_BIT = 0x10,
        /// <summary>
        /// 影响事件命令指定的所有属性，如 HL_EVENT_MOTION_LINEAR_THRESHOLD、HL_EVENT_MOTION_ANGULAR_TOLERANCE。
        /// </summary>
        HL_EVENTS_BIT = 0x20,
    }

    /// <summary>
    /// Table B-21: hlCallback
    /// </summary>
    public enum HLCallbackTypes
    {
        /// <summary>
        /// 回调函数，使线段与用户定义的自定义形状相交。
        /// </summary>
        HL_SHAPE_INTERSECT_LS,

        /// <summary>
        /// 回调函数，查找表面上与输入点最近的点，以及一个或多个近似该点附近表面的局部特征。
        /// </summary>
        HL_SHAPE_CLOSEST_FEATURES,
    }

    /// <summary>
    /// Table B-22: hlAddEventCallback, hlRemoveEventCallBack - event values
    /// </summary>
    public enum HLCallbackEvents
    {
        /// <summary>
        /// 回调时将调用代理已经超过 HL_EVENT_MOTION_LINEAR_TOLERANCE 毫米的空间坐标的位置
        /// <para>当最后运动事件被触发或当代理一直旋转超过 HL_EVENT_MOTION_ANGULAR_TOLERANCE 弧度旋转的代理上次运动事件被触发。</para>
        /// </summary>
        HL_EVENT_MOTION,
        /// <summary>
        /// 当触觉设备上的第一个按钮被按下时，将调用回调。
        /// </summary>
        HL_EVENT_1BUTTONDOWN,
        /// <summary>
        /// 当触觉设备上的第一个按钮被释放时，将调用回调。
        /// </summary>
        HL_EVENT_1BUTTONUP,
        /// <summary>
        /// 当触觉设备上的第二个按钮被按下时，将调用回调。
        /// </summary>
        HL_EVENT_2BUTTONDOWN,
        /// <summary>
        /// 当触觉设备上的第二个按钮被释放时，将调用回调。
        /// </summary>
        HL_EVENT_2BUTTONUP,
        /// <summary>
        /// 当安全开关(如果可用)被按下时，将调用回调。
        /// </summary>
        HL_EVENT_SAFETYDOWN,
        /// <summary>
        /// 当释放安全开关(如果可用)时，将调用回调函数。
        /// </summary>
        HL_EVENT_SAFETYUP,
        /// <summary>
        /// 当墨水瓶开关(如果可用)被按下时，将调用回调。
        /// </summary>
        HL_EVENT_INKWELLDOWN,
        /// <summary>
        /// 当墨水瓶开关(如果可用)被释放时，将调用回调。
        /// </summary>
        HL_EVENT_INKWELL_UP,
        /// <summary>
        /// 当触摸到场景中的形状(代理与该形状保持联系)时，将调用回调函数。
        /// </summary>
        HL_EVENT_TOUCH,
    }

    /// <summary>
    /// Table B-23: hlAddEventCallback, hlRemoveEventCallback - thread values
    /// </summary>
    public enum HLCallbackThreads
    {
        /// <summary>
        /// 当客户机程序调用 hlCheckEvents() 时，将从客户机线程调用回调函数。
        /// </summary>
        HL_CLIENT_THREAD,
        /// <summary>
        /// 回调函数将从触觉呈现引擎中运行的内部冲突线程调用。
        /// <para>大多数事件回调应该在客户端线程中处理，但是在某些情况下，由于时间需求，需要使用冲突线程回调。</para>
        /// </summary>
        HL_COLLISION_THREAD,
    }

    /// <summary>
    /// Table B-24: hlEventd - pname values
    /// </summary>
    public enum HLEventdParams
    {
        /// <summary>
        /// 设置设备工作区坐标中的最小距离，在触发运动事件之前代理的线性平移必须更改该距离。默认情况下，这个值是 1 毫米。
        /// </summary>
        HL_EVENT_MOTION_LINEAR_TOLERANCE,

        /// <summary>
        /// 设置最小角度距离，在触发运动事件之前代理的方向必须更改。默认值是 0.02 弧度。
        /// </summary>
        HL_EVENT_MOTION_ANGULAR_TOLERANCE,
    }

}
