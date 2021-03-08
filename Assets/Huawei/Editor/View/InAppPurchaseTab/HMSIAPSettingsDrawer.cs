using HmsPlugin.List;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HmsPlugin
{
    public class HMSIAPSettingsDrawer : VerticalSequenceDrawer
    {
        private Settings _iapSettings;
        private Settings _productListSettings;

        private Foldout _productsFoldout = new Foldout("Product List");

        private Toggle.Toggle _initializeOnStartToggle;

        private IAPProductManipulator _productManipulator;

        public HMSIAPSettingsDrawer()
        {
            _iapSettings = HMSIAPKitSettings.Instance.Settings;
            _productListSettings = HMSIAPProductListSettings.Instance.Settings;
            _productManipulator = new IAPProductManipulator(_productListSettings);
            _initializeOnStartToggle = new Toggle.Toggle("Initialize On Availability*", HMSIAPKitSettings.Instance.Settings.GetBool(HMSIAPKitSettings.InitializeOnStart), OnInitializeOnStartToggle).SetTooltip("Obtains product info after IAP is available.");

            _productManipulator.OnRefreshRequired += OnIAPProductChanged;
            OnIAPProductChanged();
            SetupSequence();
        }

        private void OnInitializeOnStartToggle(bool value)
        {
            HMSIAPKitSettings.Instance.Settings.SetBool(HMSIAPKitSettings.InitializeOnStart, value);
        }

        ~HMSIAPSettingsDrawer()
        {
            _productManipulator.OnRefreshRequired -= OnIAPProductChanged;
            _productManipulator.Dispose();
            _iapSettings.Dispose();
            _productListSettings.Dispose();
        }

        private void OnIAPProductChanged()
        {
            _productsFoldout.RemoveAllDrawers();
            _productsFoldout.AddDrawer(CreateIAPProductListDrawer(_productManipulator.GetAllProducts()));
        }

        private IDrawer CreateIAPProductListDrawer(IEnumerable<IAPProductEntry> products)
        {
            return ListDrawer<IAPProductEntry>.CreateButtonedLabelList(products, s => "Identifier: " + s.Identifier + " | Type: " + s.Type.ToString(), null, new List<Button.ButtonInfo<IAPProductEntry>> { new Button.ButtonInfo<IAPProductEntry>("x", 25, OnRemovePressed) }).SetEmptyDrawer(new Label.Label("No Products Found."));
        }

        private void OnRemovePressed(IAPProductEntry product)
        {
            _productManipulator.RemoveProduct(product);
        }

        private void SetupSequence()
        {
            AddDrawer(new HorizontalSequenceDrawer(new HorizontalLine(), new Label.Label("Product List").SetBold(true), new HorizontalLine()));
            AddDrawer(new Space(3));
            AddDrawer(_productsFoldout);
            AddDrawer(new Space(10));
            AddDrawer(new HMSIAPProductAdderDrawer(_productManipulator));
            AddDrawer(new HorizontalLine());
            AddDrawer(new HorizontalSequenceDrawer(new HorizontalLine(), new Label.Label("Utilities").SetBold(true), new HorizontalLine()));
            AddDrawer(_initializeOnStartToggle);
            AddDrawer(new HorizontalLine());
        }
    }
}