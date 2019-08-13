using RemoteX.Bluetooth;
using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX.Connection.Rfcomm
{
    public class RfcommRXConnectionGroup : IRXConnectionGroup
    {
        public IRXListener Listener => throw new NotImplementedException();

        public IRXScanner Scanner => throw new NotImplementedException();

        public IBluetoothManager BluetoothManager { get; }

        public RfcommRXConnectionGroup(IBluetoothManager bluetoothManager)
        {
            BluetoothManager = bluetoothManager;
        }
    }
}
