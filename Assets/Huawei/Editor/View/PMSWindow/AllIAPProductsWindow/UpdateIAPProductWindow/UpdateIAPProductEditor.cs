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
    public class UpdateIAPProductEditor : VerticalSequenceDrawer
    {
        private Label.Label productNoLabel;
        private TextField.TextField productNameTextField;
        private TextField.TextField descriptionTextField;
        private Label.Label purchaseTypeLabel;
        private Label.Label subGroupNameLabel;
        private Label.Label subGroupValueLabel;
        private Label.Label subGroupPeriodLabel;
        private Toggle.Toggle statusToggle;
        private Dropdown.StringDropdown defaultLocaleDropdown;
        private Dropdown.StringDropdown countryDropdown;
        private TextField.TextField defaultPriceTextField;
        private Label.Label currencyLabel;
        private TextArea.TextArea jsonField;

        private string selectedLocale;
        private HMSEditorUtils.CountryInfo selectedCountry;
        private List<HMSEditorUtils.CountryInfo> countryInfos;
        private Dictionary<string, string> supportedLanguages;
        private List<CreateProductEditor.SimpleGroupInfo> subGroupList;
        private AllIAPProductsEditor.Product _product;

        public UpdateIAPProductEditor(AllIAPProductsEditor.Product product)
        {
            _product = product;
            var supportedLanguagesUnsorted = HMSEditorUtils.SupportedLanguages();
            supportedLanguages = supportedLanguagesUnsorted.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
            countryInfos = HMSEditorUtils.SupportedCountries();
            countryInfos.Sort((x, y) => x.Country.CompareTo(y.Country));

            productNoLabel = new Label.Label("Product Id:");
            productNameTextField = new TextField.TextField("Product Name:", product.productName);
            descriptionTextField = new TextField.TextField("Description:", product.productDesc);
            purchaseTypeLabel = new Label.Label("Purchase Type:");
            subGroupNameLabel = new Label.Label("Sub Group:");
            subGroupPeriodLabel = new Label.Label("Sub Period:");
            subGroupValueLabel = new Label.Label();
            statusToggle = new Toggle.Toggle("Status(Active/Inactive):", product.status == "active" ? true : false);
            
            var currentLanguage = supportedLanguages.FirstOrDefault(c => c.Value == product.defaultLocale);
            int localeIndex = supportedLanguages.Keys.ToList().IndexOf(currentLanguage.Key);
            defaultLocaleDropdown = new Dropdown.StringDropdown(supportedLanguages.Keys.ToArray(), localeIndex, "Default Language", OnLanguageSelected);
            var currentCountry = countryInfos.FirstOrDefault(c => c.Region == product.country);
            int countryIndex = countryInfos.IndexOf(currentCountry);
            countryDropdown = new Dropdown.StringDropdown(countryInfos.Select(c => c.Country).ToArray(), countryIndex, "Country", OnCountrySelected);
            defaultPriceTextField = new TextField.TextField("Price:", (int.Parse(product.price) / 100f).ToString());
            currencyLabel = new Label.Label(product.currency);

            OnCountrySelected(countryIndex);
            OnLanguageSelected(localeIndex);


            AddDrawer(new HorizontalLine());
            AddDrawer(new HorizontalSequenceDrawer(productNoLabel, new Space(86), new Label.Label(product.productNo)));
            AddDrawer(new Space(5));
            AddDrawer(productNameTextField);
            AddDrawer(new Space(5));
            AddDrawer(descriptionTextField);
            AddDrawer(new Space(5));
            AddDrawer(new HorizontalSequenceDrawer(purchaseTypeLabel, new Space(60), new Label.Label(product.purchaseType)));
            if (product.purchaseType == "auto_subscription")
            {
                AddDrawer(new Space(5));
                AddDrawer(new HorizontalSequenceDrawer(subGroupNameLabel, new Space(86), subGroupValueLabel));
                AddDrawer(new Space(5));
                AddDrawer(new HorizontalSequenceDrawer(subGroupPeriodLabel, new Space(86), new Label.Label(product.numberOfUnits.ToString() + " " + GetSubPeriod(product.periodUnit))));
            }
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

            AddDrawer(new HorizontalSequenceDrawer(new Spacer(), new Button.Button("Update Product", OnUpdateProductClick).SetBGColor(Color.green).SetWidth(300), new Spacer()));
            if (product.purchaseType == "auto_subscription")
                _ = RequestSubGroups();
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

        private async void OnUpdateProductClick()
        {
            var productObj = new CreateProductEditor.ProductInfo();
            productObj.productNo = _product.productNo;
            productObj.appId = HMSEditorUtils.GetAGConnectConfig().client.app_id;
            productObj.productName = productNameTextField.GetCurrentText();
            productObj.purchaseType = _product.purchaseType;
            productObj.status = statusToggle.IsChecked() ? "active" : "inactive";
            productObj.currency = currencyLabel.GetText();
            productObj.country = selectedCountry.Region;
            productObj.defaultLocale = selectedLocale;
            productObj.productDesc = descriptionTextField.GetCurrentText();
            productObj.defaultPrice = (double.Parse(defaultPriceTextField.GetCurrentText()) * 100).ToString();

            if (_product.purchaseType == "auto_subscription")
            {
                productObj.subGroupId = _product.groupId;
                productObj.subPeriod = _product.numberOfUnits;
                productObj.subPeriodUnit = _product.periodUnit;
            }

            UpdateProductReqJson req = new UpdateProductReqJson();
            req.requestId = Guid.NewGuid().ToString();
            req.resource = productObj;

            string jsonValue = JsonUtility.ToJson(req, true);
            jsonValue = jsonValue.Replace("\n        \"languages\": [],", "");
            if (_product.purchaseType != "auto_subscription")
                jsonValue = jsonValue.Replace(",\n        \"subGroupId\": \"\",\n        \"subPeriod\": 0,\n        \"subPeriodUnit\": \"\"\n    ", "\n    ");

            var token = await HMSWebUtils.GetAccessTokenAsync();
            HMSWebRequestHelper.Instance.PutRequest("https://connect-api.cloud.huawei.com/api/pms/product-price-service/v1/manage/product",
                jsonValue,
                new Dictionary<string, string>()
                {
                    {"client_id", HMSConnectAPISettings.Instance.Settings.Get(HMSConnectAPISettings.ClientID) },
                    {"Authorization","Bearer " + token},
                    {"appId",productObj.appId}
                }, OnUpdateProductResponse);
        }

        private void OnUpdateProductResponse(UnityWebRequest response)
        {
            var responseJson = JsonUtility.FromJson<UpdateProductResJson>(response.downloadHandler.text);

            if (responseJson.error.errorCode == 0) // request was successful.
            {
                Debug.Log("[HMS PMSAPI] Product has been updated succesfully.");
                EditorUtility.DisplayDialog("HMS PMS API", "Product has been updated succesfully!", "Ok");
            }
            else
            {
                Debug.LogError($"[HMS PMSAPI] Product update failed. Error Code: {responseJson.error.errorCode}, Error Message: { responseJson.error.errorMsg }.");
            }
        }

        private async Task RequestSubGroups()
        {
            var token = await HMSWebUtils.GetAccessTokenAsync();
            HMSWebRequestHelper.Instance.PostRequest("https://connect-api.cloud.huawei.com/api/pms/product-price-service/v1/manage/product/group/list", JsonUtility.ToJson(new CreateProductEditor.GetSubGroupsReqJson() { requestId = Guid.NewGuid().ToString() }),
                new Dictionary<string, string>()
                {
                    {"client_id", HMSConnectAPISettings.Instance.Settings.Get(HMSConnectAPISettings.ClientID) },
                    {"Authorization","Bearer " + token},
                    {"appId",HMSEditorUtils.GetAGConnectConfig().client.app_id}
                }, OnRequestSubGroupsResponse);
        }

        private void OnRequestSubGroupsResponse(UnityWebRequest response)
        {
            var responseJson = JsonUtility.FromJson<CreateProductEditor.GetSubGroupResJson>(response.downloadHandler.text);

            if (responseJson.error.errorCode == 0) // request was successful.
            {
                Debug.Log("[HMS PMSAPI] Sub Groups parsed successfully.");
                var subGroup = responseJson.simpleGroups.FirstOrDefault(c => c.groupId == _product.groupId);
                if (subGroup == null)
                {
                    Debug.LogError($"[HMS PMSAPI] SubGroup could not found for product: {_product.productName}]");
                    return;
                }

                subGroupValueLabel.SetText(subGroup.groupName);
            }
            else
            {
                Debug.LogError($"[HMS PMSAPI] GetSubGroups failed. Error Code: {responseJson.error.errorCode}, Error Message: { responseJson.error.errorMsg }.");
            }
        }

        private string GetSubPeriod(string period)
        {
            switch (period)
            {
                case "D":
                    return "Day";
                case "W":
                    return "Week";
                case "M":
                    return "Month";
                case "Y":
                    return "Year";
                default:
                    break;
            }
            return string.Empty;
        }

        [Serializable]
        private class UpdateProductReqJson
        {
            public string requestId;
            public CreateProductEditor.ProductInfo resource;
        }

        [Serializable]
        private class UpdateProductResJson
        {
            public ErrorResult error;
        }

        [Serializable]
        public class ErrorResult
        {
            public int errorCode;
            public string errorMsg;
        }
    }
}
