using UnityEngine;
using UnityEngine.AI;

public class MovementPositionReachabilityChecker : MonoBehaviour
{
    private NavMeshPath _reachabilityPath;

    public bool IsReachable(Vector3 fromPosition, Vector3 toPosition) 
    {
        if (_reachabilityPath == null)
        {
            _reachabilityPath = new NavMeshPath();
        }
        return NavMesh.CalculatePath(fromPosition, toPosition, NavMesh.AllAreas, _reachabilityPath) &&
            _reachabilityPath.status == NavMeshPathStatus.PathComplete;
    }
}
