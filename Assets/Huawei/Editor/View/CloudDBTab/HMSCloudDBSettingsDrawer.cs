using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HmsPlugin
{
    public class HMSCloudDBSettingsDrawer : VerticalSequenceDrawer
    {
        private TextField.TextField packageNameTextField;

        public HMSCloudDBSettingsDrawer()
        { 
            packageNameTextField = new TextField.TextField("Java Package Name: ", "", OnPackageNameChanged);

            SetupSequence();
        }

        private void OnPackageNameChanged(string value)
        {

        }

        private void OnJsonFileSelected()
        {
            
        }

        private void SetupSequence()
        {
            AddDrawer(new HorizontalSequenceDrawer(new HorizontalLine(), new Label.Label("Creating C# Model").SetBold(true), new HorizontalLine()));
            AddDrawer(new Space(3));
            AddDrawer(packageNameTextField);
            AddDrawer(new Space(3));
            AddDrawer(new HorizontalSequenceDrawer(new Label.Label("Select Json File"), new Space(3), new Button.Button("Select", OnJsonFileSelected).SetWidth(100)));
            AddDrawer(new HorizontalLine());
            AddDrawer(new Space(5));
        }
    }
}
