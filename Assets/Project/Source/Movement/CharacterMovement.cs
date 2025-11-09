using UnityEngine;
using UnityEngine.AI;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _targetAgent;
    [SerializeField] private MovementPositionReachabilityChecker _reachabilityChecker;

    public Vector3 CharacterPosition => _targetAgent != null ? _targetAgent.transform.position : transform.position;

    public bool IsMovable => _targetAgent != null &&
        _targetAgent.isActiveAndEnabled &&
        _targetAgent.isOnNavMesh;

    public void SetDestination(Vector3 destinationPoint)
    {
        if (!IsMovable ||
            _reachabilityChecker == null ||
            !_reachabilityChecker.IsReachable(CharacterPosition, destinationPoint))
        {
            return;
        }
        _targetAgent.destination = destinationPoint;
    }
}
