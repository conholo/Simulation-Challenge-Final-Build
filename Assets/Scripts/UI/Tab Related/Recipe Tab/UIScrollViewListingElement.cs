using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIScrollViewListingElement : MonoBehaviour
{
    public event Action<DrinkTemplate> OnRecipeSelected;
    [SerializeField] private Image _image;
    [SerializeField] private Button _selectButton;
    private DrinkTemplate _template;

    private void Awake()
    {
        _selectButton.onClick.AddListener(() => OnRecipeSelected?.Invoke(_template));
    }

    public void SetDrinkTemplate(DrinkTemplate template)
    {
        _template = template;
        _image.sprite = template.ScrollViewIcon;
    }
}