using UnityEngine;
using UnityEngine.Events;

public abstract class Interactable : MonoBehaviour
{
    [field: SerializeField] public UnityEvent<bool> OnSelectionChanged { get; private set; }
    private bool _isSelected = false;

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected == value ||
                !IsSelectable)
            {
                return;
            }
            SetSelected(value);
        }
    }

    public bool IsSelectable => isActiveAndEnabled && CanSelect();

    protected virtual bool CanSelect()
    {
        return true;
    }

    public void SetSelected(bool isSelected)
    {
        _isSelected = isSelected;
        OnSelect();
        OnSelectionChanged?.Invoke(_isSelected);
    }

    public void UpdateSelection(Vector3 selectionPoint)
    {
        if (!_isSelected)
        {
            return;
        }
        OnUpdateSelection(selectionPoint);
    }

    protected virtual void OnUpdateSelection(Vector3 selectionPoint) { }

    protected virtual void OnSelect() { }

    protected virtual void OnEnable()
    {
        SetSelected(false);
    }

    public void Interact(Vector3 interactionPoint)
    {
        if (!_isSelected)
        {
            return;
        }
        OnInteract(interactionPoint);
    }

    protected abstract void OnInteract(Vector3 interactionPoint);

    protected virtual void Update()
    {
        if (!IsSelectable)
        {
            IsSelected = false;
        }
    }

    protected virtual void OnDisable()
    {
        IsSelected = false;
    }
}
