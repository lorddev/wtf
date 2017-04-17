using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using log4net;
using log4net.Config;
using log4net.Core;
using log4net.Repository;
using log4net.Repository.Hierarchy;
using Microsoft.Extensions.Logging;
using Xunit;
using ILogger = log4net.Core.ILogger;

namespace wtf.asp.tests
{
    public class LoggingTests
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(LoggingTests));

        [Fact]
        public void Test1()
        {
            var log4NetConfig = new XmlDocument();
            log4NetConfig.Load(File.OpenRead("log4net.config"));

            var repo = LogManager.CreateRepository(
                Assembly.GetEntryAssembly(), typeof(Hierarchy));
            
            XmlConfigurator.Configure(repo, log4NetConfig["log4net"]);

            Log.Info("Application - Main is invoked");
           
        }
    }
    public class Log4NetProvider //: ILoggerProvider
    {
        private IDictionary<string, ILogger> _loggers
            = new Dictionary<string, ILogger>();

        public ILogger CreateLogger(string name)
        {
            if (!_loggers.ContainsKey(name))
            {
                lock (_loggers)
                {
                    // Have to check again since another thread may have gotten the lock first
                    if (!_loggers.ContainsKey(name))
                    {
                        _loggers[name] = new Log4NetAdapter(name);
                    }
                }
            }
            return _loggers[name];
        }

        public void Dispose()
        {
            _loggers.Clear();
            _loggers = null;
        }
    }
    public class Log4NetAdapter : ILogger
    {
        public Log4NetAdapter(string name)
        {
            Name = name;
        }

        //    private ILog _logger;

        //    public Log4NetAdapter(string loggerName)
        //    {
        //        _logger = LogManager.GetLogger(loggerName);
        //    }

        //    public IDisposable BeginScopeImpl(object state)
        //    {
        //        return null;
        //    }

        //    public bool IsEnabled(LogLevel logLevel)
        //    {
        //        switch (logLevel)
        //        {
        //            case LogLevel.Debug:
        //                return _logger.IsDebugEnabled;
        //            case LogLevel.Information:
        //                return _logger.IsInfoEnabled;
        //            case LogLevel.Warning:
        //                return _logger.IsWarnEnabled;
        //            case LogLevel.Error:
        //                return _logger.IsErrorEnabled;
        //            case LogLevel.Critical:
        //                return _logger.IsFatalEnabled;
        //            default:
        //                throw new ArgumentException($"Unknown log level {logLevel}.", nameof(logLevel));
        //        }
        //    }

        //    public void Log(LogLevel logLevel, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
        //    {
        //        if (!IsEnabled(logLevel))
        //        {
        //            return;
        //        }
        //        string message = null;
        //        if (null != formatter)
        //        {
        //            message = formatter(state, exception);
        //        }
        //        else
        //        {
        //            message = LogFormatter.Formatter(state, exception);
        //        }
        //        switch (logLevel)
        //        {
        //            case LogLevel.Verbose:
        //            case LogLevel.Debug:
        //                _logger.Debug(message, exception);
        //                break;
        //            case LogLevel.Information:
        //                _logger.Info(message, exception);
        //                break;
        //            case LogLevel.Warning:
        //                _logger.Warn(message, exception);
        //                break;
        //            case LogLevel.Error:
        //                _logger.Error(message, exception);
        //                break;
        //            case LogLevel.Critical:
        //                _logger.Fatal(message, exception);
        //                break;
        //            default:
        //                _logger.Warn($"Encountered unknown log level {logLevel}, writing out as Info.");
        //                _logger.Info(message, exception);
        //                break;
        //        }
        public void Log(Type callerStackBoundaryDeclaringType, Level level, object message, Exception exception)
        {
            throw new NotImplementedException();
        }

        public void Log(LoggingEvent logEvent)
        {
            throw new NotImplementedException();
        }

        public bool IsEnabledFor(Level level)
        {
            throw new NotImplementedException();
        }

        public string Name { get; }
        public ILoggerRepository Repository { get; }
    }
    }
