using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;


// Requires a button, as that's the main point of this script.
[RequireComponent(typeof(Button))]

// This script acts as the "entry point" into the simulation for testing.  In other words,
// it kicks off a dummy version of the simulation.  Press a button corresponding to a drink and
// the simulation starts.

//public event Action<Scene, Scene> OnSceneWasLoaded;
public class MainMenu : MonoBehaviour
{

    //List<AsyncOperation> sceneToLoad = new List<AsyncOperation>();

    // Which drink are we making?
    [SerializeField] private DrinkTemplate _dummyDrinkTemplate;

    // Where is the SimulationManager, we need them.
    [SerializeField] private SimulationManager _simulationManager;
    [SerializeField] private string _targetSceneName;

    private Button _startSimulationButton;

    private void Start()
    {

        // Get the Button component on the GameObject.  This is a required component.
        _startSimulationButton = GetComponent<Button>();
        // Similar to an 'event Action<T>'.  When the button is clicked, call the
        // function in parenthesis 'StartSimulation'.
        _startSimulationButton.onClick.AddListener(StartSim);


    }

    private void OnDestroy()
    {
        // Clean up event subscription.  Frees the resource and prevents any errors in the event
        // the button gets destroyed and is notified again.
        _startSimulationButton.onClick.RemoveListener(StartSim);
    }

    private void StartSim()
    {

        Debug.Log("Select recipe");
        _simulationManager.InitializeSimulationOnRecipeSelected(_dummyDrinkTemplate);
        LoadNewScene();
    }

    private void LoadNewScene()
    {
        // NOTE: This can be used when the scene is NOT kept loaded
        DontDestroyOnLoad(_simulationManager);
        var buildIndex = SceneUtility.GetBuildIndexByScenePath(_targetSceneName);
        SceneManager.LoadScene(buildIndex);

        // NOTE: This is can only be used if Main Menu scene is kept loaded
        /*
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadSceneAsync("Demo VR Scene", LoadSceneMode.Additive);
        SceneManager.MoveGameObjectToScene(GameObject.Find("Simulation Manager"), SceneManager.GetSceneByName("Demo VR Scene"));
        // "Destroys all GameObjects associated with the given Scene and removes the Scene from the SceneManager." (SceneManager.UnloadScene - Unity - Manual)
        SceneManager.UnloadSceneAsync(currentScene);
        GameObject.Find("MainMenu").SetActive(false);
        */

    }

}
