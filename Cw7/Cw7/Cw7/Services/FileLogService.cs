using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Cw7.Services
{
    public class FileLogService : ILogService
    {
        private const string LogFile = "Log.txt";
        public void Log(object log)
        {
            File.AppendAllText(LogFile, log + Environment.NewLine);
        }
    }
}
