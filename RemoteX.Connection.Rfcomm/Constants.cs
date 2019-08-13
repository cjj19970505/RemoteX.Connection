using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX.Connection.Rfcomm
{
    internal static class Constants
    {
        public static Guid ServiceId { get; }
        public static string ServiceName { get; }
        static Constants()
        {
            ServiceId = Guid.Parse("4fb996ea-01dc-466c-8b95-9a018c289cef");
            ServiceName = "RemoteX";

        }
    }
}
