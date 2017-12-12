//-----------------------------------------------------------------------
//| by:Qcbf                                                             |
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace YoukiaCore
{

    public static class YKUtils
    {
        /// <summary>
        /// 字符串转换枚举
        /// </summary>
        public static T EnumParse<T>(string name)
        {
            return (T)Enum.Parse(typeof(T), name);
        }

        /// <summary>
        /// 枚举转字符串
        /// </summary>
        public static string StringEnumParse<T>(object value)
        {
            return Enum.GetName(typeof(T), value);
        }

        /// <summary>
        /// int转换枚举
        /// </summary>
        public static T EnumParse<T>(int value)
        {
            return (T)Enum.ToObject(typeof(T), value);
        }

        /// <summary>
        /// 冒泡排序
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="list">容器</param>
        /// <param name="func">判断方法</param>
        public static void BubbleSort<T>(this List<T> list, System.Func<T, T, bool> func)
        {
            T temp = default(T);
            for (int i = list.Count; i > 0; i--)
            {
                for (int j = 0; j < i - 1; j++)
                {
                    if (func(list[j], list[j + 1]))
                    {
                        temp = list[j];
                        list[j] = list[j + 1];
                        list[j + 1] = temp;
                    }
                }
            }
        }

        static public void BubbleSort<T>(this T[] array, Comparison<T> comparison)
        {
            T temp;//存储临时变量
            for (int i = 0; i < array.Length; i++)
                for (int j = i - 1; j >= 0; j--)
                    //if (intArray[j + 1] < intArray[j])
                    if (comparison(array[j + 1], array[j]) < 0)
                    {
                        temp = array[j + 1];
                        array[j + 1] = array[j];
                        array[j] = temp;
                    }
        }

        static public void BubbleSort<T>(this List<T> array, Comparison<T> comparison)
        {
            T temp;//存储临时变量
            for (int i = 0; i < array.Count; i++)
                for (int j = i - 1; j >= 0; j--)
                    //if (intArray[j + 1] < intArray[j])
                    if (comparison(array[j + 1], array[j]) < 0)
                    {
                        temp = array[j + 1];
                        array[j + 1] = array[j];
                        array[j] = temp;
                    }
        }

        static public uint CombineMask<T>(T[] enums) where T : IConvertible
        {
            uint flag = 0;
            for (int i = 0; i < enums.Length; i++)
            {
                flag |= enums[i].ToUInt32(null);
            }
            return flag;
        }

        static public T[] MaskToEnums<T>(this uint mask) where T : IConvertible
        {
            Array enums = Enum.GetValues(typeof(T));
            List<T> res = new List<T>();
            for (int i = 0; i < enums.Length; i++)
            {
                uint frag = (uint)enums.GetValue(i);
                if ((frag & mask) == frag)
                {
                    res.Add((T)enums.GetValue(i));
                }
            }
            return res.ToArray();
        }

        #region prop format, prop can is 100 or 100%

        public static float PropFormat(string v, float nowV, float baseV, float percentage = 0.01f)
        {
            bool reduce = v.Contains("-");
            bool add = v.Contains("+");
            if (reduce)
                v = v.Replace("-", "");
            else if (add)
                v = v.Replace("+", "");

            float value = 0;
            if (v.IndexOf("%") == -1)
            {
                value = float.Parse(v);
            }
            else if (v.IndexOf("%%") == -1)
            {
                value = (float)Math.Round(float.Parse(v.Replace("%%", "")) * percentage * baseV);
            }
            else
            {
                value = (float)Math.Round(float.Parse(v.Replace("%", "")) * percentage * nowV);
            }
            if (reduce)
                value = nowV - value;
            if (add)
                value = nowV + value;

            return value;
        }

        public static float PropFormat(string prop)
        {
            float value = 0;
            if (prop.IndexOf("%") == -1)
            {
                value = float.Parse(prop);
            }
            else
            {
                value = float.Parse(prop.Replace("%", ""));
            }
            return value;
        }

        public static string PropAdd(string a1, string a2)
        {
            if (a1.Substring(a1.Length - 2, 1) == "%")
            {
                return (float.Parse(a1.Replace("%", "")) + float.Parse(a2.Replace("%", ""))).ToString() + "%";
            }
            return (float.Parse(a1) + float.Parse(a2)).ToString();
        }

        #endregion

        #region 随机打乱list

        /// <summary>
        /// 随即打乱列表
        /// </summary>
        public static void RamdomSort<T>(this List<T> arr)
        {
            System.Random ran = new System.Random();
            int k = 0;
            T strtmp;
            for (int i = 0; i < arr.Count; i++)
            {
                k = ran.Next(0, arr.Count - 1);
                if (k != i)
                {
                    strtmp = arr[i];
                    arr[i] = arr[k];
                    arr[k] = strtmp;
                }
            }
        }

        public static void RamdomSort<T>(this List<T> arr, YKRandom random)
        {
            int k;
            T strtmp;
            for (int i = 0; i < arr.Count; i++)
            {
                k = random.randomValue(0, arr.Count - 1);
                if (k != i)
                {
                    strtmp = arr[i];
                    arr[i] = arr[k];
                    arr[k] = strtmp;
                }
            }
        }

        /// <summary>
        /// 随即打乱数组
        /// </summary>
        public static void RamdomSort<T>(this T[] arr)
        {
            System.Random ran = new System.Random();
            int k = 0;
            T strtmp;
            for (int i = 0; i < arr.Length; i++)
            {
                k = ran.Next(0, arr.Length - 1);
                if (k != i)
                {
                    strtmp = arr[i];
                    arr[i] = arr[k];
                    arr[k] = strtmp;
                }
            }
        }

        public static void RamdomSort<T>(this T[] arr, YKRandom random)
        {
            int k = 0;
            T strtmp;
            for (int i = 0; i < arr.Length; i++)
            {
                k = random.randomValue(0, arr.Length - 1);
                if (k != i)
                {
                    strtmp = arr[i];
                    arr[i] = arr[k];
                    arr[k] = strtmp;
                }
            }
        }

        #endregion

        public static string GetAtom(this ErlType[] erlArray, int index)
        {
            ErlAtom erlType = (ErlAtom)erlArray[index];
            return erlType == null ? null : erlType.Value;
        }

        public static int GetInt(this ErlType[] erlArray, int index)
        {
            ErlInt erlType = (ErlInt)erlArray[index];
            return erlType == null ? 0 : erlType.Value;
        }

        public static long GetLong(this ErlType[] erlArray, int index)
        {
            ErlLong erlType = (ErlLong)erlArray[index];
            return erlType == null ? 0 : erlType.Value;
        }

        public static string GetString(this ErlType[] erlArray, int index)
        {
            if (erlArray[index] is ErlString)
            {
                ErlString erlType = (ErlString)erlArray[index];
                return erlType.Value;
            }
            if (erlArray[index] is ErlList)
            {
                ErlList erlType = (ErlList)erlArray[index];
                return erlType.getValueString();
            }

            return null;
        }

        public static ErlArray GetErlArray(this ErlType[] erlArray, int index)
        {
            ErlArray erlType = (ErlArray)erlArray[index];
            return erlType;
        }

        public static ErlList GetErlList(this ErlType[] erlArray, int index)
        {
            if (erlArray[index] is ErlNullList)
            {
                return new ErlList(new ErlType[0]);
            }
            
            ErlList erlType = (ErlList)erlArray[index];
            return erlType;
        }

        public static string ToHexString(this byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                sb.Append(String.Format("{0:X2} ", bytes[i]));
            }
            return sb.ToString();
        }

        public static byte[] FromHexString(this string str)
        {
            string[] byteStrings = str.Split(" ".ToCharArray());
            byte[] byteOut = new byte[byteStrings.Length - 1];
            for (int i = 0; i < byteStrings.Length - 1; i++)
            {
                byteOut[i] = byte.Parse(byteStrings[i], NumberStyles.HexNumber);
            }
            return byteOut;
        }   
    }
}

