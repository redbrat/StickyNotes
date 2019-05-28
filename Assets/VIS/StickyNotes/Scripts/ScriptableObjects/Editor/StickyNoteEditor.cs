using UnityEditor;
using UnityEngine;
using VIS.ObjectDescription.Editor;

namespace VIS.ObjectDescription.ScriptableObjects.Editor
{
    [CustomEditor(typeof(StickyNote))]
    public sealed class StickyNoteEditor : StickyNoteEditorBase
    {
        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            return Instantiate(Resources.Load<Texture2D>("VIS/StickyNotes/Textures/note-icon"));
        }

        protected override Object getTarget(int index) => target;
    }
}
