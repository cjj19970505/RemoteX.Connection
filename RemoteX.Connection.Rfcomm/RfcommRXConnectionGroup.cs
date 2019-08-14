using RemoteX.Bluetooth;
using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX.Connection.Rfcomm
{
    public class RfcommRXConnectionGroup : IRXConnectionGroup
    {
        public IRXListener Listener { get; }

        public IRXScanner Scanner { get; }

        public IBluetoothManager BluetoothManager { get; }

        public RfcommRXConnectionGroup(IBluetoothManager bluetoothManager)
        {
            BluetoothManager = bluetoothManager;
            Listener = new RfcommAdvertiseRXListener(this);
            Scanner = new RfcommRXScanner(this);
            
        }
    }
}
