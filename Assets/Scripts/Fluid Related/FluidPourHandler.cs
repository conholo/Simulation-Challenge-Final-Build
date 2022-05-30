using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(LineRenderer))]
public class FluidPourHandler : MonoBehaviour
{
    private const float TwoPI = Mathf.PI * 2.0f;    
    
    [SerializeField] private int _linePointsCount;
    [SerializeField] private float _linePointOffsetMultiplier;

    private LayerMask _noFluidColliderLayerMask;
    private readonly List<Vector3> _pourPoints = new List<Vector3>();
    private Collider[] _streamEndCollisions;
    
    private LineRenderer _lineRenderer;
    private Collider _playerCollider;

    private int _mask;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _streamEndCollisions = new Collider[_linePointsCount];
        _playerCollider = FindObjectOfType<XROrigin>().GetComponent<Collider>();
    }
    
    public void SetGradientColors(Color startColor, Color endColor)
    {
        if (_lineRenderer == null)
            _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.startColor = startColor;
        _lineRenderer.endColor = endColor;
    }

    public void SetGradientWidth(float beginWidth, float endWidth)
    {
        if (_lineRenderer == null)
            _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.startWidth = beginWidth;
        _lineRenderer.endWidth = endWidth;
    }

    public void SetActive(bool isActive)
    {
        if (_lineRenderer == null)
            _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.enabled = isActive;
    }

    public void Pour(
        Vector3 origin, Vector3 direction, float pourSpeed, float drainTime, 
        Dictionary<FluidIngredientTemplate, float> pouringContainerContents, LiquidShakeState shakeState, float percentShaken, 
        Color lineColor)
    {
        CalculatePourLine(origin, direction, pourSpeed, drainTime, pouringContainerContents, shakeState, percentShaken);
        _lineRenderer.material.color = lineColor;
        _lineRenderer.startColor = lineColor;
        _lineRenderer.endColor = lineColor;
        _lineRenderer.positionCount = _pourPoints.Count;
        _lineRenderer.SetPositions(_pourPoints.ToArray());
    }

    private void CalculatePourLine(
        Vector3 origin, Vector3 direction, float pourSpeed, float drainTime, 
        Dictionary<FluidIngredientTemplate, float> pouringContainerContents, LiquidShakeState shakeState, float percentShaken)
    {
        // Reset from the previous pour.
        _pourPoints.Clear();
        
        const float gravity = 4.8f;
        // Velocity, acceleration calculations.
        var v0 = direction * pourSpeed;
        var a = v0 / drainTime;
        a.y -= gravity;
        
        // Current step along path.
        var t = 0.0f;
        // Increment each step.
        var step = drainTime / _linePointsCount;
        
        for (var i = 0; i < _linePointsCount; i++, t += step)
        {
            // Calculate the position via kinematic equation at this step of the curve.
            var t2 = t * t;

            var offset = i == 0 ? new Vector3() : 
                new Vector3(
                    Mathf.Cos(Random.Range(-TwoPI, TwoPI) * t) * Mathf.Sin(Random.Range(-TwoPI, TwoPI) * t), 
                    0.0f, 
                    Mathf.Sin( Random.Range(-TwoPI, TwoPI) * t) * Mathf.Cos(Random.Range(-TwoPI, TwoPI) * t)) 
                * (_linePointOffsetMultiplier * Time.deltaTime);
            var segmentPosition = origin + v0 * t + a * (0.5f * t2) + offset;
            _pourPoints.Add(segmentPosition);
            
            // Check all of the colliders that we hit.
            var hitCount = Physics.OverlapSphereNonAlloc(segmentPosition, 0.05f, _streamEndCollisions);
            
            // For each collider that was hit, determine how we should handle the pour.
            for (var j = 0; j < hitCount; j++)
            {
                if (_streamEndCollisions[j] == null || _streamEndCollisions[j] == _playerCollider) continue;
                if (LayerMask.LayerToName(_streamEndCollisions[j].gameObject.layer) == "Garnish") continue;
                // If there was no collider (it was destroyed), or the collider is our own, move on to the next.
                if (_streamEndCollisions[j].gameObject == gameObject) continue;

                // Check if we hit a fluid recipient.
                var recipient = _streamEndCollisions[j].GetComponent<IFluidContainer>();
                
                // If we didn't hit one, handle a splash, dispersion effect.
                if (recipient == null) return;
                // Otherwise, pour into the recipient's container, exit the function.
                AddToContainer(recipient, segmentPosition, pourSpeed, pouringContainerContents, shakeState, percentShaken);
                return;
            }
        }
    }
    
    private void AddToContainer(IFluidContainer container, Vector3 closestSegment, float pourSpeed, Dictionary<FluidIngredientTemplate, float> addRatios, LiquidShakeState shakeState, float percentShaken)
    {
        // Add to the recipients fluid volume.
        var addAmount = Mathf.Clamp(Time.deltaTime * pourSpeed, 0.0f, 1.0f);
        container.ModifyFluidContents(addAmount, addRatios, shakeState, percentShaken);
        // Add the final point of the LineRenderer, set to the position of the closest segment, but at the height
        // of the container.
        _pourPoints.Add(new Vector3(closestSegment.x, container.FillHeight, closestSegment.z));
    }

    public void RenderDebugStream()
    {
        if (_pourPoints == null || _pourPoints.Count <= 0) return;
        Gizmos.color = Color.blue;
        for (var i = 1; i < _pourPoints.Count; i++)
        {
            Gizmos.DrawLine(_pourPoints[i - 1], _pourPoints[i]);
            Gizmos.DrawWireSphere(_pourPoints[i], 0.05f);
        }
    }
}