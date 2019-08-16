using RemoteX.Bluetooth;
using RemoteX.Connection;
using RemoteX.Connection.Rfcomm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        ObservableCollection<string> ReceiveMessageList;
        IBluetoothManager BluetoothManager { get; }
        RXConnectionManager RXConnectionManager { get; }
        public RXConnPage()
        {
            ReceiveMessageList = new ObservableCollection<string>();
            InitializeComponent();
            ReceiveListView.ItemsSource = ReceiveMessageList;
            IManagerManager managerManager = DependencyService.Get<IManagerManager>();

            BluetoothManager = managerManager.BluetoothManager;
            RXConnectionManager = managerManager.RXConnectionManager;

            RfcommRXConnectionGroup rfcommConnectionGroup = new RfcommRXConnectionGroup(BluetoothManager);
            RXConnectionManager.AddConnectionGroup(rfcommConnectionGroup);
            RXConnectionManager.OnReceived += RXConnectionManager_OnReceived;
        }

        private void RXConnectionManager_OnReceived(object sender, RXReceiveMessage e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                ReceiveMessageList.Add(Encoding.UTF8.GetString(e.Bytes));
            });

        }

        private void StartAdvertiseButton_Clicked(object sender, EventArgs e)
        {
            RXConnectionManager.StartListenForConnection();
        }

        private void StartScanButton_Clicked(object sender, EventArgs e)
        {
            RXConnectionManager.StartScan();
        }

        private async void SendButton_Clicked(object sender, EventArgs e)
        {
            if (SendEntry.Text == null)
            {
                return;
            }
            await RXConnectionManager.SendAsync(new RXSendMessage { Bytes = Encoding.UTF8.GetBytes(SendEntry.Text) });
        }
    }
}