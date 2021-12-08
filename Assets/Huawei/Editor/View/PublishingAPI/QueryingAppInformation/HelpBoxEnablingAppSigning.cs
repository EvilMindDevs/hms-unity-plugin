using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HmsPlugin
{
    public class HelpBoxEnablingApp : IDrawer
    {
        private HelpBox.HelpBox _helpBox;
        private bool isAABSelected;

        public HelpBoxEnablingApp()
        {
            _helpBox = new HelpBox.HelpBox("Please Check the App Signing Enabled in the Huawei App Gallery", UnityEditor.MessageType.Error);
            isAABSelected = UnityEditor.EditorUserBuildSettings.buildAppBundle;
        }

        public void Draw()
        {
            if (isAABSelected)
            {
                _helpBox.Draw();
            }
        }
    }
} 
