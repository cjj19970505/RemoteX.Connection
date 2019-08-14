using RemoteX.Bluetooth;
using RemoteX.Connection;
using RemoteX.Connection.Rfcomm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RemoteX.RXConnExplorer.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RXConnPage : ContentPage
    {
        IBluetoothManager BluetoothManager { get; }
        RXConnectionManager RXConnectionManager { get; }
        public RXConnPage()
        {
            InitializeComponent();
            IManagerManager managerManager = DependencyService.Get<IManagerManager>();
            
            BluetoothManager = managerManager.BluetoothManager;
            RXConnectionManager = managerManager.RXConnectionManager;
            RfcommRXConnectionGroup rfcommConnectionGroup = new RfcommRXConnectionGroup(BluetoothManager);
            RXConnectionManager.AddConnectionGroup(rfcommConnectionGroup);
        }

        private void StartAdvertiseButton_Clicked(object sender, EventArgs e)
        {
            RXConnectionManager.StartListenForConnection();
        }

        private void StartScanButton_Clicked(object sender, EventArgs e)
        {
            
        }
    }
}