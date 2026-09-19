using UnityEngine;

namespace Gameplay.Interaction
{
    public enum InteractableType
    {
        Cabinet,
        Cauldron,
        Table
    }

    public class Interactable : MonoBehaviour
    {
        public InteractableType Type;
    }
}