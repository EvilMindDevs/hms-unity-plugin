using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HmsPlugin.ConnectAPI.PMSAPI
{
    public class UpdateIAPProductEditor : VerticalSequenceDrawer
    {
        private Label.Label productNoLabel;
        private TextField.TextField productNameTextField;
        private TextField.TextField descriptionTextField;
        private Label.Label purchaseTypeLabel;
        private Label.Label subGroupNameLabel;
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

        public UpdateIAPProductEditor(AllIAPProductsEditor.Product product)
        {
            supportedLanguages = HMSEditorUtils.SupportedLanguages();
            countryInfos = HMSEditorUtils.SupportedCountries();

            productNoLabel = new Label.Label("Product Id:");
            productNameTextField = new TextField.TextField("Product Name:", product.productName);
            descriptionTextField = new TextField.TextField("Description:", product.productDesc);
            purchaseTypeLabel = new Label.Label("Purchase Type:");
            subGroupNameLabel = new Label.Label("Sub Group:");
            subGroupPeriodLabel = new Label.Label("Sub Period:");
            statusToggle = new Toggle.Toggle("Status(Active/Inactive):", product.status == "active" ? true : false);
            //TODO: find selected defaultLocale and country.
            defaultLocaleDropdown = new Dropdown.StringDropdown(supportedLanguages.Keys.ToArray(), 14, "Default Language", OnLanguageSelected);
            countryDropdown = new Dropdown.StringDropdown(countryInfos.Select(c => c.Country).ToArray(), 0, "Country", OnCountrySelected);
            defaultPriceTextField = new TextField.TextField("Price:", (int.Parse(product.price) / 100).ToString());

            currencyLabel = new Label.Label(product.currency);

            AddDrawer(new HorizontalLine());
            AddDrawer(new HorizontalSequenceDrawer(productNoLabel, new Space(86), new Label.Label(product.productNo)));
            AddDrawer(new Space(5));
            AddDrawer(productNameTextField);
            AddDrawer(new Space(5));
            AddDrawer(descriptionTextField);
            AddDrawer(new Space(5));
            AddDrawer(new HorizontalSequenceDrawer(purchaseTypeLabel, new Space(60), new Label.Label(product.purchaseType)));
        }

        private void OnCountrySelected(int index)
        {

        }

        private void OnLanguageSelected(int index)
        {

        }
    }
}
