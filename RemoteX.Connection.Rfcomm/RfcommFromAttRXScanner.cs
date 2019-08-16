using RemoteX.Bluetooth;
using RemoteX.Bluetooth.Rfcomm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemoteX.Connection.Rfcomm
{
    /// <summary>
    /// This scanner read from attribute and establish a link
    /// </summary>
    public class RfcommFromAttRXScanner : IRXScanner
    {
        public IRXConnectionGroup ConnectionGroup { get; }

        public RXScannerStatus Status { get; private set; }

        public event EventHandler<IRXConnection> OnConnectionReceived;
        private UInt64 TargetMacAddress = 0;
        
        public RfcommFromAttRXScanner(IRXConnectionGroup connectionGroup)
        {
            ConnectionGroup = connectionGroup;
            (ConnectionGroup as RfcommRXConnectionGroup).BluetoothManager.RfcommScanner.Added += RfcommScanner_Added;
            (ConnectionGroup as RfcommRXConnectionGroup).BluetoothManager.RfcommScanner.Stopped += RfcommScanner_Stopped;
        }

        private void RfcommScanner_Stopped(object sender, EventArgs e)
        {
            Status = RXScannerStatus.Stopped;
        }

        private async void RfcommScanner_Added(object sender, IBluetoothDevice e)
        {
            if(e.Address == TargetMacAddress)
            {
                Stop();
                await e.RfcommConnectAsync();
                //var serviceResult = await e.GetRfcommServicesForIdAsync(Constants.ServiceId);
                var serviceResult = await e.GetRfcommServicesAsync();

                IRfcommDeviceService service = null;
                if (serviceResult.Error == BluetoothError.Success && serviceResult.Services.Count > 0)
                {
                    foreach (var ser in serviceResult.Services)
                    {
                        if (ser.ServiceId == Constants.ServiceId)
                        {
                            service = ser;
                            break;
                        }
                    }
                }
                if (service == null)
                {
                    return;
                }
                var connection = new RfcommRXConnection(service, ConnectionGroup);
                OnConnectionReceived?.Invoke(this, connection);
            }
        }

        public void Start()
        {
            Task.Run(async() =>
            {
                foreach (var device in ConnectionGroup.ConnectionManager.RXDevices)
                {
                    var result = await device.AttributrClient.ReadAsync("Rfcomm.A");
                    if(result.ReadState == Attribute.AttributeReadState.Successful)
                    {
                        var macAddress = BitConverter.ToUInt64(result.Value, 0);
                        TargetMacAddress = macAddress;
                        (ConnectionGroup as RfcommRXConnectionGroup).BluetoothManager.RfcommScanner.Start();
                        break;
                    }

                };
            }); 
        }

        public void Stop()
        {
            (ConnectionGroup as RfcommRXConnectionGroup).BluetoothManager.RfcommScanner.Stop();
            Status = RXScannerStatus.Stopping;
        }
    }
}
