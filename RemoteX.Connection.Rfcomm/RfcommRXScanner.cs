﻿using RemoteX.Bluetooth;
using RemoteX.Bluetooth.Rfcomm;
using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX.Connection.Rfcomm
{
    public class RfcommRXScanner : IRXScanner
    {
        public event EventHandler<IRXConnection> OnConnectionReceived;
        RfcommRXConnectionGroup ConnectionGroup { get; }
        public RfcommRXScanner(RfcommRXConnectionGroup connectionGroup)
        {
            ConnectionGroup = connectionGroup;
            ConnectionGroup.BluetoothManager.RfcommScanner.Added += RfcommScanner_Added;
        }

        private async void RfcommScanner_Added(object sender, Bluetooth.IBluetoothDevice e)
        {
            if(e.Address == BluetoothUtils.AddressStringToInt64("DC:53:60:DD:AE:63"))
            {
                ConnectionGroup.BluetoothManager.RfcommScanner.Stop();

                //var serviceResult = await e.GetRfcommServicesAsync();
                await e.RfcommConnectAsync();
                //var serviceResult = await e.GetRfcommServicesForIdAsync(Constants.ServiceId);
                var serviceResult = await e.GetRfcommServicesAsync();

                IRfcommDeviceService service = null;
                if(serviceResult.Error == BluetoothError.Success && serviceResult.Services.Count > 0)
                {
                    foreach(var ser in serviceResult.Services)
                    {
                        if(ser.ServiceId == Constants.ServiceId)
                        {
                            service = ser;
                        }
                    }
                }
                if(service == null)
                {
                    return;
                }
                var connection = new RfcommRXConnection(service);
                OnConnectionReceived?.Invoke(this, connection);
            }
        }

        public void Start()
        {
            ConnectionGroup.BluetoothManager.RfcommScanner.Start();
            
        }
    }
}
