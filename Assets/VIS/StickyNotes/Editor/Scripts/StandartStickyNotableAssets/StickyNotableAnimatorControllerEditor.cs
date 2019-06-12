using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace VIS.StickyNotes.Editor.StandartStickyNotableAssets
{
    [CustomEditor(typeof(AnimatorController))]
    public class StickyNotableAnimatorControllerEditor : MultipleStickyNotesEditorBase
    {
        protected override Object getTarget(int index) => (_targetsCache?.Length ?? -1) > index ? _targetsCache[index].targetObject : null;
    }
}
