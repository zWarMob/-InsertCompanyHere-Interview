namespace LogTest
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class FileLogger : ILogger
    {
        public readonly ConcurrentBag<LogLine> Lines = new ConcurrentBag<LogLine>();

        private string _logPath = @"C:\LogTest";
        private readonly Thread _runThread;
        private StreamWriter _writer;
        private int _curDay;
        private bool _stop;
        private bool _forceStop;


        private void MainLoop()
        {
            while (!this._stop)
            {
                Thread.Sleep(1000); //check every second for any new lines, if we have finished logging

                if (_curDay != DateTime.Now.Day)
                {
                    CreateNewStreamWriter();
                }

                if (this.Lines.Any())
                {
                    while (this.Lines.TryTake(out LogLine logLine))
                    {
                        if (_forceStop)
                            return;

                        this._writer.Write(logLine);
                    }
                }
            }
        }

        /// <summary>
        /// Create a new file with the current timestamp. _writer references the new stream
        /// </summary>
        private void CreateNewStreamWriter()
        {
            _curDay = DateTime.Now.Day;

            var filename = @"Log" + DateTime.Now.ToString("yyyyMMdd HHmmss fff") + ".log";

            this._writer = File.AppendText(Path.Combine(_logPath, filename));

            this._writer.Write("Timestamp".PadRight(LogLine.timestampFormat.Length, ' ') + "\t" + "Data" + Environment.NewLine);

            this._writer.AutoFlush = true;
        }

        public FileLogger()
        {
            if (!Directory.Exists(_logPath))
                Directory.CreateDirectory(_logPath);

            CreateNewStreamWriter();

            this._runThread = new Thread(this.MainLoop);
            this._runThread.Start();
        }

        public void Stop_Without_Flush()
        {
            this._forceStop = true;
        }

        public void Stop_With_Flush(object sender, EventArgs e)
        {
            Stop_With_Flush();
        }

        public void Stop_With_Flush()
        {
            this._stop = true;
        }

        public void WriteLog(string s)
        {
            this.Lines.Add(new LogLine(s));
        }
    }
}