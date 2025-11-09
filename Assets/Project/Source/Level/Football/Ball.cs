using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Ball : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidBody;
    [SerializeField] private SphereCollider _collider;
    [SerializeField, Min(0f)] private float _physicsUnfreezeDelay = 0.2f;
    [field: SerializeField] public UnityEvent OnRespawned { get; private set; }
    private IEnumerator _unfreezeCoroutine;
    private RigidbodyConstraints _initialConstraints = RigidbodyConstraints.None;
    private bool _colliderInitialState = false;

    public Vector3 Position => _rigidBody != null ? _rigidBody.position : transform.position;

    public void ApplyForce(Vector3 force)
    {
        if (_rigidBody == null)
        {
            return;
        }
        _rigidBody.AddForce(force);
    }

    public void RespawnAtPosition(Vector3 position)
    {
        if (_rigidBody == null)
        {
            return;
        }
        bool wasKinematic = _rigidBody.isKinematic;
        if (!wasKinematic)
        {
            _rigidBody.linearVelocity = Vector3.zero;
            _rigidBody.angularVelocity = Vector3.zero;
        }
        _rigidBody.Sleep();
        _rigidBody.isKinematic = true;
        _rigidBody.transform.position = position;
        _rigidBody.isKinematic = wasKinematic;
        if (_physicsUnfreezeDelay <= 0f ||
            !isActiveAndEnabled)
        {
            return;
        }
        if (_unfreezeCoroutine != null)
        {
            StopCoroutine(_unfreezeCoroutine);
            RestoreState();
        }
        _initialConstraints = _rigidBody.constraints;
        _rigidBody.constraints = RigidbodyConstraints.FreezeAll;
        if (_collider != null)
        {
            _colliderInitialState = _collider.enabled;
            _collider.enabled = false;
        }
        _unfreezeCoroutine = Unfreeze();
        StartCoroutine(_unfreezeCoroutine);
        OnRespawned?.Invoke();
    }

    private IEnumerator Unfreeze()
    {
        yield return new WaitForSeconds(_physicsUnfreezeDelay);
        RestoreState();
        _unfreezeCoroutine = null;
    }

    private void RestoreState()
    {
        if (_rigidBody != null)
        {
            _rigidBody.constraints = _initialConstraints;
        }
        if (_collider != null)
        {
            _collider.enabled = _colliderInitialState;
        }
    }

    private void OnDisable()
    {
        if (_unfreezeCoroutine != null)
        {
            RestoreState();
        }
        _unfreezeCoroutine = null;
    }
}
