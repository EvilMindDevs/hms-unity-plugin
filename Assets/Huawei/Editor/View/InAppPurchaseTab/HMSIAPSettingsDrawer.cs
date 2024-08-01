using HmsPlugin.List;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace HmsPlugin
{
    public class HMSIAPSettingsDrawer : VerticalSequenceDrawer
    {
        private HMSSettings _iapSettings;
        private HMSSettings _productListSettings;

        private Foldout _productsFoldout = new Foldout("Product List");

        private Toggle.Toggle _initializeOnStartToggle, _consumeOnInitToggle;

        private IAPProductManipulator _productManipulator;

        public HMSIAPSettingsDrawer()
        {
            _iapSettings = HMSIAPKitSettings.Instance.Settings;
            _productListSettings = HMSIAPProductListSettings.Instance.Settings;
            _productManipulator = new IAPProductManipulator(_productListSettings);
            _initializeOnStartToggle = new Toggle.Toggle("Initialize On Start*", HMSIAPKitSettings.Instance.Settings.GetBool(HMSIAPKitSettings.InitializeOnStart), OnInitializeOnStartToggle).SetTooltip("Obtains product info in Start function.");
            _consumeOnInitToggle = new Toggle.Toggle("Consume OnInitialize*", HMSIAPKitSettings.Instance.Settings.GetBool(HMSIAPKitSettings.ConsumptionOwnedItemsOnInitialize), OnConsumeToggle)
                .SetTooltip("Consumes product you did not consumed when initializing IAP.\nIt is recomended that disable this and create your own logic for consumption.");

            _productManipulator.OnRefreshRequired += OnIAPProductChanged;
            OnIAPProductChanged();
            SetupSequence();
        }

        private void OnInitializeOnStartToggle(bool value)
        {
            HMSIAPKitSettings.Instance.Settings.SetBool(HMSIAPKitSettings.InitializeOnStart, value);
        }
        private void OnConsumeToggle(bool value)
        {
            HMSIAPKitSettings.Instance.Settings.SetBool(HMSIAPKitSettings.ConsumptionOwnedItemsOnInitialize, value);
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

        private IDrawer CreateIAPProductListDrawer(IEnumerable<HMSIAPProductEntry> products)
        {
            return ListDrawer<HMSIAPProductEntry>.CreateButtonedLabelList(products, s => "Identifier: " + s.Identifier + " | Type: " + s.Type.ToString(), null, new List<Button.ButtonInfo<HMSIAPProductEntry>> { new Button.ButtonInfo<HMSIAPProductEntry>("x", 25, OnRemovePressed) }).SetEmptyDrawer(new Label.Label("No Products Found."));
        }

        private void OnRemovePressed(HMSIAPProductEntry product)
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
            AddDrawer(new Space(10));
            if (_productManipulator.GetProductCount() <= 0)
            {
                AddDrawer(new HelpBox.HelpBox("Example product would be : Identifier = com.yourcompany.yourapp.removeAds, Product Type = Non Consumable", MessageType.Info));
                AddDrawer(new Space(10));
            }

            AddDrawer(new HorizontalSequenceDrawer(new Spacer(), new Button.Button("Import from CSV", ImportFromCSV).SetWidth(150), new Button.Button("Export to CSV", ExportToCSV).SetWidth(150), new Spacer()));
            AddDrawer(new Space(10));

            AddDrawer(new HorizontalSequenceDrawer(new Spacer(), new Button.Button("Clear All Products", ClearAllIAPProducts).SetWidth(250), new Spacer()));
            AddDrawer(new Space(3));
            AddDrawer(new HorizontalSequenceDrawer(new Spacer(), new Button.Button("Create Constant Classes", CreateIAPConstants).SetWidth(250), new Spacer()));
            AddDrawer(new HorizontalLine());
            AddDrawer(new HorizontalSequenceDrawer(new HorizontalLine(), new Label.Label("Utilities").SetBold(true), new HorizontalLine()));
            AddDrawer(_initializeOnStartToggle);
            AddDrawer(_consumeOnInitToggle);
            AddDrawer(new HorizontalSequenceDrawer(new Label.Label("Import Consumables from Google IAP CSV"), new Button.Button("Import", ImportFromGoogle).SetWidth(100)));
            AddDrawer(new HorizontalLine());
        }

        private void ClearAllIAPProducts()
        {
            _productManipulator.ClearAllProducts();
        }

        private void ImportFromCSV()
        {
            try
            {
                string path = EditorUtility.OpenFilePanel("Choose a CSV File", "", "csv");
                if (!string.IsNullOrEmpty(path))
                {
                    using (var reader = new StreamReader(path))
                    {
                        reader.ReadLine();  // Skip header
                        while (!reader.EndOfStream)
                        {
                            var l = reader.ReadLine().Split(',');
                            string identifier = l[0];
                            var type = (HMSIAPProductType)int.Parse(l[1]);
                            _productManipulator.AddProduct(identifier, type);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Debug.LogError($"Error while importing products from CSV: {exception.Message}");
            }
        }

        private void ExportToCSV()
        {
            try
            {
                var exportPath = EditorUtility.SaveFilePanel("Export Product List", "", "HMSIAPProductList", "csv");
                using (var file = new StreamWriter(exportPath))
                {
                    file.WriteLine("Identifier, Type");
                    foreach (var product in _productManipulator.GetAllProducts())
                    {
                        file.WriteLine($"{product.Identifier}, {(int)product.Type}");
                    }
                }
                EditorUtility.DisplayDialog("Exported", "Product list exported to " + exportPath, "OK");
            }
            catch (Exception exception)
            {
                Debug.LogError($"Error while exporting products to CSV: {exception.Message}");
            }
        }

        private void ImportFromGoogle()
        {
            string path = EditorUtility.OpenFilePanel("Choose a CSV File", "", "csv");
            if (!string.IsNullOrEmpty(path))
            {
                using (var reader = new StreamReader(path))
                {
                    string identifier = "";
                    int i = 0;
                    while (!reader.EndOfStream)
                    {
                        i++;
                        identifier = reader.ReadLine().Split(',')[0];
                        if (i == 1) continue;
                        _productManipulator.AddProduct(identifier, HMSIAPProductType.Consumable);
                    }
                }
            }
        }

        private void CreateIAPConstants()
        {
            if (_productListSettings.Keys.Any())
            {
                using (var file = File.CreateText(Application.dataPath + "/Huawei/Scripts/Utils/HMSIAPConstants.cs"))
                {
                    file.WriteLine("public class HMSIAPConstants\n{");
                    for (int i = 0; i < _productListSettings.Keys.Count(); i++)
                    {
                        if (char.IsDigit(Regex.Replace(_productListSettings.Keys.ElementAt(i).Replace(" ", ""), @"[^0-9a-zA-Z_]+", "")[0]))
                        {
                            //Given identifier starts with numeric 
                            file.WriteLine($"\tpublic const string _{Regex.Replace(_productListSettings.Keys.ElementAt(i).Replace(" ", ""), @"[^0-9a-zA-Z_]+", "")} = \"{_productListSettings.Keys.ElementAt(i).Replace("\"", "")}\";");
                        }
                        else
                        {
                            file.WriteLine($"\tpublic const string {Regex.Replace(_productListSettings.Keys.ElementAt(i).Replace(" ", ""), @"[^0-9a-zA-Z_]+", "")} = \"{_productListSettings.Keys.ElementAt(i).Replace("\"", "")}\";");
                        }
                    }
                    file.WriteLine("}");
                }
                AssetDatabase.Refresh();
            }
        }
    }
}
