using System;
using UnityEngine;

namespace VIS.StickyNotes.Vanilla
{
    [Serializable]
    public class StickyNote
    {
        [SerializeField, HideInInspector]
        private string _headerText = "Description";
        [SerializeField, HideInInspector]
        private string _text = "This is State!";
        [SerializeField, HideInInspector]
        private Color _color = Color.yellow;

        public StickyNote()
        {

        }

        public StickyNote(string headerText, string text, Color color)
        {
            _headerText = headerText;
            _text = text;
            _color = color;
        }
    }
}
