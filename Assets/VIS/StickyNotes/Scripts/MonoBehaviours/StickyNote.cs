using UnityEngine;

namespace VIS.ObjectDescription.MonoBehaviours
{
    public class StickyNote : MonoBehaviour
    {
        [SerializeField]
        private string _headerText = "Description";
        [SerializeField]
        private string _text = "This is GameObject!";
        [SerializeField]
        private Color _color = Color.yellow;
    }
}
