using UnityEngine;
using UnityEngine.AI;

public class BridgeNavMeshObstacle : MonoBehaviour
{
    [SerializeField] private Bridge _bridge;
    [SerializeField] private NavMeshObstacle _navMeshObstacle;

    private void SetActivated(bool isRaised)
    {
        if (_navMeshObstacle == null)
        {
            return;
        }
        _navMeshObstacle.enabled = isRaised;
    }

    private void OnEnable()
    {
        if (_bridge != null) 
        {
            _bridge.OnBridgeStateChanged?.AddListener(SetActivated);
            SetActivated(_bridge.IsRaised);
        }
    }

    private void OnDisable()
    {
        if (_bridge != null)
        {
            _bridge.OnBridgeStateChanged?.RemoveListener(SetActivated);
        }
    }
}
