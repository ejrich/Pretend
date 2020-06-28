using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Pretend
{
    public interface ILog<T>
    {
        void Info(string message, params string[] args);
        void Warning(string message, params string[] args);
        void Error(string message, params string[] args);
        void Error(Exception exception, params string[] args);
    }

    public sealed class Log<T> : ILog<T>
    {
        private readonly ILogger _logger;

        public Log(ILogger<T> logger)
        {
            _logger = logger;
        }

        public void Info(string message, params string[] args)
        {
            _logger.LogInformation(message, args);
        }

        public void Warning(string message, params string[] args)
        {
            _logger.LogWarning(message, args);
        }

        public void Error(string message, params string[] args)
        {
            _logger.LogError(message, args);
        }

        public void Error(Exception exception, params string[] args)
        {
            _logger.LogError(exception, exception.Message, args);
        }
    }
}
