using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX.Connection
{
    public class RXDevice
    {
        public string DeviceName { get; internal set; }
        public string DeviceId { get; internal set; }

        internal RXDevice(string deviceName, string deviceId)
        {
            
        }
    }
}
