using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace FastCore
{
    /// <summary>
    /// 日志(输出到控制台)
    /// </summary>
    public static class LogConsole
    {
        /// <summary>
        /// 写调试日志
        /// </summary>
        /// <param name="exception">异常</param>
        /// <param name="source">来源</param>
        /// <param name="extraData">附加数据</param>
        public static void Debug(Exception exception, MethodBase source = null, string extraData = "")
        {
            // 设置类型
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write("debug");

            // 设置时间
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"({DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")})");

            // 设置消息
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(": " + exception.Message);

            // 设置来源
            if (source == null)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write($" 发生在: {exception.Source}");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write($" 发生在: {source.ReflectedType.FullName}.{source.Name}");
            }

            // 设置详情
            if (!string.IsNullOrEmpty(exception.StackTrace))
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("");
                Console.Write(exception.StackTrace);
            }

            // 设置附加数据
            if (!string.IsNullOrEmpty(extraData))
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("");
                Console.Write(extraData);
            }

            Console.WriteLine("");
            Console.ResetColor();
        }

        /// <summary>
        /// 写调试日志
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="source">来源</param>
        /// <param name="extraData">附加数据</param>
        public static void Debug(string message, MethodBase source = null, string extraData = "")
        {
            // 设置类型
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write("debug");

            // 设置时间
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"({DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")})");

            // 设置消息
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(": " + message);

            // 设置来源
            if (source != null)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write($" 发生在: {source.ReflectedType.FullName}.{source.Name}");
            }

            // 设置附加数据
            if (!string.IsNullOrEmpty(extraData))
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("");
                Console.Write(extraData);
            }

            Console.WriteLine("");
            Console.ResetColor();
        }

        /// <summary>
        /// 写信息日志
        /// </summary>
        /// <param name="exception">异常</param>
        /// <param name="source">来源</param>
        /// <param name="extraData">附加数据</param>
        public static void Info(Exception exception, MethodBase source = null, string extraData = "")
        {
            // 设置类型
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write("info ");

            // 设置时间
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"({DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")})");

            // 设置消息
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(": " + exception.Message);

            // 设置来源
            if (source == null)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write($" 发生在: {exception.Source}");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write($" 发生在: {source.ReflectedType.FullName}.{source.Name}");
            }

            // 设置详情
            if (!string.IsNullOrEmpty(extraData))
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("");
                Console.Write(exception.StackTrace);
            }

            // 设置附加数据
            if (!string.IsNullOrEmpty(extraData))
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("");
                Console.Write(extraData);
            }

            Console.WriteLine("");
            Console.ResetColor();
        }

        /// <summary>
        /// 写信息日志
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="source">来源</param>
        /// <param name="extraData">附加数据</param>
        public static void Info(string message, MethodBase source = null, string extraData = "")
        {
            // 设置类型
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write("info ");

            // 设置时间
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"({DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")})");

            // 设置消息
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(": " + message);

            // 设置来源
            if (source != null)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write($" 发生在: {source.ReflectedType.FullName}.{source.Name}");
            }

            // 设置附加数据
            if (!string.IsNullOrEmpty(extraData))
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("");
                Console.Write(extraData);
            }

            Console.WriteLine("");
            Console.ResetColor();
        }

        /// <summary>
        /// 写警告日志
        /// </summary>
        /// <param name="exception">异常</param>
        /// <param name="source">来源</param>
        /// <param name="extraData">附加数据</param>
        public static void Warn(Exception exception, MethodBase source = null, string extraData = "")
        {
            // 设置类型
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write("warn ");

            // 设置时间
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"({DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")})");

            // 设置消息
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(": " + exception.Message);

            // 设置来源
            if (source == null)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write($" 发生在: {exception.Source}");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write($" 发生在: {source.ReflectedType.FullName}.{source.Name}");
            }

            // 设置详情
            if (!string.IsNullOrEmpty(exception.StackTrace))
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("");
                Console.Write(exception.StackTrace);
            }

            // 设置附加数据
            if (!string.IsNullOrEmpty(extraData))
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("");
                Console.Write(extraData);
            }

            Console.WriteLine("");
            Console.ResetColor();
        }

        /// <summary>
        /// 写警告日志
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="source">来源</param>
        /// <param name="extraData">附加数据</param>
        public static void Warn(string message, MethodBase source = null, string extraData = "")
        {
            // 设置类型
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write("warn ");

            // 设置时间
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"({DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")})");

            // 设置消息
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(": " + message);

            // 设置来源
            if (source == null)
            { 
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write($" 发生在: {source.ReflectedType.FullName}.{source.Name}");
            }

            // 设置附加数据
            if (!string.IsNullOrEmpty(extraData))
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("");
                Console.Write(extraData);
            }

            Console.WriteLine("");
            Console.ResetColor();
        }

        /// <summary>
        /// 写错误日志
        /// </summary>
        /// <param name="exception">异常</param>
        /// <param name="source">来源</param>
        /// <param name="extraData">附加数据</param>
        public static void Error(Exception exception, MethodBase source = null, string extraData = "")
        {
            // 设置类型
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write("error");

            // 设置时间
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"({DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")})");

            // 设置消息
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($": {exception.Message}");

            // 设置来源
            if (source == null)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write($" 发生在: {exception.Source}");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write($" 发生在: {source.ReflectedType.FullName}.{source.Name}");
            }

            // 设置详情
            if (!string.IsNullOrEmpty(exception.StackTrace))
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("");
                Console.Write(exception.StackTrace);
            }

            // 设置附加数据
            if (!string.IsNullOrEmpty(extraData))
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("");
                Console.Write(extraData);
            }

            Console.WriteLine("");
            Console.ResetColor();
        }

        /// <summary>
        /// 写错误日志
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="source">来源</param>
        /// <param name="extraData">附加数据</param>
        public static void Error(string message, MethodBase source = null, string extraData = "")
        {
            // 设置类型
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write("error");

            // 设置时间
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"({DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")})");

            // 设置消息
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($": {message}");

            // 设置来源
            if (source != null)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write($" 发生在: {source.ReflectedType.FullName}.{source.Name}");
            }

            // 设置附加数据
            if (!string.IsNullOrEmpty(extraData))
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("");
                Console.Write(extraData);
            }

            Console.WriteLine("");
            Console.ResetColor();
        }

        /// <summary>
        /// 写致命日志
        /// </summary>
        /// <param name="exception">异常</param>
        /// <param name="source">来源</param>
        /// <param name="extraData">附加数据</param>
        public static void Fatal(Exception exception, MethodBase source = null, string extraData = "")
        {
            // 设置类型
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("fatal");

            // 设置时间
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"({DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")})");

            // 设置消息
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($": {exception.Message}");

            // 设置来源
            if (source == null)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write($" 发生在: {exception.Source}");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write($" 发生在: {source.ReflectedType.FullName}.{source.Name}");
            }

            // 设置详情
            if (!string.IsNullOrEmpty(exception.StackTrace))
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("");
                Console.Write(exception.StackTrace);
            }

            // 设置附加数据
            if (!string.IsNullOrEmpty(extraData))
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("");
                Console.Write(extraData);
            }

            Console.WriteLine("");
            Console.ResetColor();
        }

        /// <summary>
        /// 写致命日志
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="source">来源</param>
        /// <param name="extraData">附加数据</param>
        public static void Fatal(string message, MethodBase source = null, string extraData = "")
        {
            // 设置类型
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("fatal");

            // 设置时间
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"({DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")})");

            // 设置消息
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(": " + message);

            // 设置来源
            if (source != null)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write($" 发生在: {source.ReflectedType.FullName}.{source.Name}");
            }

            // 设置附加数据
            if (!string.IsNullOrEmpty(extraData))
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("");
                Console.Write(extraData);
            }

            Console.WriteLine("");
            Console.ResetColor();
        }
    }
}
