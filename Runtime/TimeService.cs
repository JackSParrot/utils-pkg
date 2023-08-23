using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JackSParrot.Services
{
    [CreateAssetMenu(fileName = "TimeService", menuName = "JackSParrot/Services/TimeService")]
    public class TimeService : AService
    {
        private DateTime _epoch       = new DateTime(1970, 1, 1);
        private float    _lastSeconds = 0f;
        private DateTime _lastDate;

        public void UpdateRealTime()
        {
            _lastSeconds = UnityEngine.Time.time;
            _lastDate = DateTime.Now;
        }

        public DateTime Now
        {
            get
            {
                float now = UnityEngine.Time.time;
                float diff = now - _lastSeconds;
                if (diff > 0f)
                {
                    _lastDate = _lastDate.AddSeconds(diff);
                    _lastSeconds = now;
                }

                return _lastDate;
            }
        }

        public ulong TimestampMillis => (ulong)Now.Subtract(_epoch).TotalMilliseconds;

        public long TimestampSeconds => (long)Now.Subtract(_epoch).TotalSeconds;

        public long DaysFromEpoch => (long)Now.Subtract(_epoch).TotalDays;


        public string FormatTimeShort(int timeSeconds, string separationToken = ":")
        {
            int time = timeSeconds;
            int hours = time / 3600;
            time -= hours * 3600;
            int minutes = time / 60;
            int seconds = time - minutes * 60;
            if (hours > 0)
            {
                return $"{hours.ToString()}{separationToken}{minutes.ToString()}{separationToken}{seconds.ToString()}";
            }

            return $"{minutes.ToString()}{separationToken}{seconds.ToString()}";
        }

        public string FormatTime(int timeSeconds, string daysToken = "d", string hoursToken = "h",
            string minutesToken = "m", string secondsToken = "s")
        {
            int time = timeSeconds;
            int days = timeSeconds / (3600 * 24);
            time -= days * (3600 * 24);
            int hours = time / 3600;
            time -= hours * 3600;
            int minutes = time / 60;
            int seconds = time - minutes * 60;
            if (days > 0)
            {
                return $"{days.ToString()}{daysToken} {hours.ToString()}{hoursToken}";
            }

            if (hours > 0)
            {
                return $"{hours.ToString()}{hoursToken} {minutes.ToString()}{minutesToken}";
            }

            return $"{minutes.ToString()}{minutesToken} {seconds.ToString()}{secondsToken}";
        }

        public string FormatDateEurope(int timestampSeconds)
        {
            DateTime currentDate = _epoch.AddSeconds(timestampSeconds);
            return new System.Text.StringBuilder().Append(currentDate.Day).Append("/").Append(currentDate.Month)
                .Append("/").Append(currentDate.Year).ToString();
        }

        public string FormatDateEurope(DateTime dateTime)
        {
            return new System.Text.StringBuilder().Append(dateTime.Day).Append("/").Append(dateTime.Month).Append("/")
                .Append(dateTime.Year).ToString();
        }

        public override void Cleanup()
        {
            Application.focusChanged -= OnFocusChanged;
            Status = EServiceStatus.NotInitialized;
        }

        public override List<Type> GetDependencies()
        {
            return null;
        }

        public override IEnumerator Initialize()
        {
            UpdateRealTime();
            Application.focusChanged += OnFocusChanged;
            Status = EServiceStatus.Initialized;
            yield return null;
        }

        private void OnFocusChanged(bool focused)
        {
            if (focused)
                UpdateRealTime();
        }
    }
}