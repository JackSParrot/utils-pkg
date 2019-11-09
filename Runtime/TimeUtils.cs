using System;

namespace JackSParrot.Utils
{
    public static class TimeUtils
    {
        static DateTime _epoch = new DateTime(1970, 1, 1);

        public static DateTime Now
        {
            get
            {
                return DateTime.UtcNow;
            }
        }

        public static uint Timestamp
        {
            get
            {
                return (uint)(Now.Subtract(_epoch).TotalMilliseconds);
            }
        }

        public static string FormatEurope(int timestamp)
        {
            var currentDate = _epoch.AddSeconds(timestamp);
            return new System.Text.StringBuilder().Append(currentDate.Day).Append("/").Append(currentDate.Month).Append("/").Append(currentDate.Year).ToString();
        }

        public static string FormatEurope(DateTime current)
        {
            return new System.Text.StringBuilder().Append(current.Day).Append("/").Append(current.Month).Append("/").Append(current.Year).ToString();
        }
    }
}