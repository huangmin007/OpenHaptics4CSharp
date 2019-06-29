using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OH2CSharp.HD;
using System.Runtime.InteropServices;

namespace OH2CSharp.HL
{

    #region HL Error / Error Codes
    /// <summary>
    /// 
    /// </summary>
    public struct HLError
    {
        public IntPtr errorCode;    //max 24 bytes???
        public HDErrorInfo errorInfo;

        public String GetErrorCodeStr()
        {
            return Marshal.PtrToStringAnsi(errorCode);
        }
    }

    /// <summary>
    /// HLErrorCodes
    /// </summary>
    public enum HLErrorCodes
    {
        HL_NO_ERROR,
        HL_INVALID_ENUM,
        HL_INVALID_VALUE,
        HL_INVALID_OPERATION,
        HL_STACK_OVERFLOW,
        HL_STACK_UNDERFLOW,
        HL_OUT_OF_MEMORY,
        HL_DEVICE_ERROR,
        HL_INVALID_LICENSE,
    }

    #endregion

    //===============================hlEnable, hlDisable, hlIsEnabled
    #region Capability Parameters
    /// <summary>
    /// 功能/性能属性
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

    /// <summary>
    /// hlGetString Parameters
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
}
