using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemoteX.Connection
{
    public enum RXConnectionState { Created, Connecting, Connected, Error, Closed, Destoryed}
    public interface IRXConnection
    {
        /// <summary>
        /// When a listener or scanner create a RXConnection. the initial value of ConnectionState is Created
        /// ACTION:Connect-->STATE:Connecting
        /// STATE:Connecting-->STATE:Connected/Destoryed
        /// </summary>
        RXConnectionState ConnectionState { get; }
        RXDevice RemoteRXDevice { get; }
        IRXConnectionGroup ConnectionGroup { get; }
        event EventHandler<RXConnectionState> OnConnectionStateChanged;
        event EventHandler<RXReceiveMessage> OnReceived;

        Task SendAsync(RXSendMessage sendMsg);
        Task ConnectAsync();
    }

    public struct RXConnectionInfo
    {
        public bool IsReliable { get; }
    }
}
