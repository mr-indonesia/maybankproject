using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Core.Interfaces;

namespace App.Core.Services
{
    public class LoggerService : ILogger
    {
        public void LogInfo(string message) => Console.WriteLine($"INFO: {message}");
        public void LogWarning(string message) => Console.WriteLine($"WARNING: {message}");
        public void LogError(string message) => Console.WriteLine($"ERROR: {message}");
    }
}
