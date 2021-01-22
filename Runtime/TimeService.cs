using System;

namespace JackSParrot.Utils
{
    public interface ITimeService : IDisposable
    {
        DateTime Now { get; }
        ulong TimestampMillis { get; }
        long TimestampSeconds { get; }
        long DaysFromEpoch { get; }
        string FormatDateEurope(int timestampSeconds);
        string FormatDateEurope(DateTime dateTime);
        string FormatTime(int timeSeconds);
    }

    public class UnityTimeService : ITimeService
    {
        DateTime _epoch = new DateTime(1970, 1, 1);
        float _lastSeconds = 0f;
        DateTime _lastDate;

        public UnityTimeService()
        {
            _lastSeconds = UnityEngine.Time.time;
            _lastDate = DateTime.UtcNow;
        }

        public DateTime Now
        {
            get
            {
                float now = UnityEngine.Time.time;
                float diff = now - _lastSeconds;
                if(diff > 0f)
                {
                    _lastDate = _lastDate.AddSeconds(diff);
                    _lastSeconds = now;
                }
                return _lastDate;
            }
        }

        public ulong TimestampMillis
        {
            get
            {
                return (ulong)Now.Subtract(_epoch).TotalMilliseconds;
            }
        }

        public long TimestampSeconds
        {
            get
            {
                return (long)Now.Subtract(_epoch).TotalSeconds;
            }
        }

        public long DaysFromEpoch
        {
            get
            {
                return (long)Now.Subtract(_epoch).TotalDays;
            }
        }

        public string FormatTime(int timeSeconds)
        {
            int time = timeSeconds;
            int hours = time / 3600;
            time -= timeSeconds - hours * 3600;
            int minutes = time / 60;
            int seconds = time - minutes * 60;
            var stringBuilder = new System.Text.StringBuilder();
            if (hours > 0)
            {
                stringBuilder.Append(hours.ToString()).Append(":");
            }
            stringBuilder.Append(minutes.ToString()).Append(":").Append(seconds);
            return stringBuilder.ToString();
        }

        public string FormatDateEurope(int timestampSeconds)
        {
            var currentDate = _epoch.AddSeconds(timestampSeconds);
            return new System.Text.StringBuilder().Append(currentDate.Day).Append("/").Append(currentDate.Month).Append("/").Append(currentDate.Year).ToString();
        }

        public string FormatDateEurope(DateTime dateTime)
        {
            return new System.Text.StringBuilder().Append(dateTime.Day).Append("/").Append(dateTime.Month).Append("/").Append(dateTime.Year).ToString();
        }

        public void Dispose()
        {

        }
    }
}
