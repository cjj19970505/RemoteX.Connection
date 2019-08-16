using RemoteX.Bluetooth;
using RemoteX.Connection;
using RemoteX.Connection.Rfcomm;
using RemoteX.RXConnExplorer.Attribute;
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

            LocalConnectionGroup localConnectionGroup = new LocalConnectionGroup(RXConnectionManager);
            RXConnectionManager.AddConnectionGroup(localConnectionGroup);

            RfcommRXConnectionGroup rfcommConnectionGroup = new RfcommRXConnectionGroup(BluetoothManager, RXConnectionManager);
            RXConnectionManager.AddConnectionGroup(rfcommConnectionGroup);
            RXConnectionManager.OnReceived += RXConnectionManager_OnReceived;

            Dictionary<string, byte[]> pairs = new Dictionary<string, byte[]>();
            pairs.Add("DeviceName", Encoding.UTF8.GetBytes("MY MACHINE"));
            pairs.Add("DeviceId", Guid.NewGuid().ToByteArray());
            pairs.Add("Rfcomm.N", Encoding.UTF8.GetBytes("XEON-J-LAPTOP-1"));
            pairs.Add("Rfcomm.A", BitConverter.GetBytes(BluetoothUtils.AddressStringToInt64("DC:53:60:DD:AE:63")));
            (localConnectionGroup.Scanner as LocalConnectionScanner).AddConnection(pairs);
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
            await RXConnectionManager.SendAsync(new RXSendMessage { ChannelCode=1, Bytes = Encoding.UTF8.GetBytes(SendEntry.Text) });
        }

        private async void AttDeviceButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AttributeDeviceListPage());
        }
    }
}