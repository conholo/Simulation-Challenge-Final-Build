using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// We're using the function OnCollisionEnter, therefore we need to have a collider.
[RequireComponent(typeof(Collider))]
public class StickShaker : MonoBehaviour
{
        //public event Action<int> OnShakerClosed;
        //public event Action<float> OnShakerMixed;
        private bool _shakerTopClosed = false;
        private GameObject _shakerTop;

        private const float SnapThreshold = 0.9f;

        private float _validShakeDistance = 0.1f; //1.0f;
        private float _percentPerValidUnit = 0.1f;
        private float _shakePercentComplete = 0.0f;

        private Vector3 _correctMixingVector;

        private float _distanceTravelled;
        private Vector3 _startingPos;
        private Vector3 _previousPos;
        private Vector3 _currentPos;

        private bool _shakeStarted;

        // NOTE: this script is a component of the Stick Point object (within Shaker Bottom).
        private void OnCollisionEnter(Collision other)
        {
            var checkIfShakerTop = other.gameObject.name;
            if (checkIfShakerTop == "Shaker Top(Clone)")
            {
                _shakerTop = other.gameObject;
                if (ValidateAngle(_shakerTop.transform.position))
                {
                  Debug.Log("Shaker is Closed Properly!!!");
                  _shakerTopClosed = true;
                  _shakerTop.transform.parent = transform;
                  _shakerTop.transform.localPosition = Vector3.zero;
                  var shakerTopRigidbody = _shakerTop.GetComponent<Rigidbody>();
                  shakerTopRigidbody.useGravity = false;
                  shakerTopRigidbody.isKinematic = true;
                  //OnShakerClosed?.Invoke(_shakerTopClosed);
                }
                //other.gameObject.GetComponent<Rigidbody>().isKinematic = true;  causes onCollisionExit to be called.
            }

        }

        private void OnCollisionExit(Collision other)
        {
          var checkIfShakerTop = other.gameObject.name;
          if (checkIfShakerTop == "Shaker Top(Clone)")
          {
              Debug.Log("Shaker has been open again.");
              //OnShakerMixed?.Invoke(_shakePercentComplete);
          }
        }

        private bool ValidateAngle(Vector3 incomingPosition)
        {
            var differenceDirection = Vector3.Normalize(incomingPosition - this.transform.position);
            var dot = Vector3.Dot(differenceDirection, transform.up);
            return dot >= SnapThreshold;
        }

        private void Update()
        {
            //Debug.Log($"Is Valid: {ValidateAngle(_shakerTop.transform.position)}");
            if (_shakePercentComplete >= 1.0f)
            {
                Debug.Log("Successfully mixed!");
                _shakePercentComplete = 1.0f;
            }
            if (_shakeStarted == false && _shakerTopClosed == true)
            {
                _startingPos = this.transform.position;
                _previousPos = _startingPos;
                _shakeStarted = true;
                _correctMixingVector = transform.up;
                Debug.Log("Shake Started");
            }

            _currentPos = this.transform.position;
            var difference = Vector3.Normalize(_currentPos - _previousPos);
            var dot = Math.Abs(Vector3.Dot(difference, _correctMixingVector));
            //Debug.Log($"dot = {dot} -> difference = {difference}, _correctMixingVector = {_correctMixingVector}");
            if (dot > 0.8f && dot < 1.0f)
            {
              Debug.Log($"dot = {dot} -> difference = {difference}, _correctMixingVector = {_correctMixingVector}");
              var successfulFrameDistance = Vector3.Distance(_previousPos, _currentPos);
              _distanceTravelled = successfulFrameDistance;
              Debug.Log($"_distanceTravelled = {_distanceTravelled}");
              //if (_distanceTravelled % _validShakeDistance == 0)
              if (_distanceTravelled >= _validShakeDistance)
              {
                  _shakePercentComplete += _percentPerValidUnit;
                  Debug.Log($"ShakePercentComplete = {_shakePercentComplete}");
              }
            }
            _previousPos = _currentPos;
        }

}
