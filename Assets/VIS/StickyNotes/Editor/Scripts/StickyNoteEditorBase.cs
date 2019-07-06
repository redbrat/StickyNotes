﻿using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace VIS.StickyNotes.Editor
{
    public abstract class StickyNoteEditorBase : UnityEditor.Editor
    {
        private GenericStickyNoteEditorBehaviour _stickyNoteEditorBehaviour
        {
            get
            {
                if (_stickyNoteEditorBehaviourBackingField == null)
                    _stickyNoteEditorBehaviourBackingField = new GenericStickyNoteEditorBehaviour(
                        base.OnInspectorGUI,
                        findProperty,
                        applyModifiedProperties,
                        needCloseButton,
                        closeButtonCallback,
                        () => needToDrawBaseInspector,
                        () => 1,
                        getTarget,
                        Repaint,
                        getSerializedObject
                    );

                return _stickyNoteEditorBehaviourBackingField;
            }
        }
        private GenericStickyNoteEditorBehaviour _stickyNoteEditorBehaviourBackingField;

        private void OnEnable()
        {
            _stickyNoteEditorBehaviour.OnEnable();
        }

        private void OnDisable()
        {
            _stickyNoteEditorBehaviour.OnDisable();
        }

        public override void OnInspectorGUI()
        {
            _stickyNoteEditorBehaviour.OnInspectorGUI();
        }

        protected virtual SerializedProperty findProperty(int index, string propertyName) => serializedObject.FindProperty(propertyName);
        protected virtual void applyModifiedProperties(int index) => serializedObject.ApplyModifiedProperties();
        protected virtual bool needToDrawBaseInspector => false;
        protected virtual bool needCloseButton(int index) => false;
        protected virtual Action<int> closeButtonCallback => null;
        protected virtual Object getTarget(int index) => null;
        protected virtual Func<int, SerializedObject> getSerializedObject => null;
    }
}