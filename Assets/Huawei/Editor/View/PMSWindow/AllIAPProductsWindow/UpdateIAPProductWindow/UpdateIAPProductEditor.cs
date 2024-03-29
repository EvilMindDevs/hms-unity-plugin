using System;
using System.Collections.Generic;
using System.Linq;
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
            currencyLabel = new Label.Label(product.currency);
            string priceText = product.price;
            if (int.TryParse(priceText, out int price))
            {
                defaultPriceTextField = new TextField.TextField("Price:", (price / 100f).ToString());
            }
            else
            {
                // Handle the case where the price could not be parsed to an int
                // This could be setting a default value, logging an error, etc.
                defaultPriceTextField = new TextField.TextField("Price:", "0");
            }

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

            AddDrawer(new HorizontalSequenceDrawer(new Spacer(), new Button.Button("Update Product", async () => await OnUpdateProductClickAsync()).SetBGColor(Color.green).SetWidth(300), new Spacer()));
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

        private async Task OnUpdateProductClickAsync()
        {
            var productObj = new CreateProductEditor.ProductInfo
            {
                productNo = _product.productNo,
                appId = HMSEditorUtils.GetAGConnectConfig().client.app_id,
                productName = productNameTextField.GetCurrentText(),
                purchaseType = _product.purchaseType,
                status = statusToggle.IsChecked() ? "active" : "inactive",
                currency = currencyLabel.GetText(),
                country = selectedCountry.Region,
                defaultLocale = selectedLocale,
                productDesc = descriptionTextField.GetCurrentText(),
            };

            if (double.TryParse(defaultPriceTextField.GetCurrentText(), out double defaultPrice))
            {
                productObj.defaultPrice = (defaultPrice * 100).ToString();
            }
            else
            {
                // Handle the case where the default price could not be parsed to a double
                // This could be setting a default value, logging an error, etc.
                productObj.defaultPrice = "0";
            }

            // If the product is a subscription, set additional properties
            if (_product.purchaseType == "auto_subscription")
            {
                productObj.subGroupId = _product.groupId;
                productObj.subPeriod = _product.numberOfUnits;
                productObj.subPeriodUnit = _product.periodUnit;
            }

            UpdateProductReqJson req = new UpdateProductReqJson
            {
                requestId = Guid.NewGuid().ToString(),
                resource = productObj
            };

            // Convert the request object to JSON and remove unnecessary fields
            string jsonValue = JsonUtility.ToJson(req, true);
            jsonValue = jsonValue.Replace("\n        \"languages\": [],", "");
            if (_product.purchaseType != "auto_subscription")
            {
                jsonValue = jsonValue.Replace(",\n        \"subGroupId\": \"\",\n        \"subPeriod\": 0,\n        \"subPeriodUnit\": \"\"\n    ", "\n    ");
            }

            var token = await HMSWebUtils.GetAccessTokenAsync();

            var headers = new Dictionary<string, string>
            {
                {"client_id", HMSConnectAPISettings.Instance.Settings.Get(HMSConnectAPISettings.ClientID)},
                {"Authorization", "Bearer " + token},
                {"appId", productObj.appId}
            };

            HMSWebRequestHelper.Instance.PutRequest(
                "https://connect-api.cloud.huawei.com/api/pms/product-price-service/v1/manage/product",
                jsonValue,
                headers,
                OnUpdateProductResponse
            );
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
                Debug.LogError($"[HMS PMSAPI] Product update failed. Error Code: {responseJson.error.errorCode}, Error Message: {responseJson.error.errorMsg}.");
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
                Debug.LogError($"[HMS PMSAPI] GetSubGroups failed. Error Code: {responseJson.error.errorCode}, Error Message: {responseJson.error.errorMsg}.");
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
                    // All cases are handled
                    return string.Empty;
            }
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
