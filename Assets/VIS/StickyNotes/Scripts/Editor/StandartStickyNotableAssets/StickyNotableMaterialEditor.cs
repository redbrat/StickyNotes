using UnityEditor;
using UnityEngine;
using VIS.ObjectDescription.Editor;
using VIS.ObjectDescription.ScriptableObjects;

[CustomEditor(typeof(Material))]
public class StickyNotableMaterialEditor : MaterialEditor, IAssetsStickedEventsListener
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

    public override void OnEnable()
    {
        base.OnEnable();

        if (_targetCache == null)
            setRightTarget();

        //Debug.Log($"Material OnEnable. _targetCache = {_targetCache}");
        if (_targetCache != null)
            _stickyNoteEditorBehaviour.OnEnable();
    }

    public override void OnDisable()
    {
        base.OnEnable();

        if (_targetCache != null)
            _stickyNoteEditorBehaviour.OnDisable();
    }

    public override void OnInspectorGUI()
    {
        //Debug.Log($"Material OnInspectorGUI. _targetCache = {_targetCache}");
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

    public void OnSticked()
    {
        //Debug.Log($"OnSticked - Repaint");
        OnEnable();
        //Repaint();
    }
}
