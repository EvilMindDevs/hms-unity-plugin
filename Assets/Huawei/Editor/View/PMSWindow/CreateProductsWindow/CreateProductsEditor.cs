using HmsPlugin.Window;
using OfficeOpenXml;
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
    public class CreateProductsEditor : VerticalSequenceDrawer
    {
        private TextArea.TextArea jsonField;

        public CreateProductsEditor()
        {
            jsonField = new TextArea.TextArea("").SetFieldHeight(500);

            AddDrawer(new HorizontalLine());
            AddDrawer(new HorizontalSequenceDrawer(new Label.Label("Download Product Template"), new Space(10), new Button.Button("Download", OnDownloadClick).SetBGColor(Color.green)));
            AddDrawer(new Space(5));
            //TODO: Set label width and match two button starting positions.
            AddDrawer(new HorizontalSequenceDrawer(new Label.Label("Select Product Template"), new Space(10), new Button.Button("Select", OnSelectClicked).SetBGColor(Color.yellow)));
            AddDrawer(new HorizontalLine());
            AddDrawer(jsonField);
            AddDrawer(new Space(10));
            AddDrawer(new HorizontalSequenceDrawer(new Spacer(), new Button.Button("Create Products", OnCreateProductsClick).SetBGColor(Color.green).SetWidth(300), new Spacer()));
            AddDrawer(new Spacer());
        }

        private async void OnCreateProductsClick()
        {
            if (EditorUtility.DisplayDialog("Are you sure?", "Please make sure all of the parameters are correct.\n\nDo you want to submit?", "Submit", "Cancel"))
            {
                var token = await HMSWebUtils.GetAccessTokenAsync();
                HMSWebRequestHelper.Instance.PostRequest("https://connect-api.cloud.huawei.com/api/pms/product-price-service/v1/manage/product/batchImportProducts",
                    jsonField.GetCurrentText(),
                    new Dictionary<string, string>()
                    {
                    {"client_id", HMSConnectAPISettings.Instance.Settings.Get(HMSConnectAPISettings.ClientID) },
                    {"Authorization","Bearer " + token},
                    {"appId",HMSEditorUtils.GetAGConnectConfig().client.app_id}
                    }, OnCreateProductResponse);
            }
        }

        private void OnCreateProductResponse(UnityWebRequest response)
        {
            if (string.IsNullOrEmpty(jsonField.GetCurrentText())) return;
            var responseJson = JsonUtility.FromJson<CreateProductResJson>(response.downloadHandler.text);

            if (responseJson.error.errorCode == 0) // request was successful.
            {
                Debug.Log($"[HMS PMS API] Products have been created succesfully. Success Number:{responseJson.successNumber}. Failed Number:{responseJson.failedNumber}");
                EditorUtility.DisplayDialog("HMS PMS API", "Products has been created succesfully!", "Ok");
            }
            else
            {
                Debug.LogError($"[HMS PMSAPI]: Batch Product creation failed. Error Code: {responseJson.error.errorCode}, Error Message: { responseJson.error.errorMsg }.");
                if (responseJson.failedNumber > 0 && responseJson.resultInfo.Count > 0)
                {
                    Debug.LogError("[HMS PMS API]: Several products could not be created and they are listed below.");
                    foreach (var item in responseJson.resultInfo)
                    {
                        Debug.LogError($"[HMS PMS API]: Product No:{item.productNo}, Error Code:{item.errorCode}, Error Reason:{item.errorReason}");
                    }
                }
            }
        }

        private void OnSelectClicked()
        {
            string path = EditorUtility.OpenFilePanel("Choose an Excel File", "", "xlsx");
            if (!string.IsNullOrEmpty(path))
            {
                var data = ParseExcelToJson(HMSExcelHelper.ReadExcel(path));
                string json = JsonUtility.ToJson(new CreateProductsReq() { requestId = Guid.NewGuid().ToString(), products = data }, true);

                json = json.Replace(",\n            \"subPeriod\": \"\",\n            \"subPeriodUnit\": \"\",\n            \"subGroupId\": \"\",\n            ", ",\n            ");
                json = json.Replace(",\n            \"languages\": [],", ",");

                jsonField.SetCurrentText(json);
            }
        }

        private void OnDownloadClick()
        {
            string path = EditorUtility.OpenFolderPanel("Select Directory to save the zip file", "", "");
            if (!string.IsNullOrEmpty(path))
            {
                path += @"/ProductTemplate.zip";
                HMSWebRequestHelper.Instance.GetFile("https://developer.huawei.com/consumer/cn/service/josp/agc/pcp/agc/app/views/operate/productmng/assets/template/Product%20Batch%20Adding%20or%20Modification%20Template.zip", path, (result) =>
                 {
                     string message = result ? $"File saved to {path}." : "File could not be saved. Please check Unity console.";
                     DisplayDialog.Create("Product Template File", message, "Ok", null);
                 });
            }
        }

        // 0 : ProductId
        // 1 : ProductType
        // 2 : Locale Title Description
        // 3 : Price
        // 4 : SubPeriod
        // 5 : SubgroupId
        // 6 : Status

        private List<ProductImportInfo> ParseExcelToJson(string[,] array)
        {
            var returnList = new List<ProductImportInfo>();
            for (int i = 2; i < array.GetLength(0); i++)
            {
                var product = new ProductImportInfo();
                product.defaultPriceInfo = new DefaultProductPriceInfo();

                var supportedCountry = HMSEditorUtils.SupportedCountries().FirstOrDefault(c => c.Locale == array[i, 2].Split('|')[0]);
                var prices = GetPricesFromExcel(array[i, 3].ToString());

                product.productNo = array[i, 0].ToString();
                product.status = array[i, 6].ToString() == "3" ? "inactive" : "active";
                product.productName = array[i, 2].ToString().Split('|')[1];
                product.productDesc = array[i, 2].ToString().Split('|')[2];
                product.purchaseType = GetProductType(array[i, 1]);
                product.defaultPriceInfo.country = supportedCountry.Country;
                product.defaultPriceInfo.currency = supportedCountry.Currency;
                product.defaultPriceInfo.price = prices.Find(f => f.country == supportedCountry.Region).price;
                product.defaultLocale = array[i, 2].ToString().Split('|')[0];
                product.languages = GetLanguagesFromExcel(array[i, 2]);
                product.prices = prices;
                if (!string.IsNullOrEmpty(array[i, 4]))
                {
                    product.subGroupId = array[i, 5];
                    product.subPeriod = array[i, 4].Split(' ')[0];
                    product.subPeriodUnit = GetSubPeriodUnit(array[i, 4].Split(' ')[1]);
                }

                returnList.Add(product);
            }
            return returnList;
        }

        private List<ProductImportInfo> ParseExcelToJson(List<Dictionary<string, object>> dict)
        {
            var returnList = new List<ProductImportInfo>();
            for (int i = 39; i < dict.Count; i++)
            {
                var pair = dict.ElementAt(i);
                var product = new ProductImportInfo();
                product.defaultPriceInfo = new DefaultProductPriceInfo();

                var supportedCountry = HMSEditorUtils.SupportedCountries().FirstOrDefault(c => c.Locale == pair["Locale Title Description"].ToString().Split('|')[0]);
                var prices = GetPricesFromExcel(pair["Price"].ToString());

                product.productNo = pair["ProductId"].ToString();
                product.status = pair["Status"].ToString() == "3" ? "inactive" : "active";
                product.productName = pair["Locale Title Description"].ToString().Split('|')[1];
                product.productDesc = pair["Locale Title Description"].ToString().Split('|')[2];
                product.purchaseType = GetProductType(pair["ProductType"].ToString());
                product.defaultPriceInfo.country = supportedCountry.Country;
                product.defaultPriceInfo.currency = supportedCountry.Currency;
                product.defaultPriceInfo.price = prices.Find(f => f.country == supportedCountry.Region).price;
                product.defaultLocale = pair["Locale Title Description"].ToString().Split('|')[0];
                product.languages = GetLanguagesFromExcel(pair["Locale Title Description"].ToString());
                product.prices = prices;
                product.subGroupId = pair["Subgroup ID"].ToString();
                if (!string.IsNullOrEmpty(pair["SubPeriod"].ToString()))
                {
                    product.subPeriod = pair["SubPeriod"].ToString().Split(' ')[0];
                    product.subPeriodUnit = GetSubPeriodUnit(pair["SubPeriod"].ToString().Split(' ')[1]);
                }

                returnList.Add(product);
            }
            return returnList;
        }

        private List<Language> GetLanguagesFromExcel(string value)
        {
            var returnList = new List<Language>();
            var split = value.Split('|');
            for (int i = 3; i < split.Length; i += 3)
            {
                returnList.Add(new Language { locale = split[i], productName = split[i + 1], productDesc = split[i + 2] });
            }
            return returnList;
        }

        private string GetProductType(string value)
        {
            switch (value)
            {
                case "0":
                    return "consumable";
                case "2":
                    return "auto_subscription";
                case "3":
                    return "non_consumable";
                default:
                    break;
            }
            return string.Empty;
        }

        //TODO: check if chinese and russian is also supported and add change switch to if clause and type chinese&russian words.
        private string GetSubPeriodUnit(string value)
        {
            switch (value)
            {
                case "week":
                    return "W";
                case "month":
                    return "M";
                case "months":
                    return "M";
                case "year":
                    return "Y";
                default:
                    break;
            }
            return string.Empty;
        }

        private List<Price> GetPricesFromExcel(string value)
        {
            var returnList = new List<Price>();
            var split = value.Split(';');
            for (int i = 0; i < split.Length; i += 2)
            {
                returnList.Add(new Price { country = split[i], price = split[i + 1] });
            }
            return returnList;
        }

        [Serializable]
        private class CreateProductsReq
        {
            public string requestId;
            public List<ProductImportInfo> products;
        }

        [Serializable]
        private class CreateProductResJson
        {
            public ErrorResult error;
            public int failedNumber;
            public int successNumber;
            public List<ProductImportErrorInfo> resultInfo;
        }

        #region JSON Classes

        [Serializable]
        private class ProductImportErrorInfo
        {
            public string productNo;
            public int errorCode;
            public string errorReason;
        }

        [Serializable]
        private class ErrorResult
        {
            public int errorCode;
            public string errorMsg;
        }

        [Serializable]
        private class ProductImportInfo
        {
            public string productNo;
            public string status;
            public string productName;
            public string productDesc;
            public string purchaseType;
            public DefaultProductPriceInfo defaultPriceInfo;
            public string defaultLocale;
            public string subPeriod;
            public string subPeriodUnit;
            public string subGroupId;
            public List<Language> languages;
            public List<Price> prices;
        }

        [Serializable]
        private class DefaultProductPriceInfo
        {
            public string country;
            public string price;
            public string currency;
        }

        [Serializable]
        private class Language
        {
            public string locale;
            public string productName;
            public string productDesc;
        }

        [Serializable]
        private class Price
        {
            public string country;
            public string price;
        }
        #endregion
    }
}
