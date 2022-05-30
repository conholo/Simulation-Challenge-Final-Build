using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ObjectLocationPair
{
    public GameObject Object;
    public Transform SpawnTransform;
}

[CreateAssetMenu(menuName = "Create SimulationObjectLoaderTemplate", fileName = "SimulationObjectLoaderTemplate", order = 55)]
public class SimulationObjectLoaderTemplate : ScriptableObject
{
    [SerializeField] private List<ObjectLocationPair> _spawnPairs = new List<ObjectLocationPair>();
    public List<ObjectLocationPair> SpawnPairs => _spawnPairs;
}