﻿namespace JackSParrot.Utils
{
    public enum LogLevel
    {
        Debug,
        Warning,
        Error
    };

    public interface ICustomLogger
    {
        void SetLogLevel(LogLevel level);
        void LogDebug(string message);
        void LogError(string message);
        void LogWarning(string message);
    }
}