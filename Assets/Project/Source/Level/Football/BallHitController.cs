using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class BallHitController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _characterAgent;
    [SerializeField] private Ball _ball;
    [SerializeField, Min(0f)] private float _distanceThreshold = 1f;
    [SerializeField, Min(0f)] private float _forceToApply = 10f;
    [SerializeField] private float _heightOffset = 0.5f;
    [field: SerializeField] public UnityEvent<bool> OnHitAbilityChanged { get; private set; }
    private bool _canHitBall = false;

    public bool CanHitBall
    {
        get => _canHitBall;
        private set
        {
            if (_canHitBall == value)
            {
                return;
            }
            SetCanHitBall(value);
        }
    }

    private void OnEnable()
    {
        SetCanHitBall(GetHitAbility());
    }

    public void Hit()
    {
        if (!_canHitBall ||
            _characterAgent == null ||
            _ball == null)
        {
            return;
        }
        var agentTransform = _characterAgent.transform;
        Vector3 ballPosition = _ball.Position;
        Vector3 difference = ballPosition - agentTransform.position;
        if (difference.magnitude > _distanceThreshold)
        {
            return;
        }
        difference.y += _heightOffset;
        var direction = difference.normalized;
        if (direction == Vector3.zero)
        {
            direction = Vector3.forward;
        }
        Vector3 force = _forceToApply * direction;
        _ball.ApplyForce(force);
    }

    private void FixedUpdate()
    {
        UpdateHitAbility();
    }

    private void UpdateHitAbility()
    {
        CanHitBall = GetHitAbility();
    }

    private bool GetHitAbility()
    {
        if (_characterAgent == null ||
            _ball == null)
        {
            return false;
        }
        Vector3 agentPosition = _characterAgent.transform.position;
        Vector3 ballPosition = _ball.Position;
        float distance = Vector3.Distance(ballPosition, agentPosition);
        return distance <= _distanceThreshold;
    }

    private void SetCanHitBall(bool canHitBall)
    {
        _canHitBall = canHitBall;
        OnHitAbilityChanged?.Invoke(_canHitBall);
    }

    private void OnDrawGizmos()
    {
        if (_characterAgent == null ||
            _ball == null)
        {
            return;
        }
        bool canHitBall = Application.isPlaying ? _canHitBall : GetHitAbility();
        Gizmos.color = canHitBall ? Color.green : Color.red;
        Gizmos.DrawLine(_characterAgent.transform.position, _ball.Position);
    }
}
