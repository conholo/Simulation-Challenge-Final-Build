using System;
using UnityEngine;

// We're using the function OnCollisionEnter, therefore we need to have a collider.
[RequireComponent(typeof(Collider))]
public class ShakerBottom : MonoBehaviour
{
    [SerializeField] private bool _test;
    public static event Action<float> OnMixingPercentChanged;
    // Constants
    private float PercentPerValidUnit => _test ? 50.0f : 0.001f;
    private const float SnapThreshold = 0.9f;

    // Variables
    [SerializeField] private Transform _stickLocation;
    [SerializeField] private float _validShakeDistance;
    [SerializeField] private float _maxConnectDistance;

    // State
    private float _shakePercentComplete;
    private bool _shakeStarted;
    private Vector3 _previousPosition;
    private Vector3 _currentPosition;


    // References
    [SerializeField] private ShakerTop _shakerTop;
    private FluidObject _fluidObject;
    public float PercentShaken => _shakePercentComplete;
    public bool ShakeStarted => _shakeStarted;

    private void Awake()
    {
        _fluidObject = GetComponent<FluidObject>();
        //_shakerTop = FindObjectOfType<ShakerTop>();
    }

    private bool ValidateAngle(Vector3 incomingPosition)
    {
        var differenceDirection = Vector3.Normalize(incomingPosition - transform.position);
        var dot = Vector3.Dot(differenceDirection, transform.up);
        return dot >= SnapThreshold;
    }

    private void Mix()
    {
        if (_shakerTop.IsConnectedToBottom)
        {
            if (!_shakeStarted)
            {
                _currentPosition = transform.position;
                _previousPosition = _currentPosition;
                _shakeStarted = true;
                _shakePercentComplete = _fluidObject.PercentShaken;
                _fluidObject.ToggleNoEmpty(true);
            }
            else
            {
                if (_shakePercentComplete >= 1.0f)
                {
                    _shakePercentComplete = 1.0f;
                    _fluidObject.SetShakeState(LiquidShakeState.Shaken);
                    _fluidObject.SetShakePercent(_shakePercentComplete);
                    return;
                }

                _currentPosition = transform.position;
                var successfulFrameDistance = Vector3.Distance(_previousPosition, _currentPosition);
                if (successfulFrameDistance >= _validShakeDistance)
                {
                    _shakePercentComplete += PercentPerValidUnit;
                    OnMixingPercentChanged?.Invoke(_shakePercentComplete);
                    _fluidObject.SetShakePercent(_shakePercentComplete);
                }

                _previousPosition = _currentPosition;
            }
        }
        else
        {
            _fluidObject.ToggleNoEmpty(false);
            _shakeStarted = false;
        }
    }

    private void DetectShakerTop()
    {
        if (!_shakerTop.IsHeld) return;

        Debug.DrawRay(_stickLocation.position, transform.up * _maxConnectDistance, Color.green);
        if (!Physics.Raycast(new Ray(_stickLocation.position, transform.up), out var hitInfo, _maxConnectDistance))
            return;

        var top = hitInfo.transform.GetComponent<ShakerTop>();
        if (!top) return;

        var isValidAngle = ValidateAngle(top.transform.position);
        if (!isValidAngle) return;

        _shakerTop.Snap();
    }

    private void Update()
    {
        Mix();
        DetectShakerTop();
    }
}
