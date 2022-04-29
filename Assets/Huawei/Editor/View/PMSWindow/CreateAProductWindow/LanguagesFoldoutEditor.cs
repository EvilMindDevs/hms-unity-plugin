using HmsPlugin.List;
using HmsPlugin.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HmsPlugin.ConnectAPI.PMSAPI
{
    public class LanguagesFoldoutEditor : HorizontalSequenceDrawer
    {
        public List<ProductLanguage> languages;
        private Foldout languagesFoldout;

        public LanguagesFoldoutEditor()
        {
            languages = new List<ProductLanguage>();
            languagesFoldout = new Foldout("Languages (Optional)", false);

            AddDrawer(languagesFoldout);
            RefreshLanguages();
        }

        internal void AddLanguage(string language, string name, string desc)
        {
            languages.Add(new ProductLanguage(language, name, desc));
            RefreshLanguages();
        }

        private void RefreshLanguages()
        {
            languagesFoldout.RemoveAllDrawers();
            languagesFoldout.AddDrawer(new Space(30));
            languagesFoldout.AddDrawer(new ListDrawer<ProductLanguage>(languages, CreateList, ListOrientation.Vertical));
            languagesFoldout.AddDrawer(new HorizontalSequenceDrawer(new Spacer(), new Button.Button("Add Language", OnAddLanguageClick).SetWidth(250).SetBGColor(UnityEngine.Color.green), new Spacer()));
        }

        private void OnAddLanguageClick()
        {
            languages.Add(new ProductLanguage("", "", ""));
            RefreshLanguages();
        }

        private IDrawer CreateList(ProductLanguage item)
        {
            var supportedLanguagesUnsorted = HMSEditorUtils.SupportedLanguages();
            var supportedLanguages = supportedLanguagesUnsorted.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);

            item.Language = supportedLanguages.ElementAt(0).Value;
            item.Index = 0;
            var sequence = new VerticalSequenceDrawer();
            
            sequence.AddDrawer(new Dropdown.StringDropdown(supportedLanguages.Keys.ToArray(), item.Index, "Languages", (index) =>
            {
                item.Language = supportedLanguages.ElementAt(index).Value;
                item.Index = index;
            }));
            sequence.AddDrawer(new Space(3));
            sequence.AddDrawer(new TextField.TextFieldWithData<ProductLanguage>("Product Name:", item.Name, (f, d) => { f.Name = d; }, item));
            sequence.AddDrawer(new Space(3));
            sequence.AddDrawer(new TextField.TextFieldWithData<ProductLanguage>("Product Desc:", item.Desc, (f, d) => { f.Desc = d; }, item));
            sequence.AddDrawer(new Space(3));
            sequence.AddDrawer(new HorizontalSequenceDrawer(new Spacer(), new Button.ButtonInfo<ProductLanguage>("Remove", 70, OnRemovePressed).CreateButton(item).SetBGColor(UnityEngine.Color.red)));
            sequence.AddDrawer(new HorizontalLine());
            return sequence;
        }

        private void OnRemovePressed(ProductLanguage obj)
        {
            languages.Remove(obj);
            RefreshLanguages();
        }

        public List<ProductLanguage> GetLanguages()
        {
            return languages;
        }

        public class ProductLanguage
        {
            public ProductLanguage(string language, string name, string desc)
            {
                Language = language;
                Name = name;
                Desc = desc;
            }

            public ProductLanguage()
            {

            }

            public string Language { get; set; }
            public string Name { get; set; }
            public string Desc { get; set; }
            public int Index { get; set; }
        }
    }
}
