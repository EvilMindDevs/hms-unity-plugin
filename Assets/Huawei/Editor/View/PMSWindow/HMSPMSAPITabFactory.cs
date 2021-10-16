using HmsPlugin;
using HmsPlugin.ConnectAPI.PMSAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


internal class HMSPMSAPITabFactory
{
    public static TabView CreateProductTab(TabBar tabBar)
    {
        var tab = new TabView("Create a Product");
        tabBar.AddTab(tab);
        tab.AddDrawer(new CreateProductEditor());
        return tab;
    }

    public static TabView CreateProductsTab(TabBar tabBar)
    {
        var tab = new TabView("Create Products");
        tabBar.AddTab(tab);
        tab.AddDrawer(new CreateProductsEditor());
        return tab;
    }

    public static TabView AllProductsTab(TabBar tabBar)
    {
        var tab = new TabView("All Products");
        tabBar.AddTab(tab);
        tab.AddDrawer(new AllIAPProductsEditor());
        return tab;
    }

    public static TabView UpdateProductTab(TabBar tabBar, AllIAPProductsEditor.Product product)
    {
        var tab = new TabView("Update Product");
        tabBar.AddTab(tab);
        tab.AddDrawer(new UpdateIAPProductEditor(product));
        return tab;
    }
}

