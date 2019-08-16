using RemoteX.Connection.Attribute;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteX.Connection
{
    public class AttServerSimulatorConnection : IRXConnection
    {
        public RXConnectionState ConnectionState { get; private set; }

        public IRXConnectionGroup ConnectionGroup { get; private set; }

        public event EventHandler<RXConnectionState> OnConnectionStateChanged;
        public event EventHandler<RXReceiveMessage> OnReceived;

        public Dictionary<string, byte[]> Attributes { get; }

        public RXDevice RemoteRXDevice { get; set; }

        public AttServerSimulatorConnection(RXDevice remoteDevice, IRXConnectionGroup connectionGroup)
        {
            ConnectionGroup = connectionGroup;
            RemoteRXDevice = remoteDevice;
            Attributes = new Dictionary<string, byte[]>();
            ConnectionState = RXConnectionState.Created;
        }

        public Task ConnectAsync()
        {
            return Task.Run(() =>
            {
                ConnectionState = RXConnectionState.Connected;
                OnConnectionStateChanged?.Invoke(this, ConnectionState);
            });
        }

        public Task SendAsync(RXSendMessage sendMsg)
        {
            return Task.Run(() =>
            {
                OnSend(sendMsg);
            });
        }

        protected virtual void OnSend(RXSendMessage sendMsg)
        {
            var maybeReadRequest = AttributeReadRequest.DecodeFromByteArray(sendMsg.Bytes);
            AttributeReadResult readResult = new AttributeReadResult
            {
                ReadState = AttributeReadState.Error
            };
            if (Attributes.ContainsKey(maybeReadRequest.RequestKey))
            {
                var value = Attributes[maybeReadRequest.RequestKey];
                readResult = new AttributeReadResult
                {
                    RequestGuid = maybeReadRequest.RequestGuid,
                    ReadState = AttributeReadState.Successful,
                    Value = value
                };
                
            }
            else
            {
                readResult = new AttributeReadResult
                {
                    RequestGuid = maybeReadRequest.RequestGuid,
                    ReadState = AttributeReadState.NonExsited
                };
            }
            RXReceiveMessage receiveMessage = new RXReceiveMessage
            {
                RXConnection = this,
                Bytes = readResult.EncodeToBytesArray()
            };
            OnReceived?.Invoke(this, receiveMessage);
        }

        
    }
}
