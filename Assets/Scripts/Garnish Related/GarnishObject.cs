using UnityEngine;


public enum GarnishStickType { None, Rim, Float }

// Currently present for type safety.
// Whenever a collision occurs, we want an easy way to ensure that the
// object is of type GarnishObject.
// In the future, this class we contain more behavior and data specific to garnishes.
public class GarnishObject : MonoBehaviour
{
    [SerializeField] private GarnishTemplate _garnishTemplate;
    public GarnishTemplate GarnishTemplate => _garnishTemplate;
    public GarnishStickType TestStickType;
}