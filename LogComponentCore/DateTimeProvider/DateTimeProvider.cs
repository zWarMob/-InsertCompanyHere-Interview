using System;
using System.Collections.Generic;
using System.Text;

namespace LogComponentCore.DateTimeProvider
{
    public class DateTimeProvider
    {
        public DateTime Now { get { return DateTime.Now - Difference; }}
        public TimeSpan Difference { get; set; }

        public DateTimeProvider(DateTime dateTimeNow) : this (DateTime.Now - dateTimeNow) {}
        
        public DateTimeProvider(TimeSpan difference)
        {
            Difference = difference;
        }
    }
}
