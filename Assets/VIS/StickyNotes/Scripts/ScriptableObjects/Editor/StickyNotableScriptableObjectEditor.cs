using UnityEditor;
using VIS.ObjectDescription.Editor;

namespace VIS.ObjectDescription.ScriptableObjects.Editor
{
    public abstract class StickyNotableScriptableObjectEditor : StickyNoteEditorBase
    {
        private SerializedObject _targetCache;

        protected sealed override SerializedProperty findProperty(string propertyName)
        {
            if (_targetCache == null)
                setRightTarget();
            return _targetCache.FindProperty(propertyName);
        }

        protected override void applyModifiedProperties()
        {
            if (_targetCache == null)
                setRightTarget();
            _targetCache.ApplyModifiedProperties();
        }

        protected override bool needToDrawBaseInspector => true;

        private void setRightTarget()
        {
            var assetPath = AssetDatabase.GetAssetPath(target);
            var asset = AssetDatabase.LoadAssetAtPath<StickyNote>(assetPath);
            _targetCache = new SerializedObject(asset);
        }
    }
}
