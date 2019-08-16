using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX.Connection
{
    public class LocalConnectionGroup : IRXConnectionGroup
    {
        public RXConnectionManager ConnectionManager { get; }
        public IRXListener Listener { get; }

        public IRXScanner Scanner { get; }
        public LocalConnectionGroup(RXConnectionManager connectionManager)
        {
            ConnectionManager = connectionManager;
            Listener = new LocalConnectionListener(this);
            Scanner = new LocalConnectionScanner(this);
        }
        
    }

    public class LocalConnectionListener : IRXListener
    {
        public IRXConnectionGroup ConnectionGroup { get; }

        public event EventHandler<IRXConnection> ConnectionReceived;

        public LocalConnectionListener(IRXConnectionGroup connectionGroup)
        {
            ConnectionGroup = connectionGroup;
            
        }

        public void Start()
        {
            
        }
    }

    public class LocalConnectionScanner : IRXScanner
    {
        public RXScannerStatus Status { get; private set; }

        public IRXConnectionGroup ConnectionGroup { get; }

        public event EventHandler<IRXConnection> OnConnectionReceived;

        public List<AttServerSimulatorConnection> _ConnectionList;

        public LocalConnectionScanner(IRXConnectionGroup connectionGroup)
        {
            _ConnectionList = new List<AttServerSimulatorConnection>();
            ConnectionGroup = connectionGroup;
            Status = RXScannerStatus.Created;
            
        }
        public void Start()
        {
            Status = RXScannerStatus.Started;
            foreach(var connection in _ConnectionList)
            {
                OnConnectionReceived?.Invoke(this, connection);
                connection.ConnectAsync();
            }
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void AddConnection(Dictionary<string, byte[]> connectionMetadata)
        {
            var deviceName = Encoding.UTF8.GetString(connectionMetadata["DeviceName"]);
            var deviceId = new Guid(connectionMetadata["DeviceId"]);
            var rxDevice = new RXDevice(ConnectionGroup.ConnectionManager, deviceName, deviceId);
            AttServerSimulatorConnection connection = new AttServerSimulatorConnection(rxDevice, this.ConnectionGroup);
            foreach(var pair in connectionMetadata)
            {
                connection.Attributes.Add(pair.Key, pair.Value);
            }
            _ConnectionList.Add(connection);
            if(Status == RXScannerStatus.Started)
            {
                connection.ConnectAsync();
            }
        }
    }
}
