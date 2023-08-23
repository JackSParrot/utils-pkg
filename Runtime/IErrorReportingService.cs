using System;

public interface IErrorReportingService : IDisposable
{
    void Initialize(string userId);
    void SetContextValue(string context, string value);
    void SetBreadcrumb(string breadcrumb);
}