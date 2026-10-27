using System;

namespace ModernizeMe
{
    internal static class ConsoleEx
    {
        public static void Log(string message, string source)
        {
            ConsoleColor color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(string.Format("[{0}] {1}", source, message));
            Console.ForegroundColor = color;
        }
    }
}
