using System;
using HmsPlugin.Button;
using HmsPlugin.Extensions;
using UnityEngine;

namespace HmsPlugin.Window
{
    public class ModalDialog : ModalWindow
    {
        private IDrawer _footer;
        private Action _onOk;
        private Action _onCancel;

        private const int MAX_BUTTON_SIZE = 80;

        public static ModalDialog CreateDialog(string title, Vector2 size, IDrawer contentDrawer, Action OnOk = null, Action onCancel = null)
        {
            var dialog = Create<ModalDialog>(title, size, contentDrawer);
            dialog.SetupTwoButtonFooter(size);
            if (OnOk != null) dialog.SetOkCallback(OnOk);
            if (onCancel != null) dialog.SetCancelCallback(onCancel);

            return dialog;
        }

        public static ModalDialog CreateOneButtonDialog(string title, Vector2 size, IDrawer contentDrawer, Action OnOk = null)
        {
            var dialog = Create<ModalDialog>(title, size, contentDrawer);
            dialog.SetupOneButtonFooter();
            if (OnOk != null) dialog.SetOkCallback(OnOk);

            return dialog;
        }

        private void SetupOneButtonFooter()
        {
            _footer = new VerticalSequenceDrawer(new Space(10), new Button.Button("OK", Ok));
        }

        private void SetupTwoButtonFooter(Vector2 size)
        {
            int buttonSize;
            int spaceSize;

            if (size.x < MAX_BUTTON_SIZE * 2)
            {
                buttonSize = (int)(size.x / 2);
                spaceSize = 0;
            }
            else
            {
                buttonSize = MAX_BUTTON_SIZE;
                spaceSize = (int)(size.x - MAX_BUTTON_SIZE * 2);
            }

            _footer = new HorizontalSequenceDrawer(new Button.Button("Cancel", Cancel).SetWidth(buttonSize), new Space(spaceSize), new Button.Button("OK", Ok).SetWidth(buttonSize));
        }

        private ModalDialog()
        {
        }

        public void SetOkCallback(Action callback)
        {
            _onOk = callback;
        }

        public void SetCancelCallback(Action callback)
        {
            _onCancel = callback;
        }

        protected override void Draw(Rect region)
        {
            base.Draw(region);

            if (_footer != null)
            {
                _footer.Draw();
            }
        }

        protected virtual void Cancel()
        {
            _onCancel.InvokeSafe();
            Close();
        }

        protected virtual void Ok()
        {
            _onOk.InvokeSafe();
            Close();
        }
    }
}