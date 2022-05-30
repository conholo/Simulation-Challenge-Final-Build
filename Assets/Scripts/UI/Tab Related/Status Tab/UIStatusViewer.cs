using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIStatusViewer : MonoBehaviour
{
    [SerializeField] private UIStatusIngredientListing _ingredientListingPrefab;
    [SerializeField] private UIStatusGarnishListing _garnishListingPrefab;
    [SerializeField] private Transform _ingredientListingParent;
    [SerializeField] private Transform _garnishListingParent;
    [SerializeField] private TMP_Text _mixingProgressBarPercentText;
    [SerializeField] private Slider _mixingProgressBar;
    [SerializeField] private TMP_Text _name;
    private readonly List<UIStatusIngredientListing> _ingredientListings = new List<UIStatusIngredientListing>();
    private readonly List<UIStatusGarnishListing> _garnishListings = new List<UIStatusGarnishListing>();

    public void InitializeViewer()
    {
        SimulationManager.OnDrinkStatusChanged += UpdateStatus;
        SimulationManager.OnNewDrinkSimulationBegin += InitializeNewDrink;
        ShakerBottom.OnMixingPercentChanged += UpdateMixerDisplay;
    }

    private void OnDestroy()
    {
        SimulationManager.OnDrinkStatusChanged -= UpdateStatus;
        SimulationManager.OnNewDrinkSimulationBegin -= InitializeNewDrink;
        ShakerBottom.OnMixingPercentChanged -= UpdateMixerDisplay;
    }

    private void InitializeNewDrink(DrinkStatusReport statusReport)
    {
        ResetViewer();
        var activeTemplate = statusReport.Template;
        _name.SetText(activeTemplate.Name);

        foreach (var fluidIngredient in activeTemplate.Requirements)
        {
            var ingredientStatusListing = Instantiate(_ingredientListingPrefab, _ingredientListingParent);
            ingredientStatusListing.UpdateListing(fluidIngredient.Ingredient.Sprite, fluidIngredient.Ingredient.Name, 0.0f, 0.0f);
            _ingredientListings.Add(ingredientStatusListing);
            ingredientStatusListing.Template = fluidIngredient.Ingredient as FluidIngredientTemplate;
        }

        if (activeTemplate.GarnishNeeded)
        {
            var garnishListing = Instantiate(_garnishListingPrefab, _garnishListingParent);
            garnishListing.UpdateListing(activeTemplate.GarnishTemplate.Sprite, activeTemplate.GarnishTemplate.Name, 1, 0);
            _garnishListings.Add(garnishListing);
            garnishListing.Template = activeTemplate.GarnishTemplate;
        }
    }

    private void ResetViewer()
    {
        foreach(var ingredientListing in _ingredientListings)
            Destroy(ingredientListing.gameObject);
        
        _ingredientListings.Clear();

        foreach(var garnishListing in _garnishListings)
            Destroy(garnishListing.gameObject);

        _garnishListings.Clear();
        _mixingProgressBar.value = 0.0f;
    }

    private void UpdateMixerDisplay(float pct)
    {
        _mixingProgressBar.value = pct;
        _mixingProgressBarPercentText.SetText("Percent Mixed: " + (pct * 100.0f).ToString("n2"));
    }

    private void UpdateStatus(DrinkStatusReport statusReport)
    {
        foreach (var fluidIngredientProgress in statusReport.DrinkIngredientsProgress)
        {
            var match = _ingredientListings.FirstOrDefault(t => t.Template == fluidIngredientProgress.Template);
            if (match == null) continue;
            match.UpdateListing(fluidIngredientProgress.Template.Sprite, fluidIngredientProgress.Template.Name, fluidIngredientProgress.CurrentPercent, fluidIngredientProgress.RequiredPercent);
        }

        foreach (var garnishProgress in statusReport.GarnishProgress)
        {
            var match = _garnishListings.FirstOrDefault(t => t.Template == garnishProgress.Template);
            if (match == null) continue;
            match.UpdateListing(garnishProgress.Template.Sprite, garnishProgress.Template.Name, garnishProgress.RequiredCount, garnishProgress.CurrentCount);
        }

        _mixingProgressBar.enabled = statusReport.ShakeMixtureProgress.RequiresShake;
        _mixingProgressBarPercentText.enabled = statusReport.ShakeMixtureProgress.RequiresShake;
        
        if (!statusReport.ShakeMixtureProgress.RequiresShake) return;
        _mixingProgressBarPercentText.SetText("Percent Mixed: " + (statusReport.ShakeMixtureProgress.PercentShaken * 100.0f).ToString("n2"));
        _mixingProgressBar.value = statusReport.ShakeMixtureProgress.PercentShaken;
    }
}