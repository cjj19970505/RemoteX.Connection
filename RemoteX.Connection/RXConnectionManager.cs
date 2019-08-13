using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemoteX.Connection
{
    public class RXConnectionManager
    {
        public event EventHandler Connected;
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
            }
        }

        private void Listener_ConnectionReceived(object sender, IRXConnection e)
        {
            _RXConnectionList.Add(e);
            if(_RXConnectionList.Count == 1)
            {
                Connected?.Invoke(this, null);
            }
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
