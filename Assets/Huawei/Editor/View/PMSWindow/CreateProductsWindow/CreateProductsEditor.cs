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

        private void OnCreateProductsClick()
        {
            throw new NotImplementedException();
        }

        private void OnSelectClicked()
        {
            string path = EditorUtility.OpenFilePanel("Choose an Excel File", "", "xlsx");
            if (!string.IsNullOrEmpty(path))
            {
                //TODO: read excel file.
                //TODO: generate json classes depending on excel parse.
                //TODO: sent the json to AGC and handle the callback.
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
    }
}
