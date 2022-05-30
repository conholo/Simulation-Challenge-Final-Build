using UnityEngine;
using UnityEngine.Rendering;

public class UIRenderFinishedDrinkCamera : MonoBehaviour
{
    private RenderTexture _result;
    public RenderTexture GetTargetTexture => _result;

    private Camera _camera;
    private Vector3 _lastPosition;
    private Vector3 _lastRotation;

    private void Awake()
    {
        _result = new RenderTexture(1024, 1024, 0) {dimension = TextureDimension.Tex2D, enableRandomWrite = true};
        _camera = GetComponent<Camera>();
    }

    private void Update()
    {
        _camera.targetTexture = _result;
    }

    public void TickManipulate(Vector3 hitPoint)
    {
        
    }
}
