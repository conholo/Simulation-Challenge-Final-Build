using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// We're using the function OnCollisionEnter, therefore we need to have a collider.
[RequireComponent(typeof(Collider))]
public class GarnishHandler : MonoBehaviour
{
    public event Action<GarnishObject, int> NotifyOnGarnishAdded;
    private List<IGarnishStick> _garnishStickObjects = new List<IGarnishStick>();
    
    private void Awake()
    {
        _garnishStickObjects = GetComponentsInChildren<IGarnishStick>().ToList();
        _garnishStickObjects.ForEach(t => t.OnGarnishAdded += OnGarnishAdded);
    }

    private void OnDestroy()
    {
        _garnishStickObjects.ForEach(t => t.OnGarnishAdded -= OnGarnishAdded);
    }

    public void MoveGarnishSticker(float height)
    {
        foreach (var garnishStickObject in _garnishStickObjects)
        {
            if (!(garnishStickObject is FloatingGarnishPlane)) continue;
            
            garnishStickObject.StickTransform.position =
                new Vector3(garnishStickObject.StickTransform.position.x, height, garnishStickObject.StickTransform.position.z);
        }
    }
    
    private void OnGarnishAdded(GarnishObject garnishObject, int count)
    {
        NotifyOnGarnishAdded?.Invoke(garnishObject, count);
    }
}