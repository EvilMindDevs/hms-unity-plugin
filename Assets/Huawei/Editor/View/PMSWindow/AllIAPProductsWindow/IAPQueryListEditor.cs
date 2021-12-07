using HmsPlugin.List;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HmsPlugin.ConnectAPI.PMSAPI
{
    public class IAPQueryListEditor : VerticalSequenceDrawer
    {
        private ListDrawer<AllIAPProductsEditor.Product> _listDrawer;
        private List<AllIAPProductsEditor.Product> _productInfo;


        public IAPQueryListEditor(List<AllIAPProductsEditor.Product> products)
        {
            _productInfo = products;
            _listDrawer = new ListDrawer<AllIAPProductsEditor.Product>(_productInfo, CreateList, ListOrientation.Vertical).SetEmptyDrawer(new Label.Label("No products found."));
            AddDrawer(new HorizontalSequenceDrawer(new Spacer(), new Label.Label("Query Result").SetBold(true).SetFontSize(15), new Spacer()));
            AddDrawer(new HorizontalLine());
            //AddDrawer(new Space(5));
            AddDrawer(_listDrawer);
            AddDrawer(new HorizontalLine());
        }

        private IDrawer CreateList(AllIAPProductsEditor.Product item)
        {
            return new HorizontalSequenceDrawer(new Space(20), new Label.Label(item.productNo), new Spacer(), 
                (item.status == "delete") ? 
                new Button.ButtonWithData<AllIAPProductsEditor.Product>("Deleted", OnDeletedItemClick, item).SetWidth(60).SetBGColor(Color.red) : 
                new Button.ButtonWithData<AllIAPProductsEditor.Product>("Edit", OnEditItemClick, item).SetWidth(60).SetBGColor(Color.yellow)); ;
        }

        private void OnEditItemClick(AllIAPProductsEditor.Product item)
        {
            HMSUpdateIAPProductWindow.ShowWindow(item);
        }
        private void OnDeletedItemClick(AllIAPProductsEditor.Product item)
        {
            //TODO: add action for deleted product if PMS API supports restore products feature.
        }
    }
}
