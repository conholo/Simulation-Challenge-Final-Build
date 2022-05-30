using UnityEngine;

public class TestSelectRecipe : MonoBehaviour
{
    [SerializeField] private DrinkTemplate _template;

    private void Start()
    {
        SimulationManager.Instance.InitializeSimulationOnRecipeSelected(_template);
    }
}
