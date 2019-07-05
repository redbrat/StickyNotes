using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace VIS.StickyNotes.Editor
{
    internal class GenericStickyNoteEditorBehaviour
    {
        private const float _bodyPadding = 8f;
        private const int _contentMargin = 6;
        private const float _headerHeight = 40f;
        private const float _consoleHeight = 30f;
        private const float _consoleEnterButtonWidth = 80f;
        private const float _editButtonWidth = 60f;
        private const float _colorPickerWidth = 30f;
        private const float _closeButtonWidth = 22f;

        private SerializedProperty[] _descriptionPropsCache;
        private SerializedProperty[] _textPropsCache;
        private SerializedProperty[] _colorPropsCache;
        private SerializedProperty[] _modePropsCache;
        private SerializedProperty[] _consoleTextPropsCache;

        private StickyNoteState[] _states;

        private readonly GUIStyle _borderStyles;
        private readonly GUIStyle _headerStyles;
        private readonly GUIStyle _mainStyles;
        private readonly GUIStyle _buttonStyles;
        private readonly GUIStyle _descriptionStyles;
        private readonly GUIStyle _labelStyles;
        private readonly GUIStyle _textAreaStyles;
        private readonly GUIStyle _topPanelStyle;

        private Rect[] _rects;

        private Action _baseOnInspectorGUIAction;
        private Func<int, string, SerializedProperty> _findPropertyFunc;
        private Action<int> _applyModifiedPropertiesAction;
        private Func<int, bool> _needCloseButtonFunc;
        private Action<int> _closeButtonCallbacks;
        private Func<bool> _needToDrawBaseInspectorFunc;
        private Func<int> _notesCountFunc;
        private Func<int, Object> _getTargetFunc;
        private Action _repaintAction;
        private Func<int, SerializedObject> _getSerializedObject;

        private GUISkin _skin;
        private Texture _closeTexture;

        internal GenericStickyNoteEditorBehaviour(
            Action baseOnInspectorGUIAction,
            Func<int, string, SerializedProperty> findPropertyFunc,
            Action<int> applyModifiedPropertiesAction,
            Func<int, bool> needCloseButtonFunc,
            Action<int> closeButtonCallbacks,
            Func<bool> needToDrawBaseInspectorFunc,
            Func<int> notesCountFunc,
            Func<int, Object> getTargetFunc,
            Action repaintAction,
            Func<int, SerializedObject> getSerializedObject)
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
            _getSerializedObject = getSerializedObject;

            _skin = Resources.Load<GUISkin>($"Vis/StickyNotes/StickyNoteGuiSkin");
            _closeTexture = Resources.Load<Texture>($"Vis/StickyNotes/Textures/close-icon");

            _borderStyles = _skin.GetStyle($"Border");
            _headerStyles = _skin.GetStyle($"Header");
            _mainStyles = _skin.GetStyle($"Main");
            _buttonStyles = _skin.GetStyle($"Button");
            _descriptionStyles = _skin.GetStyle($"Description");
            _labelStyles = _skin.GetStyle($"Label");
            _textAreaStyles = _skin.GetStyle($"TextArea");
            _topPanelStyle = _skin.GetStyle($"TopPanel");

            _labelStyles.margin = new RectOffset(_contentMargin * 2, _contentMargin * 2, _contentMargin, _contentMargin);
        }

        internal void OnEnable()
        {
            if (_skin == null)
                return;
            var count = _notesCountFunc();

            _descriptionPropsCache = new SerializedProperty[count];
            _textPropsCache = new SerializedProperty[count];
            _colorPropsCache = new SerializedProperty[count];
            _modePropsCache = new SerializedProperty[count];
            _consoleTextPropsCache = new SerializedProperty[count];

            //_borderStyles = new GUIStyle[count];
            //_headerStyles = new GUIStyle[count];
            //_mainStyles = new GUIStyle[count];
            //_buttonStyles = new GUIStyle[count];
            //_descriptionStyles = new GUIStyle[count];
            //_labelStyles = new GUIStyle[count];
            //_textAreaStyles = new GUIStyle[count];

            _rects = new Rect[count];

            _states = new StickyNoteState[count];

            for (int i = 0; i < count; i++)
            {
                _descriptionPropsCache[i] = _findPropertyFunc(i, "_headerText");
                _textPropsCache[i] = _findPropertyFunc(i, "_text");
                _colorPropsCache[i] = _findPropertyFunc(i, "_color");
                _modePropsCache[i] = _findPropertyFunc(i, "_mode");
                _consoleTextPropsCache[i] = _findPropertyFunc(i, "_consoleText");

                //_borderStyles[i] = new GUIStyle();
                //_borderStyles[i].normal.background = registerDestroyableObject(new Texture2D(1, 1));
                //_borderStyles[i].normal.background.SetPixel(0, 0, Color.white * 0.3f);
                //_borderStyles[i].normal.background.Apply();

                //_mainStyles[i] = new GUIStyle();
                //_mainStyles[i].normal.background = registerDestroyableObject(new Texture2D(1, 1));
                //_headerStyles[i] = new GUIStyle();
                //_headerStyles[i].normal.background = registerDestroyableObject(new Texture2D(1, 1));

                //_descriptionStyles[i] = new GUIStyle(_skin.label);
                //_labelStyles[i] = new GUIStyle(_skin.label);
                //_textAreaStyles[i] = new GUIStyle(_skin.textArea);

                //_descriptionStyles[i].fontSize = 20;
                //_descriptionStyles[i].padding.left = 6;
            }
        }

        private List<Object> _destroyable = new List<Object>();
        private T registerDestroyableObject<T>(T obj) where T : Object
        {
            _destroyable.Add(obj);
            return obj;
        }

        internal void OnDisable()
        {
            for (int i = 0; i < _destroyable.Count; i++)
                Object.DestroyImmediate(_destroyable[i]);
            _destroyable.Clear();

            _textPropsCache = null;
            _colorPropsCache = null;
            _descriptionPropsCache = null;

            //_borderStyles = null;
            //_mainStyles = null;
            //_headerStyles = null;
            //_buttonStyles = null;
            //_descriptionStyles = null;
            //_labelStyles = null;
            //_textAreaStyles = null;
            _skin = null;
            _closeTexture = null;

            _states = null;
            _rects = null;
        }

        internal void OnInspectorGUI()
        {
            if (_needToDrawBaseInspectorFunc())
                _baseOnInspectorGUIAction();

            if (_skin == null)
                return;
            var count = _notesCountFunc();
            for (int i = 0; i < count; i++)
            {
                //_buttonStyles[i] = new GUIStyle(_skin.button);
                //_buttonStyles[i].normal.background = registerDestroyableObject(new Texture2D(1, 1));
                //_buttonStyles[i].active.background = registerDestroyableObject(new Texture2D(1, 1));
                //_buttonStyles[i].hover.background = registerDestroyableObject(new Texture2D(1, 1));

                //_mainStyles[i].normal.background.SetPixel(0, 0, _colorPropsCache[i].colorValue);
                //_mainStyles[i].normal.background.Apply();
                //_headerStyles[i].normal.background.SetPixel(0, 0, _colorPropsCache[i].colorValue * 0.9f);
                //_headerStyles[i].normal.background.Apply();
                //_buttonStyles[i].normal.background.SetPixel(0, 0, _colorPropsCache[i].colorValue * 0.8f);
                //_buttonStyles[i].normal.background.Apply();
                //_buttonStyles[i].active.background.SetPixel(0, 0, _colorPropsCache[i].colorValue * 0.6f);
                //_buttonStyles[i].active.background.Apply();
                //_buttonStyles[i].hover.background.SetPixel(0, 0, _colorPropsCache[i].colorValue * 0.7f);
                //_buttonStyles[i].hover.background.Apply();

                //_labelStyles[i].margin = new RectOffset(_contentMargin * 2, _contentMargin * 2, _contentMargin, _contentMargin);

                //if (Event.current.type == EventType.Repaint)
                //    _rects[i] = GUILayoutUtility.GetRect(new GUIContent(_textPropsCache[i].stringValue), _labelStyles[i], GUILayout.ExpandWidth(false));

                var borderRect = GUILayoutUtility.GetRect(new GUIContent(_textPropsCache[i].stringValue), _labelStyles, GUILayout.ExpandWidth(false));
                borderRect.x = _bodyPadding;
                borderRect.width = EditorGUIUtility.currentViewWidth - _bodyPadding * 4f;
                var otherStuff = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth - _bodyPadding * 2f, _contentMargin * 2f + _headerHeight);
                borderRect.height += otherStuff.size.y;

                var consoleRect = default(Rect);
                if (_modePropsCache[i].enumValueIndex == (int)StickyNoteMode.Console)
                {
                    consoleRect = GUILayoutUtility.GetRect(GUIContent.none, _textAreaStyles/*, GUILayout.ExpandWidth(false)*/);
                    borderRect.height += consoleRect.size.y;
                    consoleRect.size += Vector2.right * 3f;
                    consoleRect.position -= Vector2.right * 5f + Vector2.up * 6f;
                }

                var mainRect = borderRect;
                mainRect.x += 1;
                mainRect.y += 1 + _headerHeight;
                mainRect.width -= 2;
                mainRect.height -= 2 + _headerHeight;

                var headerRect = mainRect;
                headerRect.y -= _headerHeight;
                headerRect.height = _headerHeight;

                //var descriptionRect = headerRect;
                //descriptionRect.x += _contentMargin;
                //descriptionRect.y += _contentMargin;
                //descriptionRect.height -= _contentMargin * 2;
                //descriptionRect.width -= _contentMargin * 4 + _editButtonWidth + _colorPickerWidth + (_needCloseButtonFunc(i) ? _closeButtonWidth + _contentMargin * 2f : 0);

                //var editButtonRect = headerRect;
                //editButtonRect.x = editButtonRect.x + editButtonRect.width - _editButtonWidth - _contentMargin * 1 - (_needCloseButtonFunc(i) ? _closeButtonWidth + _contentMargin * 2f : 0);
                //editButtonRect.width = _editButtonWidth;
                //editButtonRect.y += _contentMargin;
                //editButtonRect.height -= _contentMargin * 2;

                //var colorPickerRect = headerRect;
                //colorPickerRect.x = colorPickerRect.x + colorPickerRect.width - _editButtonWidth - _colorPickerWidth - _contentMargin * 2 - (_needCloseButtonFunc(i) ? _closeButtonWidth + _contentMargin * 2f : 0);
                //colorPickerRect.width = _colorPickerWidth;
                //colorPickerRect.y += _contentMargin;
                //colorPickerRect.height -= _contentMargin * 2;

                //var closeButtonRect = editButtonRect;
                //closeButtonRect.width = _closeButtonWidth;
                //closeButtonRect.x += editButtonRect.width + _contentMargin;

                var textRect = mainRect;
                textRect.x += _contentMargin;
                textRect.y += _contentMargin;
                textRect.height -= _contentMargin * 2;
                textRect.width -= _contentMargin * 2;

                var e = Event.current;

                Handles.BeginGUI();
                switch (_states[i])
                {
                    case StickyNoteState.View:
                        {
                            Handles.DrawSolidRectangleWithOutline(borderRect, _colorPropsCache[i].colorValue, Color.white * 0.3f);
                            Handles.DrawSolidRectangleWithOutline(headerRect, _colorPropsCache[i].colorValue * 0.9f, _colorPropsCache[i].colorValue * 0.9f);
                            //GUI.Box(borderRect, string.Empty, _borderStyles);
                            //GUI.Box(mainRect, string.Empty, _mainStyles);
                            //GUI.Box(headerRect, string.Empty, _headerStyles);

                            GUI.Label(textRect, _textPropsCache[i].stringValue, _labelStyles);

                            if (_modePropsCache[i].enumValueIndex == (int)StickyNoteMode.Console)
                            {
                                //var consoleTextRect = consoleRect;
                                //consoleTextRect.size -= (_consoleEnterButtonWidth + 4) * Vector2.right;
                                //var consoleButtonRect = consoleRect;
                                //consoleButtonRect.position = consoleTextRect.position + consoleTextRect.size * Vector2.right + Vector2.right * 4;
                                //consoleButtonRect.size = new Vector2(_consoleEnterButtonWidth, consoleButtonRect.height);

                                if (Event.current.isKey &&
                                    Event.current.type == EventType.KeyDown &&
                                    (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter))
                                {
                                    if (GUI.GetNameOfFocusedControl() == "Console")
                                        enterConsoleText(i);
                                }

                                GUI.SetNextControlName("Console");
                                Handles.DrawSolidRectangleWithOutline(/*consoleTextRect*/consoleRect, _colorPropsCache[i].colorValue * 0.7f, _colorPropsCache[i].colorValue * 0.7f);
                                _consoleTextPropsCache[i].stringValue = EditorGUI.TextField(/*consoleTextRect*/consoleRect, _consoleTextPropsCache[i].stringValue, _textAreaStyles);

                                //Handles.DrawSolidRectangleWithOutline(consoleButtonRect, _colorPropsCache[i].colorValue * 0.8f, _colorPropsCache[i].colorValue * 0.7f);
                                //if (GUI.Button(consoleButtonRect, new GUIContent($"Enter"), _buttonStyles))
                                //    enterConsoleText(i);
                            }

                            GUILayout.BeginArea(headerRect);
                            GUILayout.BeginHorizontal(_topPanelStyle);
                            GUILayout.Label(_descriptionPropsCache[i].stringValue, _descriptionStyles);
                            GUILayout.FlexibleSpace();
                            var buttonContent = default(GUIContent);
                            var lastRect = default(Rect);
                            var buttonRect = default(Rect);

                            if (_modePropsCache[i].enumValueIndex == (int)StickyNoteMode.Console)
                            {
                                buttonContent = new GUIContent("Clear");
                                lastRect = GUILayoutUtility.GetLastRect();
                                buttonRect = new Rect(lastRect.position + Vector2.Scale(lastRect.size, Vector2.right), _buttonStyles.CalcSize(buttonContent));
                                Handles.DrawSolidRectangleWithOutline(buttonRect, _colorPropsCache[i].colorValue * 0.8f, _colorPropsCache[i].colorValue * 0.7f);
                                if (GUILayout.Button(buttonContent, _buttonStyles))
                                    _textPropsCache[i].stringValue = string.Empty;

                                buttonContent = new GUIContent("Copy");
                                lastRect = GUILayoutUtility.GetLastRect();
                                buttonRect = new Rect(lastRect.position + Vector2.Scale(lastRect.size, Vector2.right), _buttonStyles.CalcSize(buttonContent));
                                Handles.DrawSolidRectangleWithOutline(buttonRect, _colorPropsCache[i].colorValue * 0.8f, _colorPropsCache[i].colorValue * 0.7f);
                                if (GUILayout.Button(buttonContent, _buttonStyles))
                                    EditorGUIUtility.systemCopyBuffer = _textPropsCache[i].stringValue;
                            }

                            buttonContent = new GUIContent("Edit");
                            lastRect = GUILayoutUtility.GetLastRect();
                            buttonRect = new Rect(lastRect.position + Vector2.Scale(lastRect.size, Vector2.right), _buttonStyles.CalcSize(buttonContent));
                            Handles.DrawSolidRectangleWithOutline(buttonRect, _colorPropsCache[i].colorValue * 0.8f, _colorPropsCache[i].colorValue * 0.7f);
                            if (GUILayout.Button(buttonContent, _buttonStyles))
                                _states[i] = StickyNoteState.Edit;
                            if (_needCloseButtonFunc(i))
                            {
                                buttonContent = new GUIContent(_closeTexture, "Remove note");
                                buttonRect = new Rect(buttonRect.position + Vector2.Scale(buttonRect.size, Vector2.right), _buttonStyles.CalcSize(buttonContent));
                                Handles.DrawSolidRectangleWithOutline(buttonRect, _colorPropsCache[i].colorValue * 0.8f, _colorPropsCache[i].colorValue * 0.7f);
                                if (GUI.Button(buttonRect, buttonContent, _buttonStyles))
                                    _closeButtonCallbacks?.Invoke(i);
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.EndArea();

                            //GUI.Label(descriptionRect, _descriptionPropsCache[i].stringValue, _descriptionStyles);

                            //if (GUI.Button(editButtonRect, "Edit", _buttonStyles))
                            //    _states[i] = StickyNoteState.Edit;

                            //if (_needCloseButtonFunc(i) && GUI.Button(closeButtonRect, new GUIContent(_closeTexture, "Remove note"), _buttonStyles))
                            //    _closeButtonCallbacks?.Invoke(i);

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

                            if (_modePropsCache[i].enumValueIndex == (int)StickyNoteMode.Console)
                                _applyModifiedPropertiesAction(i);
                        }
                        break;
                    case StickyNoteState.Edit:
                        {
                            Handles.BeginGUI();
                            Handles.DrawSolidRectangleWithOutline(borderRect, Color.white * 0.3f, _colorPropsCache[i].colorValue);
                            Handles.DrawSolidRectangleWithOutline(headerRect, Color.white * 0.3f, _colorPropsCache[i].colorValue * 0.9f);
                            Handles.EndGUI();
                            //GUI.Box(borderRect, string.Empty, _borderStyles);
                            //GUI.Box(mainRect, string.Empty, _mainStyles);
                            //GUI.Box(headerRect, string.Empty, _headerStyles);

                            _textPropsCache[i].stringValue = EditorGUI.TextArea(textRect, _textPropsCache[i].stringValue, _textAreaStyles);


                            GUILayout.BeginArea(headerRect);
                            GUILayout.BeginHorizontal(_topPanelStyle);
                            _descriptionPropsCache[i].stringValue = EditorGUILayout.TextField(_descriptionPropsCache[i].stringValue, _textAreaStyles);
                            GUILayout.FlexibleSpace();
                            _colorPropsCache[i].colorValue = EditorGUILayout.ColorField(GUIContent.none, _colorPropsCache[i].colorValue/*, false, false, false*/, GUILayout.MaxHeight(30f), GUILayout.MaxWidth(30f));
                            _applyModifiedPropertiesAction(i);
                            GUILayout.Space(4f);

                            var buttonContent = new GUIContent("Back");
                            var lastRect = GUILayoutUtility.GetLastRect();
                            var buttonRect = new Rect(lastRect.position + Vector2.Scale(lastRect.size, Vector2.right), _buttonStyles.CalcSize(buttonContent));
                            Handles.DrawSolidRectangleWithOutline(buttonRect, _colorPropsCache[i].colorValue * 0.8f, _colorPropsCache[i].colorValue * 0.7f);
                            if (GUILayout.Button(buttonContent, _buttonStyles))
                                _states[i] = StickyNoteState.View;
                            if (_needCloseButtonFunc(i))
                            {
                                buttonContent = new GUIContent(_closeTexture, "Remove note");
                                buttonRect = new Rect(buttonRect.position + Vector2.Scale(buttonRect.size, Vector2.right), _buttonStyles.CalcSize(buttonContent));
                                Handles.DrawSolidRectangleWithOutline(buttonRect, _colorPropsCache[i].colorValue * 0.8f, _colorPropsCache[i].colorValue * 0.7f);
                                if (GUI.Button(buttonRect, buttonContent, _buttonStyles))
                                    _closeButtonCallbacks?.Invoke(i);
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.EndArea();

                            //_colorPropsCache[i].colorValue = EditorGUI.ColorField(colorPickerRect, GUIContent.none, _colorPropsCache[i].colorValue, false, false, false/*, new ColorPickerHDRConfig(0, 0, 0, 0)*/);
                            //_descriptionPropsCache[i].stringValue = EditorGUI.TextField(descriptionRect, _descriptionPropsCache[i].stringValue, _textAreaStyles);

                            _applyModifiedPropertiesAction(i);

                            //if (GUI.Button(editButtonRect, "Back", _buttonStyles))
                            //    _states[i] = StickyNoteState.View;

                            //if (_needCloseButtonFunc(i) && GUI.Button(closeButtonRect, new GUIContent(_closeTexture, "Remove note"), _buttonStyles))
                            //    _closeButtonCallbacks?.Invoke(i);

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
                        }
                        break;
                }
                Handles.EndGUI();

                var target = _getTargetFunc(i);
                if (target != null)
                {
                    if (target is Component)
                        UnityEditorInternal.ComponentUtility.MoveComponentDown(target as Component);
                }
            }
        }

        private void enterConsoleText(int i)
        {
            (_getTargetFunc(i) as IStickyNote).WriteLine(_consoleTextPropsCache[i].stringValue);
            _getSerializedObject(i).Update();
            _consoleTextPropsCache[i].stringValue = string.Empty;
        }
    }
}
