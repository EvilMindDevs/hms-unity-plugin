using HmsPlugin;
using HmsPlugin.ConnectAPI.PMSAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


internal class HMSPMSAPITabFactory
{
    public static TabView CreateCreateProductTab(TabBar tabBar)
    {
        var tab = new TabView("Create a Product");
        tabBar.AddTab(tab);

        //tab.AddDrawer(new HorizontalLine());
        tab.AddDrawer(new CreateProductEditor());
        //tab.AddDrawer(new HorizontalLine());
        tab.AddDrawer(new Spacer());
        return tab;
    }
}

