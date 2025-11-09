using System;
using UnityEngine.InputSystem;

public static class InputActionHelpers
{
    public static bool TryEnable(InputActionReference reference, out InputAction action,
        Action<InputAction.CallbackContext> onStart = null,
        Action<InputAction.CallbackContext> onPerform = null,
        Action<InputAction.CallbackContext> onCancel = null)
    {
        action = null;
        if (reference == null)
        {
            return false;
        }
        action = reference.action;
        if (action == null)
        {
            return false;
        }
        if (onStart != null)
        {
            action.started += onStart;
        }
        if (onPerform != null)
        {
            action.performed += onPerform;
        }
        if (onCancel != null)
        {
            action.canceled += onCancel;
        }
        action.Enable();
        return true;
    }

    public static void Disable(ref InputAction action,
        Action<InputAction.CallbackContext> onStart = null,
        Action<InputAction.CallbackContext> onPerform = null,
        Action<InputAction.CallbackContext> onCancel = null)
    {
        if (action == null)
        {
            return;
        }
        if (onStart != null)
        {
            action.started -= onStart;
        }
        if (onPerform != null)
        {
            action.performed -= onPerform;
        }
        if (onCancel != null)
        {
            action.canceled -= onCancel;
        }
        action.Disable();
        action = null;
    }
}
