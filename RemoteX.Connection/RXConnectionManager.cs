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
            await e.ConnectAsync();
        }

        private async void Listener_ConnectionReceived(object sender, IRXConnection e)
        {
            _RXConnectionList.Add(e);
            await e.ConnectAsync();
        }

        public async Task SendAsync(int channelCode, byte[] sendBytes)
        {
            var rxConnection = _SelectConnection(channelCode);
            await rxConnection.SendAsync(sendBytes);
        }

        public IRXConnection _SelectConnection(int channelCode)
        {
            return _RXConnectionList[0];
        }
    }
}
