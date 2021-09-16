using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Playground
{
    public static class Logger
    {
        public static void Log(string message, [CallerFilePath] string callerFilePath = null)
        {
            var callerClassName = Path.GetFileNameWithoutExtension(callerFilePath);
            Console.WriteLine($"[{callerClassName}]\t{message}");
        }
    }
}