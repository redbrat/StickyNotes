using UnityEditor;
using UnityEngine;
using VIS.StickyNotes.Editor;

namespace VIS.StickyNotes.MonoBehaviours.Editor
{
    [CustomEditor(typeof(StickyNote))]
    public sealed class StickyNoteEditor : StickyNoteEditorBase
    {
        protected override Object getTarget(int index) => target;
    }
}
