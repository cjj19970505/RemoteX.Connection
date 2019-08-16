using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX.Connection
{
    /// <summary>
    /// 完全按照DeviceWatcherStatusl来设计的
    /// </summary>
    public enum RXScannerStatus
    {
        Created = 0,
        Started = 1,
        EnumerationCompleted = 2,
        Stopping = 3,
        Stopped = 4,
        Aborted = 5
    }
    public interface IRXScanner
    {
        IRXConnectionGroup ConnectionGroup { get; }
        event EventHandler<IRXConnection> OnConnectionReceived;
        RXScannerStatus Status { get; }
        void Start();
        void Stop();
    }
}
