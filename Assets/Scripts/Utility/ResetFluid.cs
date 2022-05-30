using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StartingColors
{
    public Color SideColor;
    public Color TopColor;
}

public class ResetFluid : MonoBehaviour
{
    [SerializeField] private List<FluidObject> _allFluids = new List<FluidObject>();
    private List<Vector3> _startingPositions = new List<Vector3>();
    private List<StartingColors> _startingColors = new List<StartingColors>();

    private void Start()
    {
        _startingPositions = _allFluids.Select(t => t.transform.position).ToList();
        _startingColors = _allFluids.Select(t => new StartingColors()
        {
            SideColor = t.GetComponent<FluidObject>().SideColor, 
            TopColor = t.GetComponent<FluidObject>().TopColor
        }).ToList();
    }
    
    public void ResetFluids()
    {
        _allFluids.ForEach(t => t.SetFluidLevels(1.0f));
        for (var i = 0; i < _allFluids.Count; i++)
        {
            _allFluids[i].transform.position = _startingPositions[i];
            _allFluids[i].transform.rotation = Quaternion.identity;
            _allFluids[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
            _allFluids[i].GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            _allFluids[i].GetComponent<Rigidbody>().ResetInertiaTensor();
            _allFluids[i].SetColors(_startingColors[i].SideColor, _startingColors[i].TopColor);
        }
    }
}
