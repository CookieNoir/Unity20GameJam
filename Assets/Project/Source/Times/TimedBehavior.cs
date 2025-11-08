using UnityEngine;
using UnityEngine.Events;

public class TimedBehavior : MonoBehaviour
{
    [SerializeField] private TimeDome _timeDome;
    [SerializeField] private TimeDomeCollider[] _colliders;
    [field: SerializeField] public UnityEvent<bool> OnTimeUsageChanged { get; private set; }
    private bool _isUsingTime = false;

    public bool IsUsingTime
    {
        get => _isUsingTime;
        private set
        {
            if (_isUsingTime == value)
            {
                return;
            }
            SetIsUsingTime(value);
        }
    }

    public TimeDome TimeDome
    {
        get => _timeDome;
        set
        {
            if (_timeDome == value)
            {
                return;
            }
            _timeDome = value;
            IsUsingTime = false;
        }
    }

    private void SetIsUsingTime(bool value)
    {
        _isUsingTime = value;
        OnTimeUsageChanged?.Invoke(_isUsingTime);
    }

    private bool GetTimeUsage()
    {
        if (_timeDome == null ||
            _colliders == null)
        {
            return false;
        }
        for (int i = 0; i < _colliders.Length; ++i)
        {
            var collider = _colliders[i];
            if (collider == null ||
                !collider.IsFullyInside(_timeDome))
            {
                return false;
            }
        }
        return true;
    }

    private void OnEnable()
    {
        SetIsUsingTime(GetTimeUsage());
    }

    private void FixedUpdate()
    {
        IsUsingTime = GetTimeUsage();
    }

    private void OnDisable()
    {
        IsUsingTime = false;
    }
}
