using UnityEngine;

public class TimedRigidBody : MonoBehaviour
{
    [SerializeField] private TimedBehavior _timedBehavior;
    [SerializeField] private Rigidbody _rigidBody;
    [SerializeField] private bool _isKinematic;
    private Vector3 _linearVelocityOnEnable;
    private Vector3 _angularVelocityOnEnable;
    private bool _wakeUpOnEnable;
    private bool _isEnabled;

    private void OnEnable()
    {
        _linearVelocityOnEnable = Vector3.zero;
        _angularVelocityOnEnable = Vector3.zero;
        _wakeUpOnEnable = false;
        if (_timedBehavior != null)
        {
            _timedBehavior.OnTimeUsageChanged?.AddListener(UpdateRigidbodyActivity);
            SetRigidbodyEnabled(_timedBehavior.IsUsingTime);
        }
        else
        {
            SetRigidbodyEnabled(false);
        }
    }

    private void OnDisable()
    {
        if (_timedBehavior != null)
        {
            _timedBehavior.OnTimeUsageChanged?.RemoveListener(UpdateRigidbodyActivity);
        }
    }

    private void UpdateRigidbodyActivity(bool isUsingTime)
    {
        if (_isEnabled == isUsingTime)
        {
            return;
        }
        SetRigidbodyEnabled(isUsingTime);
    }

    private void SetRigidbodyEnabled(bool isUsingTime)
    {
        _isEnabled = isUsingTime;
        if (_rigidBody == null)
        {
            return;
        }
        if (_isEnabled)
        {
            _rigidBody.IsSleeping();
            _rigidBody.WakeUp();
            _rigidBody.isKinematic = _isKinematic;
            if (!_isKinematic)
            {
                _rigidBody.linearVelocity = _linearVelocityOnEnable;
                _rigidBody.angularVelocity = _angularVelocityOnEnable;
            }
            if (_wakeUpOnEnable)
            {
                _rigidBody.WakeUp();
            }
        }
        else
        {
            _linearVelocityOnEnable = _rigidBody.linearVelocity;
            _angularVelocityOnEnable = _rigidBody.angularVelocity;
            _wakeUpOnEnable = !_rigidBody.IsSleeping();
            _rigidBody.isKinematic = true;
            if (_wakeUpOnEnable)
            {
                _rigidBody.Sleep();
            }
        }
    }
}
