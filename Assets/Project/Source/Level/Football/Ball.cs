using System.Collections;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidBody;
    [SerializeField, Min(0f)] private float _respawnDuration = 0.2f;
    private IEnumerator _unfreezeCoroutine;
    private RigidbodyConstraints _initialConstraints = RigidbodyConstraints.None;

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
        _rigidBody.isKinematic = true;
        _rigidBody.MovePosition(position);
        _rigidBody.isKinematic = wasKinematic;
        if (_respawnDuration <= 0f)
        {
            return;
        }
        if (_unfreezeCoroutine != null)
        {
            StopCoroutine(_unfreezeCoroutine);
            _rigidBody.constraints = _initialConstraints;
        }
        _initialConstraints = _rigidBody.constraints;
        _rigidBody.constraints = RigidbodyConstraints.FreezeAll;
        _rigidBody.transform.position = position;
        _unfreezeCoroutine = Unfreeze();
        StartCoroutine(_unfreezeCoroutine);
    }

    private IEnumerator Unfreeze()
    {

        yield return new WaitForSeconds(_respawnDuration);
        if (_rigidBody == null)
        {
            _unfreezeCoroutine = null;
            yield break;
        }
        _rigidBody.constraints = _initialConstraints;
        _unfreezeCoroutine = null;
    }

    private void OnDisable()
    {
        if (_unfreezeCoroutine != null &&
            _rigidBody != null)
        {
            _rigidBody.constraints = _initialConstraints;
        }
        _unfreezeCoroutine = null;
    }
}
