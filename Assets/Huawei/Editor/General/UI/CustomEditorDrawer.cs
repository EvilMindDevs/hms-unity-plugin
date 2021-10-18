namespace HmsPlugin
{
    public class CustomEditorDrawer : UnityEditor.Editor
    {
        protected VerticalSequenceDrawer _drawer = new VerticalSequenceDrawer();

        public override void OnInspectorGUI()
        {
            _drawer.Draw();
        }
    }
}