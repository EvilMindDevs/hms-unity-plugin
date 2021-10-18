using UnityEngine;

namespace HmsPlugin
{
    //Doesn't work inside Inspector in Unity 2019 (works in the EditorWindow and also in 2018)
    public class Spacer : IDrawer
    {
        public void Draw()
        {
            GUILayout.FlexibleSpace();
        }
    }
}