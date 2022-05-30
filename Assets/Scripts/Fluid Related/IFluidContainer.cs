using System.Collections.Generic;
using UnityEngine;

public interface IFluidContainer
{
    float FillHeight { get; }
    void ModifyFluidContents(float amountToChange, Dictionary<FluidIngredientTemplate, float> fluidRatiosToAdd, LiquidShakeState currentShakeState, float percentShaken);
    public void ToggleIsCorked(bool isCorked);
}