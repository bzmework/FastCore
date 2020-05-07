using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace FastCore
{
    /// <summary>
    /// 日志输出目标
    /// </summary>
    [Flags]
    public enum LogTarget
    {
        /// <summary>控制台</summary>
        Console = 1,

        /// <summary>调试窗口</summary>
        Debug = 2,

        /// <summary>跟踪窗口</summary>
        Trace = 4,

        /// <summary>文件</summary>
        File = 8,

        /// <summary>邮件</summary>
        Email = 16
    }

    /// <summary>
    /// 日志
    /// </summary>
    public static class Log
    {
        private static LogTarget _target = LogTarget.Console;

        /// <summary>
        /// 配置日志
        /// </summary>
        /// <param name="target">输出目标</param>
        public static void Configure(LogTarget target)
        {
            _target = target;
        }

        /// <summary>
        /// 写调试日志
        /// </summary>
        /// <param name="exception">异常</param>
        /// <param name="source">来源</param>
        /// <param name="extraData">附加数据</param>
        public static void Debug(Exception exception, MethodBase source = null, string extraData = "")
        {
            if ((_target & LogTarget.Console) == LogTarget.Console) 
            {
                LogConsole.Debug(exception, source, extraData);
            }
            
            if ((_target & LogTarget.Debug) == LogTarget.Debug)
            {
                LogDebug.Debug(exception, source, extraData);
            }

            if ((_target & LogTarget.Trace) == LogTarget.Trace)
            {
                LogTrace.Debug(exception, source, extraData);
            }
        }

        /// <summary>
        /// 写调试日志
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="source">来源</param>
        /// <param name="extraData">附加数据</param>
        public static void Debug(string message, MethodBase source = null, string extraData = "")
        {
            if ((_target & LogTarget.Console) == LogTarget.Console)
            {
                LogConsole.Debug(message, source, extraData);
            }
            
            if ((_target & LogTarget.Debug) == LogTarget.Debug)
            {
                LogDebug.Debug(message, source, extraData);
            }

            if ((_target & LogTarget.Trace) == LogTarget.Trace)
            {
                LogTrace.Debug(message, source, extraData);
            }
        }

        /// <summary>
        /// 写调信息日志
        /// </summary>
        /// <param name="exception">异常</param>
        /// <param name="source">来源</param>
        /// <param name="extraData">附加数据</param>
        public static void Info(Exception exception, MethodBase source = null, string extraData = "")
        {
            if ((_target & LogTarget.Console) == LogTarget.Console)
            {
                LogConsole.Info(exception, source, extraData);
            }
            
            if ((_target & LogTarget.Debug) == LogTarget.Debug)
            {
                LogDebug.Info(exception, source, extraData);
            }

            if ((_target & LogTarget.Trace) == LogTarget.Trace)
            {
                LogTrace.Info(exception, source, extraData);
            }
        }

        /// <summary>
        /// 写信息日志
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="source">来源</param>
        /// <param name="extraData">附加数据</param>
        public static void Info(string message, MethodBase source = null, string extraData = "")
        {
            if ((_target & LogTarget.Console) == LogTarget.Console)
            {
                LogConsole.Info(message, source, extraData);
            }
            
            if ((_target & LogTarget.Debug) == LogTarget.Debug)
            {
                LogDebug.Info(message, source, extraData);
            }

            if ((_target & LogTarget.Trace) == LogTarget.Trace)
            {
                LogTrace.Info(message, source, extraData);
            }
        }

        /// <summary>
        /// 写调警告日志
        /// </summary>
        /// <param name="exception">异常</param>
        /// <param name="source">来源</param>
        /// <param name="extraData">附加数据</param>
        public static void Warn(Exception exception, MethodBase source = null, string extraData = "")
        {
            if ((_target & LogTarget.Console) == LogTarget.Console)
            {
                LogConsole.Warn(exception, source, extraData);
            }

            if ((_target & LogTarget.Debug) == LogTarget.Debug)
            {
                LogDebug.Warn(exception, source, extraData);
            }

            if ((_target & LogTarget.Trace) == LogTarget.Trace)
            {
                LogTrace.Warn(exception, source, extraData);
            }
        }

        /// <summary>
        /// 写警告日志
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="source">来源</param>
        /// <param name="extraData">附加数据</param>
        public static void Warn(string message, MethodBase source = null, string extraData = "")
        {
            if ((_target & LogTarget.Console) == LogTarget.Console)
            {
                LogConsole.Warn(message, source, extraData);
            }

            if ((_target & LogTarget.Debug) == LogTarget.Debug)
            {
                LogDebug.Warn(message, source, extraData);
            }

            if ((_target & LogTarget.Trace) == LogTarget.Trace)
            {
                LogTrace.Warn(message, source, extraData);
            }
        }

        /// <summary>
        /// 写错误日志
        /// </summary>
        /// <param name="exception">异常</param>
        /// <param name="source">来源</param>
        /// <param name="extraData">附加数据</param>
        public static void Error(Exception exception, MethodBase source = null, string extraData = "")
        {
            if ((_target & LogTarget.Console) == LogTarget.Console)
            {
                LogConsole.Error(exception, source, extraData);
            }

            if ((_target & LogTarget.Debug) == LogTarget.Debug)
            {
                LogDebug.Error(exception, source, extraData);
            }

            if ((_target & LogTarget.Trace) == LogTarget.Trace)
            {
                LogTrace.Error(exception, source, extraData);
            }
        }

        /// <summary>
        /// 写错误日志
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="source">来源</param>
        /// <param name="extraData">附加数据</param>
        public static void Error(string message, MethodBase source = null, string extraData = "")
        {
            if ((_target & LogTarget.Console) == LogTarget.Console)
            {
                LogConsole.Error(message, source, extraData);
            }

            if ((_target & LogTarget.Debug) == LogTarget.Debug)
            {
                LogDebug.Error(message, source, extraData);
            }

            if ((_target & LogTarget.Trace) == LogTarget.Trace)
            {
                LogTrace.Error(message, source, extraData);
            }
        }

        /// <summary>
        /// 写致命日志
        /// </summary>
        /// <param name="exception">异常</param>
        /// <param name="source">来源</param>
        /// <param name="extraData">附加数据</param>
        public static void Fatal(Exception exception, MethodBase source = null, string extraData = "")
        {
            if ((_target & LogTarget.Console) == LogTarget.Console)
            {
                LogConsole.Fatal(exception, source, extraData);
            }

            if ((_target & LogTarget.Debug) == LogTarget.Debug)
            {
                LogDebug.Fatal(exception, source, extraData);
            }

            if ((_target & LogTarget.Trace) == LogTarget.Trace)
            {
                LogTrace.Fatal(exception, source, extraData);
            }
        }

        /// <summary>
        /// 写致命日志
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="source">来源</param>
        /// <param name="extraData">附加数据</param>
        public static void Fatal(string message, MethodBase source = null, string extraData = "")
        {
            if ((_target & LogTarget.Console) == LogTarget.Console)
            {
                LogConsole.Fatal(message, source, extraData);
            }

            if ((_target & LogTarget.Debug) == LogTarget.Debug)
            {
                LogDebug.Fatal(message, source, extraData);
            }

            if ((_target & LogTarget.Trace) == LogTarget.Trace)
            {
                LogTrace.Fatal(message, source, extraData);
            }
        }
    }
}
