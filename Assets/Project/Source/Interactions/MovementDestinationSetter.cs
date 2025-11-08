using UnityEngine;
using UnityEngine.AI;

public class MovementDestinationSetter : Interactable
{
    [SerializeField] private NavMeshAgent _targetAgent;
    private NavMeshPath _reachabilityPath;

    protected override bool CanSelect()
    {
        return _targetAgent != null &&
            _targetAgent.isActiveAndEnabled &&
            _targetAgent.isOnNavMesh;
    }

    protected override void OnInteract(Vector3 interactionPoint)
    {
        if (_targetAgent == null)
        {
            return;
        }
        if (_reachabilityPath == null)
        {
            _reachabilityPath = new NavMeshPath();
        }
        if (!NavMesh.CalculatePath(_targetAgent.transform.position, interactionPoint, NavMesh.AllAreas, _reachabilityPath) ||
            _reachabilityPath.status != NavMeshPathStatus.PathComplete)
        {
            return;
        }
        _targetAgent.destination = interactionPoint;
    }
}
