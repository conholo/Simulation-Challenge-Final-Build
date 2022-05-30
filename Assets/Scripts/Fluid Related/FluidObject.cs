using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum LiquidShakeState { UnderShaken, Shaken, OverShaken }


[RequireComponent(typeof(FluidContainerContentsManager), typeof(FluidPourHandler))]
public class FluidObject : MonoBehaviour
{
    public event Action<Dictionary<FluidIngredientTemplate, float>, LiquidShakeState, float> OnFluidChanged;

    [SerializeField] private FluidIngredientTemplate _defaultFluid;
    [Range(0, 1)]
    [SerializeField] private float _defaultFillPercent;
    private float _currentPercentFill = 0.0f;
    
    private readonly Dictionary<FluidIngredientTemplate, float> _currentContents = new Dictionary<FluidIngredientTemplate, float>();
    private LiquidShakeState _shakeState = LiquidShakeState.UnderShaken;
    private Color _currentTopColor = Color.clear;
    private Color _currentSideColor = Color.clear;
    private float _percentShaken;

    
    public float CurrentPercentFill => _currentPercentFill;
    public Dictionary<FluidIngredientTemplate, float> Contents => _currentContents;
    public LiquidShakeState ShakeState => _shakeState;
    public float PercentShaken => _percentShaken;
    public Color TopColor => _currentTopColor;
    public Color SideColor => _currentSideColor;


    private FluidContainerContentsManager _containerContentsManager;

    private void OnValidate()
    {
        _containerContentsManager = GetComponent<FluidContainerContentsManager>();
        
        if (Mathf.Abs(_defaultFillPercent - _currentPercentFill) > Mathf.Epsilon)
        {
            _currentPercentFill = _defaultFillPercent;
            _containerContentsManager.SetFillPercent(_currentPercentFill);
        }

        SetColorsFromTemplate();
        
        _containerContentsManager.SetColors(_currentTopColor, _currentSideColor);
    }

    private void SetColorsFromTemplate()
    {
        if (_defaultFluid != null && _currentTopColor != _defaultFluid.TopColor)
            _currentTopColor = _defaultFluid.TopColor;
        
        if (_defaultFluid != null && _currentSideColor != _defaultFluid.SideColor)
            _currentSideColor = _defaultFluid.SideColor;
    }

    private void Awake()
    {
        _containerContentsManager = GetComponent<FluidContainerContentsManager>();
        
        SetColorsFromTemplate();
        if (_defaultFluid != null) 
            _currentContents.Add(_defaultFluid, _defaultFillPercent);

        foreach (var kvp in _currentContents)
        {
            //Debug.Log($"Container: {gameObject.name} contains {kvp.Value * 100.0f}% {kvp.Key.Name}");
        }
    }

    private void Start()
    {
        _currentTopColor = _defaultFluid == null ? _currentTopColor : _defaultFluid.TopColor;
        _currentSideColor = _defaultFluid == null ? _currentSideColor : _defaultFluid.SideColor;
        _currentPercentFill = _defaultFillPercent;
        
        _containerContentsManager.SetColors(_currentTopColor, _currentSideColor);
        _containerContentsManager.SetFillPercent(_currentPercentFill);
        _containerContentsManager.OnFluidContentsChanged += OnFluidContentsChanged;
    }

    private void OnDestroy()
    {
        _containerContentsManager.OnFluidContentsChanged -= OnFluidContentsChanged;
    }

    private void Update()
    {
        _containerContentsManager.Tick(_currentContents, _shakeState, _percentShaken);
    }

    private void ValidateFluidMixing(FluidIngredientTemplate key, float fractionalToRecipientContribution)
    {
        if (SimulationManager.IsEasyMode(gameObject) && SimulationManager.Instance.InProgress)
        {
            var ingredient = key as IngredientTemplate;
            var match = SimulationManager.GoldenValueTemplate.Requirements
                .FirstOrDefault(t => t.Ingredient == ingredient);
                
            if (match == null) return;

            if (_currentContents.ContainsKey(key))
            {
                // If we're within a certain threshold of the quantity, don't allow adding anymore.
                if (Mathf.Abs(match.Quantity - _currentContents[key]) < 0.01f)
                    return;

                _currentContents[key] += fractionalToRecipientContribution;
            }
            else
            {
                _currentContents.Add(key, fractionalToRecipientContribution);
            }
        }
        else
        {
            if (_currentContents.ContainsKey(key))
                _currentContents[key] += fractionalToRecipientContribution;
            else
                _currentContents.Add(key, fractionalToRecipientContribution);
        }
    }
    
    private float UpdateAndSumFluidContents(Dictionary<FluidIngredientTemplate, float> incomingContents, float changeAmount)
    {
        changeAmount = Mathf.Abs(changeAmount);
        var startingSideColor = _currentSideColor;
        var startingTopColor = _currentTopColor;
        foreach(var fluid in incomingContents)
        {
            _currentSideColor = Color.Lerp(startingSideColor, fluid.Key.SideColor, 0.5f);
            _currentTopColor = Color.Lerp(startingTopColor, fluid.Key.TopColor, 0.5f);
        }
        
        var sumIncomingContents = incomingContents.Sum(t => t.Value);
        
        foreach (var kvp in incomingContents)
        {
            var relativeToIncomingContribution = kvp.Value / sumIncomingContents;
            var fractionalToRecipientContribution = relativeToIncomingContribution * changeAmount;
            //Debug.Log($"{kvp.Key.Name} contributes: {relativeToIncomingContribution * 100.0f}%.  This equals {fractionalToRecipientContribution * 100.0f}% this frame.");

            ValidateFluidMixing(kvp.Key, fractionalToRecipientContribution);
            //Debug.Log($"Container: {gameObject.name} contains {kvp.Value * 100.0f}% {kvp.Key.Name}");
        }

        var newPercent = _currentContents.Sum(t => t.Value);
        Debug.Log($"{gameObject} has {_currentContents.Count} fluids");
        foreach(var fluid in _currentContents)
            Debug.Log($"{gameObject} now contains {fluid.Value * 100.0f}% {fluid.Key.Name}");
        return newPercent;
    }

    private void OnFluidContentsChanged(float newVolumePercent, Dictionary<FluidIngredientTemplate, float> incomingRatios, float changeAmount, LiquidShakeState incomingShakeState, float percentShaken)
    {
        var isAddingLiquid = Mathf.Sign(changeAmount) > 0;
        _currentPercentFill = isAddingLiquid
            ? UpdateAndSumFluidContents(incomingRatios, changeAmount)
            :  newVolumePercent;

        switch (isAddingLiquid)
        {
            // If fluid is being added to this container, the only way for the new fluid in this container to be shaken is if
            // it was empty and the incoming fluid was already shaken - otherwise, the combined fluids yield an unshaken drink.
            case true:
            {
                _percentShaken = percentShaken;
                _shakeState = IsEmpty() && incomingShakeState == LiquidShakeState.Shaken ? LiquidShakeState.Shaken : LiquidShakeState.UnderShaken;
                break;
            }
            case false:
            {
                // Uniform loss of liquid between all liquids in container.
                var averageLossForAllLiquids = changeAmount / _currentContents.Count;
                foreach(var fluid in _currentContents.Keys.ToList())
                    _currentContents[fluid] = _currentContents[fluid] - averageLossForAllLiquids <= 0.0f ? 0.0f : _currentContents[fluid] - averageLossForAllLiquids;

                TryEmpty();
                break;
            }
        }

        // Update the colors and fill percent of the manager so they can change the graphics.
        _containerContentsManager.SetColors(_currentTopColor, _currentSideColor);
        _containerContentsManager.SetFillPercent(_currentPercentFill);
        
        // Notify any listeners of the change.
        OnFluidChanged?.Invoke(_currentContents, _shakeState, percentShaken);
    }

    private void TryEmpty()
    {
        if (_currentContents.Count <= 0 || _currentPercentFill > 0.05f) return;
        
        _currentSideColor = Color.black;
        _currentTopColor = Color.black;
        _currentContents.Clear();
        _shakeState = LiquidShakeState.UnderShaken;
        _currentPercentFill = 0.0f;
        _percentShaken = 0.0f;
    }

    public void SetFluidLevels(float percent)
    {
        _currentPercentFill = percent;
        _containerContentsManager.SetFillPercent(_currentPercentFill);
    }

    public void SetColors(Color sideColor, Color topColor)
    {
        _currentSideColor = sideColor;
        _currentTopColor = topColor;
        _containerContentsManager.SetColors(_currentTopColor, _currentSideColor);
    }

    private bool IsEmpty()
    {
        return _currentPercentFill <= 0.05f;
    }

    public void SetShakeState(LiquidShakeState state)
    {
        _shakeState = state;
    }

    public void SetShakePercent(float shakePercentComplete)
    {
        _percentShaken = shakePercentComplete;
    }

    public void ToggleNoEmpty(bool noEmpty)
    {
        _containerContentsManager.ToggleIsCorked(noEmpty);
    }
}