using UnityEngine;
using UnityEngine.UI;

public class UIBoardManager : MonoBehaviour
{
    [SerializeField] private UIStatusTabManager _statusManager;
    [SerializeField] private UIRecipeTabManager _recipeListingManager;
    [SerializeField] private UISettingsTabManager _settingsManager;

    private void Start()
    {
        _statusManager.ToggleTabIsActive(true);
        _settingsManager.ToggleTabIsActive(true);
        _statusManager.Initialize();
        _statusManager.ToggleTabIsActive(false);
        _settingsManager.ToggleTabIsActive(false);
        _recipeListingManager.ToggleTabIsActive(true);

        _recipeListingManager.OnStartNewSimulation += StartNewSimulation;
    }

    private void StartNewSimulation(DrinkTemplate selectedObject)
    {
        SimulationManager.Instance.InitializeSimulationOnRecipeSelected(selectedObject);
        OnStatusTabSelected();
    }

    public void OnStatusTabSelected()
    { 
        _statusManager.ToggleTabIsActive(true);
        _settingsManager.ToggleTabIsActive(false);
        _recipeListingManager.ToggleTabIsActive(false);
    }
    
    public void OnSettingsTabSelected()
    { 
        _statusManager.ToggleTabIsActive(false);
        _settingsManager.ToggleTabIsActive(true);
        _recipeListingManager.ToggleTabIsActive(false);
    }

    public void OnRecipeListingsTabSelected()
    { 
        _statusManager.ToggleTabIsActive(false);
        _settingsManager.ToggleTabIsActive(false);
        _recipeListingManager.ToggleTabIsActive(true);
    }
}