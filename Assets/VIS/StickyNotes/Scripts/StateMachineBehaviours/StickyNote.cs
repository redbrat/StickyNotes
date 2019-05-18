using UnityEngine;

namespace VIS.ObjectDescription.StateMachineBehaviours
{
    public class StickyNote : StateMachineBehaviour
    {
        [SerializeField]
        private string _headerText = "Description";
        [SerializeField]
        private string _text = "This is State!";
        [SerializeField]
        private Color _color = Color.yellow;
    }
}
