using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

/// <summary>
/// 相关的辅助功能
/// </summary>
namespace OH2CSharp.Utilities
{

    /// <summary>
    /// 位置信息
    /// <para>默认数据，可以不用压内存</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector3D
    {
        /// <summary>
        /// x
        /// </summary>
        [MarshalAs(UnmanagedType.R8)]
        public double X;
        /// <summary>
        /// y
        /// </summary>
        [MarshalAs(UnmanagedType.R8)]
        public double Y;
        /// <summary>
        /// z
        /// </summary>
        [MarshalAs(UnmanagedType.R8)]
        public double Z;
        /// <summary>
        /// HHD
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 HHD;

        #region Vector3D Static Fucntions

        public static double Magnitude(ref Vector3D vec)
        {
            return Math.Sqrt(vec.X * vec.X + vec.Y * vec.Y + vec.Z * vec.Z);
        }

        public static Boolean IsZero(ref Vector3D vec, double epsilon)
        {
            return Math.Abs(vec.X) < epsilon && Math.Abs(vec.Y) < epsilon && Math.Abs(vec.Z) < epsilon;
        }

        public static void Add(ref Vector3D res, ref Vector3D vec1, ref Vector3D vec2)
        {
            res.X = vec1.X + vec2.X;
            res.Y = vec1.Y + vec2.Y;
            res.Z = vec1.Z + vec2.Z;
        }

        public static void Subtrace(ref Vector3D res, ref Vector3D vec1, ref Vector3D vec2)
        {
            res.X = vec1.X - vec2.X;
            res.Y = vec1.Y - vec2.Y;
            res.Z = vec1.Z - vec2.Z;
        }

        public static void Scale(ref Vector3D res, Vector3D vec1, double s)
        {
            res.X = vec1.X * s;
            res.Y = vec1.Y * s;
            res.Z = vec1.Z * s;
        }

        public static void ScaleInPlace(ref Vector3D res, double s)
        {
            res.X *= s;
            res.Y *= s;
            res.Z *= s;
        }

        public static void ScaleNonUniform(ref Vector3D res, Vector3D vec1, Vector3D vec2)
        {
            res.X = vec1.X * vec2.X;
            res.Y = vec1.Y * vec2.Y;
            res.Z = vec1.Z * vec2.Z;
        }

        public static void ScaleNonUniformInPlace(ref Vector3D res, Vector3D vec1)
        {
            res.X = res.X * vec1.X;
            res.Y = res.Y * vec1.Y;
            res.Z = res.Z * vec1.Z;
        }

        public static void Normalize(ref Vector3D res, ref Vector3D vec1)
        {
            Scale(ref res, vec1, 1.0 / Magnitude(ref vec1));
        }

        public static void NormalizeInPlace(ref Vector3D res)
        {
            double mag = Magnitude(ref res);
            if (mag == 0) return;
            ScaleInPlace(ref res, 1.0 / mag);
        }

        public static void CrossProduct(ref Vector3D res, ref Vector3D vec1, ref Vector3D vec2)
        {
            res.X = vec1.Y * vec2.Z - vec1.Z * vec2.Y;
            res.Y = vec1.Z * vec2.X - vec1.X * vec2.Z;
            res.Z = vec1.X * vec2.Y - vec1.Y * vec2.X;
        }

        public static double DotProduct(ref Vector3D vec1, ref Vector3D vec2)
        {
            return vec1.X * vec2.X + vec1.Y * vec2.Y + vec1.Z * vec2.Z;
        }

        public static double Distance(ref Vector3D vec1, ref Vector3D vec2)
        {
            Vector3D v3 = new Vector3D();
            Subtrace(ref v3, ref vec1, ref vec2);
            return Magnitude(ref v3);
        }


        #endregion

    }

    /// <summary>
    /// 校准状态
    /// </summary>
    //[StructLayout(LayoutKind.Sequential)]
    public struct CalibrationStatus
    {
        /// <summary>
        /// status
        /// </summary>
        public UInt32 Status;

        /// <summary>
        /// HHD
        /// </summary>
        public UInt32 HHD;
    }

    /// <summary>
    /// HDDLL相关的功能函数
    /// <para>注意：有些函数只是作为源码参考，不建议直接调用，以免更多的函数调用连接</para>
    /// </summary>
    public class HDDLLUtils
    {
        /// <summary>
        /// 结构对象转为指针
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="structure"></param>
        /// <returns></returns>
        public static IntPtr StructToIntPtr<T>(T structure)
        {
            int size = Marshal.SizeOf(structure);
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(structure, ptr, true);

            return ptr;
        }

        /// <summary>
        /// 指针转为结构对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        /// <returns></returns>
        public static T IntPtrToStruct<T>(IntPtr ptr)
        {
            T obj = (T)Marshal.PtrToStructure(ptr, typeof(T));
            //Marshal.FreeHGlobal(ptr);     //注意：要自已手动释放内存

            return obj;
        }

        /// <summary>
        /// Double array to IntPtr
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static IntPtr DoubleArrToIntPtr(double[] array)
        {
            IntPtr ptr = Marshal.AllocHGlobal(array.Length);
            Marshal.Copy(array, 0, ptr, array.Length);

            return ptr;
        }

        /// <summary>
        /// IntPtr to Double Array
        /// </summary>
        /// <param name="ptr"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static double[] IntPtrToDoubleArr(IntPtr ptr, int length)
        {
            double[] arr = new double[length];
            Marshal.Copy(ptr, arr, 0, length);
            //Marshal.FreeHGlobal(ptr);     //注意：要自已手动释放内存

            return arr;
        }

        /// <summary>
        /// 将托管 str 的内容复制到非托管内存
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static IntPtr StringToIntPtr(string str)
        {
            IntPtr ptr = Marshal.StringToHGlobalAnsi(str);
            return ptr;
        }

        /// <summary>
        /// 将非托管 ANSI 字符串中第一个 null 字符之前的所有字符复制到托管 System.String，并将每个 ANSI 字符扩展为 Unicode 字符。
        /// </summary>
        /// <param name="ptr"></param>
        /// <returns></returns>
        public static string IntPtrToString(IntPtr ptr)
        {
            return Marshal.PtrToStringAnsi(ptr);
        }
    }
}
