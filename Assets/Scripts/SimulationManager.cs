using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;


// The 'hub', or managerial class, designed to manage all work done on a drink.
// The drink template, DrinkObject, is created by the SimulationManager who carries
// out all of the detection, reporting any changes here.  The changes made to drinks
// will be judged/scored/processed here.
public class SimulationManager : MonoBehaviour
{
    [SerializeField] private ShakerBottom _shakerBottomPrefab;
    [SerializeField] private ShakerTop _shakerTopPrefab;
    [SerializeField] private Transform _shakerTopSpawn;
    [SerializeField] private Transform _shakerBottomSpawn;
    
    public static event Action<DrinkStatusReport> OnDrinkStatusChanged;
    public static event Action<DrinkStatusReport> OnNewDrinkSimulationBegin;
    
    [SerializeField] private bool _easyMode = true;
    public static DrinkTemplate GoldenValueTemplate => _instance._goldenValueTemplate;

    private static SimulationManager _instance;
    public static SimulationManager Instance => _instance;
    public bool InProgress { get; private set; }

    [SerializeField] private float _fluidAmountThreshold;
    // The template prefab object we'll instantiate everytime a new drink is to be made.
    [SerializeField] private DrinkObject _drinkObject;
    // The golden value template, or "the right answer", for how to make the current drink selected
    // by the user.
    private DrinkTemplate _goldenValueTemplate;
    public static double PercentThresholdSuccess => _instance._fluidAmountThreshold;

    // The active DrinkObject, or the object that is currently responsible for handling and reporting
    // changes made to the drink that the user is creating.
    private DrinkObject _activeDrinkObject;
    private readonly List<GameObject> _loadedObjectsForCurrentSimulation = new List<GameObject>();

    private DrinkStatusReport _statusReport;

    private ShakerBottom _activeShakerBottom;
    private ShakerTop _activeShakerTop;

    private void Awake()
    {
        // Singleton
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        InProgress = false;
    }

    public static bool IsEasyMode(GameObject obj)
    {
        return _instance._easyMode && (obj.GetComponentInParent<DrinkObject>() != null || obj.GetComponentInParent<ShakerBottom>() != null);
    }
    

    // Performs all initialization required at the time of recipe selection.
    // This currently includes storing the golden value, or "the right answer", which will
    // be later accessed when scoring changes made to the drink.
    // This also includes the instantiation of a new template everytime a new recipe starts.
    // This also includes subscribing, or listening, to the DrinkObjects event 'OnGarnishAddedToDrinkObject'.
    // The function that listens to this event is 'OnGarnishAddedToDrinkObject'.  This is where validation occurs based
    // on the correct recipe.
    // Once all initialization is done by the SimulationManager, the DrinkObject is told to initialize all of it's
    // components - instantiating any GameObjects (models), loading any effects, or calling the initialization of its
    // own collision/detection handlers.
    public void InitializeSimulationOnRecipeSelected(DrinkTemplate drinkTemplate)
    {
        if (_activeDrinkObject != null)
        {
            Destroy(_activeDrinkObject.gameObject);
            if(_loadedObjectsForCurrentSimulation.Count > 0)
                _loadedObjectsForCurrentSimulation.ForEach(t => Destroy(t.gameObject));
            
            _loadedObjectsForCurrentSimulation.Clear();
        }
        
        _goldenValueTemplate = drinkTemplate;

        foreach (var objectLoad in drinkTemplate.ObjectLoaderTemplate.SpawnPairs)
            _loadedObjectsForCurrentSimulation.Add(Instantiate(objectLoad.Object, objectLoad.SpawnTransform.position, Quaternion.identity));

        _activeDrinkObject = Instantiate(_drinkObject, transform.position, Quaternion.identity);
        _activeDrinkObject.transform.localScale = Vector3.one * 0.1f;
        _activeDrinkObject.OnGarnishAdded += OnGarnishAddedToDrinkObject;
        _activeDrinkObject.OnFluidChanged += OnDrinkObjectLiquidChanged;
        _activeDrinkObject.InitializeDrinkObject(drinkTemplate.ContainerTemplate);

        _statusReport = new DrinkStatusReport(drinkTemplate);
        OnNewDrinkSimulationBegin?.Invoke(_statusReport);
        InProgress = true;
    }

    private void OnDrinkObjectLiquidChanged(Dictionary<FluidIngredientTemplate, float> contents, LiquidShakeState shakeState, float shakePercent)
    {
        _statusReport.ShakeMixtureProgress.ShakeState = shakeState;
        _statusReport.ShakeMixtureProgress.PercentShaken = shakePercent;
        foreach (var fluidContent in contents)
            _statusReport.UpdateFluidIngredientProgress(fluidContent.Key, fluidContent.Value);
        
        OnDrinkStatusChanged?.Invoke(_statusReport);
    }

    // Validation of garnish changes done here.
    private void OnGarnishAddedToDrinkObject(GarnishObject garnish, int count)
    {
        _statusReport.UpdateGarnishProgress(garnish.GarnishTemplate, count);
        OnDrinkStatusChanged?.Invoke(_statusReport);
    }
    
    public void ResetSimulation()
    {
        SceneManager.LoadScene(0);
    }
}

public class DrinkStatusReport
{
    public List<FluidIngredientProgress> DrinkIngredientsProgress { get; }
    public List<GarnishProgress> GarnishProgress { get; }
    public ShakeMixtureProgress ShakeMixtureProgress { get; } = new ShakeMixtureProgress();
    public DrinkTemplate Template { get; }

    public DrinkStatusReport(DrinkTemplate template)
    {
        Template = template;
        DrinkIngredientsProgress = new List<FluidIngredientProgress>();
        
        foreach(var requiredIngredients in template.Requirements)
        {
            DrinkIngredientsProgress
                .Add(
                    new FluidIngredientProgress(requiredIngredients.Ingredient as FluidIngredientTemplate, 
                    template.GetQuantityForIngredient(requiredIngredients.Ingredient))
                    );
        }
        
        GarnishProgress = new List<GarnishProgress>{new GarnishProgress(template.GarnishTemplate, template.GarnishNeeded ? 1 : 0)};
    }

    public void UpdateFluidIngredientProgress(FluidIngredientTemplate template, float currentPercent)
    {
        var match = DrinkIngredientsProgress.FirstOrDefault(t => t.Template == template);
        if (match == null) return;
        match.CurrentPercent = currentPercent;
    }

    public void UpdateGarnishProgress(GarnishTemplate template, int changeAmount)
    {
        var match = GarnishProgress.FirstOrDefault(t => t.Template == template);
        if (match == null) return;
        match.CurrentCount += changeAmount;
    }
}

public class FluidIngredientProgress
{
    public float CurrentPercent;
    public float RequiredPercent;
    public FluidIngredientTemplate Template;

    public FluidIngredientProgress(FluidIngredientTemplate template, float requiredPercent)
    {
        RequiredPercent = requiredPercent;
        Template = template;
    }
}

public class GarnishProgress
{
    public int CurrentCount;
    public int RequiredCount;
    public GarnishTemplate Template;

    public GarnishProgress(GarnishTemplate template, int requiredCount)
    {
        Template = template;
        RequiredCount = requiredCount;
    }
}



public class ShakeMixtureProgress
{
    public bool RequiresShake { get; set; }
    public float PercentShaken { get; set; } = 0.0f;
    public LiquidShakeState ShakeState { get; set; } = LiquidShakeState.UnderShaken;
}