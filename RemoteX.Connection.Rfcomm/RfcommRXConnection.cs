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

        Task _ReadInputStreamTask;

        /// <summary>
        /// 这个构造器是Client用的
        /// </summary>
        /// <param name="deviceService"></param>
        internal RfcommRXConnection(IRfcommDeviceService deviceService)
        {
            DeviceService = deviceService;
            ConnectionState = RXConnectionState.Created;
        }
        
        /// <summary>
        /// 这个构造器是Server用的
        /// Server接收到连接后就是连上了，所以server直接调用一个Connect就可以连接上
        /// </summary>
        /// <param name="rfcommConnection"></param>
        internal RfcommRXConnection(IRfcommConnection rfcommConnection)
        {
            RfcommConnection = rfcommConnection;
            ConnectionState = RXConnectionState.Created;
        }

        public event EventHandler<RXConnectionState> OnConnectionStateChanged;
        public event EventHandler<RXMessage> OnReceived;

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
                while (true)
                {
                    byte[] buffer = new byte[255];
                    try
                    {
                        var readSize = inputStream.Read(buffer, 0, buffer.Length);
                        var readBytes = new byte[readSize];
                        Array.Copy(buffer, 0, readBytes, 0, readSize);
                        var msg = new RXMessage { Bytes = readBytes };
                        OnReceived?.Invoke(this, msg);
                    }
                    catch(Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("fuck");
                    }
                }
            });
        }

        public async Task SendAsync(byte[] sendBytes)
        {
            await RfcommConnection.OutputStream.WriteAsync(sendBytes, 0, sendBytes.Length);
            await RfcommConnection.OutputStream.FlushAsync();
        }
    }
}
