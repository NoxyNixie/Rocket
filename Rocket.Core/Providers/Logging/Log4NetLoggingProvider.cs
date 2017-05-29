﻿using System;
using log4net;
using log4net.Config;
using System.Diagnostics;
using System.IO;
using Rocket.API.Providers.Logging;

namespace Rocket.Core.Providers.Logging
{
    public class Log4NetLoggingProvider : IRocketLoggingProvider
    {
        public bool EchoNativeOutput { get; } = true;

        public void Load(bool isReload)
        {
            string logConfiguration = "log4net.config.xml";
            try
            {
                if (!File.Exists(logConfiguration))
                {
                    var config =
                       @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<log4net>
    <appender name=""RollingFile"" type=""log4net.Appender.RollingFileAppender"">
        <file value=""Logs/Rocket.log"" />
        <appendToFile value=""false"" />
        <maximumFileSize value=""5MB"" />
        <maxSizeRollBackups value=""-1"" />
        <layout type=""log4net.Layout.PatternLayout"">
            <conversionPattern value=""%level %thread %logger - %message%newline"" />
        </layout>
    </appender>
    <root>
        <level value=""DEBUG"" />
        <appender-ref ref=""RollingFile"" />
    </root>
</log4net>";
                    File.WriteAllText(logConfiguration, config);
                }

                XmlConfigurator.Configure(new FileInfo(logConfiguration));

                AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
                {
                    Log(LogLevel.ERROR, e.ExceptionObject);
                };
            }
            catch (Exception ex)
            {
                File.AppendAllText("error.txt", ex.ToString());
            }
        }


        private static ILog GetLogger()
        {
            StackFrame frame = new StackFrame(2);
            var method = frame.GetMethod();
            return LogManager.GetLogger(method.DeclaringType);
        }

        public bool IsDebugEnabled => GetLogger().IsDebugEnabled;
        public bool IsInfoEnabled => GetLogger().IsInfoEnabled;
        public bool IsWarnEnabled => GetLogger().IsWarnEnabled;
        public bool IsErrorEnabled => GetLogger().IsErrorEnabled;
        public bool IsFatalEnabled => GetLogger().IsFatalEnabled;

        public void Log(LogLevel level, object message, Exception exception = null)
        {
            switch (level)
            {
                case LogLevel.DEBUG:
                    GetLogger().Debug(message, exception);
                    break;
                case LogLevel.INFO:
                    GetLogger().Info(message, exception);
                    break;
                case LogLevel.WARN:
                    GetLogger().Warn(message, exception);
                    break;
                case LogLevel.ERROR:
                    GetLogger().Error(message, exception);
                    break;
                case LogLevel.FATAL:
                    GetLogger().Fatal(message, exception);
                    break;
            }
        }

        public void Unload(bool isReload = false)
        {
            //
        }

        public void Log(LogLevel level, Exception exception)
        {
            Log(level, null, exception);
        }
    }
}
