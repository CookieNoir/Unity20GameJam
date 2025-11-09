using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputActionButton : MonoBehaviour
{
    [SerializeField] private InputActionReference _inputAction;
    [field: SerializeField] public UnityEvent OnButtonPressed { get; private set; }
    private InputAction _action;

    private void OnEnable()
    {
        InputActionHelpers.TryEnable(_inputAction, out _action,
            onStart: ButtonPressed);
    }

    private void OnDisable()
    {
        InputActionHelpers.Disable(ref _action,
            onStart: ButtonPressed);
    }

    private void ButtonPressed(InputAction.CallbackContext ctx)
    {
        OnButtonPressed?.Invoke();
    }
}
