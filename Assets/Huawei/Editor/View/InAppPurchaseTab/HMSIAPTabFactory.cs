using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HmsPlugin
{
    internal class HMSIAPTabFactory
    {
        public static TabView CreateTab(string title)
        {
            var tabView = new TabView(title);
            tabView.AddDrawer(new HMSIAPSettingsDrawer());


            return tabView;
        }
    }
}
