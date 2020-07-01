namespace LogComponentCore
{
    using System;
    using System.Text;

    /// <summary>
    /// This is the object that the diff. loggers (filelogger, consolelogger etc.) will operate on. The LineText() method will be called to get the text (formatted) to log
    /// </summary>
    public class LogLine
    {
        #region Private Fields

        #endregion

        #region Constructors

        public LogLine(string text)
        {
            this.Text = text;
            this.Timestamp = DateTime.Now;
        }

        #endregion

        #region Public Methods

        public const string timestampFormat = "yyyy-MM-dd HH:mm:ss:fff";

        public override string ToString()
        {
            return this.ToString(timestampFormat);
        }

        public string ToString(string timestampFormat)
        {
            StringBuilder sb = new StringBuilder(this.Timestamp.ToString(timestampFormat));
            sb.Append("\t");
            sb.Append(this.Text);
            sb.Append(".");
            sb.Append(Environment.NewLine);

            return sb.ToString();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The text to be display in logline
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The Timestamp is initialized when the log is added. Th
        /// </summary>
        public virtual DateTime Timestamp { get; set; }
  

        #endregion
    }
}