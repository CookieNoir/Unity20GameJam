using UnityEngine;
using UnityEngine.EventSystems;

public class Interactor : MonoBehaviour, IPointerEnterHandler, IPointerMoveHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private InteractionManager _interactionManager;
    [SerializeField] private Camera _camera;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_interactionManager == null)
        {
            return;
        }
        _interactionManager.CanSelect = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_interactionManager == null)
        {
            return;
        }
        _interactionManager.CanSelect = false;
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (_interactionManager == null)
        {
            return;
        }
        _interactionManager.Select(_camera, eventData.position);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_interactionManager == null)
        {
            return;
        }
        _interactionManager.Interact(_camera, eventData.position);
    }
}
