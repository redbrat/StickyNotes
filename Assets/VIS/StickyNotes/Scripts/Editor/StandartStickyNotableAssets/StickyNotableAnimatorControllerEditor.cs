using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VIS.ObjectDescription.ScriptableObjects;

namespace VIS.ObjectDescription.Editor.StandartStickyNotableAssets
{
    //[CustomEditor(typeof(AnimatorController))]
    public class StickyNotableAnimatorControllerEditor : UnityEditor.Editor, IAssetsStickedEventsListener
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
                        () => needToDrawBaseInspector
                    );

                return _stickyNoteEditorBehaviourBackingField;
            }
        }
        private GenericStickyNoteEditorBehaviour _stickyNoteEditorBehaviourBackingField;

        private SerializedObject _targetCache;

        public void OnEnable()
        {
            if (_targetCache == null)
                setRightTarget();

            if (_targetCache != null)
                _stickyNoteEditorBehaviour.OnEnable();
        }

        public void OnDisable()
        {
            if (_targetCache != null)
                _stickyNoteEditorBehaviour.OnDisable();
        }

        public override void OnInspectorGUI()
        {
            if (_targetCache == null)
                base.OnInspectorGUI();
            else
                _stickyNoteEditorBehaviour.OnInspectorGUI();
        }

        private SerializedProperty findProperty(string propertyName)
        {
            return _targetCache.FindProperty(propertyName);
        }

        private void applyModifiedProperties()
        {
            _targetCache.ApplyModifiedProperties();
        }

        private bool needToDrawBaseInspector => true;

        private void setRightTarget()
        {
            var assetPath = AssetDatabase.GetAssetPath(target);
            var asset = AssetDatabase.LoadAssetAtPath<StickyNote>(assetPath);
            if (asset != null)
                _targetCache = new SerializedObject(asset);
        }

        public void OnSticked() => OnEnable();

        public void OnUnsticked()
        {
            _targetCache = null;
            OnEnable();
        }
    }
}
