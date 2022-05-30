using UnityEngine;

public class LiquidWobble : MonoBehaviour
{
    [SerializeField] private float _recoveryRate;
    [SerializeField] private float _wobbleSpeed;
    [SerializeField] private float _maxWobble;
    private Material _fluidMaterial;

    private Vector2 _wobbleAdd;
    private Vector2 _wobble;

    public Vector2 Wobble => _wobble;

    private Vector3 _velocity;
    private Vector3 _lastRotation;
    private Vector3 _angularVelocity;
    private Vector3 _lastPosition;

    private float _pulse;
    private float _counter;
    private static readonly int WobbleX = Shader.PropertyToID("_WobbleX");
    private static readonly int WobbleZ = Shader.PropertyToID("_WobbleZ");

    private void Awake()
    {
        _fluidMaterial = transform.GetChild(0).GetComponent<MeshRenderer>().material;
    }

    private void Update()
    {
        _counter += Time.deltaTime;

        _wobbleAdd.x = Mathf.Lerp(_wobbleAdd.x, 0.0f, Time.deltaTime * _recoveryRate);
        _wobbleAdd.y = Mathf.Lerp(_wobbleAdd.y, 0.0f, Time.deltaTime * _recoveryRate);

        _pulse = 2.0f * Mathf.PI * _wobbleSpeed;
        
        _wobble.x = _wobbleAdd.x * Mathf.Sin(_pulse * _counter);
        _wobble.y = _wobbleAdd.y * Mathf.Cos(_pulse * _counter);
 
        _fluidMaterial.SetFloat(WobbleX, _wobble.x);
        _fluidMaterial.SetFloat(WobbleZ, _wobble.y);

        _velocity = (_lastPosition - transform.position) / Time.deltaTime;
        _angularVelocity = _lastRotation - transform.rotation.eulerAngles;

        _wobbleAdd.x += Mathf.Clamp((_velocity.x + _angularVelocity.z * 0.2f) * _maxWobble, -_maxWobble, _maxWobble);
        _wobbleAdd.y += Mathf.Clamp((_velocity.z + _angularVelocity.x * 0.2f) * _maxWobble, -_maxWobble, _maxWobble);
        
        _lastPosition = transform.position;
        _lastRotation = transform.rotation.eulerAngles;

    }
}