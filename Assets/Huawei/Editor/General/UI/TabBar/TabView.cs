namespace HmsPlugin
{
    public class TabView : ScrollView
    {
        public TabView(string title)
        {
            Title = title;
        }


        public string Title { get; private set; }
    }
}
