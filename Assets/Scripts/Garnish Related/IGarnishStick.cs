using System;
using UnityEngine;

public interface IGarnishStick
{
    Transform StickTransform { get; }
    event Action<GarnishObject, int> OnGarnishAdded;
}