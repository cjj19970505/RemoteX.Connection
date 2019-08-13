using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX.Connection
{
    public interface IRXListener
    {
        event EventHandler<IRXConnection> ConnectionReceived;

        /// <summary>
        /// Get corresponding ConnectionGroup
        /// </summary>
        IRXConnectionGroup ConnectionGroup { get; }
        void Start();
    }
}
