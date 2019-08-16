using RemoteX.Connection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RemoteX.RXConnExplorer.Attribute
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AttributeDeviceListPage : ContentPage
    {
        RXConnectionManager RXConnectionManager { get; }
        ObservableCollection<RXDevice> DeviceListSource { get; }
        public AttributeDeviceListPage()
        {
            DeviceListSource = new ObservableCollection<RXDevice>();
            InitializeComponent();
            DeviceListView.ItemsSource = DeviceListSource;
            IManagerManager managerManager = DependencyService.Get<IManagerManager>();
            RXConnectionManager = managerManager.RXConnectionManager;
            foreach(var device in RXConnectionManager.RXDevices)
            {
                DeviceListSource.Add(device);
            }
            
        }

        private async void DeviceListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if(e.SelectedItem == null)
            {
                return;
            }
            var selectedDevice = e.SelectedItem as RXDevice;
            (sender as ListView).SelectedItem = null;
            await Navigation.PushAsync(new AttClientPage(selectedDevice));
            
        }
    }
}