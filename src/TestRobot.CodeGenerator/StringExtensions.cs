using System;
using System.Collections.Generic;
using System.Text;

namespace TestRobot.CodeGenerator
{
    internal static class StringExtensions
    {
        public static string FirstCharToUpperCase(this string str)
        {
            if (string.IsNullOrEmpty(str) || char.IsLower(str[0])) {
                return str;
            }
            return char.ToUpper(str[0]) + str.Substring(1);
        }
    }
}
