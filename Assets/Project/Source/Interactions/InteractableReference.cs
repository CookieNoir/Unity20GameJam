using UnityEngine;

public class InteractableReference : MonoBehaviour
{
    [field: SerializeField] public Interactable Interactable { get; private set; }
}
