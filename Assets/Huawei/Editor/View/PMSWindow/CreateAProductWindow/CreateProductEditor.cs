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
        private Dropdown.StringDropdown subGroupDropdown;
        private Dropdown.StringDropdown subPeriodDropdown;
        private Toggle.Toggle statusToggle;
        private Label.Label currencyLabel;
        private Dropdown.StringDropdown countryDropdown;
        private Dropdown.StringDropdown defaultLocaleDropdown;
        private TextField.TextField defaultPriceTextField;
        private TextField.TextField descriptionTextField;
        private TextArea.TextArea jsonField;

        private readonly int InitialLanguageIndex = 15;
        private readonly int InitialCountryIndex = 0;

        private LanguagesFoldoutEditor languagesFoldout;
        private string[] purchaseTypes = { "Consumable", "Non_Consumable", "Auto_Subscription" };
        private List<SubPeriod> subPeriods;

        public List<HMSEditorUtils.CountryInfo> countryInfos;
        private Dictionary<string, string> supportedLanguages;
        private VerticalSequenceDrawer bottomDrawer;

        private HMSEditorUtils.CountryInfo selectedCountry;
        private int selectedPurchaseType;
        private int selectedSubPeriodIndex;
        private int selectedSubGroupIndex;
        private string selectedLocale;
        private CreateProductReqJson jsonClass;
        private List<SimpleGroupInfo> subGroupList;

        //TODO: Add check for inputs and warn user if fields are not filled.
        public CreateProductEditor()
        {
            var supportedLanguagesUnsorted = HMSEditorUtils.SupportedLanguages();
            supportedLanguages = supportedLanguagesUnsorted.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
            countryInfos = HMSEditorUtils.SupportedCountries();
            countryInfos.Sort((x, y) => x.Country.CompareTo(y.Country));
            subPeriods = SetupSubPeriods();


            productNoTextField = new TextField.TextField("Product Id:", "").SetHeight(20);
            productNameTextField = new TextField.TextField("Product Name:", "").SetHeight(20);
            descriptionTextField = new TextField.TextField("Description:", "").SetHeight(20);
            purchaseTypeDropdown = new Dropdown.StringDropdown(purchaseTypes, 0, "Purchase Type:", OnPurchaseTypeChanged);
            subPeriodDropdown = new Dropdown.StringDropdown(subPeriods.Select(c => c.Content).ToArray(), 0, "Sub Period:", OnSubPeriodChanged);
            subGroupDropdown = new Dropdown.StringDropdown(new string[] { }, 0, "Sub Group:", OnSubGroupChanged);
            statusToggle = new Toggle.Toggle("Status(Active/Inactive):");
            currencyLabel = new Label.Label();
            countryDropdown = new Dropdown.StringDropdown(countryInfos.Select(c => c.Country).ToArray(), InitialCountryIndex, "Country", OnCountrySelected);
            defaultLocaleDropdown = new Dropdown.StringDropdown(supportedLanguages.Keys.ToArray(), InitialLanguageIndex, "Default Language", OnLanguageSelected);
            defaultPriceTextField = new TextField.TextField("Price:", "");
            bottomDrawer = new VerticalSequenceDrawer();
            jsonField = new TextArea.TextArea("").SetFieldHeight(500);

            OnCountrySelected(InitialCountryIndex);
            OnLanguageSelected(InitialLanguageIndex);
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
            AddDrawer(bottomDrawer);
            GenerateBottomDrawer(false);
        }

        private async void OnGetButtonClick()
        {
            await RequestSubGroups();
        }

        private void OnSubGroupChanged(int index)
        {
            selectedSubGroupIndex = index;
        }

        private void OnSubPeriodChanged(int index)
        {
            selectedSubPeriodIndex = index;
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
            GenerateBottomDrawer(selectedIndex == 2);
        }

        private void GenerateBottomDrawer(bool isSubscriptionSelected)
        {
            if (bottomDrawer != null)
                bottomDrawer.RemoveAllDrawers();
            if (isSubscriptionSelected)
            {
                bottomDrawer.AddDrawer(subPeriodDropdown);
                bottomDrawer.AddDrawer(new HorizontalSequenceDrawer(subGroupDropdown, new Button.Button("Get", OnGetButtonClick).SetBGColor(Color.yellow).SetWidth(100)));
            }
            bottomDrawer.AddDrawer(statusToggle);
            bottomDrawer.AddDrawer(new Space(5));
            bottomDrawer.AddDrawer(defaultLocaleDropdown);
            bottomDrawer.AddDrawer(new Space(5));
            bottomDrawer.AddDrawer(countryDropdown);
            bottomDrawer.AddDrawer(new Space(5));
            bottomDrawer.AddDrawer(defaultPriceTextField);
            bottomDrawer.AddDrawer(new Space(5));
            bottomDrawer.AddDrawer(new HorizontalSequenceDrawer(new Label.Label("Currency:"), new Space(92), currencyLabel));
            bottomDrawer.AddDrawer(new Space(5));
            bottomDrawer.AddDrawer(languagesFoldout);
            bottomDrawer.AddDrawer(new Space(15));
            bottomDrawer.AddDrawer(new HorizontalSequenceDrawer(new Spacer(), new Button.Button("Generate JSON", OnGenerateJsonClick).SetWidth(300).SetBGColor(Color.yellow), new Spacer()));
            bottomDrawer.AddDrawer(new HorizontalLine());
            bottomDrawer.AddDrawer(jsonField);
            bottomDrawer.AddDrawer(new Space(10));
            bottomDrawer.AddDrawer(new HorizontalSequenceDrawer(new Spacer(), new Button.Button("Create Product", OnCreateProductClick).SetWidth(300).SetBGColor(Color.green), new Spacer()));
        }

        private Boolean CheckParameters() 
        {
            if (productNoTextField.GetCurrentText() == "")
            {
                EditorUtility.DisplayDialog("Missing Parameters!", "Product id can not be empty.", "Ok");
                return false;
            }
            else if (productNameTextField.GetCurrentText() == "")
            {
                EditorUtility.DisplayDialog("Missing Parameters!", "Product name can not be empty.", "Ok");
                return false;
            }
            else if (descriptionTextField.GetCurrentText() == "")
            {
                EditorUtility.DisplayDialog("Missing Parameters!", "Product description can not be empty.", "Ok");
                return false;
            }
            else if (defaultPriceTextField.GetCurrentText() == "" || !double.TryParse(defaultPriceTextField.GetCurrentText(), out _))
            {
                EditorUtility.DisplayDialog("Wrong Parameter!", "Please check your price parameter.", "Ok");
                return false;
            }
            else if (selectedPurchaseType == 2
                && subGroupList == null)
            {
                EditorUtility.DisplayDialog("Missing Parameter!", "Please select your subgroup.", "Ok");
                return false;
            }
            else if (languagesFoldout.GetLanguages().Count > 0)
            {
                foreach (var item in languagesFoldout.GetLanguages())
                {
                    if (item.Desc == "" || item.Name == "") 
                    {
                        EditorUtility.DisplayDialog("Missing Parameter!", "Please check your language parameters.", "Ok");
                        return false;
                    }
                    else if ( (item.Language == selectedLocale) && (item.Desc != descriptionTextField.GetCurrentText() || item.Name != productNameTextField.GetCurrentText()) ) // Extra language can same with default locale language if it has same name and description 
                    {
                        EditorUtility.DisplayDialog("Wrong Parameter!", "Default LanguageInfo is not the same in languages.", "Ok");
                        return false;
                    }
                }
            }
            return true;
        }

        private void OnGenerateJsonClick()
        {
            if(CheckParameters())    
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

            if (selectedPurchaseType == 2)
            {
                jsonClass.product.subGroupId = subGroupList[selectedSubGroupIndex].groupId;
                jsonClass.product.subPeriodUnit = subPeriods[selectedSubPeriodIndex].PeriodUnit;
                jsonClass.product.subPeriod = subPeriods[selectedSubPeriodIndex].Period;
            }

            if (languagesFoldout.GetLanguages().Count > 0)
                jsonClass.product.languages = Language.FromProductLanguage(languagesFoldout.GetLanguages());

            string jsonValue = EditorJsonUtility.ToJson(jsonClass, true);

            if (languagesFoldout.GetLanguages().Count <= 0)
                jsonValue = jsonValue.Replace("\n        \"languages\": [],", "");
            if (selectedPurchaseType != 2)
                jsonValue = jsonValue.Replace(",\n        \"subGroupId\": \"\",\n        \"subPeriod\": 0,\n        \"subPeriodUnit\": \"\"\n    ", "\n    ");

            jsonField.SetCurrentText(jsonValue);
        }

        private async void OnCreateProductClick()
        {
            if (jsonClass == null)
            {
                if (CheckParameters()) 
                {
                    GenerateJsonClass();
                }
                else 
                {
                    return;
                }
            }
            if(EditorUtility.DisplayDialog("Are you sure?", "Please make sure all of the parameters are correct.\n\nDo you want to submit?", "Submit", "Cancel")) 
            {
                var token = await HMSWebUtils.GetAccessTokenAsync();
                HMSWebRequestHelper.Instance.PostRequest("https://connect-api.cloud.huawei.com/api/pms/product-price-service/v1/manage/product",
                    jsonField.GetCurrentText(),
                    new Dictionary<string, string>()
                    {
                    {"client_id", HMSConnectAPISettings.Instance.Settings.Get(HMSConnectAPISettings.ClientID) },
                    {"Authorization","Bearer " + token},
                    {"appId",jsonClass.product.appId}
                    }, OnCreateProductResponse);
            }
        }

        private void OnCreateProductResponse(UnityWebRequest response)
        {
            var responseJson = JsonUtility.FromJson<CreateProductResJson>(response.downloadHandler.text);

            if (responseJson.error.errorCode == 0) // request was successful.
            {
                Debug.Log("[HMS PMSAPI] Product has been created succesfully.");
                EditorUtility.DisplayDialog("HMS PMS API", "Product has been created succesfully!", "Ok");
                jsonClass = null;
            }
            else
            {
                Debug.LogError($"[HMS PMSAPI] Product creation failed. Error Code: {responseJson.error.errorCode}, Error Message: { responseJson.error.errorMsg }.");
            }
        }

        private List<SubPeriod> SetupSubPeriods()
        {
            var returnList = new List<SubPeriod>();
            returnList.Add(new SubPeriod { Period = 1, PeriodUnit = "W", Content = "1 Week" });
            returnList.Add(new SubPeriod { Period = 1, PeriodUnit = "M", Content = "1 Month" });
            returnList.Add(new SubPeriod { Period = 2, PeriodUnit = "M", Content = "2 Months" });
            returnList.Add(new SubPeriod { Period = 3, PeriodUnit = "M", Content = "3 Months" });
            returnList.Add(new SubPeriod { Period = 6, PeriodUnit = "M", Content = "6 Months" });
            returnList.Add(new SubPeriod { Period = 1, PeriodUnit = "Y", Content = "1 Year" });
            return returnList;
        }

        private async Task RequestSubGroups()
        {
            var token = await HMSWebUtils.GetAccessTokenAsync();
            HMSWebRequestHelper.Instance.PostRequest("https://connect-api.cloud.huawei.com/api/pms/product-price-service/v1/manage/product/group/list", JsonUtility.ToJson(new GetSubGroupsReqJson() { requestId = Guid.NewGuid().ToString() }),
                new Dictionary<string, string>()
                {
                    {"client_id", HMSConnectAPISettings.Instance.Settings.Get(HMSConnectAPISettings.ClientID) },
                    {"Authorization","Bearer " + token},
                    {"appId",HMSEditorUtils.GetAGConnectConfig().client.app_id}
                }, OnRequestSubGroupsResponse);
        }

        private void OnRequestSubGroupsResponse(UnityWebRequest response)
        {
            var responseJson = JsonUtility.FromJson<GetSubGroupResJson>(response.downloadHandler.text);

            if (responseJson.error.errorCode == 0) // request was successful.
            {
                Debug.Log("[HMS PMSAPI] Sub Groups parsed successfully.");
                subGroupList = responseJson.simpleGroups;
                subGroupDropdown.SetOptions(responseJson.simpleGroups.Select(c => c.groupName).ToArray());
            }
            else
            {
                Debug.LogError($"[HMS PMSAPI] GetSubGroups failed. Error Code: {responseJson.error.errorCode}, Error Message: { responseJson.error.errorMsg }.");
            }
        }

        [Serializable]
        public class GetSubGroupsReqJson
        {
            public string requestId;
            public int pageNum;
            public int pageSize;
            public string orderBy;
        }

        [Serializable]
        public class GetSubGroupResJson
        {
            public ErrorResult error;
            public int totalNumber;
            public List<SimpleGroupInfo> simpleGroups;
        }

        [Serializable]
        public class SimpleGroupInfo
        {
            public string appId;
            public string groupId;
            public string groupName;
            public int totalProductNumber;
            public int effectiveProductNumber;
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
        public class ErrorResult
        {
            public int errorCode;
            public string errorMsg;
        }

        [Serializable]
        public class ProductInfo
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
            public List<Language> languages;
            public string subGroupId;
            public int subPeriod;
            public string subPeriodUnit;
        }

        [Serializable]
        public class Language
        {
            public string locale;
            public string productName;
            public string productDesc;

            public static List<Language> FromProductLanguage(List<LanguagesFoldoutEditor.ProductLanguage> productLanguages)
            {
                var languages = new List<Language>();
                foreach (var item in productLanguages)
                {
                    languages.Add(new Language() { locale = item.Language, productDesc = item.Desc, productName = item.Name });
                }
                return languages;
            }
        }

        [Serializable]
        private class SubPeriod
        {
            public int Period;
            public string PeriodUnit;
            public string Content;

            public override string ToString()
            {
                return Content;
            }
        }
    }
}
