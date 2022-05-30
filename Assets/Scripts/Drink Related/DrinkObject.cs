using System;
using System.Collections.Generic;
using UnityEngine;


// The most important object in our scene.  Delegates work to various components responsible for
// reading input, collisions and changes made to the drink.  All changes reported by these workers are then
// sent here.  The data can then be processed by the DrinkObject however it needs, eventually sending the data
// to the SimulationManager so it can process the changes made to the drink.
public class DrinkObject : MonoBehaviour
{
    private ContainerObject _containerObject;
    private FluidObject _fluidObject;
    private GarnishHandler _garnishHandler;

    //private GameObject _stickShakerObject;

    // DUPLICATED COMMENT - See ContainerObject.cs
    // Action is a delegate - meaning it delegates work for handling some kind of functionality.
    // Inside the brackets <> indicates what parameters are to be used for a listener of the given event.
    // For this example, any function that listens to this event Action, must have the same function signature
    // as: void (GarnishObject, int);
    // Actions by default return void, Func<T> on the other hand, returns a value other than void.
    // For example, Func<GarnishObject, int> requires a listener to have a signature of the following type:
    // GarnishObject (int);  Meaning, the function returns a GarnishObject and accepts an int.
    public event Action<GarnishObject, int> OnGarnishAdded;
    public event Action<Dictionary<FluidIngredientTemplate, float>, LiquidShakeState, float> OnFluidChanged;


    public void InitializeDrinkObject(ContainerTemplate containerTemplate)
    {
        // Get the component responsible for notifying us of changes to the fluid content.

        // The "cloning" of the provided prefab.  Creates a new GameObject in the scene
        // at runtime which is a clone of the prefab.  The second argument states which transform
        // should act as the cloned objects parent.
        _containerObject = Instantiate(containerTemplate.Prefab, transform);
        
        _containerObject.transform.localScale = Vector3.one;
        _fluidObject = _containerObject.GetComponent<FluidObject>();
        _garnishHandler = _containerObject.GetComponent<GarnishHandler>();

        // This is the DrinkObject subscribing to the ContainerObjects event.
        // Whenever the _containerObjects event 'OnGarnishAdded' is called,
        // we want the function following the '+=' to be called.  Note that this function,
        // 'NotifySimulationManagerOnGarnishAdded', matches the signature of the 'OnGarnishAdded'.

        _fluidObject.OnFluidChanged += NotifySimulationManagerOnFluidVolumeChanged;
        //_containerObject.OnGarnishAdded += NotifySimulationManagerOnGarnishAdded;
        _garnishHandler.NotifyOnGarnishAdded += NotifySimulationManagerOnGarnishAdded;

        // Set the parent of the new container as the DrinkObject.  Keeps the hierarchy in the scene
        // cleaner.
        _containerObject.transform.parent = transform;
    }

    private void NotifySimulationManagerOnFluidVolumeChanged(Dictionary<FluidIngredientTemplate, float> contents, LiquidShakeState shakeState, float percentShaken)
    {
        //Debug.Log("Fluid content changed!  Here's the new breakdown:");
        //foreach (var fluid in contents)
        //    Debug.Log($"\t{fluid.Key.Name}: {fluid.Value}");

        OnFluidChanged?.Invoke(contents, shakeState, percentShaken);
        _garnishHandler.MoveGarnishSticker(_fluidObject.CurrentPercentFill);
    }

    // Listening function for when the container objects event 'OnGarnishAdded' is called.
    private void NotifySimulationManagerOnGarnishAdded(GarnishObject garnish, int count)
    {
        // Notify the world we got hit!
        Debug.Log($"We got hit by a garnish! {garnish.name}");

        // Invoke our event to whoever cares.
        // The '?.' means that if no one is listening, don't bother calling Invoke(),
        // however, if someone is listening, call Invoke().
        OnGarnishAdded?.Invoke(garnish, count);
    }
}
