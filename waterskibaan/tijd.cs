using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace groenschermfrom
{
    public class tijd
    {
        public DateTime bookingStart;
        public DateTime bookingEnd;

       
        public void timer()
        {
           /* DateTime dateNow = DateTime.Now;
            DateTime dateThen = dateNow.AddHours(1);
            bookingStart = dateNow;
            bookingEnd = dateThen;
           */
        }
        public static int get_time_left() 
        {
            
            DateTime dateNow = new DateTime(2020, 12, 15, 11, 00, 0);
            DateTime dateOnPoint = DateTime.Now;
            DateTime dateThen = dateOnPoint.AddHours(1);
            int timeLeft = 0;
            if (dateThen != dateNow.AddHours(1))
            {
              TimeSpan dateTimeMid = dateThen.Subtract(dateNow);
              timeLeft = dateTimeMid.Minutes;
            }
            return timeLeft;
        }

        public int timeLeft(DateTime start, DateTime end)
        {
            DateTime now = DateTime.Now;
            int test = now.CompareTo(end);
            return test;
        }

    }
}
