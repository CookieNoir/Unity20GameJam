using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class CharacterMovementInteractable : Interactable
{
    [SerializeField] private CharacterMovement _characterMovement;
    [SerializeField] private MovementPositionReachabilityChecker _reachabilityChecker;
    [SerializeField] private GameObject _movementCursorObject;
    [SerializeField, Min(0f)] private float _maxDistanceFromNavMesh = 0.5f;
    [field: SerializeField] public UnityEvent<bool> OnMovementAbilityChanged { get; private set; }
    private bool _canMove = false;

    public bool CanMove
    {
        get => _canMove;
        private set
        {
            if (_canMove == value)
            {
                return;
            }
            SetCanMove(value);
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        SetCanMove(false);
    }

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
            CanMove = false;
        }
    }

    private void SetCanMove(bool canMove)
    {
        _canMove = canMove;
        OnMovementAbilityChanged?.Invoke(_canMove);
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
            CanMove = false;
            return;
        }
        bool isReachable = _reachabilityChecker.IsReachable(_characterMovement.CharacterPosition, selectionPoint);
        CanMove = isReachable;
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
