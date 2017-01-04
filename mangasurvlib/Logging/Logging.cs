using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mangasurvlib.Logging
{
    public static class ApplicationLogging
    {
        public static ILoggerFactory LoggerFactory { get; } = new LoggerFactory();
        public static ILogger CreateLogger<T>() =>
          LoggerFactory.CreateLogger<T>();

        public static void ConfigureLogger()
        {
            // Add NLog provider
            ApplicationLogging.LoggerFactory.AddNLog();

            // Configure target
            NLog.Config.LoggingConfiguration config = new NLog.Config.LoggingConfiguration();
            NLog.Targets.FileTarget fileTarget = new NLog.Targets.FileTarget();
            fileTarget.Name = "logfile";
            fileTarget.Layout = "${time} |${level}| ${logger}  ${message}";
            fileTarget.FileName = "${basedir}/LogFiles/${shortdate}.log";

            NLog.Targets.ConsoleTarget consoleTarget = new NLog.Targets.ConsoleTarget();
            consoleTarget.Name = "console";
            consoleTarget.Layout = "${time} |${level}| ${message}";

            config.AddTarget("logfile", fileTarget);
            config.AddTarget("console", consoleTarget);

            config.LoggingRules.Add(new NLog.Config.LoggingRule("*", NLog.LogLevel.Trace, fileTarget));
            config.LoggingRules.Add(new NLog.Config.LoggingRule("*", NLog.LogLevel.Trace, consoleTarget));

            NLog.LogManager.Configuration = config;
        }
    }
}
