using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorFilesApp.Server
{
    public static class AppLogger
    {
        public static ILoggerFactory LoggerFactory { get; set; }
        public static ILogger<T> CreateLogger<T>()
        {
            return LoggerFactory.CreateLogger<T>();
        }

        public static ILogger CreateLogger(string categoryName)
        {
            return LoggerFactory.CreateLogger(categoryName);
        }

        public static ILogger CreateLogger(Type type)
        {
            return LoggerFactory.CreateLogger(type);
        }
    }
}
