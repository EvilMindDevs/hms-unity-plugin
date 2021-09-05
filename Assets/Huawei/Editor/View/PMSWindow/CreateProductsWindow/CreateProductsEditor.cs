using HmsPlugin.Window;
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
            var token = await HMSWebUtils.GetAccessTokenAsync();
            HMSWebRequestHelper.PostRequest("https://connect-api.cloud.huawei.com/api/pms/product-price-service/v1/manage/product/batchImportProducts",
                jsonField.GetCurrentText(),
                new Dictionary<string, string>()
                {
                    {"client_id", HMSConnectAPISettings.Instance.Settings.Get(HMSConnectAPISettings.ClientID) },
                    {"Authorization","Bearer " + token},
                    {"appId",HMSEditorUtils.GetAGConnectConfig().client.app_id}
                }, OnCreateProductResponse);
        }

        private void OnCreateProductResponse(UnityWebRequest response)
        {
            if (string.IsNullOrEmpty(jsonField.GetCurrentText())) return;
            var responseJson = JsonUtility.FromJson<CreateProductResJson>(response.downloadHandler.text);

            if (responseJson.error.errorCode == 0) // request was successful.
            {
                Debug.Log($"[HMS PMS API] Products have been created succesfully. Success Number:{responseJson.successNumber}. Failed Number:{responseJson.failedNumber}");
                if (responseJson.failedNumber > 0 && responseJson.resultInfo.Count > 0)
                {
                    Debug.LogError("[HMS PMS API]: Several products could not be created and they are listed below.");
                    foreach (var item in responseJson.resultInfo)
                    {
                        Debug.LogError($"[HMS PMS API]: Product No:{item.productNo}, Error Code:{item.errorCode}, Error Reason:{item.errorReason}");
                    }
                }
                EditorUtility.DisplayDialog("HMS PMS API", "Products has been created succesfully!", "Ok");
            }
            else
            {
                Debug.LogError($"[HMS PMSAPI]: Batch Product creation failed. Error Code: {responseJson.error.errorCode}, Error Message: { responseJson.error.errorMsg }.");
            }
        }

        private void OnSelectClicked()
        {
            string path = EditorUtility.OpenFilePanel("Choose an Excel CSV File", "", "csv");
            if (!string.IsNullOrEmpty(path))
            {
                var data = HMSCSVReader.Read(path);
                jsonField.SetCurrentText(JsonUtility.ToJson(new CreateProductsReq() { requestId = Guid.NewGuid().ToString(), products = ParseExcelToJson(data) }, true));
            }
        }

        private void OnDownloadClick()
        {
            string path = EditorUtility.OpenFolderPanel("Select Directory to save the zip file", "", "");
            if (!string.IsNullOrEmpty(path))
            {
                path += @"/ProductTemplate.zip";
                HMSWebRequestHelper.GetFile("https://developer.huawei.com/consumer/cn/service/josp/agc/pcp/agc/app/views/operate/productmng/assets/template/Product%20Batch%20Adding%20or%20Modification%20Template.zip", path, (result) =>
                 {
                     string message = result ? $"File saved to {path}." : "File could not be saved. Please check Unity console.";
                     DisplayDialog.Create("Product Template File", message, "Ok", null);
                 });

            }
        }

        private List<ProductImportInfo> ParseExcelToJson(List<Dictionary<string, object>> dict)
        {
            var returnList = new List<ProductImportInfo>();
            for (int i = 39; i < dict.Count; i++)
            {
                var pair = dict.ElementAt(i);
                returnList.Add(new ProductImportInfo
                {
                    productNo = pair["ProductId"].ToString(),
                    status = pair["Status"].ToString(),
                    productName = pair["Locale Title Description"].ToString().Split('|')[1],
                    productDesc = pair["Locale Title Description"].ToString().Split('|')[2],
                    purchaseType = GetProductType(pair["ProductType"].ToString()),
                    defaultLocale = pair["Locale Title Description"].ToString().Split('|')[0],
                    languages = GetLanguagesFromExcel(pair["Locale Title Description"].ToString()),
                    prices = GetPricesFromExcel(pair["Price"].ToString())
                });
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
            public string defaultLocale;
            public List<Language> languages;
            public List<Price> prices;
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
    }
}
