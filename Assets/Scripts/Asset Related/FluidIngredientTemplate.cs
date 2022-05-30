using UnityEngine;

[CreateAssetMenu(menuName = "Create FluidIngredient", fileName = "FluidIngredient", order = 0)]
public class FluidIngredientTemplate : IngredientTemplate
{
    [SerializeField] private Color _sideColor;
    [SerializeField] private Color _topColor;

    public Color SideColor => _sideColor;
    public Color TopColor => _topColor;
}