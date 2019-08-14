using RemoteX.Bluetooth;
using RemoteX.Connection;
using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX.RXConnExplorer
{
    public interface IManagerManager
    {
        IBluetoothManager BluetoothManager { get; }
        RXConnectionManager RXConnectionManager { get; }

    }
}
