using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX.Connection
{
    public class RXDevice
    {
        
        public string DeviceName { get; internal set; }
        public Guid DeviceId { get; internal set; }

        public RXConnectionManager ConnectionManager { get; }

        public RXDevice(RXConnectionManager rxConnectionManager, string deviceName, Guid deviceId)
        {
            ConnectionManager = rxConnectionManager;
            DeviceName = deviceName;
            DeviceId = deviceId;
        }
    }
}
