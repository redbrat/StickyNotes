using UnityEngine;

namespace VIS.ObjectDescription.StateMachineBehaviours
{
    public class StickyNote : StateMachineBehaviour
    {
        [SerializeField, HideInInspector]
        private string _headerText = "Description";
        [SerializeField, HideInInspector]
        private string _text = "This is State!";
        [SerializeField, HideInInspector]
        private Color _color = Color.yellow;
    }
}
