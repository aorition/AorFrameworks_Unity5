using System;
using System.Collections.Generic;

namespace AorBaseUtility
{
    public static class BaseUtility
    {

        #region ArrayToString

        public static string ArrayToString(int[] array)
        {

            if (array == null) return null;

            string o = "[";
            for (int i = 0; i < array.Length; i++)
            {
                if (i > 0)
                {
                    o += ",";
                }

                o += array[i].ToString();
            }

            return o + "]";
        }

        public static string ArrayToString(char[] array)
        {

            if (array == null) return null;

            string o = "[";
            for (int i = 0; i < array.Length; i++)
            {
                if (i > 0)
                {
                    o += ",";
                }

                o += array[i].ToString();
            }

            return o + "]";
        }

        public static string ArrayToString(string[] array)
        {

            if (array == null) return null;

            string o = "[";
            for (int i = 0; i < array.Length; i++)
            {
                if (i > 0)
                {
                    o += ",";
                }

                o += array[i];
            }

            return o + "]";
        }

        public static string ArrayToString(float[] array)
        {

            if (array == null) return null;

            string o = "[";
            for (int i = 0; i < array.Length; i++)
            {
                if (i > 0)
                {
                    o += ",";
                }

                o += array[i];
            }

            return o + "]";
        }

        public static string ArrayToString(double[] array)
        {

            if (array == null) return null;

            string o = "[";
            for (int i = 0; i < array.Length; i++)
            {
                if (i > 0)
                {
                    o += ",";
                }

                o += array[i];
            }

            return o + "]";
        }

        public static string ArrayToString(uint[] array)
        {

            if (array == null) return null;

            string o = "[";
            for (int i = 0; i < array.Length; i++)
            {
                if (i > 0)
                {
                    o += ",";
                }

                o += array[i];
            }

            return o + "]";
        }

        public static string ArrayToString(long[] array)
        {

            if (array == null) return null;

            string o = "[";
            for (int i = 0; i < array.Length; i++)
            {
                if (i > 0)
                {
                    o += ",";
                }

                o += array[i];
            }

            return o + "]";
        }

        public static string ArrayToString(ulong[] array)
        {

            if (array == null) return null;

            string o = "[";
            for (int i = 0; i < array.Length; i++)
            {
                if (i > 0)
                {
                    o += ",";
                }

                o += array[i];
            }

            return o + "]";
        }

        public static string ArrayToString<T>(T[] array) where T : class
        {

            if (array == null) return null;

            string o = "[";
            for (int i = 0; i < array.Length; i++)
            {
                if (i > 0)
                {
                    o += ",";
                }

                o += array[i];
            }

            return o + "]";
        }

        #endregion

    }
}
