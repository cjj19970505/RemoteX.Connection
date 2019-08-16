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

        public RXConnectionManager ConnectionManager { get; }

        public RfcommRXConnectionGroup(IBluetoothManager bluetoothManager, RXConnectionManager connectionManager)
        {
            BluetoothManager = bluetoothManager;
            Listener = new RfcommAdvertiseRXListener(this);
            //Scanner = new RfcommRXScanner(this);
            Scanner = new RfcommFromAttRXScanner(this);
            ConnectionManager = connectionManager;


        }
    }
}
