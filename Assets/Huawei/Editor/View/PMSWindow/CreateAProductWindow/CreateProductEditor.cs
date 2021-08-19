using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace HmsPlugin.ConnectAPI.PMSAPI
{
    public class CreateProductEditor : VerticalSequenceDrawer
    {
        private TextField.TextField productNoTextField;
        private TextField.TextField productNameTextField;
        private Dropdown.StringDropdown purchaseTypeDropdown;
        private Toggle.Toggle statusToggle;
        private Label.Label currencyLabel;
        private Dropdown.StringDropdown countryDropdown;
        private Dropdown.StringDropdown defaultLocaleDropdown;
        private TextField.TextField defaultPriceTextField;
        private TextField.TextField descriptionTextField;
        private TextArea.TextArea jsonField;

        private LanguagesFoldoutEditor languagesFoldout;
        private string[] purchaseTypes = { "Consumable", "Non_Consumable", "Auto_Subscription" };

        public List<HMSEditorUtils.CountryInfo> countryInfos;
        private Dictionary<string, string> supportedLanguages;

        private HMSEditorUtils.CountryInfo selectedCountry;
        private int selectedPurchaseType;
        private string selectedLocale;
        private CreateProductReqJson jsonClass;

        public CreateProductEditor()
        {
            supportedLanguages = HMSEditorUtils.SupportedLanguages();
            countryInfos = HMSEditorUtils.SupportedCountries();


            productNoTextField = new TextField.TextField("Product Id:", "");
            productNameTextField = new TextField.TextField("Product Name:", "");
            descriptionTextField = new TextField.TextField("Description:", "");
            purchaseTypeDropdown = new Dropdown.StringDropdown(purchaseTypes, 0, "Purchase Type:", OnPurchaseTypeChanged);
            statusToggle = new Toggle.Toggle("Status(Active/Inactive):");
            currencyLabel = new Label.Label();
            countryDropdown = new Dropdown.StringDropdown(countryInfos.Select(c => c.Country).ToArray(), 0, "Country", OnCountrySelected);
            defaultLocaleDropdown = new Dropdown.StringDropdown(supportedLanguages.Keys.ToArray(), 14, "Default Language", OnLanguageSelected);
            defaultPriceTextField = new TextField.TextField("Price:", "");
            jsonField = new TextArea.TextArea("").SetFieldHeight(300);

            OnCountrySelected(0);
            OnLanguageSelected(14);
            languagesFoldout = new LanguagesFoldoutEditor();

            AddDrawer(new HorizontalLine());
            AddDrawer(productNoTextField);
            AddDrawer(new Space(5));
            AddDrawer(productNameTextField);
            AddDrawer(new Space(5));
            AddDrawer(descriptionTextField);
            AddDrawer(new Space(5));
            AddDrawer(purchaseTypeDropdown);
            AddDrawer(new Space(5));
            AddDrawer(statusToggle);
            AddDrawer(new Space(5));
            AddDrawer(defaultLocaleDropdown);
            AddDrawer(new Space(5));
            AddDrawer(countryDropdown);
            AddDrawer(new Space(5));
            AddDrawer(defaultPriceTextField);
            AddDrawer(new Space(5));
            AddDrawer(new HorizontalSequenceDrawer(new Label.Label("Currency:"), new Space(92), currencyLabel));
            AddDrawer(new Space(5));
            AddDrawer(languagesFoldout);

            AddDrawer(new Space(15));
            AddDrawer(new HorizontalSequenceDrawer(new Spacer(), new Button.Button("Generate JSON", OnGenerateJsonClick).SetWidth(300).SetBGColor(Color.yellow), new Spacer()));
            AddDrawer(new HorizontalLine());

            AddDrawer(jsonField);
            AddDrawer(new Space(300));
            AddDrawer(new HorizontalSequenceDrawer(new Spacer(), new Button.Button("Create Product", OnCreateProductClick).SetWidth(300).SetBGColor(Color.green), new Spacer()));
        }

        private void OnCountrySelected(int index)
        {
            selectedCountry = countryInfos[index];
            currencyLabel.SetText(selectedCountry.Currency);
        }

        private void OnLanguageSelected(int index)
        {
            var selectedLanguage = supportedLanguages.ElementAt(index);
            selectedLocale = selectedLanguage.Value;
        }

        private void OnPurchaseTypeChanged(int selectedIndex)
        {
            selectedPurchaseType = selectedIndex;
        }

        private void OnGenerateJsonClick()
        {
            GenerateJsonClass();
        }

        private void GenerateJsonClass()
        {
            jsonClass = new CreateProductReqJson();
            jsonClass.requestId = Guid.NewGuid().ToString();
            jsonClass.product = new ProductInfo();
            jsonClass.product.productNo = productNoTextField.GetCurrentText();
            jsonClass.product.appId = HMSEditorUtils.GetAGConnectConfig().client.app_id;
            jsonClass.product.productName = productNameTextField.GetCurrentText();
            jsonClass.product.purchaseType = purchaseTypes[selectedPurchaseType].ToLower();
            jsonClass.product.status = statusToggle.IsChecked() ? "active" : "inactive";
            jsonClass.product.currency = currencyLabel.GetText();
            jsonClass.product.country = selectedCountry.Region;
            jsonClass.product.defaultLocale = selectedLocale;
            jsonClass.product.productDesc = descriptionTextField.GetCurrentText();
            jsonClass.product.defaultPrice = (double.Parse(defaultPriceTextField.GetCurrentText()) * 100).ToString();

            jsonField.SetCurrentText(EditorJsonUtility.ToJson(jsonClass, true));
        }

        private async void OnCreateProductClick()
        {
            if (jsonClass != null)
            {
                GenerateJsonClass();
            }

            var token = await HMSWebUtils.GetAccessTokenAsync();
            HMSWebRequestHelper.PostRequest("https://connect-api.cloud.huawei.com/api/pms/product-price-service/v1/manage/product",
                EditorJsonUtility.ToJson(jsonClass),
                new Dictionary<string, string>()
                {
                    {"client_id", HMSConnectAPISettings.Instance.Settings.Get(HMSConnectAPISettings.ClientID) },
                    {"Authorization","Bearer " + token},
                    {"appId",jsonClass.product.appId}
                }, OnCreateProductResponse);
        }

        private void OnCreateProductResponse(UnityWebRequest response)
        {
            var responseJson = JsonUtility.FromJson<CreateProductResJson>(response.downloadHandler.text);

            if (responseJson.error.errorCode == 0) // request was successful.
            {
                Debug.Log("[HMS PMSAPI] Product has been created succesfully.");
                EditorUtility.DisplayDialog("HMS PMS API", "Product has been created succesfully!", "Ok");
            }
            else
            {
                Debug.LogError($"[HMS PMSAPI] Product creation failed. Error Code: {responseJson.error.errorCode}, Error Message: { responseJson.error.errorMsg }.");
            }
        }

        [Serializable]
        private class CreateProductReqJson
        {
            public string requestId;
            public ProductInfo product;
        }

        [Serializable]
        private class CreateProductResJson
        {
            public ErrorResult error;
        }

        [Serializable]
        private class ErrorResult
        {
            public int errorCode;
            public string errorMsg;
        }

        [Serializable]
        private class ProductInfo
        {
            public string productNo;
            public string appId;
            public string productName;
            public string purchaseType;
            public string status;
            public string currency;
            public string country;
            public string defaultLocale;
            public string productDesc;
            public string defaultPrice;
        }
    }
}
