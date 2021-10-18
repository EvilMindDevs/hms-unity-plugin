﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HmsPlugin
{
    public class HMSGameServiceTabFactory
    {
        public static TabView CreateTab(string title)
        {
            var tabView = new TabView(title);
            tabView.AddDrawer(new HMSGameServiceSettingsDrawer());

            return tabView;
        }
    }
}
