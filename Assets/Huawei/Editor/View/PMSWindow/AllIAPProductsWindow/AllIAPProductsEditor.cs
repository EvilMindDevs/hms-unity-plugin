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
    public class AllIAPProductsEditor : VerticalSequenceDrawer
    {
        private TextField.TextField productNoTextField;
        private TextField.TextField productNameTextField;
        private Dropdown.StringDropdown purchaseTypeDropdown;
        private Dropdown.EnumFlagsDropdown statusDropdown;
        private Dropdown.StringDropdown countryDropdown;

        private List<HMSEditorUtils.CountryInfo> countryInfos;
        private string[] purchaseTypes = { "Consumable", "Non_Consumable", "Auto_Subscription" };
        private int selectedPurchaseType;
        private HMSEditorUtils.CountryInfo selectedCountry;
        private int pageNum = 1;
        private int pageSize = 20;
        private Status productStatus = Status.Active | Status.Inactive;

        public AllIAPProductsEditor()
        {
            countryInfos = HMSEditorUtils.SupportedCountries();

            productNoTextField = new TextField.TextField("Product Id:", "");
            productNameTextField = new TextField.TextField("Product Name:", "");
            purchaseTypeDropdown = new Dropdown.StringDropdown(purchaseTypes, 0, "Purchase Type:", OnPurchaseTypeChanged);
            statusDropdown = new Dropdown.EnumFlagsDropdown(productStatus, "Status");
            countryDropdown = new Dropdown.StringDropdown(countryInfos.Select(c => c.Country).ToArray(), 0, "Country", OnCountrySelected);

            AddDrawer(new HorizontalLine());
            AddDrawer(productNoTextField);
            AddDrawer(new Space(5));
            AddDrawer(productNameTextField);
            AddDrawer(new Space(5));
            AddDrawer(purchaseTypeDropdown);
            AddDrawer(new Space(5));
            AddDrawer(statusDropdown);
            AddDrawer(new Space(5));
            AddDrawer(countryDropdown);
            AddDrawer(new Space(10));
            AddDrawer(new HorizontalSequenceDrawer(new Spacer(), new Button.Button("Query", OnQueryButtonClick).SetBGColor(Color.green).SetWidth(200), new Spacer()));
            AddDrawer(new HorizontalLine());
        }

        private async void OnQueryButtonClick()
        {
            var req = new SearchProductsReq();
            req.productName = productNameTextField.GetCurrentText();
            req.productNo = productNoTextField.GetCurrentText();
            req.purchaseType = purchaseTypes[selectedPurchaseType].ToLower();
            req.pageNum = pageNum;
            req.pageSize = pageSize;
            req.requestId = new GUID().ToString();
            req.status = GetStatus();
            req.country = selectedCountry.Region;


            var token = await HMSWebUtils.GetAccessTokenAsync();
            HMSWebRequestHelper.PostRequest("https://connect-api.cloud.huawei.com/api/pms/product-price-service/v1/manage/product/list", JsonUtility.ToJson(req), new Dictionary<string, string>()
            {
                {"client_id", HMSConnectAPISettings.Instance.Settings.Get(HMSConnectAPISettings.ClientID) },
                {"Authorization","Bearer " + token},
                {"appId",HMSEditorUtils.GetAGConnectConfig().client.app_id}
            }, OnQueryResponse);
        }

        private void OnQueryResponse(UnityWebRequest response)
        {
            Debug.Log("annen");
        }

        private void OnPurchaseTypeChanged(int index)
        {
            selectedPurchaseType = index;
        }

        private void OnCountrySelected(int index)
        {
            selectedCountry = countryInfos[index];
        }

        private string GetStatus()
        {
            switch (productStatus)
            {
                case Status.Active:
                    return "active";
                case Status.Inactive:
                    return "inactive";
                case Status.Deleted:
                    return "delete";
                case Status.Active | Status.Inactive:
                    return "active,inactive";
                case Status.Active | Status.Deleted:
                    return "active,deleted";
                case Status.Deleted | Status.Inactive:
                    return "inactive,deleted";
                case Status.Active | Status.Deleted | Status.Inactive:
                    return "active,inactive,deleted";
                default:
                    break;
            }
            return string.Empty;
        }

        [Serializable]
        private class SearchProductsReq
        {
            public string requestId;
            public string productNo;
            public string productName;
            public string purchaseType;
            public string status;
            public int pageNum;
            public int pageSize;
            public string country;
        }

        [Flags]
        private enum Status
        {
            Active = 1 << 0,
            Inactive = 1 << 1,
            Deleted = 1 << 2
        }
    }
}