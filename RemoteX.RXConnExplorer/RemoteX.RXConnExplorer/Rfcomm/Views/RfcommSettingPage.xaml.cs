using RemoteX.Bluetooth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RemoteX.RXConnExplorer.Rfcomm.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RfcommSettingPage : ContentPage
    {
        IBluetoothManager BluetoothManager { get; }
        public RfcommSettingPage()
        {
            InitializeComponent();
            IManagerManager managerManager = DependencyService.Get<IManagerManager>();
            
        }
    }
}