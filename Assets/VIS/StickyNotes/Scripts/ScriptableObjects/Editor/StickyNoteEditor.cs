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
            var tex = Resources.Load<Texture2D>("VIS/StickyNotes/Textures/note-icon");
            Debug.Log($"tex = {tex}. {width}, {height}");
            //tex.SetPixel(0, 0, Color.yellow);
            //tex.Apply();
            return tex;
        }
    }
}
