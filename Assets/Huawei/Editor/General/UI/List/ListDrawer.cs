using HmsPlugin.Button;
using HmsPlugin.Label;
using System;
using System.Collections.Generic;

namespace HmsPlugin.List
{
    public enum ListOrientation
    {
        Horizontal,
        Vertical
    }

    public class ListDrawer<T> : IDrawer where T : class
    {
        private IDrawer _emptyDrawer = null;
        private AlteratingListDrawer _listDrawer;

        public ListDrawer(IEnumerable<T> list, Func<T, IDrawer> CreateDrawerDelegate, ListOrientation orientation)
        {
            _listDrawer = new AlteratingListDrawer(orientation);
            foreach (var item in list)
            {
                _listDrawer.AddDrawer(CreateDrawerDelegate.Invoke(item));
            }
        }

        public ListDrawer(IEnumerable<T> list, Func<T, IDrawer> CreateDrawerDelegate) : this(list, CreateDrawerDelegate, ListOrientation.Vertical)
        {

        }

        // will draw this instead when list size == 0
        public ListDrawer<T> SetEmptyDrawer(IDrawer drawer)
        {
            _emptyDrawer = drawer;
            return this;
        }

        public void Draw()
        {
            if (_listDrawer.Count() == 0 && _emptyDrawer != null)
            {
                _emptyDrawer.Draw();
            }
            else
            {
                _listDrawer.Draw();
            }
        }

        // maybe we can from here, into some kinda generic, but still a separate factory
        public static ListDrawer<T> CreateLabelList(IEnumerable<T> list, Func<T, string> labelDlg, Func<T, string> tooltipDlg = null)
        {
            return new ListDrawer<T>(list, item => new DelegatedLabel<T>(item, labelDlg, tooltipDlg));
        }

        public static ListDrawer<T> CreateButtonedLabelList(IEnumerable<T> list, Func<T, string> labelDlg, Func<T, string> tooltipDlg = null, List<ButtonInfo<T>> buttons = null)
        {

            return new ListDrawer<T>(list, item =>
            {
                var sequence = new HorizontalSequenceDrawer();
                sequence.AddDrawer(new DelegatedLabel<T>(item, labelDlg, tooltipDlg).EnableRichText());

                if (buttons != null)
                {
                    foreach (var buttonInfo in buttons)
                    {
                        sequence.AddDrawer(buttonInfo.CreateButton(item));
                    }
                }
                return sequence;
            });
        }
    }
}