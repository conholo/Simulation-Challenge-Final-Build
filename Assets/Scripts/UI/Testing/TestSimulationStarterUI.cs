using UnityEngine;
using UnityEngine.UI;

// Requires a button, as that's the main point of this script.
[RequireComponent(typeof(Button))]

// This script acts as the "entry point" into the simulation for testing.  In other words,
// it kicks off a dummy version of the simulation.  Press a button corresponding to a drink and
// the simulation starts.

public class TestSimulationStarterUI : MonoBehaviour
{
    // Which drink are we making?
    [SerializeField] private DrinkTemplate _dummyDrinkTemplate;
    
    // Where is the SimulationManager, we need them.
    [SerializeField] private SimulationManager _simulationManager;
    
    // The button responsible for notifying the SimulationManager.
    private Button _startSimulationButton;
    
    private void Start()
    {
        // Get the Button component on the GameObject.  This is a required component.
        _startSimulationButton = GetComponent<Button>();
        
        // Similar to an 'event Action<T>'.  When the button is clicked, call the 
        // function in parenthesis 'NotifySimulationManagerOnStart'.  
        _startSimulationButton.onClick.AddListener(NotifySimulationManagerOnStart);
    }

    private void OnDestroy()
    {
        // Clean up event subscription.  Frees the resource and prevents any errors in the event
        // the button gets destroyed and is notified again.
        _startSimulationButton.onClick.RemoveListener(NotifySimulationManagerOnStart);
    }

    private void NotifySimulationManagerOnStart()
    {
        // The button was clicked, so let's notify the SimulationManager to start.
        _simulationManager.InitializeSimulationOnRecipeSelected(_dummyDrinkTemplate);
    }
}
