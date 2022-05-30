using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GarnishRimStick : MonoBehaviour, IGarnishStick
{
    public Transform StickTransform => transform;
    public event Action<GarnishObject, int> OnGarnishAdded;
    [SerializeField] private Transform _stickPoint;
    
    private void OnTriggerEnter(Collider other)
    {
        var garnish = other.gameObject.GetComponent<GarnishObject>();
        if (garnish == null || garnish.TestStickType != GarnishStickType.Rim) return;
        //if (garnish == null || garnish.GarnishTemplate.StickType != GarnishStickType.Rim) return;

        var interactable = garnish.GetComponent<XRGrabInteractable>();
        interactable.enabled = false;
        garnish.GetComponent<Rigidbody>().useGravity = false;
        garnish.GetComponent<Rigidbody>().isKinematic = true;
        garnish.GetComponent<Collider>().enabled = false;
        
        garnish.transform.SetParent(_stickPoint);
        garnish.transform.localPosition = Vector3.zero;
        
        OnGarnishAdded?.Invoke(garnish, 1);
    }
}