namespace LogComponent
{
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

        private readonly Thread _runThread;
        private StreamWriter _writer;
        private int _curDay;
        private bool _stop;
        private bool _forceStop;


        private void MainLoop()
        {
            while (!this._stop)
            {
                if (_curDay != DateTime.Now.Day)
                {
                    CreateNewStreamWriter();
                }

                if (this.Lines.Any())
                {
                    while (this.Lines.TryTake(out LogLine logLine))
                    {
                        if (_forceStop)
                        {
                            this._writer.Close();
                            return;
                        }

                        this._writer.Write(logLine);
                    }
                }
            }

            this._writer.Close();
        }

        /// <summary>
        /// Create a new file with the current timestamp. _writer references the new stream
        /// </summary>
        private void CreateNewStreamWriter()
        {
            _curDay = DateTime.Now.Day;

            OpenLogFileName = @"Log" + DateTime.Now.ToString("yyyyMMdd HHmmss fff") + ".log";

            this._writer = File.AppendText(Path.Combine(LogPath, OpenLogFileName));

            this._writer.Write("Timestamp".PadRight(LogLine.timestampFormat.Length, ' ') + "\t" + "Data" + Environment.NewLine);

            this._writer.AutoFlush = true;
        }

        public FileLogger()
        {
            if (!Directory.Exists(LogPath))
                Directory.CreateDirectory(LogPath);

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