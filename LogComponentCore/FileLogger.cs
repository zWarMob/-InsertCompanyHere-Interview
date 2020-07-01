namespace LogComponent
{
    using LogComponentCore.DateTimeProvider;
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Linq;
    using System.Threading;

    public class FileLogger : ILogger
    {
        public readonly ConcurrentBag<LogLine> Lines = new ConcurrentBag<LogLine>();
        public string LogPath = @"C:\LogTest";
        public string OpenLogFileName;

        private DateTimeProvider _dateTimeProvider;
        private readonly Thread _runThread;
        private StreamWriter _writer;
        private int _curDay;
        private bool _stop;
        private bool _forceStop;


        private void MainLoop()
        {
            while (!_stop)
            {
                if (_curDay != _dateTimeProvider.Now.Day)
                {
                    CreateNewStreamWriter();
                }

                if (Lines.Any())
                {
                    while (Lines.TryTake(out LogLine logLine))
                    {
                        if (_forceStop)
                        {
                            _writer.Close();
                            return;
                        }

                        _writer.Write(logLine);
                    }
                }
                else if (_forceStop)
                {
                    _writer.Close();
                    return;
                }
            }

            _writer.Close();
        }

        /// <summary>
        /// Create a new file with the current timestamp. _writer references the new stream
        /// </summary>
        private void CreateNewStreamWriter()
        {
            if (_writer != null)
                _writer.Close();

            _curDay = _dateTimeProvider.Now.Day;

            OpenLogFileName = @"Log" + _dateTimeProvider.Now.ToString("yyyyMMdd HHmmss fff") + ".log";

            _writer = File.AppendText(Path.Combine(LogPath, OpenLogFileName));

            _writer.Write("Timestamp".PadRight(LogLine.timestampFormat.Length, ' ') + "\t" + "Data" + Environment.NewLine);

            _writer.AutoFlush = true;
        }

        public FileLogger(DateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;

            if (!Directory.Exists(LogPath))
                Directory.CreateDirectory(LogPath);

            CreateNewStreamWriter();

            _runThread = new Thread(MainLoop);
            _runThread.Start();
        }
        public FileLogger() : this(new DateTimeProvider(DateTime.Now)) { }

        public void Stop_Without_Flush()
        {
            _forceStop = true;
        }

        public void Stop_With_Flush(object sender, EventArgs e)
        {
            Stop_With_Flush();
        }

        public void Stop_With_Flush()
        {
            _stop = true;
        }

        public void WriteLog(string s)
        {
            Lines.Add(new LogLine(s));
        }
    }
}