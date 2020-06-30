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

        private readonly Thread _runThread;
        private StreamWriter _writer;
        private bool _exit;




        public FileLogger()
        {
            if (!Directory.Exists(@"C:\LogTest")) 
                Directory.CreateDirectory(@"C:\LogTest");

            this._writer = File.AppendText(@"C:\LogTest\Log" + DateTime.Now.ToString("yyyyMMdd HHmmss fff") + ".log");
            
            this._writer.Write("Timestamp".PadRight(25, ' ') + "\t" + "Data".PadRight(15, ' ') + "\t" + Environment.NewLine);

            this._writer.AutoFlush = true;

            this._runThread = new Thread(this.MainLoop);
            this._runThread.Start();
        }

        private bool _QuitWithFlush = false;


        DateTime _curDate = DateTime.Now;



        private void MainLoop()
        {
            while (!this._exit)
            {
                if (this.Lines.Count > 0)
                {
                    int f = 0;
                    List<LogLine> _handled = new List<LogLine>();

                    foreach (LogLine logLine in this.Lines)
                    {
                        f++;

                        if (f > 5)
                            continue;
                        
                        if (!this._exit || this._QuitWithFlush)
                        {
                            _handled.Add(logLine);

                            StringBuilder stringBuilder = new StringBuilder();

                            if ((DateTime.Now - _curDate).Days != 0)
                            {
                                _curDate = DateTime.Now;

                                this._writer = File.AppendText(@"C:\LogTest\Log" + DateTime.Now.ToString("yyyyMMdd HHmmss fff") + ".log");

                                this._writer.Write("Timestamp".PadRight(25, ' ') + "\t" + "Data".PadRight(15, ' ') + "\t" + Environment.NewLine);

                                stringBuilder.Append(Environment.NewLine);

                                this._writer.Write(stringBuilder.ToString());

                                this._writer.AutoFlush = true;
                            }

                            stringBuilder.Append(logLine.Timestamp.ToString("yyyy-MM-dd HH:mm:ss:fff"));
                            stringBuilder.Append("\t");
                            stringBuilder.Append(logLine.LineText());
                            stringBuilder.Append("\t");

                            stringBuilder.Append(Environment.NewLine);

                            this._writer.Write(stringBuilder.ToString());
                        }
                    }

                    for (int y = 0; y < _handled.Count; y++)
                    {
                        //this.Lines.Remove(_handled[y]);   
                    }

                    if (this._QuitWithFlush == true && this.Lines.Count == 0) 
                        this._exit = true;

                    Thread.Sleep(50);
                }
            }
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
            this._QuitWithFlush = true;

            return Task.Run(() => {
                while (Lines.Any())
                    Thread.Sleep(500);
            });
        }

        public void WriteLog(string s)
        {
            this.Lines.Add(new LogLine() { Text = s, Timestamp = DateTime.Now });
        }
    }
}