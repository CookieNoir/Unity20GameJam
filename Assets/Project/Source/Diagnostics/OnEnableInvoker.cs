using UnityEngine;
using UnityEngine.Events;

public class OnEnableInvoker : MonoBehaviour
{
    [field: SerializeField] public UnityEvent OnEnabled { get; private set; }

    private void OnEnable()
    {
        OnEnabled?.Invoke();
    }
}
