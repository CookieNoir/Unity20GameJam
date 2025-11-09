using UnityEngine;
using UnityEngine.AI;

public class BallRespawner : MonoBehaviour
{
    [SerializeField] private Ball _ball;
    [SerializeField] private NavMeshAgent _characterAgent;
    [SerializeField] private float _heightOffset = 1f;
    [SerializeField, Min(0f)] private float _radius = 1f;
    [SerializeField, Min(0f)] private float _maxDistance = 1f;
    [SerializeField, Min(1)] private int _fallbackMaxIterations = 10;
    [SerializeField] private LayerMask _groundLayerMask = 0;

    public void TryRespawn()
    {
        if (_ball == null ||
            _characterAgent == null)
        {
            return;
        }
        var agentTransform = _characterAgent.transform;
        Vector3 characterAgentPosition = agentTransform.position;
        Vector3 positionOffset = agentTransform.forward;
        positionOffset.y = 0f;
        positionOffset.Normalize();
        if (positionOffset == Vector3.zero)
        {
            positionOffset = Vector3.forward;
        }
        positionOffset.y = _heightOffset;
        Vector3 position = characterAgentPosition + positionOffset;
        if (IsRespawned(position))
        {
            return;
        }
        // Fallback
        for (int i = 0; i < _fallbackMaxIterations; ++i)
        {
            Vector2 positionXZOffset = Random.insideUnitCircle.normalized;
            if (positionXZOffset == Vector2.zero)
            {
                positionXZOffset = Vector2.up; // Forward in 3D
            }
            positionXZOffset *= _radius;
            positionOffset = new Vector3(positionXZOffset.x, _heightOffset, positionXZOffset.y);
            position = characterAgentPosition + positionOffset;
            if (IsRespawned(position))
            {
                return;
            }
        }
    }

    private bool IsRespawned(Vector3 position)
    {
        if (!Physics.Raycast(position, Vector3.down, _maxDistance, _groundLayerMask))
        {
            return false;
        }
        _ball.RespawnAtPosition(position);
        return true;
    }
}
