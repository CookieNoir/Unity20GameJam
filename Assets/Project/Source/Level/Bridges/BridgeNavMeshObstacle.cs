using UnityEngine;
using UnityEngine.AI;

public class BridgeNavMeshObstacle : MonoBehaviour
{
    [SerializeField] private NavMeshObstacle _navMeshObstacle;
    [SerializeField] private bool _isActiveOnEnable;

    public void Activate()
    {
        SetActivated(true);
    }

    public void Deactivate()
    {
        SetActivated(false);
    }

    public void SetActivated(bool isActivated)
    {
        if (_navMeshObstacle == null)
        {
            return;
        }
        _navMeshObstacle.enabled = isActivated;
    }

    private void OnEnable()
    {
        SetActivated(_isActiveOnEnable);
    }

    private void OnDisable()
    {
        SetActivated(false);
    }
}
