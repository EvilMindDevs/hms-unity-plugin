using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HmsPlugin.ConnectAPI.PMSAPI
{
    public class CreateProductEditor : VerticalSequenceDrawer
    {
        TextField.TextField productNoTextField;
        TextField.TextField productNameTextField;
        Dropdown.StringDropdown purchaseTypeDropdown;
        Toggle.Toggle statusToggle;
        TextField.TextField currencyTextField;
        TextField.TextField countryTextField;
        TextField.TextField defaultLocaleTextField;
        TextField.TextField defaultPriceTextField;
        TextField.TextField descriptionTextField;

        private LanguagesFoldoutEditor languagesFoldout;
        private string[] purchaseTypes = { "Consumable", "Non-Consumable", "Auto_Subscription" };

        public CreateProductEditor()
        {
            productNoTextField = new TextField.TextField("Product Id:", "");
            productNameTextField = new TextField.TextField("Product Name:", "");
            descriptionTextField = new TextField.TextField("Description:", "");
            purchaseTypeDropdown = new Dropdown.StringDropdown(purchaseTypes, 0, "Purchase Type:", OnPurchaseTypeChanged);
            statusToggle = new Toggle.Toggle("Status:");
            currencyTextField = new TextField.TextField("Currency:", "");
            countryTextField = new TextField.TextField("Country:", "");//TODO: change country to be a dropdown and change currency with selected country.
            defaultLocaleTextField = new TextField.TextField("Default Language:", ""); //TODO: change this to a dropdown with all locales added.
            defaultPriceTextField = new TextField.TextField("Price:", "");
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
            AddDrawer(currencyTextField);
            AddDrawer(new Space(5));
            AddDrawer(countryTextField);
            AddDrawer(new Space(5));
            AddDrawer(defaultLocaleTextField);
            AddDrawer(new Space(5));
            AddDrawer(defaultPriceTextField);
            AddDrawer(new Space(5));
            AddDrawer(languagesFoldout);

            AddDrawer(new Space(15));
            AddDrawer(new HorizontalSequenceDrawer(new Spacer(), new Button.Button("Generate JSON", OnCreateProductClick).SetWidth(300).SetBGColor(UnityEngine.Color.yellow), new Spacer()));
            AddDrawer(new HorizontalLine());

            AddDrawer(new TextField.TextField("").SetFieldHeight(300));
        }

        private void OnPurchaseTypeChanged(int selectedIndex)
        {

        }

        private void OnCreateProductClick()
        {

        }
    }
}
