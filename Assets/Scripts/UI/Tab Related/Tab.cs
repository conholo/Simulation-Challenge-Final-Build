using UnityEngine;

public abstract class Tab : MonoBehaviour
{
    [SerializeField] private GameObject _tabContentHolder;
    
    public virtual void ToggleTabIsActive(bool active)
    {
        _tabContentHolder.SetActive(active);
    }
}