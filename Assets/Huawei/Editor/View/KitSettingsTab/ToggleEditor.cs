namespace HmsPlugin
{
    public abstract class ToggleEditor
    {
        protected Toggle.Toggle _toggle;
        public bool Enabled { get; set; }
        public abstract void EnableToggle();
        public abstract void DisableToggle();
        public abstract void RemoveToggleTabView(bool removeTabs);
        public abstract void RefreshToggles();
    }
}
