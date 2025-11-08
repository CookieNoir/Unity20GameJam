using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    [SerializeField] private LayerMask _interactablesLayerMask = 0;
    [SerializeField] private bool _canSelectAtStart = false;
    private Interactable _lastSelectedInteractable;
    private bool _canSelect;

    public bool CanSelect
    {
        get => _canSelect;
        set
        {
            if (_canSelect == value)
            {
                return;
            }
            SetCanSelect(value);
        }
    }

    public void Interact(Camera camera, Vector2 screenPosition)
    {
        if (!_canSelect ||
            _lastSelectedInteractable == null)
        {
            return;
        }
        if (!TryHit(camera, screenPosition, out Vector3 hitPoint, out var interactable) ||
            _lastSelectedInteractable != interactable)
        {
            return;
        }
        _lastSelectedInteractable.Interact(hitPoint);
    }

    private bool TryHit(Camera camera, Vector2 screenPosition, out Vector3 hitPoint, out Interactable interactable)
    {
        hitPoint = Vector3.zero;
        interactable = null;
        if (camera == null)
        {
            return false;
        }
        Ray ray = camera.ScreenPointToRay(screenPosition);
        if (!Physics.Raycast(ray, out var hit, float.PositiveInfinity, _interactablesLayerMask))
        {
            return false;
        }
        hitPoint = hit.point;
        var collider = hit.collider;
        if (collider.TryGetComponent(out interactable))
        {
            return true;
        }
        if (!collider.TryGetComponent(out InteractableReference reference))
        {
            return false;
        }
        interactable = reference.Interactable;
        return interactable != null;
    }

    public void Select(Camera camera, Vector2 screenPosition)
    {
        if (!_canSelect)
        {
            return;
        }
        if (!TryHit(camera, screenPosition, out _, out var interactable))
        {
            Deselect();
            return;
        }
        if (_lastSelectedInteractable == interactable)
        {
            return;
        }
        Deselect();
        if (!interactable.IsSelectable)
        {
            return;
        }
        _lastSelectedInteractable = interactable;
        _lastSelectedInteractable.IsSelected = true;
    }

    private void SetCanSelect(bool canSelect)
    {
        _canSelect = isActiveAndEnabled && canSelect;
        if (!_canSelect &&
            _lastSelectedInteractable != null)
        {
            _lastSelectedInteractable.IsSelected = false;
            _lastSelectedInteractable = null;
        }
    }

    private void Deselect()
    {
        if (_lastSelectedInteractable == null)
        {
            return;
        }
        _lastSelectedInteractable.IsSelected = false;
        _lastSelectedInteractable = null;
    }

    private void ValidateSelection()
    {
        if (_lastSelectedInteractable == null)
        {
            return;
        }
        if (!_lastSelectedInteractable.IsSelectable)
        {
            _lastSelectedInteractable.IsSelected = false;
        }
        if (!_lastSelectedInteractable.IsSelected)
        {
            _lastSelectedInteractable = null;
        }
    }

    private void OnEnable()
    {
        SetCanSelect(_canSelectAtStart);
    }

    private void Update()
    {
        ValidateSelection();
    }
}
