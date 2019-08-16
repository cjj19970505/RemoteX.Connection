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
    public partial class AttClientPage : ContentPage
    {
        public RXDevice RXDevice { get; }
        ObservableCollection<string> AttributeItemListSource;
        public AttClientPage(RXDevice rxDevice)
        {
            AttributeItemListSource = new ObservableCollection<string>();
            RXDevice = rxDevice;
            InitializeComponent();
            BindingContext = RXDevice;
            AttributeItemListView.ItemsSource = AttributeItemListSource;
        }

        private async void FetchButton_Clicked(object sender, EventArgs e)
        {
            var name = await RXDevice.AttributrClient.ReadAsync("DeviceName");
            var id = await RXDevice.AttributrClient.ReadAsync("DeviceId");
            Device.BeginInvokeOnMainThread(() =>
            {
                AttributeItemListSource.Add(Encoding.UTF8.GetString(name.Value));
                AttributeItemListSource.Add(new Guid(id.Value).ToString());
            });
            

        }
    }
}