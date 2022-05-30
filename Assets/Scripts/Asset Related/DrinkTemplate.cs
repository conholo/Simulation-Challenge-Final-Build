using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DrinkIngredientRequirements
{
    [SerializeField] private IngredientTemplate _ingredient;
    [SerializeField] private float _quantity;

    public IngredientTemplate Ingredient => _ingredient;
    public float Quantity => _quantity;
}

// Template class that defines the "golden" values associated with a drink.
// These are the values that should be used when comparing against a drink made
// by the user.  Again, this class uses the CreateAssetMenu attribute as it inherits
// from ScriptableObject.  This means we can create assets of this type to be stored in the 
// Asset Folder.
[CreateAssetMenu(menuName = "Create Drink", fileName = "Drink", order = 51)]
public class DrinkTemplate : ScriptableObject
{
    // Which type of container does this drink go in?
    [SerializeField] private ContainerTemplate _containerTemplate;
    [SerializeField] private SimulationObjectLoaderTemplate _objectLoadTemplate;
    // Which ingredients does it need?
    [SerializeField] private List<DrinkIngredientRequirements> _ingredientRequirements;
    // Does it need a garnish?
    [SerializeField] private bool _requiresShake; 
    [SerializeField] private bool _garnishNeeded;
    [SerializeField] private GarnishTemplate _garnish;
    [SerializeField] private Sprite _scrollViewIcon;    
    [SerializeField] private Sprite _recipeListingInfographic;    
    
    // Public Accessor for the container.
    public ContainerTemplate ContainerTemplate => _containerTemplate;
    public SimulationObjectLoaderTemplate ObjectLoaderTemplate => _objectLoadTemplate;
    public bool RequiresShake => _requiresShake;
    public bool GarnishNeeded => _garnishNeeded;
    public GarnishTemplate GarnishTemplate => _garnish;
    public string Name => name;

    public List<DrinkIngredientRequirements> Requirements => _ingredientRequirements;
    public Sprite ScrollViewIcon => _scrollViewIcon;
    public Sprite RecipeListingInfographic => _recipeListingInfographic;

    // Does this Drink Recipe require the given ingredient? 
    public bool IngredientIsInDrink(IngredientTemplate ingredient)
    {
        // If the given ingredient is contained somewhere in the required list, return true.
        foreach(var requirement in _ingredientRequirements)
            if (requirement.Ingredient == ingredient)
                return true;

        // It wasn't found, return false.
        return false;
    }
    
    public DrinkIngredientRequirements FindRequirementEntryOfType(IngredientTemplate template)
    {
        // If the incoming ingredient isn't required, return null. 
        if (!IngredientIsInDrink(template)) return null;
        
        // Find the matching requirement.
        // This is O(n) but I'm not worried about the performance hit as we won't
        // have a very large number of ingredients per drink.
        foreach(var requirement in _ingredientRequirements)
            if (requirement.Ingredient == template)
                return requirement;

        return null;
    }

    public float GetQuantityForIngredient(IngredientTemplate ingredient)
    {
        // See if this drink needs the provided ingredient.
        var requirement = FindRequirementEntryOfType(ingredient);
        
        // If the above value is 'null', it doesn't, therefore return -1.
        if (requirement == null) return -1;

        // It is needed, so return the amount specified in the requirement.
        return requirement.Quantity;
    }
    
    public string GetGarnishDescription()
    {
        return _garnishNeeded ? $"Garnish Required: {_garnish.Name}" : "No Garnish Required";
    }
}