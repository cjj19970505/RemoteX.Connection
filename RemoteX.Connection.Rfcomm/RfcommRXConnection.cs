using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RemoteX.Bluetooth;
using RemoteX.Bluetooth.Rfcomm;

namespace RemoteX.Connection.Rfcomm
{
    public class RfcommRXConnection : IRXConnection
    {
        public RXConnectionState ConnectionState { get; private set; }

        /// <summary>
        /// If the host is client, DeviceService indicates the Rfcomm Service on the server(Remote Device)
        /// </summary>
        public IRfcommDeviceService DeviceService { get; }
        /// <summary>
        /// If the host is server, ServiceProvider indicates the Rfcomm Service on the server(Remote Device)
        /// </summary>
        public IRfcommConnection RfcommConnection { get; private set; }
        public IRXConnectionGroup ConnectionGroup { get; }

        public RXDevice RemoteRXDevice => throw new NotImplementedException();

        Task _ReadInputStreamTask;

        /// <summary>
        /// 这个构造器是Client用的
        /// </summary>
        /// <param name="deviceService"></param>
        internal RfcommRXConnection(IRfcommDeviceService deviceService, IRXConnectionGroup connectionGroup)
        {
            DeviceService = deviceService;
            ConnectionState = RXConnectionState.Created;
            ConnectionGroup = connectionGroup;
        }

        /// <summary>
        /// 这个构造器是Server用的
        /// Server接收到连接后就是连上了，所以server直接调用一个Connect就可以连接上
        /// </summary>
        /// <param name="rfcommConnection"></param>
        internal RfcommRXConnection(IRfcommConnection rfcommConnection, IRXConnectionGroup connectionGroup)
        {
            RfcommConnection = rfcommConnection;
            ConnectionState = RXConnectionState.Created;
            ConnectionGroup = connectionGroup;
        }

        public event EventHandler<RXConnectionState> OnConnectionStateChanged;
        public event EventHandler<RXReceiveMessage> OnReceived;

        public async Task ConnectAsync()
        {
            ConnectionState = RXConnectionState.Connecting;
            OnConnectionStateChanged?.Invoke(this, ConnectionState);
            //Host is client
            if (DeviceService != null)
            {
                try
                {
                    await DeviceService.ConnectAsync();
                    RfcommConnection = DeviceService.RfcommConnection;
                    _ReadInputStreamTask = _ReadInputStreamTaskAsync();
                    ConnectionState = RXConnectionState.Connected;
                    OnConnectionStateChanged?.Invoke(this, ConnectionState);

                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e);
                    ConnectionState = RXConnectionState.Destoryed;
                    OnConnectionStateChanged?.Invoke(this, ConnectionState);
                }
            }
            else
            {
                /*Host is Server*/
                ConnectionState = RXConnectionState.Connected;
                _ReadInputStreamTask = _ReadInputStreamTaskAsync();
                OnConnectionStateChanged?.Invoke(this, ConnectionState);
            }
        }

        private Task _ReadInputStreamTaskAsync()
        {
            return Task.Run(() =>
            {

                var inputStream = RfcommConnection.InputStream;
                bool disconnected = false;
                while (true)
                {
                    byte[] buffer = new byte[255];
                    var readSize = inputStream.Read(buffer, 0, buffer.Length);
                    if (readSize == 0)
                    {
                        disconnected = true;
                        break;
                    }
                    var readBytes = new byte[readSize];
                    Array.Copy(buffer, 0, readBytes, 0, readSize);
                    var msg = new RXReceiveMessage
                    {
                        RXConnection = this,
                        Bytes = readBytes
                    };
                    OnReceived?.Invoke(this, msg);
                }
                if (disconnected)
                {
                    RfcommConnection.Dispose();
                    ConnectionState = RXConnectionState.Destoryed;
                    OnConnectionStateChanged?.Invoke(this, ConnectionState);
                }
            });
        }

        public async Task SendAsync(RXSendMessage sendMsg)
        {
            var sendByteList = new List<byte>();
            sendByteList.AddRange(BitConverter.GetBytes(sendMsg.ChannelCode));
            sendByteList.AddRange(sendMsg.Bytes);

            await RfcommConnection.OutputStream.WriteAsync(sendByteList.ToArray(), 0, sendByteList.Count);
            await RfcommConnection.OutputStream.FlushAsync();
        }
    }
}
