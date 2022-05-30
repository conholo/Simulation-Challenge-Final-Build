using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// We're using the function OnCollisionEnter, therefore we need to have a collider.
[RequireComponent(typeof(Collider))]
// The point of this script is for Garnish Objects to only pay attention to collisions if it's to the Container instantiated by Drink Object.
public class IgnoreCollisionWithDrinkObject : MonoBehaviour
{
    private void Start()
    {
        foreach (var container in FindObjectsOfType<ContainerObject>())
            Physics.IgnoreCollision(container.gameObject.GetComponent<Collider>(), GetComponent<Collider>(), true);
    }
}
