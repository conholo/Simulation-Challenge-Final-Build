using UnityEngine;

[CreateAssetMenu(menuName = "Create SolidIngredient", fileName = "SolidIngredient", order = 0)]
public class SolidIngredientTemplate : IngredientTemplate
{
    // The prefab that we'll be referencing when we want to actually instantiate this object into the scene.
    [SerializeField] private GameObject _prefab;
    public GameObject Prefab => _prefab;
}