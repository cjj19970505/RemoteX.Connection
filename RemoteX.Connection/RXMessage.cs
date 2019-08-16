using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX.Connection
{
    public struct RXSendMessage
    {
        public Int16 ChannelCode { get; set; }
        public byte[] Bytes { get; set; }
    }

    public struct RXReceiveMessage
    {
        /// <summary>
        /// Indicates which Connection this Message come from
        /// </summary>
        public IRXConnection RXConnection { get; set; }
        public Int16 ChannelCode { get; set; }
        public byte[] Bytes { get; set; }
    }
}
