using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace mangasurvlib.Logging
{
    public static class ApplicationLogging
    {
        public const string _MARK = "===========================================";

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

        public static void StartLogging(this ILogger logger, Assembly asb)
        {
            logger.LogInformation(_MARK);
            logger.LogInformation("Date:\t\t'{0}'", DateTime.Now.ToString("dd.MM.yyyy"));
            logger.LogInformation("User:\t\t'{0}'", System.Security.Claims.ClaimsPrincipal.Current);
            logger.LogInformation("Application:\t'{0}'", asb.GetName().Name);
            logger.LogInformation("Version:\t\t'{0}'", asb.GetName().Version.ToString());
            logger.LogInformation("");
        }

        public static void EndLogging(this ILogger logger)
        {
            logger.LogInformation(_MARK);
        }
    }
}
