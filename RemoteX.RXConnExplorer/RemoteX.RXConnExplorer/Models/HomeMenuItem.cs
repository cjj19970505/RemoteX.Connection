using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX.RXConnExplorer.Models
{
    public enum MenuItemType
    {
        Browse,
        About,
        RfcommSetting
    }
    public class HomeMenuItem
    {
        public MenuItemType Id { get; set; }

        public string Title { get; set; }
    }
}
