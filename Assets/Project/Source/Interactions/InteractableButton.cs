using UnityEngine;
using UnityEngine.Events;

public class InteractableButton : Interactable
{
    [field: SerializeField] public UnityEvent OnClick { get; private set; }

    protected override void OnInteract(Vector3 interactionPoint)
    {
        OnClick?.Invoke();
    }
}
