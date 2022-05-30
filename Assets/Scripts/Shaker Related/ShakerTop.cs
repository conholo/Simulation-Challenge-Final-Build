using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;

public class ShakerTop : MonoBehaviour
{
    public bool IsHeld => _interactable != null && _interactable.isSelected && !IsConnectedToBottom;
    public bool IsConnectedToBottom { get; private set; }

    [SerializeField] private Transform _snapPoint;
    private XRGrabInteractable _interactable;
    private Rigidbody _rigidbody;
    private Collider _collider;

    private void Awake()
    {
        _interactable = GetComponent<XRGrabInteractable>();
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();

        _interactable.selectEntered.AddListener(OnGrab);
        _interactable.selectExited.AddListener(OnRelease);
    }

    private void OnRelease(SelectExitEventArgs arg0)
    {
        if (IsConnectedToBottom)
        {
            Snap();
        }
        else
        {
            transform.parent = null;
            _rigidbody.isKinematic = false;
            _collider.enabled = true;
        }
    }

    private void OnGrab(SelectEnterEventArgs selectEnteredArgs)
    {
        _collider.enabled = true;

        if (!IsConnectedToBottom) return;

        _rigidbody.isKinematic = false;
        IsConnectedToBottom = false;
        transform.parent = null;
    }

    public void Snap()
    {
        IsConnectedToBottom = true;
        transform.position = _snapPoint.position;
        transform.rotation = _snapPoint.rotation;
        transform.parent = _snapPoint;
        _rigidbody.isKinematic = true;
        _collider.enabled = false;
        _interactable.interactorsSelecting.Clear();
    }
}
