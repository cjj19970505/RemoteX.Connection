using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemoteX.Connection
{
    public class RXConnectionManager
    {
        public RXDevice LocalRXDevice { get; }
        List<IRXConnectionGroup> _RXConnectionGroupList;
        List<IRXConnection> _RXConnectionList;
        public event EventHandler<RXReceiveMessage> OnReceived;
        public RXDevice[] RXDevices
        {
            get
            {
                var deviceList = new List<RXDevice>();
                foreach(var connection in _RXConnectionList)
                {
                    if(!deviceList.Contains(connection.RemoteRXDevice))
                    {
                        deviceList.Add(connection.RemoteRXDevice);
                    }
                }
                return deviceList.ToArray();
            }
        }

        public IRXConnection[] Connections
        {
            get
            {
                return _RXConnectionList.ToArray();
            }
        }
        
        public RXConnectionManager()
        {
            LocalRXDevice = new RXDevice(this, Environment.MachineName, Guid.NewGuid());
            _RXConnectionGroupList = new List<IRXConnectionGroup>();
            _RXConnectionList = new List<IRXConnection>();
        }

        public void AddConnectionGroup(IRXConnectionGroup connectionGroup)
        {
            _RXConnectionGroupList.Add(connectionGroup);
        }

        public void StartListenForConnection()
        {
            foreach(var connectionGroup in _RXConnectionGroupList)
            {
                connectionGroup.Listener.ConnectionReceived += Listener_ConnectionReceived;
                
            }
            foreach(var connectionGroup in _RXConnectionGroupList)
            {
                connectionGroup.Listener.Start();
            }
        }

        public void StartScan()
        {
            foreach(var connectionGroup in _RXConnectionGroupList)
            {
                connectionGroup.Scanner.OnConnectionReceived += Scanner_OnConnectionReceived;
            }
            foreach(var connectionGroup in _RXConnectionGroupList)
            {
                connectionGroup.Scanner.Start();
            }
        }

        private async void Scanner_OnConnectionReceived(object sender, IRXConnection e)
        {
            _RXConnectionList.Add(e);
            var connection = e;
            connection.OnReceived += Connection_OnReceived;
            connection.OnConnectionStateChanged += Connection_OnConnectionStateChanged;
            await e.ConnectAsync();
        }
        private async void Listener_ConnectionReceived(object sender, IRXConnection e)
        {
            _RXConnectionList.Add(e);
            var connection = e;
            connection.OnReceived += Connection_OnReceived;
            connection.OnConnectionStateChanged += Connection_OnConnectionStateChanged;
            await e.ConnectAsync();
        }

        private void Connection_OnConnectionStateChanged(object sender, RXConnectionState e)
        {
            var connection = sender as IRXConnection;
            if(e == RXConnectionState.Destoryed)
            {
                _RXConnectionList.Remove(connection);
                var connectionGroup = connection.ConnectionGroup;
                if(connectionGroup.Scanner.Status == RXScannerStatus.Aborted || connectionGroup.Scanner.Status == RXScannerStatus.Stopped)
                {
                    connectionGroup.Scanner.Start();
                }
            }
        }

        
        private void Connection_OnReceived(object sender, RXReceiveMessage e)
        {
            OnReceived?.Invoke(this, e);
        }
        public async Task SendAsync(RXSendMessage rxMessage)
        {
            var rxConnection = _SelectConnection(_RXConnectionList.ToArray(), rxMessage);
            if(rxConnection == null)
            {
                System.Diagnostics.Debug.WriteLine("NO CONNECTION, SEND SHIT");
            }
            else
            {
                await rxConnection.SendAsync(rxMessage);
            }
        }

        public async Task SendAsync(RXDevice rxDevice, RXSendMessage rxMessage)
        {
            
        }

        public IRXConnection _SelectConnection(IRXConnection[] preFilteredConnection, RXSendMessage message)
        {
            if(preFilteredConnection.Length == 0)
            {
                
                return null;
            }
            IRXConnection connection = null;
            foreach(var conn in preFilteredConnection)
            {
                if(conn.ConnectionGroup is LocalConnectionGroup)
                {
                    if(message.ChannelCode != 0)
                    {
                        continue;
                    }
                    
                }
                connection = conn;
                break;
            }
            return connection;
        }
    }
}
