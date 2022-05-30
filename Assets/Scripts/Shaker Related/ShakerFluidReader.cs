using UnityEngine;

public class ShakerFluidReader : MonoBehaviour
{
    private FluidObject _fluidObject;
    private ShakerBottom _shakerBottom;

    private void Awake()
    {
        _fluidObject = GetComponentInParent<FluidObject>();
        _shakerBottom = GetComponent<ShakerBottom>();
    }

    private void Update()
    {
    }
}

