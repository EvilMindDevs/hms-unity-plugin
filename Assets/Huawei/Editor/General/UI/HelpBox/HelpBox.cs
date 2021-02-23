using UnityEditor;

namespace HmsPlugin.HelpBox
{
    public class HelpBox : IDrawer
    {
        private MessageType _type;
        private string _text;

        private int? _fontSize = null;

        public HelpBox(string message, MessageType type)
        {
            _text = message;
            _type = type;
        }

        public HelpBox(string message, MessageType type, int fontSize)
        {
            _text = message;
            _type = type;
            _fontSize = fontSize;
        }

        public void Draw()
        {
            int prevFontSize = 12;
            if (_fontSize.HasValue)
            {
                prevFontSize = EditorStyles.helpBox.fontSize;
                EditorStyles.helpBox.fontSize = _fontSize.Value;
            }

            EditorGUILayout.HelpBox(_text, _type);

            if (_fontSize.HasValue)
            {
                EditorStyles.helpBox.fontSize = prevFontSize;
            }
        }
    }
}