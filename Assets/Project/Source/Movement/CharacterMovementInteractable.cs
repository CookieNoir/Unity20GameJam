using UnityEngine;
using UnityEngine.AI;

public class CharacterMovementInteractable : Interactable
{
    [SerializeField] private CharacterMovement _characterMovement;
    [SerializeField] private MovementPositionReachabilityChecker _reachabilityChecker;
    [SerializeField] private GameObject _movementCursorObject;
    [SerializeField, Min(0f)] private float _maxDistanceFromNavMesh = 0.5f;

    protected override bool CanSelect()
    {
        return _characterMovement != null;
    }

    protected override void OnSelect()
    {
        if (_movementCursorObject == null)
        {
            return;
        }
        if (!IsSelected)
        {
            _movementCursorObject.SetActive(false);
        }
    }

    protected override void OnUpdateSelection(Vector3 selectionPoint)
    {
        if (_characterMovement == null ||
            _movementCursorObject == null ||
            _reachabilityChecker == null)
        {
            return;
        }
        bool isOnNavMesh = NavMesh.SamplePosition(selectionPoint, out var hit, _maxDistanceFromNavMesh, NavMesh.AllAreas);
        if (!isOnNavMesh)
        {
            _movementCursorObject.SetActive(false);
            return;
        }
        bool isReachable = _reachabilityChecker.IsReachable(_characterMovement.CharacterPosition, selectionPoint);
        _movementCursorObject.SetActive(isReachable);
        if (isReachable)
        {
            _movementCursorObject.transform.position = hit.position;
        }
    }

    protected override void OnInteract(Vector3 interactionPoint)
    {
        if (_characterMovement == null)
        {
            return;
        }
        _characterMovement.SetDestination(interactionPoint);
    }
}
