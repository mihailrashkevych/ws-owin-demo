using System;
using System.Collections.Generic;
using System.IO;

namespace WsOwinDemo.Logging
{
    public class LogWriter
    {
        private static Queue<Log> _logQueue;
        private static string _logPath;
        private static int _maxLogAge;
        private static int _queueSize;
        protected static DateTime LastFlushed = DateTime.Now;

       public LogWriter(string logPath, int maxLogAge = 300, int queueSize = 2)
        {
            _logPath = logPath;
            _maxLogAge = maxLogAge;
            _queueSize = queueSize;
            _logQueue = new Queue<Log>();
        }


        public void WriteToLog(string message)
        {
            lock (_logQueue)
            {
                var logEntry = new Log(message);
                _logQueue.Enqueue(logEntry);

                if (_logQueue.Count >= _queueSize || DoPeriodicFlush())
                {
                    FlushLog();
                }
            }
        }

        private bool DoPeriodicFlush()
        {
            var logAge = DateTime.Now - LastFlushed;
            if (logAge.TotalSeconds >= _maxLogAge)
            {
                LastFlushed = DateTime.Now;
                return true;
            }
            return false;
        }

        private void FlushLog()
        {
            while (_logQueue.Count > 0)
            {
                Log entry = _logQueue.Dequeue();
                string logPath = entry.LogDate + "_" + _logPath;

                using (var fs = File.Open(logPath, FileMode.Append, FileAccess.Write))
                {
                    using (var log = new StreamWriter(fs))
                    {
                        log.WriteLine($"{entry.LogTime}\t{entry.Message}");
                    }
                }
            }
        }
    }

    public class Log
    {
        public string Message { get; set; }
        public string LogTime { get; set; }
        public string LogDate { get; set; }

        public Log(string message)
        {
            Message = message;
            LogDate = DateTime.Now.ToString("yyyy-MM-dd");
            LogTime = DateTime.Now.ToString("hh:mm:ss.fff tt");
        }
    }
}