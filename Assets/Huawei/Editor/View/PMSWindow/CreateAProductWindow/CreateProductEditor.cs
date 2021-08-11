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
        TextField.TextField appIdTextField;
        TextField.TextField productNameTextField;
        Dropdown.EnumDropdown purchaseTypeDropdown;
        Toggle.Toggle statusToggle;
        TextField.TextField currencyTextField;
        TextField.TextField countryTextField;
        TextField.TextField defaultLocaleTextField;
        TextField.TextField defaultPriceTextField;
        TextField.TextField descriptionTextField;

        private Foldout languages;

        public CreateProductEditor()
        {
            productNoTextField = new TextField.TextField("Product No:", "");
            appIdTextField = new TextField.TextField("App Id:", "");
            productNameTextField = new TextField.TextField("Product Name:", "");
            purchaseTypeDropdown = new Dropdown.EnumDropdown(PurchaseType.NonConsumable);
            statusToggle = new Toggle.Toggle("Status:");
            currencyTextField = new TextField.TextField("Currency:", "");
            countryTextField = new TextField.TextField("Country:", "");//TODO: change country to be a dropdown and change currency with selected country.
            defaultLocaleTextField = new TextField.TextField("Default Language:", ""); //TODO: change this to a dropdown with all locales added.
            defaultPriceTextField = new TextField.TextField("Price:", "");
            descriptionTextField = new TextField.TextField("Description:", "");
            languages = new Foldout("Languages");

            AddDrawer(new HorizontalLine());
            AddDrawer(productNoTextField);
            AddDrawer(new Space(5));
            AddDrawer(appIdTextField);
            AddDrawer(new Space(5));
            AddDrawer(productNameTextField);
            AddDrawer(new Space(5));
            AddDrawer(new HorizontalSequenceDrawer(new Label.Label("Purchase Type:"), new Space(56), purchaseTypeDropdown));
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
            AddDrawer(descriptionTextField);
            AddDrawer(new Space(5));
            AddDrawer(languages);

            languages.AddDrawer(new TextField.TextField("annen"));

            AddDrawer(new Space(15));
            AddDrawer(new HorizontalSequenceDrawer(new Spacer(), new Button.Button("Create Product", OnCreateProductClick).SetWidth(300), new Spacer()));
            AddDrawer(new HorizontalLine());

            AddDrawer(new TextField.TextField("").SetFieldHeight(300));
        }

        private void OnCreateProductClick()
        {

        }

        enum PurchaseType
        {
            Consumable,
            NonConsumable,
            Subcription
        }
    }
}
