using System;
using UnityEngine;

public sealed class UIRecipeTabManager : Tab
{
    public event Action<DrinkTemplate> OnStartNewSimulation;
    [SerializeField] private Transform _parent;
    [SerializeField] private UIRecipeViewer _singleRecipeViewer;
    [SerializeField] private UIRecipeListingsScrollViewer _scrollViewer;
    
    private void Awake()
    {
        _scrollViewer.Initialize();
        _scrollViewer.OnRecipeSelected += ActivateRecipeViewer;
        _singleRecipeViewer.OnReturnToRecipeListingsScreenRequested += ReturnToRecipeListings;
        _singleRecipeViewer.OnRequestStartSimulationWithDrink += StartNewSimulation;
    }

    private void StartNewSimulation(DrinkTemplate selectedTemplate)
    {
        OnStartNewSimulation?.Invoke(selectedTemplate);
    }

    private void ReturnToRecipeListings()
    {
        _scrollViewer.gameObject.SetActive(true);
        _singleRecipeViewer.gameObject.SetActive(false);
        _scrollViewer.DisplayCarousel();
    }

    private void ActivateRecipeViewer(DrinkTemplate templateToDisplay)
    {
        _scrollViewer.gameObject.SetActive(false);
        _singleRecipeViewer.gameObject.SetActive(true);
        _singleRecipeViewer.UpdateDisplayRecipe(templateToDisplay);
    }

    public override void ToggleTabIsActive(bool active)
    {
        _parent.gameObject.SetActive(active);
        _singleRecipeViewer.gameObject.SetActive(false);
        _scrollViewer.gameObject.SetActive(active);
    }
}