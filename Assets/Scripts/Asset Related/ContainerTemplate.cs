using UnityEngine;

// CreateAssetMenu is an attribute that allows for the creation of Assets in the AssetFolder of a Unity
// project.  Most commonly used on types that inherit from ScriptableObject.
[CreateAssetMenu(menuName = "Create Container", fileName = "Container", order = 53)]
public class ContainerTemplate : ScriptableObject
{
    // The prefab that we'll be referencing when we want to actually instantiate this object into the scene.
    [SerializeField] private ContainerObject _prefab;
    [SerializeField] private Sprite _sprite;
    // A public, readonly reference to the prefab.
    public ContainerObject Prefab => _prefab;
    
    // A public, readonly reference to the name of this container.
    // 'name' is the name of this asset in the asset folder.
    public string Name => name;
    public Sprite Sprite => _sprite;
}