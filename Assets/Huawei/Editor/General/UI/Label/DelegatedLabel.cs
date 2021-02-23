using System;

namespace HmsPlugin.Label
{
    // Calls delegate on the element for each draw
    public class DelegatedLabel<T> : Label
    {
        private T _item;
        private Func<T, string> _labelDlg;
        private Func<T, string> _tooltipDlg;

        public DelegatedLabel(T item, Func<T, string> labelDlg, Func<T, string> tooltipDlg = null)
        {
            _item = item;
            _labelDlg = labelDlg;
            _tooltipDlg = tooltipDlg;
        }

        public override void Draw()
        {
            var label = _labelDlg.Invoke(_item);
            var tooltip = _tooltipDlg != null ? _tooltipDlg.Invoke(_item) : null;

            Draw(label, tooltip);
        }
    }
}