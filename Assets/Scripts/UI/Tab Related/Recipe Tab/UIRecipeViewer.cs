using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIRecipeViewer : MonoBehaviour
{
    public event Action OnReturnToRecipeListingsScreenRequested;
    public event Action<DrinkTemplate> OnRequestStartSimulationWithDrink;
    [SerializeField] private Image _drinkModelImage;
    [SerializeField] private Button _returnToRecipeSelectionScreenButton;
    [SerializeField] private Button _requestMakeDrinkButton;
    [SerializeField] private Image _drinkTemplateInfographic;
    
    private UIRenderFinishedDrinkCamera _uiFinalDrinkRenderer;
    private DrinkTemplate _currentTemplate;

    private void Awake()
    {
        _uiFinalDrinkRenderer = FindObjectOfType<UIRenderFinishedDrinkCamera>();
        _returnToRecipeSelectionScreenButton.onClick.AddListener(() => OnReturnToRecipeListingsScreenRequested?.Invoke());
        _requestMakeDrinkButton.onClick.AddListener(RequestMakeDrink);
    }

    private void RequestMakeDrink()
    {
        if (_currentTemplate == null) return;
        OnRequestStartSimulationWithDrink?.Invoke(_currentTemplate);
    }

    public void UpdateDisplayRecipe(DrinkTemplate template)
    {
        _currentTemplate = template;
        _drinkTemplateInfographic.sprite = template.RecipeListingInfographic;
        //var texture = _uiFinalDrinkRenderer.GetTargetTexture.Texture2DFromRenderTexture();
        //var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),  new Vector2(0.5f, 0.5f));
        //_drinkModelImage.sprite = sprite;
    }
}
