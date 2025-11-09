using System;
using UnityEngine;
using UnityEngine.Events;

public class ElectricParticle : MonoBehaviour
{
    [SerializeField] private Transform _movableTransform;
    [SerializeField] private TimedBehavior _timedBehavior;
    [SerializeField, Min(0f)] private float _movementSpeed = 1f;
    [field: SerializeField] public UnityEvent OnPathCompleted { get; private set; }
    public event Action<ElectricParticle> OnEndReached;
    public event Action<ElectricParticle> BeforeReleased;
    public float ExistenceTime { get; private set; } = 0f;
    private LineRendererPath _electricPath;
    private float _distanceTraveled;
    private bool _isEndReached = false;
    private IPoolReleaser<ElectricParticle> _pool;

    public bool IsEndReached => _isEndReached;

    public void SetPool(IPoolReleaser<ElectricParticle> pool)
    {
        _pool = pool;
    }

    public void ResetState()
    {
        ExistenceTime = 0f;
        _distanceTraveled = 0f;
        _isEndReached = false;
    }

    public void SetDomeAndPath(TimeDome timeDome, LineRendererPath electricPath)
    {
        if (_timedBehavior != null)
        {
            _timedBehavior.TimeDome = timeDome;
        }
        _electricPath = electricPath;
        UpdatePosition();
    }

    private void FixedUpdate()
    {
        if (_isEndReached ||
            _timedBehavior == null ||
            _electricPath == null ||
            !_timedBehavior.IsUsingTime)
        {
            return;
        }
        float deltaTime = Time.deltaTime;
        ExistenceTime += deltaTime;
        float step = deltaTime * _movementSpeed;
        _distanceTraveled += step;
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        if (_electricPath == null)
        {
            return;
        }
        if (_electricPath.TryGetPositionByDistance(_distanceTraveled, out Vector3 position) &&
            _distanceTraveled < _electricPath.TotalDistance)
        {
            if (_movableTransform != null)
            {
                _movableTransform.position = position;
            }
            return;
        }
        ReachEnd();
    }

    private void ReachEnd()
    {
        _isEndReached = true;
        OnEndReached?.Invoke(this);
        OnPathCompleted?.Invoke();
    }

    public void Release()
    {
        BeforeReleased?.Invoke(this);
        if (_pool == null ||
            _pool as MonoBehaviour == null)
        {
            Destroy(gameObject);
            return;
        }
        _pool.Release(this);
    }
}
