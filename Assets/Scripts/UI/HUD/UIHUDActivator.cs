using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class UIHUDActivator : MonoBehaviour
{
    [Range(0, 1)]
    [SerializeField] private float _threshold;
    [SerializeField] private Canvas _hud;

    private XRRayInteractor _rayInteractor;

    private void Awake()
    {
        _hud.enabled = false;
        _rayInteractor = GetComponentInParent<XRRayInteractor>();
    }

    private void Update()
    {
        if (_rayInteractor.isSelectActive) return;
        
        var dot = Vector3.Dot(transform.up, Vector3.up);
        _hud.enabled =  Mathf.Abs(dot) < _threshold;
        _rayInteractor.enabled = !_hud.enabled;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, transform.up * 10.0f);
    }
}
