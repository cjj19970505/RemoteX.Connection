using RemoteX.Bluetooth.Rfcomm;
using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX.Connection.Rfcomm
{

    public class RfcommAdvertiseRXListener:IRXListener
    {
        public event EventHandler<IRXConnection> ConnectionReceived;
        public IRXConnectionGroup ConnectionGroup { get; }
        public IRfcommServiceProvider ServiceProvier { get; }

        internal RfcommAdvertiseRXListener(RfcommRXConnectionGroup rfcommConnectionGroup)
        {
            ConnectionGroup = rfcommConnectionGroup;
            var createProviderTask = (ConnectionGroup as RfcommRXConnectionGroup).BluetoothManager.CreateRfcommServiceProviderAsync(Constants.ServiceId);
            createProviderTask.Wait();
            ServiceProvier = createProviderTask.Result;
            ServiceProvier.OnConnectionReceived += ServiceProvier_OnConnectionReceived;
        }

        public void Start()
        {
            ServiceProvier.StartAdvertising();
        }

        private void ServiceProvier_OnConnectionReceived(object sender, IRfcommConnection e)
        {
            RfcommRXConnection rxConnection = new RfcommRXConnection(e);
            ConnectionReceived?.Invoke(this, rxConnection);
        }
    }
}
