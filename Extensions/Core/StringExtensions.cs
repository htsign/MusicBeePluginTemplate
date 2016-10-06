using System;
using System.Text.RegularExpressions;

namespace MusicBeePlugin.Extensions.Core
{
    public static class StringExtension
    {
        /// <summary>
        /// <paramref name="separator"/>で文字列を分割します。
        /// </summary>
        /// <param name="self"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string[] Split(this string self, string separator)
            => self.Replace(separator, "\0").Split('\0');

        /// <summary>
        /// 空文字で文字列を結合します。
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static string Join(this string[] self) => self.Join("");

        /// <summary>
        /// <paramref name="selarator"/>で文字列を結合します。
        /// </summary>
        /// <param name="self"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string Join(this string[] self, string separator)
            => string.Join(separator, self);

        /// <summary>
        /// 指定の文字列で挟まれているかを返します。
        /// </summary>
        /// <param name="self"></param>
        /// <param name="start">文字列の先頭</param>
        /// <param name="end">文字列の末尾</param>
        /// <returns></returns>
        public static bool IsWrappedWith(this string self, string start, string end)
            => self.StartsWith(start) && self.EndsWith(end);

        /// <summary>
        /// 文字列が.NET Frameworkの正規表現エンジンで評価できるかを返します。
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool IsRegexPattern(this string self)
        {
            try
            {
                new Regex(self).ToString();
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }
    }
}
