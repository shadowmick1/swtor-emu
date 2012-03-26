namespace NodeViewer
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public class Log
    {
        protected static Log instance;
        protected List<LogRecord> log = new List<LogRecord>();

        public event LogAddedHandler OnLogAdded;

        public void AddLog(string message)
        {
            this.AddLog(DateTime.Now, message);
        }

        public void AddLog(DateTime time, string message)
        {
            LogRecord item = new LogRecord(time, message);
            lock (this)
            {
                this.log.Add(item);
            }
            if (this.OnLogAdded != null)
            {
                this.OnLogAdded(item);
            }
        }

        public static Log Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Log();
                }
                return instance;
            }
        }

        public List<LogRecord> Records
        {
            get
            {
                return this.log;
            }
        }

        public delegate void LogAddedHandler(Log.LogRecord record);

        public class LogRecord
        {
            public string Message;
            public DateTime Time;

            public LogRecord(DateTime time, string message)
            {
                this.Time = time;
                this.Message = message;
            }
        }
    }
}

