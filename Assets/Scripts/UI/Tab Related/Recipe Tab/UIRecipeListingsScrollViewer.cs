using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRecipeListingsScrollViewer : MonoBehaviour
{
    
    private enum CardPosition { Left, Center, Right }

    private const string AssetPath = "Drink Templates";
    public event Action<DrinkTemplate> OnRecipeSelected;

    [SerializeField] private RectTransform _pivot;
    [SerializeField] private float _animationTime;
    [SerializeField] private UIScrollViewListingElement[] _carousel = new UIScrollViewListingElement[3];
    [SerializeField] private Button _nextButton;
    [SerializeField] private Button _previousButton;
    
    [SerializeField] private RectTransform _previousPosition;
    [SerializeField] private RectTransform _centerPosition;
    [SerializeField] private RectTransform _nextPosition;
    
    private readonly List<DrinkTemplate> _availableTemplates = new List<DrinkTemplate>();

    private readonly Dictionary<CardPosition, UIScrollViewListingElement> _cards =
        new Dictionary<CardPosition, UIScrollViewListingElement>();

    private readonly Dictionary<CardPosition, RectTransform> _cardPositions =
        new Dictionary<CardPosition, RectTransform>();
    
    private int _activeDrinkTemplateIndex;
    private bool _isAnimating;

    private void Awake()
    {
        _nextButton.onClick.AddListener(DisplayNext);
        _previousButton.onClick.AddListener(DisplayPrevious);
        
        _cards.Add(CardPosition.Left, _carousel[0]);
        _cards.Add(CardPosition.Center, _carousel[1]);
        _cards.Add(CardPosition.Right, _carousel[2]);

        foreach (var card in _carousel)
            card.OnRecipeSelected += template => OnRecipeSelected?.Invoke(template);
        
        _cardPositions.Add(CardPosition.Left, _previousPosition);
        _cardPositions.Add(CardPosition.Center, _centerPosition);
        _cardPositions.Add(CardPosition.Right, _nextPosition);
    }

    private void ShiftCardsLeft()
    {
        var center = _cards[CardPosition.Center];
        _cards[CardPosition.Center] = _cards[CardPosition.Right];
        _cards[CardPosition.Right] = _cards[CardPosition.Left];
        _cards[CardPosition.Left] = center;

        _activeDrinkTemplateIndex = GetNextIndex();
        _cards[CardPosition.Right].SetDrinkTemplate(_availableTemplates[GetNextIndex()]);
        _cards[CardPosition.Right].transform.position = _cardPositions[CardPosition.Right].position;
    }
    
    private void ShiftCardsRight()
    {
        var center = _cards[CardPosition.Center];
        _cards[CardPosition.Center] = _cards[CardPosition.Left];
        _cards[CardPosition.Left] = _cards[CardPosition.Right];
        _cards[CardPosition.Right] = center;
        
        _activeDrinkTemplateIndex = GetPreviousIndex();
        _cards[CardPosition.Left].SetDrinkTemplate(_availableTemplates[GetPreviousIndex()]);
        _cards[CardPosition.Left].transform.position = _cardPositions[CardPosition.Left].position;
    }

    private void ToggleParentToPivot(bool isParent)
    {
        foreach (var card in _carousel)
            card.transform.SetParent(isParent ? _pivot.transform : transform);
    }
    
    private void DisplayPrevious()
    {
        if (_isAnimating) return;
        StartCoroutine(MovePivot(false));
    }

    private void DisplayNext()
    {
        if (_isAnimating) return;
        StartCoroutine(MovePivot(true));
    }

    private IEnumerator MovePivot(bool left)
    {
        _isAnimating = true;
        _cards[CardPosition.Left].gameObject.SetActive(true);
        _cards[CardPosition.Right].gameObject.SetActive(true);

        ToggleParentToPivot(true);
        
        var timer = 0.0f;
        var target = left ? _previousPosition.position : _nextPosition.position;

        while (timer < _animationTime)
        {
            timer += Time.deltaTime;

            var percent = timer / _animationTime;

            _pivot.transform.position = Vector3.Lerp(_centerPosition.position, target, percent);
            yield return null;
        }

        _pivot.transform.position = target;
        
        ToggleParentToPivot(false);
        _pivot.transform.position = _centerPosition.position;
        
        if(left)
            ShiftCardsLeft();
        else
            ShiftCardsRight();
        
        _cards[CardPosition.Left].gameObject.SetActive(false);
        _cards[CardPosition.Right].gameObject.SetActive(false);
        _isAnimating = false;
    }

    private void SetCarouselTemplates()
    {
        _carousel[0].SetDrinkTemplate(_availableTemplates[GetPreviousIndex()]);
        _carousel[1].SetDrinkTemplate(_availableTemplates[_activeDrinkTemplateIndex]);
        _carousel[2].SetDrinkTemplate(_availableTemplates[GetNextIndex()]);
    }

    public void Initialize()
    {
        var drinkTemplates = Resources.LoadAll<DrinkTemplate>(AssetPath);
        foreach (var drinkTemplate in drinkTemplates)
            _availableTemplates.Add(drinkTemplate);

        SetCarouselTemplates();
    }

    private int GetPreviousIndex()
    {
        return _activeDrinkTemplateIndex == 0 ? _availableTemplates.Count - 1 : _activeDrinkTemplateIndex - 1;
    }

    private int GetNextIndex()
    {
        return _activeDrinkTemplateIndex == _availableTemplates.Count - 1 ? 0 : _activeDrinkTemplateIndex + 1;
    }

    public void DisplayCarousel()
    {
    }
}