using System;
using log4net;
using log4net.Config;
using log4net.Core;
using TechTracker.Common.Utils.Configuration;


[assembly: XmlConfigurator(Watch = true)]

namespace TechTracker.Common.Tracer
{
    /// <summary>
    ///     Log4Net Wrapper
    /// </summary>
    public class LoggerWrapper : ILog
    {
        private static ILog _log;

        public LoggerWrapper(IAppConfigManager appConfig)
        {
            _log = LogManager.GetLogger(appConfig.GetSetting("LoggerName"));
        }

        public virtual ILogger Logger => _log.Logger;

        public virtual void Debug(object message)
        {
            _log.Debug(message);
        }

        public virtual void Debug(object message, Exception exception)
        {
            _log.Debug(message, exception);
        }

        public virtual void DebugFormat(string format, params object[] args)
        {
            _log.DebugFormat(format, args);
        }

        public virtual void DebugFormat(string format, object arg0)
        {
            _log.DebugFormat(format, arg0);
        }

        public virtual void DebugFormat(string format, object arg0, object arg1)
        {
            _log.DebugFormat(format, arg0, arg1);
        }

        public virtual void DebugFormat(string format, object arg0, object arg1, object arg2)
        {
            _log.DebugFormat(format, arg0, arg1, arg2);
        }

        public virtual void DebugFormat(IFormatProvider provider, string format, params object[] args)
        {
            _log.DebugFormat(provider, format, args);
        }

        public virtual void Info(object message)
        {
            _log.Info(message);
        }

        public virtual void Info(object message, Exception exception)
        {
            _log.Info(message, exception);
        }

        public virtual void InfoFormat(string format, params object[] args)
        {
            _log.InfoFormat(format, args);
        }

        public virtual void InfoFormat(string format, object arg0)
        {
            _log.InfoFormat(format, arg0);
        }

        public virtual void InfoFormat(string format, object arg0, object arg1)
        {
            _log.InfoFormat(format, arg0, arg1);
        }

        public virtual void InfoFormat(string format, object arg0, object arg1, object arg2)
        {
            _log.InfoFormat(format, arg0, arg1, arg2);
        }

        public virtual void InfoFormat(IFormatProvider provider, string format, params object[] args)
        {
            _log.InfoFormat(provider, format, args);
        }

        public virtual void Warn(object message)
        {
            _log.Warn(message);
        }

        public virtual void Warn(object message, Exception exception)
        {
            _log.Warn(message, exception);
        }

        public virtual void WarnFormat(string format, params object[] args)
        {
            _log.WarnFormat(format, args);
        }

        public virtual void WarnFormat(string format, object arg0)
        {
            _log.WarnFormat(format, arg0);
        }

        public virtual void WarnFormat(string format, object arg0, object arg1)
        {
            _log.WarnFormat(format, arg0, arg1);
        }

        public virtual void WarnFormat(string format, object arg0, object arg1, object arg2)
        {
            _log.WarnFormat(format, arg0, arg1, arg2);
        }

        public virtual void WarnFormat(IFormatProvider provider, string format, params object[] args)
        {
            _log.WarnFormat(provider, format, args);
        }

        public virtual void Error(object message)
        {
            _log.Error(message);
        }

        public virtual void Error(object message, Exception exception)
        {
            _log.Error(message, exception);
        }

        public virtual void ErrorFormat(string format, params object[] args)
        {
            _log.ErrorFormat(format, args);
        }

        public virtual void ErrorFormat(string format, object arg0)
        {
            _log.ErrorFormat(format, arg0);
        }

        public virtual void ErrorFormat(string format, object arg0, object arg1)
        {
            _log.ErrorFormat(format, arg0, arg1);
        }

        public virtual void ErrorFormat(string format, object arg0, object arg1, object arg2)
        {
            _log.ErrorFormat(format, arg0, arg1, arg2);
        }

        public virtual void ErrorFormat(IFormatProvider provider, string format, params object[] args)
        {
            _log.ErrorFormat(provider, format, args);
        }

        public virtual void Fatal(object message)
        {
            _log.Fatal(message);
        }

        public virtual void Fatal(object message, Exception exception)
        {
            _log.Fatal(message, exception);
        }

        public virtual void FatalFormat(string format, params object[] args)
        {
            _log.FatalFormat(format, args);
        }

        public virtual void FatalFormat(string format, object arg0)
        {
            _log.FatalFormat(format, arg0);
        }

        public virtual void FatalFormat(string format, object arg0, object arg1)
        {
            _log.FatalFormat(format, arg0, arg1);
        }

        public virtual void FatalFormat(string format, object arg0, object arg1, object arg2)
        {
            _log.FatalFormat(format, arg0, arg1, arg2);
        }

        public virtual void FatalFormat(IFormatProvider provider, string format, params object[] args)
        {
            _log.FatalFormat(provider, format, args);
        }

        public virtual bool IsDebugEnabled => _log.IsDebugEnabled;
        public virtual bool IsInfoEnabled => _log.IsInfoEnabled;
        public virtual bool IsWarnEnabled => _log.IsWarnEnabled;
        public virtual bool IsErrorEnabled => _log.IsErrorEnabled;
        public virtual bool IsFatalEnabled => _log.IsFatalEnabled;
    }
}