using RemoteX.Bluetooth.Rfcomm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemoteX.Connection.Rfcomm
{

    public class RfcommAdvertiseRXListener:IRXListener
    {
        public event EventHandler<IRXConnection> ConnectionReceived;
        public IRXConnectionGroup ConnectionGroup { get; }
        public IRfcommServiceProvider ServiceProvier { get; private set; }

        internal RfcommAdvertiseRXListener(RfcommRXConnectionGroup rfcommConnectionGroup)
        {
            ConnectionGroup = rfcommConnectionGroup;
            Task.Run(async() =>
            {
                ServiceProvier = await (ConnectionGroup as RfcommRXConnectionGroup).BluetoothManager.CreateRfcommServiceProviderAsync(Constants.ServiceId);
                System.Diagnostics.Debug.WriteLine("ServiceProvider");
                ServiceProvier.OnConnectionReceived += ServiceProvier_OnConnectionReceived;
                
            });
            
            
        }

        public void Start()
        {
            ServiceProvier.StartAdvertising();
        }

        private void ServiceProvier_OnConnectionReceived(object sender, IRfcommConnection e)
        {
            RfcommRXConnection rxConnection = new RfcommRXConnection(e, ConnectionGroup);
            ConnectionReceived?.Invoke(this, rxConnection);
        }
    }
}
