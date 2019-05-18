using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using VIS.ObjectDescription.Editor;
using VIS.ObjectDescription.ScriptableObjects;

[CustomEditor(typeof(AnimatorController))]
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
                    () => needToDrawBaseInspector,
                    () => _targetsCache.Length
                );

            return _stickyNoteEditorBehaviourBackingField;
        }
    }
    private GenericStickyNoteEditorBehaviour _stickyNoteEditorBehaviourBackingField;

    private SerializedObject[] _targetsCache;

    public void OnEnable()
    {
        if (_targetsCache == null)
            setRightTarget();

        //Debug.Log($"Material OnEnable. _targetCache = {_targetCache}");
        if (_targetsCache != null)
            _stickyNoteEditorBehaviour.OnEnable();
    }

    public void OnDisable()
    {
        if (_targetsCache != null)
            _stickyNoteEditorBehaviour.OnDisable();
    }

    public override void OnInspectorGUI()
    {
        //Debug.Log($"Material OnInspectorGUI. _targetCache = {_targetCache}");
        if (_targetsCache == null)
            base.OnInspectorGUI();
        else
            _stickyNoteEditorBehaviour.OnInspectorGUI();
    }

    private SerializedProperty findProperty(int index, string propertyName)
    {
        return _targetsCache[index].FindProperty(propertyName);
    }

    private void applyModifiedProperties(int index)
    {
        _targetsCache[index].ApplyModifiedProperties();
    }

    private bool needToDrawBaseInspector => true;

    private void setRightTarget()
    {
        var assetPath = AssetDatabase.GetAssetPath(target);
        var assets = AssetDatabase.LoadAllAssetsAtPath(assetPath).Where(a => a is StickyNote).Select(a => a as StickyNote);
        if (assets != null && assets.Count() > 0)
            _targetsCache = assets.Select(a => new SerializedObject(a)).ToArray();
    }

    public void OnSticked()
    {
        _targetsCache = null;
        OnEnable();
    }
    public void OnUnsticked()
    {
        _targetsCache = null;
        OnEnable();
    }
}
