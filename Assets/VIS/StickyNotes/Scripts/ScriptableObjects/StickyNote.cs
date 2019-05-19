using UnityEngine;

namespace VIS.ObjectDescription.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Note", menuName = "VIS/Create Sticky Note", order = 0)]
    public class StickyNote : ScriptableObject
    {
        [SerializeField, HideInInspector]
        private string _headerText = "Description";
        [SerializeField, HideInInspector]
        private string _text = "This is an Asset!";
        [SerializeField, HideInInspector]
        private Color _color = Color.yellow;
    }
}
