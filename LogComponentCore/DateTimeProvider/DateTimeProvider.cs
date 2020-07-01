using System;
using System.Collections.Generic;
using System.Text;

namespace LogComponentCore
{
    public class DateTimeProvider
    {
        public DateTime Now { get { return DateTime.Now - Difference; }}
        public TimeSpan Difference { get; set; }

        public DateTimeProvider() : this(TimeSpan.Zero) { }

        public DateTimeProvider(DateTime dateTimeNow) : this (DateTime.Now - dateTimeNow) { }
        
        public DateTimeProvider(TimeSpan difference)
        {
            Difference = difference;
        }

        public void SetDateTime(DateTime dateTime)
        {
            Difference = DateTime.Now - dateTime;
        }

    }
}
