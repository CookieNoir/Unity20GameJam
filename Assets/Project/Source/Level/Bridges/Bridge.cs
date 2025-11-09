using UnityEngine;
using UnityEngine.Events;

public class Bridge : MonoBehaviour
{
    [SerializeField] private bool _isRaisedAtStart = false;
    [field: SerializeField] public UnityEvent<bool> OnBridgeStateChanged { get; private set; }
    private bool _isAnyStateSet = false;
    private bool _isRaised = false;

    public bool IsRaised
    {
        get => _isRaised;
        set
        {
            if (_isAnyStateSet &&
                IsRaised == value)
            {
                return;
            }
            SetIsRaised(value);
        }
    }

    private void OnEnable()
    {
        if (!_isAnyStateSet)
        {
            SetIsRaised(_isRaisedAtStart);
        }
    }

    private void SetIsRaised(bool isLowered)
    {
        _isRaised = isLowered;
        _isAnyStateSet = true;
        OnBridgeStateChanged?.Invoke(_isRaised);
    }
}
