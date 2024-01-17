using System.IO;
using UnityEngine;

namespace HmsPlugin
{
    internal class HelpboxAGConnectFile : IDrawer
    {
        private HelpBox.HelpBox _helpBox;

        private bool hasAGConnectFile;

        public HelpboxAGConnectFile()
        {
            _helpBox = new HelpBox.HelpBox("Please import your agconnect-services.json file to StreamingAssets folder", UnityEditor.MessageType.Error);
            hasAGConnectFile = File.Exists(Application.streamingAssetsPath + "/agconnect-services.json");
        }

        public void Draw()
        {
            if (!hasAGConnectFile)
                _helpBox.Draw();
        }
    }
}
