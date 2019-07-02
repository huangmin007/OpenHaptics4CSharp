using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

/// <summary>
/// 相关的辅助功能
/// </summary>
namespace OH4CSharp.Utilities
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
        public uint HHD;

        public Vector3D(double x = 0.0, double y = 0.0, double z = 0.0)
        {
            X = x;
            Y = y;
            Z = z;
            HHD = 0;
        }
        
        public void ResetZero()
        {
            X = 0.0;
            Y = 0.0;
            Z = 0.0;
        }

        public bool IsZero(double epsilon)
        {
            return Math.Abs(X) < epsilon && Math.Abs(Y) < epsilon && Math.Abs(Z) < epsilon;
        }

        public double Magnitude()
        {
            return Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        public double this[int index]
        {
            get
            {
                switch(index)
                {
                    case 0:return X;
                    case 1:return Y;
                    case 2:return Z;
                    default:throw new ArgumentException("超出索引范围 3.");
                }
            }
            set
            {
                switch(index)
                {
                    case 0:X = value;return;
                    case 1:Y = value;return;
                    case 2:Z = value;return;
                    default: throw new ArgumentException("超出索引范围 3.");
                }
            }
        }

        #region Vector3D Override Operator 如果需要自已写
        public static Vector3D operator+ (Vector3D v1, Vector3D v2)
        {
            return new Vector3D(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }
        public static Vector3D operator+ (Vector3D v1, double[] d)
        {
            if (d.Length != 3)throw new ArgumentException("数组长度必须为 3 ");
            return new Vector3D(v1.X + d[0], v1.Y + d[1], v1.Z + d[2]);
        }
        public static Vector3D operator+ (double[] d, Vector3D v1)
        {
            if (d.Length != 3) throw new ArgumentException("数组长度必须为 3 ");
            return new Vector3D(v1.X + d[0], v1.Y + d[1], v1.Z + d[2]);
        }

        public static Vector3D operator- (Vector3D v1, Vector3D v2)
        {
            return new Vector3D(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }
        public static Vector3D operator- (Vector3D v1, double[] d)
        {
            if (d.Length != 3) throw new ArgumentException("数组长度必须为 3 ");
            return new Vector3D(v1.X - d[0], v1.Y - d[1], v1.Z - d[2]);
        }
        public static Vector3D operator -(double[] d, Vector3D v1)
        {
            if (d.Length != 3) throw new ArgumentException("数组长度必须为 3 ");
            return new Vector3D(d[0] - v1.X, d[1] - v1.Y, d[2] - v1.Z);
        }

        public static Vector3D operator* (Vector3D v1, Vector3D v2)
        {
            return new Vector3D(v1.X * v2.X, v1.Y * v2.Y, v1.Z * v2.Z);
        }
        public static Vector3D operator *(Vector3D v1, double s)
        {
            return new Vector3D(v1.X * s, v1.Y * s, v1.Z * s);
        }
        public static Vector3D operator *(double s, Vector3D vec)
        {
            return new Vector3D(vec.X * s, vec.Y * s, vec.Z * s);
        }

        public static Vector3D operator/ (Vector3D v1, Vector3D v2)
        {
            return new Vector3D(v1.X / v2.X, v1.Y / v2.Y, v1.Z / v2.Z);
        }
        public static Vector3D operator/ (Vector3D v1, double s)
        {
            return new Vector3D(v1.X / s, v1.Y / s, v1.Z / s);
        }

        public static bool operator== (Vector3D v1, Vector3D v2)
        {
            return v1.X == v2.X && v1.Y == v2.Y && v1.Z == v2.Z;
        }
        public static bool operator !=(Vector3D v1, Vector3D v2)
        {
            return v1.X != v2.X || v1.Y != v2.Y || v1.Z != v2.Z;
        }

        #endregion

        #region Vector3D Static Fucntions 如果有需要自已写重载

        public static double Magnitude(ref Vector3D v3d)
        {
            return Math.Sqrt(v3d.X * v3d.X + v3d.Y * v3d.Y + v3d.Z * v3d.Z);
        }
        public static double Magnitude(ref double[] v3d)
        {
            if (v3d.Length != 3) throw new ArgumentException("数据长度应为3.");
            return Math.Sqrt(v3d[0] * v3d[0] + v3d[1] * v3d[1] + v3d[2] * v3d[2]);
        }

        public static Boolean IsZero(ref Vector3D v3d, double epsilon)
        {
            return Math.Abs(v3d.X) < epsilon && Math.Abs(v3d.Y) < epsilon && Math.Abs(v3d.Z) < epsilon;
        }
        public static Boolean IsZero(ref double[] v3d, double epsilon)
        {
            if (v3d.Length != 3) throw new ArgumentException("数据长度应为3.");
            return Math.Abs(v3d[0]) < epsilon && Math.Abs(v3d[1]) < epsilon && Math.Abs(v3d[2]) < epsilon;
        }

        public static void Add(ref Vector3D v3d, ref Vector3D v1, ref Vector3D v2)
        {
            v3d.X = v1.X + v2.X;
            v3d.Y = v1.Y + v2.Y;
            v3d.Z = v1.Z + v2.Z;
        }
        public static void Add(ref double[] v3d, ref double[] v1, ref double[] v2)
        {
            if(v3d.Length != 3 || v1.Length != 3 || v2.Length != 3)
                throw new ArgumentException("数组长度必须为 3 ");
            v3d[0] = v1[0] + v2[0];
            v3d[1] = v1[1] + v2[1];
            v3d[2] = v1[2] + v2[2];
        }


        public static void Subtrace(ref Vector3D v3d, ref Vector3D v1, ref Vector3D v2)
        {
            v3d.X = v1.X - v2.X;
            v3d.Y = v1.Y - v2.Y;
            v3d.Z = v1.Z - v2.Z;
        }
        public static void Subtrace(ref double[] v3d, double[] v1, double[] v2)
        {
            if (v3d.Length != 3 || v1.Length != 3 || v2.Length != 3)
                throw new ArgumentException("数组长度必须为 3 ");

            v3d[0] = v1[0] - v2[0];
            v3d[1] = v1[1] - v2[1];
            v3d[2] = v1[2] - v2[2];
        }

        public static void Scale(ref Vector3D v3d, ref Vector3D v1, double s)
        {
            v3d.X = v1.X * s;
            v3d.Y = v1.Y * s;
            v3d.Z = v1.Z * s;
        }
        public static void Scale(ref double[] v3d, ref double[] v1, double s)
        {
            if (v3d.Length != 3 || v1.Length != 3)
                throw new ArgumentException("数组长度必须为 3 ");
            v3d[0] = v1[0] * s;
            v3d[1] = v1[1] * s;
            v3d[2] = v1[2] * s;
        }

        public static void ScaleInPlace(ref Vector3D v3d, double s)
        {
            v3d.X *= s;
            v3d.Y *= s;
            v3d.Z *= s;
        }
        public static void ScaleInPlace(ref double[] v3d, double s)
        {
            if (v3d.Length != 3) throw new ArgumentException("数组长度必须为 3 ");
            v3d[0] *= s;
            v3d[1] *= s;
            v3d[2] *= s;
        }

        public static void ScaleNonUniform(ref Vector3D v3d, Vector3D v1, Vector3D v2)
        {
            v3d.X = v1.X * v2.X;
            v3d.Y = v1.Y * v2.Y;
            v3d.Z = v1.Z * v2.Z;
        }


        public static void ScaleNonUniformInPlace(ref Vector3D v3d, Vector3D v1)
        {
            v3d.X = v3d.X * v1.X;
            v3d.Y = v3d.Y * v1.Y;
            v3d.Z = v3d.Z * v1.Z;
        }

        public static void Normalize(ref Vector3D v3d, ref Vector3D v1)
        {
            Scale(ref v3d, ref v1, 1.0 / Magnitude(ref v1));
        }

        public static void NormalizeInPlace(ref Vector3D v3d)
        {
            double mag = Magnitude(ref v3d);
            if (mag == 0) return;
            ScaleInPlace(ref v3d, 1.0 / mag);
        }

        public static void CrossProduct(ref Vector3D v3d, ref Vector3D v1, ref Vector3D v2)
        {
            v3d.X = v1.Y * v2.Z - v1.Z * v2.Y;
            v3d.Y = v1.Z * v2.X - v1.X * v2.Z;
            v3d.Z = v1.X * v2.Y - v1.Y * v2.X;
        }

        public static double DotProduct(ref Vector3D v1, ref Vector3D v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
        }

        public static double Distance(ref Vector3D v1, ref Vector3D v2)
        {
            Vector3D v3 = new Vector3D();
            Subtrace(ref v3, ref v1, ref v2);
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