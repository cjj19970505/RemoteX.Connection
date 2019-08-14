using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemoteX.Connection
{
    public class RXConnectionManager
    {
        List<IRXConnectionGroup> _RXConnectionGroupList;
        List<IRXConnection> _RXConnectionList;
        public event EventHandler<RXMessage> OnReceived;
        
        public RXConnectionManager()
        {
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
                connectionGroup.Listener.Start();
            }
        }

        public void StartScan()
        {
            foreach(var connectionGroup in _RXConnectionGroupList)
            {
                connectionGroup.Scanner.Start();
                connectionGroup.Scanner.OnConnectionReceived += Scanner_OnConnectionReceived;
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
            
        }

        
        private void Connection_OnReceived(object sender, RXMessage e)
        {
            OnReceived?.Invoke(this, e);
        }
        public async Task SendAsync(RXMessage rxMessage)
        {
            var rxConnection = _SelectConnection(rxMessage);
            await rxConnection.SendAsync(rxMessage.Bytes);
        }

        public IRXConnection _SelectConnection(RXMessage message)
        {
            return _RXConnectionList[0];
        }
    }
}
