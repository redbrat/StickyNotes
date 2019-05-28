using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace VIS.ObjectDescription.Editor.StandartStickyNotableAssets
{
    [CustomEditor(typeof(AnimatorController))]
    public class StickyNotableAnimatorControllerEditor : MultipleStickyNotesEditorBase
    {
        protected override Object getTarget(int index) => _targetsCache[index].targetObject;
    }
}
