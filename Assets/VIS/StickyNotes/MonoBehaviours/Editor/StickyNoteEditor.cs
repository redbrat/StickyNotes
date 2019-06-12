using UnityEditor;
using UnityEngine;
using VIS.ObjectDescription.Editor;

namespace VIS.ObjectDescription.MonoBehaviours.Editor
{
    [CustomEditor(typeof(StickyNote))]
    public sealed class StickyNoteEditor : StickyNoteEditorBase
    {
        protected override Object getTarget(int index) => target;
    }
}
