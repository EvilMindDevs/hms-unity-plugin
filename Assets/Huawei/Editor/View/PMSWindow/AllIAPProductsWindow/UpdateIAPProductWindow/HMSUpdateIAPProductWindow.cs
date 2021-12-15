using HmsPlugin.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace HmsPlugin.ConnectAPI.PMSAPI
{
    public class HMSUpdateIAPProductWindow : HMSEditorWindow
    {
        private static AllIAPProductsEditor.Product _product;

        public static void ShowWindow(AllIAPProductsEditor.Product product)
        {
            _product = product;
            var window = GetWindow(typeof(HMSUpdateIAPProductWindow), false, "Update Product");
            window.minSize = new UnityEngine.Vector2(400, 700);
            //window.ShowModal();
        }

        public override IDrawer CreateDrawer()
        {
            var tabBar = new TabBar();
            HMSPMSAPITabFactory.UpdateProductTab(tabBar, _product);
            return tabBar;
        }
    }
}
