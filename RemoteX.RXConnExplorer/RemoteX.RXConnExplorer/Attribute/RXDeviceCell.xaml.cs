using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RemoteX.RXConnExplorer.Attribute
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RXDeviceCell : ViewCell
    {
        public RXDeviceCell()
        {
            InitializeComponent();
        }
    }
}