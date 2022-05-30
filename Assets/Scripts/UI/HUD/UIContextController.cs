using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class UIContextController : MonoBehaviour
{
    [SerializeField] private UIDisplayItem _uiDisplayItem;
    private XRRayInteractor _rayInteractor;
    private IXRSelectInteractable _currentInteractable;

    private void Awake()
    {
        _rayInteractor = GetComponent<XRRayInteractor>();
    }

    private void Start()
    {
        _rayInteractor.hoverEntered.AddListener(OnHovered);
        _rayInteractor.hoverExited.AddListener(OnHoveredExited);
        _rayInteractor.selectEntered.AddListener(OnSelectedBegin);
        _rayInteractor.selectExited.AddListener(OnSelectedEnd);
    }

    private void OnHoveredExited(HoverExitEventArgs exitArgs)
    {
        _uiDisplayItem.ClearActiveDisplays();
    }

    private void OnSelectedBegin(SelectEnterEventArgs selectionArgs)
    {
        var interactable = selectionArgs.interactableObject;

        if (interactable == null) return;

        _currentInteractable = interactable;
    }
    
    private void OnSelectedEnd(SelectExitEventArgs selectionArgs)
    {
        _currentInteractable = null;
    }

    private void OnDestroy()
    {
        _rayInteractor.hoverEntered.RemoveListener(OnHovered);
    }

    private void Update()
    {
        if (_currentInteractable == null) return;
        
        _uiDisplayItem.UpdateDisplay(_currentInteractable.transform.gameObject);
    }

    private void OnHovered(HoverEnterEventArgs hoveredArgs)
    {
        // If we're already holding something, don't show info on hover.
        if (_currentInteractable != null) return;
        
        var interactable = hoveredArgs.interactableObject;

        if (interactable == null) return;
        
        _uiDisplayItem.UpdateDisplay(interactable.transform.gameObject);
    }
}
