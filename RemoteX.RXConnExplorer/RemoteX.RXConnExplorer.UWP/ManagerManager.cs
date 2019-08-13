using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemoteX.Bluetooth;
using RemoteX.Bluetooth.Win10;
using RemoteX.Connection;
using Windows.UI.Core;

[assembly:Xamarin.Forms.Dependency(typeof(RemoteX.RXConnExplorer.UWP.ManagerManager))]
namespace RemoteX.RXConnExplorer.UWP
{
    public class ManagerManager : IManagerManager
    {
        private IBluetoothManager _BluetoothManager;
        private RXConnectionManager _RXConnectionManager;
        public CoreDispatcher Dispatcher { get; set; }
        public IBluetoothManager BluetoothManager
        {
            get
            {
                if(_BluetoothManager == null)
                {
                    if(Dispatcher == null)
                    {
                        throw new Exception("Dispatcher Not Set");
                    }
                }
                _BluetoothManager = new BluetoothManager(Dispatcher);
                return _BluetoothManager;
            }
        }
        public RXConnectionManager RXConnectionManager { get; internal set; }

        
    }
}
