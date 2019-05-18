using UnityEditor;

namespace VIS.ObjectDescription.Editor
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
                        () => needToDrawBaseInspector,
                        () => 1
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
    }
}
