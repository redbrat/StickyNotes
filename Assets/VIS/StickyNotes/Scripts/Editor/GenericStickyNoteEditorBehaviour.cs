using System;
using UnityEditor;
using UnityEngine;

namespace VIS.ObjectDescription.Editor
{
    internal class GenericStickyNoteEditorBehaviour
    {
        private const float _bodyPadding = 8f;
        private const int _contentMargin = 6;
        private const float _headerHeight = 30f;
        private const float _editButtonWidth = 60f;
        private const float _colorPickerWidth = 18f;

        private SerializedProperty _descriptionPropCache;
        private SerializedProperty _textPropCache;
        private SerializedProperty _colorPropCache;

        private StickyNoteState _state;

        private GUIStyle _borderStyle;
        private GUIStyle _headerStyle;
        private GUIStyle _mainStyle;
        private GUIStyle _buttonStyle;
        private GUIStyle _descriptionStyle;
        private GUIStyle _textStyle;

        private Action _baseOnInspectorGUIAction;
        private Func<string, SerializedProperty> _findPropertyFunc;
        private Action _applyModifiedPropertiesAction;
        private Func<bool> _needToDrawBaseInspectorFunc;

        internal GenericStickyNoteEditorBehaviour(
            Action baseOnInspectorGUIAction,
            Func<string, SerializedProperty> findPropertyFunc,
            Action applyModifiedPropertiesAction,
            Func<bool> needToDrawBaseInspectorFunc)
        {
            _baseOnInspectorGUIAction = baseOnInspectorGUIAction;
            _findPropertyFunc = findPropertyFunc;
            _applyModifiedPropertiesAction = applyModifiedPropertiesAction;
            _needToDrawBaseInspectorFunc = needToDrawBaseInspectorFunc;
        }

        internal void OnEnable()
        {
            _descriptionPropCache = _findPropertyFunc("_headerText");
            _textPropCache = _findPropertyFunc("_text");
            _colorPropCache = _findPropertyFunc("_color");

            _borderStyle = new GUIStyle();
            _borderStyle.normal.background = new Texture2D(1, 1);
            _borderStyle.normal.background.SetPixel(0, 0, Color.black);
            _borderStyle.normal.background.Apply();

            _mainStyle = new GUIStyle();
            _mainStyle.normal.background = new Texture2D(1, 1);
            _headerStyle = new GUIStyle();
            _headerStyle.normal.background = new Texture2D(1, 1);

            _descriptionStyle = new GUIStyle();
            _textStyle = new GUIStyle();

            _descriptionStyle.richText = true;
            _textStyle.richText = true;

            _descriptionStyle.fontSize = 20;
            _textStyle.fontSize = 16;
            _textStyle.wordWrap = true;
        }

        internal void OnDisable()
        {
            _textPropCache = null;
            _colorPropCache = null;
            _descriptionPropCache = null;

            _borderStyle = null;
            _mainStyle = null;
            _headerStyle = null;
            _buttonStyle = null;
            _descriptionStyle = null;
            _textStyle = null;

            _state = StickyNoteState.View;
        }

        internal void OnInspectorGUI()
        {
            if (_needToDrawBaseInspectorFunc())
                _baseOnInspectorGUIAction();

            _buttonStyle = new GUIStyle(GUI.skin.button);
            _buttonStyle.normal.background = new Texture2D(1, 1);
            _buttonStyle.active.background = new Texture2D(1, 1);

            _mainStyle.normal.background.SetPixel(0, 0, _colorPropCache.colorValue);
            _mainStyle.normal.background.Apply();
            _headerStyle.normal.background.SetPixel(0, 0, _colorPropCache.colorValue * 0.9f);
            _headerStyle.normal.background.Apply();
            _buttonStyle.normal.background.SetPixel(0, 0, _colorPropCache.colorValue * 0.9f);
            _buttonStyle.normal.background.Apply();
            _buttonStyle.active.background.SetPixel(0, 0, _colorPropCache.colorValue * 0.9f);
            _buttonStyle.active.background.Apply();

            _textStyle.margin = new RectOffset(_contentMargin, _contentMargin, _contentMargin, _contentMargin);
            var assumedTextRect = GUILayoutUtility.GetRect(new GUIContent(_textPropCache.stringValue), _textStyle, GUILayout.ExpandWidth(false));
            var borderRect = assumedTextRect;
            borderRect.x = _bodyPadding;
            borderRect.width = EditorGUIUtility.currentViewWidth - _bodyPadding * 2f;
            var otherStuff = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth - _bodyPadding * 2f, _contentMargin * 2f + _headerHeight);
            borderRect.height += otherStuff.size.y;

            var mainRect = borderRect;
            mainRect.x += 1;
            mainRect.y += 1 + _headerHeight;
            mainRect.width -= 2;
            mainRect.height -= 2 + _headerHeight;

            var headerRect = mainRect;
            headerRect.y -= _headerHeight;
            headerRect.height = _headerHeight;

            var descriptionRect = headerRect;
            descriptionRect.x += _contentMargin;
            descriptionRect.y += _contentMargin;
            descriptionRect.height -= _contentMargin * 2;
            descriptionRect.width -= _contentMargin * 4 + _editButtonWidth + _colorPickerWidth;

            var editButtonRect = headerRect;
            editButtonRect.x = editButtonRect.x + editButtonRect.width - _editButtonWidth - _contentMargin * 1;
            editButtonRect.width = _editButtonWidth;
            editButtonRect.y += _contentMargin;
            editButtonRect.height -= _contentMargin * 2;

            var colorPickerRect = headerRect;
            colorPickerRect.x = colorPickerRect.x + colorPickerRect.width - _editButtonWidth - _colorPickerWidth - _contentMargin * 2;
            colorPickerRect.width = _colorPickerWidth;
            colorPickerRect.y += _contentMargin;
            colorPickerRect.height -= _contentMargin * 2;

            var textRect = mainRect;
            textRect.x += _contentMargin;
            textRect.y += _contentMargin;
            textRect.height -= _contentMargin * 2;
            textRect.width -= _contentMargin * 2;

            switch (_state)
            {
                case StickyNoteState.View:
                    GUI.Box(borderRect, string.Empty, _borderStyle);
                    GUI.Box(mainRect, string.Empty, _mainStyle);
                    GUI.Box(headerRect, string.Empty, _headerStyle);

                    GUI.Label(textRect, _textPropCache.stringValue, _textStyle);

                    GUI.Label(descriptionRect, _descriptionPropCache.stringValue, _descriptionStyle);

                    if (GUI.Button(editButtonRect, "Edit", _buttonStyle))
                        _state = StickyNoteState.Edit;
                    break;
                case StickyNoteState.Edit:
                    GUI.Box(borderRect, string.Empty, _borderStyle);
                    GUI.Box(mainRect, string.Empty, _mainStyle);
                    GUI.Box(headerRect, string.Empty, _headerStyle);

                    _textPropCache.stringValue = GUI.TextArea(textRect, _textPropCache.stringValue);

                    _colorPropCache.colorValue = EditorGUI.ColorField(colorPickerRect, GUIContent.none, _colorPropCache.colorValue, false, false, false);
                    _descriptionPropCache.stringValue = GUI.TextField(descriptionRect, _descriptionPropCache.stringValue);

                    _applyModifiedPropertiesAction();

                    if (GUI.Button(editButtonRect, "Back", _buttonStyle))
                        _state = StickyNoteState.View;
                    break;
                default:
                    break;
            }
        }
    }
}
