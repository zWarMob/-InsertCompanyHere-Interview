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
        private readonly ConcurrentBag<LogLine> Lines = new ConcurrentBag<LogLine>();

        private StreamWriter _writer;
        private int _curDay;

        private readonly Thread _runThread;
        private bool _exit;


        private void MainLoop()
        {
            while (!this._exit)
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
                        this._writer.Write(logLine);

                        // slow down the logger so it collects unlogged lines in memory
                        //Moved elsewhere. Some initialization on the logger with options can be set. Good for tests
                        //Thread.Sleep(500); 
                    }
                }
            }
        }

        /// <summary>
        /// update 
        /// </summary>
        private void CreateNewStreamWriter()
        {
            _curDay = DateTime.Now.Day;

            this._writer = File.AppendText(@"C:\LogTest\Log" + DateTime.Now.ToString("yyyyMMdd HHmmss fff") + ".log");

            this._writer.Write("Timestamp".PadRight(LogLine.timestampFormat.Length, ' ') + "\t" + "Data" + Environment.NewLine);

            this._writer.AutoFlush = true;
        }

        public FileLogger()
        {
            if (!Directory.Exists(@"C:\LogTest"))
                Directory.CreateDirectory(@"C:\LogTest");

            CreateNewStreamWriter();

            this._runThread = new Thread(this.MainLoop);
            this._runThread.Start();
        }

        public void Stop_Without_Flush()
        {
            this._exit = true;
        }

        public async void Stop_With_Flush(object sender, EventArgs e)
        {
            await Stop_With_Flush();
        }

        public Task Stop_With_Flush()
        {
            return Task.Run(() =>
            {
                while (Lines.Any())
                    Thread.Sleep(500);
                this._exit = true;
            });
        }

        public void WriteLog(string s)
        {
            this.Lines.Add(new LogLine(s));
        }
    }
}