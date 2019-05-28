using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace VIS.ObjectDescription.Editor
{
    internal class GenericStickyNoteEditorBehaviour
    {
        private const float _bodyPadding = 8f;
        private const int _contentMargin = 6;
        private const float _headerHeight = 30f;
        private const float _editButtonWidth = 60f;
        private const float _colorPickerWidth = 18f;
        private const float _closeButtonWidth = 18f;

        private SerializedProperty[] _descriptionPropsCache;
        private SerializedProperty[] _textPropsCache;
        private SerializedProperty[] _colorPropsCache;

        private StickyNoteState[] _states;

        private GUIStyle[] _borderStyles;
        private GUIStyle[] _headerStyles;
        private GUIStyle[] _mainStyles;
        private GUIStyle[] _buttonStyles;
        private GUIStyle[] _descriptionStyles;
        private GUIStyle[] _textStyles;

        private Action _baseOnInspectorGUIAction;
        private Func<int, string, SerializedProperty> _findPropertyFunc;
        private Action<int> _applyModifiedPropertiesAction;
        private Func<int, bool> _needCloseButtonFunc;
        private Action<int> _closeButtonCallbacks;
        private Func<bool> _needToDrawBaseInspectorFunc;
        private Func<int> _notesCountFunc;
        private Func<int, Object> _getTargetFunc;
        private Action _repaintAction;

        internal GenericStickyNoteEditorBehaviour(
            Action baseOnInspectorGUIAction,
            Func<int, string, SerializedProperty> findPropertyFunc,
            Action<int> applyModifiedPropertiesAction,
            Func<int, bool> needCloseButtonFunc,
            Action<int> closeButtonCallbacks,
            Func<bool> needToDrawBaseInspectorFunc,
            Func<int> notesCountFunc,
            Func<int, Object> getTargetFunc,
            Action repaintAction)
        {
            _baseOnInspectorGUIAction = baseOnInspectorGUIAction;
            _findPropertyFunc = findPropertyFunc;
            _applyModifiedPropertiesAction = applyModifiedPropertiesAction;
            _needToDrawBaseInspectorFunc = needToDrawBaseInspectorFunc;
            _notesCountFunc = notesCountFunc;
            _needCloseButtonFunc = needCloseButtonFunc;
            _closeButtonCallbacks = closeButtonCallbacks;
            _getTargetFunc = getTargetFunc;
            _repaintAction = repaintAction;
        }

        internal void OnEnable()
        {
            var count = _notesCountFunc();

            _descriptionPropsCache = new SerializedProperty[count];
            _textPropsCache = new SerializedProperty[count];
            _colorPropsCache = new SerializedProperty[count];

            _borderStyles = new GUIStyle[count];
            _headerStyles = new GUIStyle[count];
            _mainStyles = new GUIStyle[count];
            _buttonStyles = new GUIStyle[count];
            _descriptionStyles = new GUIStyle[count];
            _textStyles = new GUIStyle[count];

            _states = new StickyNoteState[count];

            for (int i = 0; i < count; i++)
            {
                _descriptionPropsCache[i] = _findPropertyFunc(i, "_headerText");
                _textPropsCache[i] = _findPropertyFunc(i, "_text");
                _colorPropsCache[i] = _findPropertyFunc(i, "_color");

                _borderStyles[i] = new GUIStyle();
                _borderStyles[i].normal.background = new Texture2D(1, 1);
                _borderStyles[i].normal.background.SetPixel(0, 0, Color.black);
                _borderStyles[i].normal.background.Apply();

                _mainStyles[i] = new GUIStyle();
                _mainStyles[i].normal.background = new Texture2D(1, 1);
                _headerStyles[i] = new GUIStyle();
                _headerStyles[i].normal.background = new Texture2D(1, 1);

                _descriptionStyles[i] = new GUIStyle();
                _textStyles[i] = new GUIStyle();

                _descriptionStyles[i].richText = true;
                _textStyles[i].richText = true;

                _descriptionStyles[i].fontSize = 20;
                _textStyles[i].fontSize = 16;
                _textStyles[i].wordWrap = true;
            }
        }

        internal void OnDisable()
        {
            _textPropsCache = null;
            _colorPropsCache = null;
            _descriptionPropsCache = null;

            _borderStyles = null;
            _mainStyles = null;
            _headerStyles = null;
            _buttonStyles = null;
            _descriptionStyles = null;
            _textStyles = null;

            _states = null;
        }

        internal void OnInspectorGUI()
        {
            if (_needToDrawBaseInspectorFunc())
                _baseOnInspectorGUIAction();

            var count = _notesCountFunc();
            for (int i = 0; i < count; i++)
            {
                _buttonStyles[i] = new GUIStyle(GUI.skin.button);
                _buttonStyles[i].normal.background = new Texture2D(1, 1);
                _buttonStyles[i].active.background = new Texture2D(1, 1);

                _mainStyles[i].normal.background.SetPixel(0, 0, _colorPropsCache[i].colorValue);
                _mainStyles[i].normal.background.Apply();
                _headerStyles[i].normal.background.SetPixel(0, 0, _colorPropsCache[i].colorValue * 0.9f);
                _headerStyles[i].normal.background.Apply();
                _buttonStyles[i].normal.background.SetPixel(0, 0, _colorPropsCache[i].colorValue * 0.9f);
                _buttonStyles[i].normal.background.Apply();
                _buttonStyles[i].active.background.SetPixel(0, 0, _colorPropsCache[i].colorValue * 0.9f);
                _buttonStyles[i].active.background.Apply();

                _textStyles[i].margin = new RectOffset(_contentMargin, _contentMargin, _contentMargin, _contentMargin);
                var assumedTextRect = GUILayoutUtility.GetRect(new GUIContent(_textPropsCache[i].stringValue), _textStyles[i], GUILayout.ExpandWidth(false));
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
                descriptionRect.width -= _contentMargin * 4 + _editButtonWidth + _colorPickerWidth + (_needCloseButtonFunc(i) ? _closeButtonWidth + _contentMargin * 2f : 0);

                var editButtonRect = headerRect;
                editButtonRect.x = editButtonRect.x + editButtonRect.width - _editButtonWidth - _contentMargin * 1 - (_needCloseButtonFunc(i) ? _closeButtonWidth + _contentMargin * 2f : 0);
                editButtonRect.width = _editButtonWidth;
                editButtonRect.y += _contentMargin;
                editButtonRect.height -= _contentMargin * 2;

                var colorPickerRect = headerRect;
                colorPickerRect.x = colorPickerRect.x + colorPickerRect.width - _editButtonWidth - _colorPickerWidth - _contentMargin * 2 - (_needCloseButtonFunc(i) ? _closeButtonWidth + _contentMargin * 2f : 0);
                colorPickerRect.width = _colorPickerWidth;
                colorPickerRect.y += _contentMargin;
                colorPickerRect.height -= _contentMargin * 2;

                var closeButtonRect = editButtonRect;
                closeButtonRect.width = _closeButtonWidth;
                closeButtonRect.x += editButtonRect.width + _contentMargin;

                var textRect = mainRect;
                textRect.x += _contentMargin;
                textRect.y += _contentMargin;
                textRect.height -= _contentMargin * 2;
                textRect.width -= _contentMargin * 2;

                var e = Event.current;

                switch (_states[i])
                {
                    case StickyNoteState.View:
                        GUI.Box(borderRect, string.Empty, _borderStyles[i]);
                        GUI.Box(mainRect, string.Empty, _mainStyles[i]);
                        GUI.Box(headerRect, string.Empty, _headerStyles[i]);

                        GUI.Label(textRect, _textPropsCache[i].stringValue, _textStyles[i]);

                        GUI.Label(descriptionRect, _descriptionPropsCache[i].stringValue, _descriptionStyles[i]);

                        if (GUI.Button(editButtonRect, "Edit", _buttonStyles[i]))
                            _states[i] = StickyNoteState.Edit;

                        if (_needCloseButtonFunc(i) && GUI.Button(closeButtonRect, "x", _buttonStyles[i]))
                            _closeButtonCallbacks?.Invoke(i);

                        if (e.modifiers == EventModifiers.Control || e.modifiers == EventModifiers.Command)
                        {
                            switch (e.keyCode)
                            {
                                case KeyCode.E:
                                    _states[i] = StickyNoteState.Edit;
                                    _repaintAction();
                                    break;
                            }
                        }
                        break;
                    case StickyNoteState.Edit:
                        GUI.Box(borderRect, string.Empty, _borderStyles[i]);
                        GUI.Box(mainRect, string.Empty, _mainStyles[i]);
                        GUI.Box(headerRect, string.Empty, _headerStyles[i]);

                        _textPropsCache[i].stringValue = GUI.TextArea(textRect, _textPropsCache[i].stringValue);

                        _colorPropsCache[i].colorValue = EditorGUI.ColorField(colorPickerRect, GUIContent.none, _colorPropsCache[i].colorValue, false, false, false);
                        _descriptionPropsCache[i].stringValue = GUI.TextField(descriptionRect, _descriptionPropsCache[i].stringValue);

                        _applyModifiedPropertiesAction(i);

                        if (GUI.Button(editButtonRect, "Back", _buttonStyles[i]))
                            _states[i] = StickyNoteState.View;

                        if (_needCloseButtonFunc(i) && GUI.Button(closeButtonRect, "x", _buttonStyles[i]))
                            _closeButtonCallbacks?.Invoke(i);

                        if (e.modifiers == EventModifiers.Control || e.modifiers == EventModifiers.Command)
                        {
                            switch (e.keyCode)
                            {
                                case KeyCode.Return:
                                case KeyCode.KeypadEnter:
                                    _states[i] = StickyNoteState.View;
                                    _repaintAction();
                                    break;
                            }
                        }
                        break;
                }

                var target = _getTargetFunc(i);
                //if (target == null)
                //    Debug.Log("target == null");
                //else
                //    Debug.Log($"target = type {target.GetType().FullName}, name {target.name}");
                if (target != null)
                {
                    if (target is Component)
                        UnityEditorInternal.ComponentUtility.MoveComponentDown(target as Component);
                    //else if (target is StateMachineBehaviour) //Эта фигня не работает, а отдельной Utility для StateMachineBehaviour'в нет
                    //{
                    //    Debug.Log($"StateMachinBeh. {target.name}");
                    //    var smb = target as StateMachineBehaviour;
                    //    var path = AssetDatabase.GetAssetPath(smb);
                    //    var mainAsset = AssetDatabase.LoadMainAssetAtPath(path);
                    //    var allSubAssets = AssetDatabase.LoadAllAssetsAtPath(path);
                    //    for (int j = 0; j < allSubAssets.Length; j++)
                    //    {
                    //        if (allSubAssets[j] == smb && j != allSubAssets.Length - 1)
                    //        {
                    //            AssetDatabase.RemoveObjectFromAsset(smb);
                    //            AssetDatabase.AddObjectToAsset(smb, mainAsset);
                    //            EditorUtility.SetDirty(mainAsset);
                    //            AssetDatabase.SaveAssets();
                    //            break;
                    //        }
                    //    }
                    //}
                }
            }
        }
    }
}
