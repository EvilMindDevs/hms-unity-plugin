namespace HmsPlugin
{
    internal class ModelingTabFactory
    {
        public static TabView CreateTab(string title)
        {
            var tabView = new TabView(title);
            tabView.AddDrawer(new ModelingSettingsDrawer());
            return tabView;
        }
    }
}
