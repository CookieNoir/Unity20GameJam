using UnityEngine;

public class BallInteractable : Interactable
{
    [SerializeField] private BallHitController _ballHitController;
    [SerializeField] private BallRespawner _ballRespawner;

    protected override void OnInteract(Vector3 interactionPoint)
    {
        if (_ballHitController != null &&
            _ballHitController.CanHitBall)
        {
            _ballHitController.Hit();
            return;
        }
        if (_ballRespawner != null)
        {
            _ballRespawner.TryRespawn();
        }
    }
}
