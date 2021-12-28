using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
namespace CSharpLib
{
    internal class VARS
    {
        internal static string version = "3.1.1";
        internal static string UserClassPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"CSharpLib");
        internal static string UserRootDir = Path.GetDirectoryName(UserClassPath);
    }
    /// <summary>
    /// Class containing methods for string manipulation.
    /// </summary>
    public static class Strings
    {
        /// <summary>
        /// Gets a substring between the specified start and stop index.
        /// </summary>
        /// <param name="str">The string to get the substring from.</param>
        /// <param name="startIndex">The index of the string to start at.</param>
        /// <param name="stopIndex">The index of the string to stop at.</param>
        /// <returns></returns>
        public static string GetSubstring(this string str, int startIndex, int stopIndex)
        {
            return str.Substring(startIndex, (stopIndex - startIndex));
        }
        /// <summary>
        /// Gets a substring between the specified start and stop strings.
        /// </summary>
        /// <param name="str">the string to get the substring from.</param>
        /// <param name="startString">The string to use as the start position.</param>
        /// <param name="endString">The string to use as the stop position.</param>
        /// <returns></returns>
        public static string GetSubstring(this string str, string startString, string endString)
        {
            var index1 = str.IndexOf(startString) != -1 ? str.IndexOf(startString) : 0;
            var index2 = str.IndexOf(endString) != -1 ? str.IndexOf(endString) : (str.Length - 1);
            return str.GetSubstring(index1, index2);
        }
        /// <summary>
        /// Trims all trailing occurences of the specified string.
        /// </summary>
        /// <param name="target">The target string.</param>
        /// <param name="trimString">The string to remove.</param>
        /// <returns></returns>
        public static string TrimEnd(this string target, string trimString)
        {
            if (string.IsNullOrEmpty(trimString))
            {
                return target;
            }
            string result = target;
            while (result.EndsWith(trimString))
            {
                result = result.Substring(0, result.Length - trimString.Length);
            }
            return result;
        }
        /// <summary>
        /// Trims all leading occurences of the specified string.
        /// </summary>
        /// <param name="target">The target string.</param>
        /// <param name="trimString">The string to remove.</param>
        /// <returns></returns>
        public static string TrimStart(this string target, string trimString)
        {
            if (string.IsNullOrEmpty(trimString))
            {
                return target;
            }

            string result = target;
            while (result.StartsWith(trimString))
            {
                result = result.Substring(trimString.Length);
            }
            return result;
        }
        /// <summary>
        /// Splits a string at the specified index.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="index">The index to split the string at.</param>
        /// <returns></returns>
        public static string[] Split(this string source, params int[] index)
        {
            index = index.Distinct().OrderBy(x => x).ToArray();
            string[] output = new string[index.Length + 1];
            int pos = 0;

            for (int i = 0; i < index.Length; pos = index[i++])
            {
                output[i] = source.Substring(pos, index[i] - pos);
            }

            output[index.Length] = source.Substring(pos);
            return output;
        }
        /// <summary>
        /// Splits a string using the specified separator.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="separator">The string used to split the source string.</param>
        /// <returns></returns>
        public static string[] Split(this string source, string separator)
        {
            return source.Split(new string[] { separator }, StringSplitOptions.None);
        }
        /// <summary>
        /// Splits a string using the specified separator.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="separator">The string used to split the source string.</param>
        /// <param name="options">The options used when making the string array.</param>
        /// <returns></returns>
        public static string[] Split(this string source, string separator, StringSplitOptions options)
        {
            return source.Split(new string[] { separator }, options);
        }
    }
    /// <summary>
    /// Class containing methods for various collections.
    /// </summary>
    public static class Collections
    {
        #region GetLastElement
        /// <summary>
        /// Returns the last element in the specified string array. If the array has no elements, it returns null.
        /// </summary>
        /// <param name="array">The string array to get the last element from.</param>
        /// <returns></returns>
        public static string GetLastElement(this string[] array)
        {
            int length = array.Length - 1;
            if (length > -1)
                return array[length];
            else
                return null;
        }
        /// <summary>
        /// Returns the last element in the specified byte array. If the array has no elements, it returns null.
        /// </summary>
        /// <param name="array">The byte array to get the last element from.</param>
        /// <returns></returns>
        public static byte? GetLastElement(this byte[] array)
        {
            int length = array.Length - 1;
            if (length > -1)
                return array[length];
            else
                return null;
        }
        /// <summary>
        /// Returns the last element in the specified char array.  If the array has no elements, it returns null.
        /// </summary>
        /// <param name="array">The char array to get the last element from.</param>
        /// <returns></returns>
        public static char? GetLastElement(this char[] array)
        {
            int length = array.Length - 1;
            if (length > -1)
                return array[length];
            else
                return null;
        }
        /// <summary>
        /// Returns the last element in the specified decimal array. If the array has no elements, it returns null.
        /// </summary>
        /// <param name="array">The decimal array to get the last element from.</param>
        /// <returns></returns>
        public static decimal? GetLastElement(this decimal[] array)
        {
            int length = array.Length - 1;
            if (length > -1)
                return array[length];
            else
                return null;
        }
        /// <summary>
        /// Returns the last element in the specified long array. If the array has no elements, it returns null.
        /// </summary>
        /// <param name="array">The long array to get the last element from.</param>
        /// <returns></returns>
        public static long? GetLastElement(this long[] array)
        {
            int length = array.Length - 1;
            if (length > -1)
                return array[length];
            else
                return null;
        }
        /// <summary>
        /// Returns the last element in the specified double array. If the array has no elements, it returns null.
        /// </summary>
        /// <param name="array">The double array to get the last element from.</param>
        /// <returns></returns>
        public static double? GetLastElement(this double[] array)
        {
            int length = array.Length - 1;
            if (length > -1)
                return array[length];
            else
                return null;
        }
        /// <summary>
        /// Returns the last element in the specified float array. If the array has no elements, it returns null.
        /// </summary>
        /// <param name="array">The float array to get the last element from.</param>
        /// <returns></returns>
        public static float? GetLastElement(this float[] array)
        {
            int length = array.Length - 1;
            if (length > -1)
                return array[length];
            else
                return null;
        }
        /// <summary>
        /// Returns the last element in the specified int array. If the array has no elements, it returns null.
        /// </summary>
        /// <param name="array">The int array to get the last element from.</param>
        /// <returns></returns>
        public static int? GetLastElement(this int[] array)
        {
            int length = array.Length - 1;
            if (length > -1)
                return array[length];
            else
                return null;
        }
        /// <summary>
        /// Returns the last element in the specified sbyte array. If the array has no elements, it returns null.
        /// </summary>
        /// <param name="array">The sbyte array to get the last element from.</param>
        /// <returns></returns>
        public static sbyte? GetLastElement(this sbyte[] array)
        {
            int length = array.Length - 1;
            if (length > -1)
                return array[length];
            else
                return null;
        }
        /// <summary>
        /// Returns the last element in the specified short array. If the array has no elements, it returns null.
        /// </summary>
        /// <param name="array">The short array to get the last element from.</param>
        /// <returns></returns>
        public static short? GetLastElement(this short[] array)
        {
            int length = array.Length - 1;
            if (length > -1)
                return array[length];
            else
                return null;
        }
        /// <summary>
        /// Returns the last element in the specified uint array. If the array has no elements, it returns null.
        /// </summary>
        /// <param name="array">The uint array to get the last element from.</param>
        /// <returns></returns>
        public static uint? GetLastElement(this uint[] array)
        {
            int length = array.Length - 1;
            if (length > -1)
                return array[length];
            else
                return null;
        }
        /// <summary>
        /// Returns the last element in the specified ulong array. If the array has no elements, it returns null.
        /// </summary>
        /// <param name="array">The ulong array to get the last element from.</param>
        /// <returns></returns>
        public static ulong? GetLastElement(this ulong[] array)
        {
            int length = array.Length - 1;
            if (length > -1)
                return array[length];
            else
                return null;
        }
        /// <summary>
        /// Returns the last element in the specified ushort array. If the array has no elements, it returns null.
        /// </summary>
        /// <param name="array">The ushort array to get the last element from.</param>
        /// <returns></returns>
        public static ushort? GetLastElement(this ushort[] array)
        {
            int length = array.Length - 1;
            if (length > -1)
                return array[length];
            else
                return null;
        }
        #endregion
        #region GetLastItem
        /// <summary>
        /// Returns the last item in the specified string list. If the list is empty, it returns null.
        /// </summary>
        /// <param name="list">The string list to get the last item from.</param>
        /// <returns></returns>
        public static string GetLastItem(this List<string> list)
        {
            if (list.Count > 0)
                return list[list.Count - 1];
            else
                return null;
        }
        /// <summary>
        /// Returns the last item in the specified byte list. If the list is empty, it returns null.
        /// </summary>
        /// <param name="list">The byte list to get the last item from.</param>
        /// <returns></returns>
        public static byte? GetLastItem(this List<byte> list)
        {
            if (list.Count > 0)
                return list[list.Count - 1];
            else
                return null;
        }
        /// <summary>
        /// Returns the last item in the specified char list. If the list is empty, it returns null.
        /// </summary>
        /// <param name="list">The char list to get the last item from.</param>
        /// <returns></returns>
        public static char? GetLastItem(this List<char> list)
        {
            if (list.Count > 0)
                return list[list.Count - 1];
            else
                return null;
        }
        /// <summary>
        /// Returns the last item in the specified decimal list. If the list is empty, it returns null.
        /// </summary>
        /// <param name="list">The decimal list to get the last item from.</param>
        /// <returns></returns>
        public static decimal? GetLastItem(this List<decimal> list)
        {
            if (list.Count > 0)
                return list[list.Count - 1];
            else
                return null;
        }
        /// <summary>
        /// Returns the last item in the specified long list. If the list is empty, it returns null.
        /// </summary>
        /// <param name="list">The long list to get the last item from.</param>
        /// <returns></returns>
        public static long? GetLastItem(this List<long> list)
        {
            if (list.Count > 0)
                return list[list.Count - 1];
            else
                return null;
        }
        /// <summary>
        /// Returns the last item in the specified double list. If the list is empty, it returns null.
        /// </summary>
        /// <param name="list">The double list to get the last item from.</param>
        /// <returns></returns>
        public static double? GetLastItem(this List<double> list)
        {
            if (list.Count > 0)
                return list[list.Count - 1];
            else
                return null;
        }
        /// <summary>
        /// Returns the last item in the specified float list. If the list is empty, it returns null.
        /// </summary>
        /// <param name="list">The float list to get the last item from.</param>
        /// <returns></returns>
        public static float? GetLastItem(this List<float> list)
        {
            if (list.Count > 0)
                return list[list.Count - 1];
            else
                return null;
        }
        /// <summary>
        /// Returns the last item in the specified int list. If the list is empty, it returns null.
        /// </summary>
        /// <param name="list">The int list to get the last item from.</param>
        /// <returns></returns>
        public static int? GetLastItem(this List<int> list)
        {
            if (list.Count > 0)
                return list[list.Count - 1];
            else
                return null;
        }
        /// <summary>
        /// Returns the last item in the specified sbyte list. If the list is empty, it returns null.
        /// </summary>
        /// <param name="list">The sbyte list to get the last item from.</param>
        /// <returns></returns>
        public static sbyte? GetLastItem(this List<sbyte> list)
        {
            if (list.Count > 0)
                return list[list.Count - 1];
            else
                return null;
        }
        /// <summary>
        /// Returns the last item in the specified short list. If the list is empty, it returns null.
        /// </summary>
        /// <param name="list">The short list to get the last item from.</param>
        /// <returns></returns>
        public static short? GetLastItem(this List<short> list)
        {
            if (list.Count > 0)
                return list[list.Count - 1];
            else
                return null;
        }
        /// <summary>
        /// Returns the last item in the specified uint list. If the list is empty, it returns null.
        /// </summary>
        /// <param name="list">The uint list to get the last item from.</param>
        /// <returns></returns>
        public static uint? GetLastItem(this List<uint> list)
        {
            if (list.Count > 0)
                return list[list.Count - 1];
            else
                return null;
        }
        /// <summary>
        /// Returns the last item in the specified ulong list. If the list is empty, it returns null.
        /// </summary>
        /// <param name="list">The ulong list to get the last item from.</param>
        /// <returns></returns>
        public static ulong? GetLastItem(this List<ulong> list)
        {
            if (list.Count > 0)
                return list[list.Count - 1];
            else
                return null;
        }
        /// <summary>
        /// Returns the last item in the specified ushort list. If the list is empty, it returns null.
        /// </summary>
        /// <param name="list">The ushort list to get the last item from.</param>
        /// <returns></returns>
        public static ushort? GetLastItem(this List<ushort> list)
        {
            if (list.Count > 0)
                return list[list.Count - 1];
            else
                return null;
        }
        #endregion
        #region GetMaxValue
        /// <summary>
        /// Returns the largest number in the specified int array.
        /// </summary>
        /// <param name="array">The int array to check.</param>
        /// <returns></returns>
        public static int GetMaxValue(this int[] array) { return array.Max(); }
        /// <summary>
        /// Returns the largest number in the specified decimal array.
        /// </summary>
        /// <param name="array">The decimal array to check.</param>
        /// <returns></returns>
        public static decimal GetMaxValue(this decimal[] array) { return array.Max(); }
        /// <summary>
        /// Returns the largest number in the specified double array.
        /// </summary>
        /// <param name="array">The double array to check.</param>
        /// <returns></returns>
        public static double GetMaxValue(this double[] array) { return array.Max(); }
        /// <summary>
        /// Returns the largest number in the specified float array.
        /// </summary>
        /// <param name="array">The float array to check.</param>
        /// <returns></returns>
        public static float GetMaxValue(this float[] array) { return array.Max(); }
        /// <summary>
        /// Returns the largest number in the specified long array.
        /// </summary>
        /// <param name="array">The long array to check.</param>
        /// <returns></returns>
        public static long GetMaxValue(this long[] array) { return array.Max(); }
        /// <summary>
        /// Returns the largest number in the specified short array.
        /// </summary>
        /// <param name="array">The short array to check.</param>
        /// <returns></returns>
        public static short GetMaxValue(this short[] array) { return array.Max(); }
        /// <summary>
        /// Returns the largest number in the specified uint array.
        /// </summary>
        /// <param name="array">The uint array to check.</param>
        /// <returns></returns>
        public static uint GetMaxValue(this uint[] array) { return array.Max(); }
        /// <summary>
        /// Returns the largest number in the specified ulong array.
        /// </summary>
        /// <param name="array">The ulong array to check.</param>
        /// <returns></returns>
        public static ulong GetMaxValue(this ulong[] array) { return array.Max(); }
        /// <summary>
        /// Returns the largest number in the specified ushort array.
        /// </summary>
        /// <param name="array">The ushort array to check.</param>
        /// <returns></returns>
        public static ushort GetMaxValue(this ushort[] array) { return array.Max(); }
        #endregion
        #region GetMax
        /// <summary>
        /// Returns a Pair sequence where the key is the index of the largest number in the specified array and its corresponding value is that number.
        /// </summary>
        /// <param name="array">The int array for retrieving the largest number.</param>
        /// <returns></returns>
        public static KeyValuePair<int, int> GetMax(this int[] array) { return new KeyValuePair<int, int>(array.ToList().IndexOf(array.Max()), array.Max()); }
        /// <summary>
        /// Returns a Pair sequence where the key is the index of the largest number in the specified array and its corresponding value is that number.
        /// </summary>
        /// <param name="array">The decimal array for retrieving the largest number.</param>
        /// <returns></returns>
        public static KeyValuePair<int, decimal> GetMax(this decimal[] array) { return new KeyValuePair<int, decimal>(array.ToList().IndexOf(array.Max()), array.Max()); }
        /// <summary>
        /// Returns a Pair sequence where the key is the index of the largest number in the specified array and its corresponding value is that number.
        /// </summary>
        /// <param name="array">The double array for retrieving the largest number.</param>
        /// <returns></returns>
        public static KeyValuePair<int, double> GetMax(this double[] array) { return new KeyValuePair<int, double>(array.ToList().IndexOf(array.Max()), array.Max()); }
        /// <summary>
        /// Returns a Pair sequence where the key is the index of the largest number in the specified array and its corresponding value is that number.
        /// </summary>
        /// <param name="array">The float array for retrieving the largest number.</param>
        /// <returns></returns>
        public static KeyValuePair<int, float> GetMax(this float[] array) { return new KeyValuePair<int, float>(array.ToList().IndexOf(array.Max()), array.Max()); }
        /// <summary>
        /// Returns a Pair sequence where the key is the index of the largest number in the specified array and its corresponding value is that number.
        /// </summary>
        /// <param name="array">The long array for retrieving the largest number.</param>
        /// <returns></returns>
        public static KeyValuePair<int, long> GetMax(this long[] array) { return new KeyValuePair<int, long>(array.ToList().IndexOf(array.Max()), array.Max()); }
        /// <summary>
        /// Returns a Pair sequence where the key is the index of the largest number in the specified array and its corresponding value is that number.
        /// </summary>
        /// <param name="array">The short array for retrieving the largest number.</param>
        /// <returns></returns>
        public static KeyValuePair<int, short> GetMax(this short[] array) { return new KeyValuePair<int, short>(array.ToList().IndexOf(array.Max()), array.Max()); }
        /// <summary>
        /// Returns a Pair sequence where the key is the index of the largest number in the specified array and its corresponding value is that number.
        /// </summary>
        /// <param name="array">The uint array for retrieving the largest number.</param>
        /// <returns></returns>
        public static KeyValuePair<int, uint> GetMax(this uint[] array) { return new KeyValuePair<int, uint>(array.ToList().IndexOf(array.Max()), array.Max()); }
        /// <summary>
        /// Returns a Pair sequence where the key is the index of the largest number in the specified array and its corresponding value is that number.
        /// </summary>
        /// <param name="array">The ulong array for retrieving the largest number.</param>
        /// <returns></returns>
        public static KeyValuePair<int, ulong> GetMax(this ulong[] array) { return new KeyValuePair<int, ulong>(array.ToList().IndexOf(array.Max()), array.Max()); }
        /// <summary>
        /// Returns a Pair sequence where the key is the index of the largest number in the specified array and its corresponding value is that number.
        /// </summary>
        /// <param name="array">The ushort array for retrieving the largest number.</param>
        /// <returns></returns>
        public static KeyValuePair<int, ushort> GetMax(this ushort[] array) { return new KeyValuePair<int, ushort>(array.ToList().IndexOf(array.Max()), array.Max()); }
        #endregion
        #region GetMin
        /// <summary>
        /// Returns a Pair sequence where the key is the index of the smallest number in the specified array and its corresponding value is that number.
        /// </summary>
        /// <param name="array">The int array for retrieving the smallest number.</param>
        /// <returns></returns>
        public static KeyValuePair<int, int> GetMin(this int[] array) { return new KeyValuePair<int, int>(array.ToList().IndexOf(array.Min()), array.Min()); }
        /// <summary>
        /// Returns a Pair sequence where the key is the index of the smallest number in the specified array and its corresponding value is that number.
        /// </summary>
        /// <param name="array">The decimal array for retrieving the smallest number.</param>
        /// <returns></returns>
        public static KeyValuePair<int, decimal> GetMin(this decimal[] array) { return new KeyValuePair<int, decimal>(array.ToList().IndexOf(array.Min()), array.Min()); }
        /// <summary>
        /// Returns a Pair sequence where the key is the index of the smallest number in the specified array and its corresponding value is that number.
        /// </summary>
        /// <param name="array">The double array for retrieving the smallest number.</param>
        /// <returns></returns>
        public static KeyValuePair<int, double> GetMin(this double[] array) { return new KeyValuePair<int, double>(array.ToList().IndexOf(array.Min()), array.Min()); }
        /// <summary>
        /// Returns a Pair sequence where the key is the index of the smallest number in the specified array and its corresponding value is that number.
        /// </summary>
        /// <param name="array">The float array for retrieving the smallest number.</param>
        /// <returns></returns>
        public static KeyValuePair<int, float> GetMin(this float[] array) { return new KeyValuePair<int, float>(array.ToList().IndexOf(array.Min()), array.Min()); }
        /// <summary>
        /// Returns a Pair sequence where the key is the index of the smallest number in the specified array and its corresponding value is that number.
        /// </summary>
        /// <param name="array">The long array for retrieving the smallest number.</param>
        /// <returns></returns>
        public static KeyValuePair<int, long> GetMin(this long[] array) { return new KeyValuePair<int, long>(array.ToList().IndexOf(array.Min()), array.Min()); }
        /// <summary>
        /// Returns a Pair sequence where the key is the index of the smallest number in the specified array and its corresponding value is that number.
        /// </summary>
        /// <param name="array">The short array for retrieving the smallest number.</param>
        /// <returns></returns>
        public static KeyValuePair<int, short> GetMin(this short[] array) { return new KeyValuePair<int, short>(array.ToList().IndexOf(array.Min()), array.Min()); }
        /// <summary>
        /// Returns a Pair sequence where the key is the index of the smallest number in the specified array and its corresponding value is that number.
        /// </summary>
        /// <param name="array">The uint array for retrieving the smallest number.</param>
        /// <returns></returns>
        public static KeyValuePair<int, uint> GetMin(this uint[] array) { return new KeyValuePair<int, uint>(array.ToList().IndexOf(array.Min()), array.Min()); }
        /// <summary>
        /// Returns a Pair sequence where the key is the index of the smallest number in the specified array and its corresponding value is that number.
        /// </summary>
        /// <param name="array">The ulong array for retrieving the smallest number.</param>
        /// <returns></returns>
        public static KeyValuePair<int, ulong> GetMin(this ulong[] array) { return new KeyValuePair<int, ulong>(array.ToList().IndexOf(array.Min()), array.Min()); }
        /// <summary>
        /// Returns a Pair sequence where the key is the index of the smallest number in the specified array and its corresponding value is that number.
        /// </summary>
        /// <param name="array">The ulong array for retrieving the smallest number.</param>
        /// <returns></returns>
        public static KeyValuePair<int, ushort> GetMin(this ushort[] array) { return new KeyValuePair<int, ushort>(array.ToList().IndexOf(array.Min()), array.Min()); }
        #endregion
        #region GetMinValue
        /// <summary>
        /// Returns the smallest number in the specified int array.
        /// </summary>
        /// <param name="array">The int array to check.</param>
        /// <returns></returns>
        public static int GetMinValue(this int[] array) { return array.Min(); }
        /// <summary>
        /// Returns the smallest number in the specified decimal array.
        /// </summary>
        /// <param name="array">The decimal array to check.</param>
        /// <returns></returns>
        public static decimal GetMinValue(this decimal[] array) { return array.Min(); }
        /// <summary>
        /// Returns the smallest number in the specified double array.
        /// </summary>
        /// <param name="array">The double array to check.</param>
        /// <returns></returns>
        public static double GetMinValue(this double[] array) { return array.Min(); }
        /// <summary>
        /// Returns the smallest number in the specified float array.
        /// </summary>
        /// <param name="array">The float array to check.</param>
        /// <returns></returns>
        public static float GetMinValue(this float[] array) { return array.Min(); }
        /// <summary>
        /// Returns the smallest number in the specified long array.
        /// </summary>
        /// <param name="array">The long array to check.</param>
        /// <returns></returns>
        public static long GetMinValue(this long[] array) { return array.Min(); }
        /// <summary>
        /// Returns the smallest number in the specified short array.
        /// </summary>
        /// <param name="array">The short array to check.</param>
        /// <returns></returns>
        public static short GetMinValue(this short[] array) { return array.Min(); }
        /// <summary>
        /// Returns the smallest number in the specified uint array.
        /// </summary>
        /// <param name="array">The uint array to check.</param>
        /// <returns></returns>
        public static uint GetMinValue(this uint[] array) { return array.Min(); }
        /// <summary>
        /// Returns the smallest number in the specified ulong array.
        /// </summary>
        /// <param name="array">The ulong array to check.</param>
        /// <returns></returns>
        public static ulong GetMinValue(this ulong[] array) { return array.Min(); }
        /// <summary>
        /// Returns the smallest number in the specified ushort array.
        /// </summary>
        /// <param name="array">The ushort array to check.</param>
        /// <returns></returns>
        public static ushort GetMinValue(this ushort[] array) { return array.Min(); }
        #endregion
        /// <summary>
        /// Clears an array of all contents.
        /// </summary>
        /// <param name="array">The array to clear.</param>
        public static void ClearArray(Array array) { Array.Clear(array, 0, array.Length); }
        /// <summary>
        /// Returns the character from the given string at the specified index.
        /// </summary>
        /// <param name="str">The string to search.</param>
        /// <param name="index">The index of the desired character.</param>
        /// <returns></returns>
        public static char GetChar(string str, int index) { return str[index]; }
        /// <summary>
        /// Returns the index of the first occurrence of the given character from the specified string.
        /// </summary>
        /// <param name="str">The string to search.</param>
        /// <param name="c">The character to search for.</param>
        /// <returns></returns>
        public static int GetIndexOfChar(string str, char c) { return str.IndexOf(c); }
    }
    /// <summary>
    /// Class for manipulating shortcuts.
    /// </summary>
    public class Shortcuts
    {
        /// <summary>
        /// Returns the target path of the specified shortcut (.lnk) file.
        /// </summary>
        /// <param name="pathofshortcut">Full or relative path of the shortcut file.</param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        public static string GetTargetPathofShortcut(string pathofshortcut)
        {
            if (System.IO.File.Exists(pathofshortcut))
            {
                WshShell shell = new WshShell();
                IWshShortcut link = (IWshShortcut)shell.CreateShortcut(pathofshortcut);
                return link.TargetPath;
            }
            else
            {
                throw new FileNotFoundException("The specified file doesn't exist.");
            }
        }
        /// <summary>
        /// Creates a shortcut that targets a file.
        /// </summary>
        /// <param name="TargetFile">The file that the shortcut targets.</param>
        /// <param name="ShortcutFile">The shortcut file.</param>
        /// <param name="Description">Optional parameter. Sets the shortcut file's description.</param>
        /// <param name="Arguments">Optional parameter. Sets the shortcut file's arguments.</param>
        /// <param name="IconLocation">Optional parameter. Sets the icon for the shortcut file.</param>
        /// <param name="WorkingDirectory">Optional parameter. Sets the working directory.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void CreateShortcutToFile(string TargetFile, string ShortcutFile, string Description = null, string Arguments = null, string IconLocation = null, string WorkingDirectory = null)
        {
            if (String.IsNullOrEmpty(TargetFile))
            {
                throw new ArgumentNullException("TargetFile");
            }

            if (String.IsNullOrEmpty(ShortcutFile))
            {
                throw new ArgumentNullException("ShortcutFile");
            }

            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(ShortcutFile);
            shortcut.TargetPath = TargetFile;
            if (!String.IsNullOrEmpty(Description))
            {
                shortcut.Description = Description;
            }

            if (!String.IsNullOrEmpty(Arguments))
            {
                shortcut.Arguments = Arguments;
            }

            if (!String.IsNullOrEmpty(IconLocation))
            {
                shortcut.IconLocation = IconLocation;
            }

            if (!String.IsNullOrEmpty(WorkingDirectory))
            {
                shortcut.WorkingDirectory = WorkingDirectory;
            }

            shortcut.Save();
        }
        /// <summary>
        /// Creates a shortcut that targets a folder.
        /// </summary>
        /// <param name="TargetPath">The folder that the shortcut targets.</param>
        /// <param name="ShortcutFile">The shortcut file.</param>
        /// <param name="Description">Optional parameter. Sets the shortcut file's description.</param>
        /// <param name="Arguments">Optional parameter. Sets the shortcut file's arguments.</param>
        /// <param name="HotKey">Optional parameter. Sets the hotkey.</param>
        /// <param name="WorkingDirectory">Optional parameter. Sets the working directory.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void CreateShortcutToFolder(string TargetPath, string ShortcutFile, string Description = null, string Arguments = null, string HotKey = null, string WorkingDirectory = null)
        {
            if (String.IsNullOrEmpty(TargetPath))
            {
                throw new ArgumentNullException("TargetPath");
            }

            if (String.IsNullOrEmpty(ShortcutFile))
            {
                throw new ArgumentNullException("ShortcutFile");
            }

            var wshShell = new WshShell();
            IWshShortcut shorcut = (IWshShortcut)wshShell.CreateShortcut(ShortcutFile);
            shorcut.TargetPath = TargetPath;
            if (!String.IsNullOrEmpty(Description))
            {
                shorcut.Description = Description;
            }

            if (!String.IsNullOrEmpty(Arguments))
            {
                shorcut.Arguments = Arguments;
            }

            if (!String.IsNullOrEmpty(HotKey))
            {
                shorcut.Hotkey = HotKey;
            }

            if (!String.IsNullOrEmpty(WorkingDirectory))
            {
                shorcut.WorkingDirectory = WorkingDirectory;
            }

            shorcut.Save();
        }
    }
    /// <summary>
    /// Class for string encryption and decryption.
    /// </summary>
    public class Encryption
    {
        private const int Keysize = 256;
        private const int DerivationIterations = 1000;
        /// <summary>
        /// Encrypts a string with the specified password, and returns the encrypted string.
        /// </summary>
        /// <param name="text">The text to encrypt.</param>
        /// <param name="password">The password or passphrase to use for encryption.</param>
        /// <returns></returns>
        public static string Encrypt(string text, string password)
        {
            // Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
            // so that the same Salt and IV values can be used when decrypting.  
            var saltStringBytes = Generate256BitsOfRandomEntropy();
            var ivStringBytes = Generate256BitsOfRandomEntropy();
            var plainTextBytes = Encoding.UTF8.GetBytes(text);
            var passw = new Rfc2898DeriveBytes(password, saltStringBytes, DerivationIterations);
            var keyBytes = passw.GetBytes(Keysize / 8);
            using (var symmetricKey = new RijndaelManaged())
            {
                symmetricKey.BlockSize = 256;
                symmetricKey.Mode = CipherMode.CBC;
                symmetricKey.Padding = PaddingMode.PKCS7;
                using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                            cryptoStream.FlushFinalBlock();
                            // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
                            var cipherTextBytes = saltStringBytes;
                            cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                            cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                            memoryStream.Close();
                            cryptoStream.Close();
                            return Convert.ToBase64String(cipherTextBytes);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Returns the specified encrypted string with the password used to encrypt.
        /// </summary>
        /// <param name="encryptedText">The encrypted string to be decrypted.</param>
        /// <param name="password">The password used to encrypt the string.</param>
        /// <returns></returns>
        public static string Decrypt(string encryptedText, string password)
        {
            // Get the complete stream of bytes that represent:
            // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
            var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(encryptedText);
            // Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
            var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
            // Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
            var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
            // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
            var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();
            var passw = new Rfc2898DeriveBytes(password, saltStringBytes, DerivationIterations);
            var keyBytes = passw.GetBytes(Keysize / 8);
            using (var symmetricKey = new RijndaelManaged())
            {
                symmetricKey.BlockSize = 256;
                symmetricKey.Mode = CipherMode.CBC;
                symmetricKey.Padding = PaddingMode.PKCS7;
                using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                {
                    using (var memoryStream = new MemoryStream(cipherTextBytes))
                    {
                        using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            var plainTextBytes = new byte[cipherTextBytes.Length];
                            var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                            memoryStream.Close();
                            cryptoStream.Close();
                            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Hashes the given string and returns the resulting hash. NOTE: This method is one-way: the generated hash cannot be decrypted.
        /// </summary>
        /// <param name="text">String to generate the hash from.</param>
        /// <returns></returns>
        public static string GenerateHash(string text)
        {
            var crypt = new SHA256Managed();
            var hash = new StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(text));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }
        private static byte[] Generate256BitsOfRandomEntropy()
        {
            var randomBytes = new byte[32]; // 32 Bytes will give us 256 bits.
            var rngCsp = new RNGCryptoServiceProvider();
            // Fill the array with cryptographically secure random bytes.
            rngCsp.GetBytes(randomBytes);
            return randomBytes;
        }
    }
}