using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX.Connection
{
    public interface IRXConnectionGroup
    {
        IRXListener Listener { get; }
        IRXScanner Scanner { get; }
    }
}
