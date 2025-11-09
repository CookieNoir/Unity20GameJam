using UnityEngine;

public class BridgeAnimator : MonoBehaviour
{
    [SerializeField] private Bridge _bridge;
    [SerializeField] private Transform _bridgeTransform;
    [SerializeField] private Vector3 _loweredRotation = Vector3.zero;
    [SerializeField] private Vector3 _raisedRotation = Vector3.zero;
    [SerializeField, Min(0f)] private float _transitionDuration = 1f;
    [SerializeField] private bool _scheduleOnDisabled = true;

    private void OnEnable()
    {
        if (_bridge != null)
        {
            _bridge.OnBridgeStateChanged?.AddListener(SetRotation);
            SetRotation(_bridge.IsRaised);
        }
    }

    private void OnDisable()
    {
        if (_bridge != null)
        {
            _bridge.OnBridgeStateChanged?.RemoveListener(SetRotation);
            SetRotation(_bridge.IsRaised);
        }
    }

    private void SetRotation(bool isRaised)
    {
        if (_bridgeTransform == null)
        {
            return;
        }
        _bridgeTransform.localRotation = Quaternion.Euler(isRaised ? _raisedRotation : _loweredRotation);
    }
}
