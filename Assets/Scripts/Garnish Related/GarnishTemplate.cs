using UnityEngine;

// CreateAssetMenu is an attribute that allows for the creation of Assets in the AssetFolder of a Unity
// project.  Most commonly used on types that inherit from ScriptableObject.
[CreateAssetMenu(menuName = "Create Garnish", fileName = "Garnish", order = 52)]
public class GarnishTemplate : ScriptableObject
{
    // The prefab that we'll be referencing when we want to actually instantiate this object into the scene.
    [SerializeField] private GarnishObject _prefab;
    [SerializeField] private Sprite _sprite;
    [SerializeField] private GarnishStickType _stickType;
    
    // A public, readonly reference to the name of this container.
    // 'name' is the name of this asset in the asset folder.
    public string Name => name;
    public Sprite Sprite => _sprite;
    public GarnishObject Prefab => _prefab;
    public GarnishStickType StickType => _stickType;
}