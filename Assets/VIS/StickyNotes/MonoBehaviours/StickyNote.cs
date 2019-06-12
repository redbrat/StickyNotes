using UnityEngine;

namespace VIS.ObjectDescription.MonoBehaviours
{
    public class StickyNote : MonoBehaviour
    {
        [SerializeField, HideInInspector]
        private string _headerText = "Description";
        [SerializeField, HideInInspector]
        private string _text = "This is GameObject!";
        [SerializeField, HideInInspector]
        private Color _color = Color.yellow;
    }
}
