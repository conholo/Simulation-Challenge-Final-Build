using UnityEngine;

// We're using the function OnCollisionEnter, therefore we need to have a collider.
[RequireComponent(typeof(Collider))]
public class ContainerObject : MonoBehaviour
{
    private ContainerTemplate _template;
    public ContainerTemplate ContainerTemplate => _template;
    
    public void SetContainerTemplate(ContainerTemplate template)
    {
        _template = template;
    }
}
