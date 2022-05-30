using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class FloatingGarnishPlane : MonoBehaviour, IGarnishStick
{
    [Range(-2, 2)]
    [SerializeField] private float _manualHeightOffset;
    [SerializeField] private bool _debugRadius;
    [SerializeField] private float _radius;
    [SerializeField] private float _minYDeviation = -1.0f;
    [SerializeField] private float _maxYDeviation = 1.0f;
    [SerializeField] private float _bobbleSpeed;

    private System.Random _rng;

    public Transform StickTransform => transform;
    public event Action<GarnishObject, int> OnGarnishAdded;

    private FluidContainerContentsManager _fluidContainerManager;
    private LiquidWobble _wobbleManager;

    private readonly List<FloatingGarnishData> _stuckGarnishes = new List<FloatingGarnishData>();

    class FloatingGarnishData
    {
        public GarnishObject Object;
        public float RandomSpeedOffset;
        public float Offset;
    }
    
    private void Awake()
    {
        _rng = new System.Random(Guid.NewGuid().GetHashCode());
        _fluidContainerManager = GetComponentInParent<FluidContainerContentsManager>();
        _wobbleManager = GetComponentInParent<LiquidWobble>();
    }

    private Vector3 GetRandomPositionInsideRadius(float radius)
    {
        var theta = (float)_rng.NextDouble() * 2.0f * Mathf.PI;
        var rngOffset = Mathf.Sqrt((float) _rng.NextDouble());
        var randomHeightOffset = Random.Range(_minYDeviation, _maxYDeviation);
        return transform.position + new Vector3(Mathf.Cos(theta) * rngOffset, randomHeightOffset, Mathf.Sin(theta) * rngOffset) * radius;
    }

    private void Update()
    {
        transform.position = _fluidContainerManager.FillHeightPosition + Vector3.up * _manualHeightOffset;
        if (_stuckGarnishes.Count <= 0 || _fluidContainerManager.FillPercent <= 0.0f) return;
        _stuckGarnishes.ForEach(AnimateGarnish);
    }

    private void AnimateGarnish(FloatingGarnishData data)
    {
        data.Object.transform.rotation = Quaternion.Euler(transform.up);
        var currentHeight = data.Offset + transform.position.y;
        var targetHeight = currentHeight + Mathf.Sin(Time.time * _bobbleSpeed * data.RandomSpeedOffset) * _maxYDeviation;
        data.Object.transform.position = Vector3.up * targetHeight + Vector3.right * data.Object.transform.position.x + Vector3.forward * data.Object.transform.position.z;
    }

    private void OnTriggerEnter(Collider other)
    {
        var garnish = other.gameObject.GetComponent<GarnishObject>();
        
        //if (garnish == null || garnish.GarnishTemplate.StickType != GarnishStickType.Float || _stuckGarnishes.Any(t => t.Object == garnish)) return;
        if (garnish == null || garnish.TestStickType != GarnishStickType.Float || _stuckGarnishes.Any(t => t.Object == garnish)) return;

        Debug.Log($"Hit with garnish with name {other.name}");
        garnish.GetComponent<Rigidbody>().useGravity = false;
        garnish.GetComponent<Rigidbody>().isKinematic = true;
        garnish.GetComponent<Collider>().enabled = false;
        
        garnish.transform.parent = transform;
        garnish.transform.position = GetRandomPositionInsideRadius(_radius);

        var garnishFloatData = new FloatingGarnishData
        {
            Object = garnish,
            Offset = garnish.transform.position.y - transform.position.y,
            RandomSpeedOffset =(float) _rng.NextDouble() * 2.0f - 1.0f
        };
        _stuckGarnishes.Add(garnishFloatData);
        OnGarnishAdded?.Invoke(garnish, 1);
    }

    private void OnDrawGizmos()
    {
        if (!_debugRadius) return;
        
        Gizmos.color = Color.green;
        for (var i = 0; i < 360; i++)
        {
            var theta = (float)i / 360 * Mathf.PI * 2.0f;
            var position = transform.position + new Vector3(Mathf.Sin(theta) * _radius, 0.0f, Mathf.Cos(theta) * _radius);
            
            Gizmos.DrawLine(transform.position, position);
        }
    }
}