using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIStatusGarnishListing : MonoBehaviour
{
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _amountText;
    [SerializeField] private Image _image;
    [SerializeField] private Image _greenLightImage;
    public GarnishTemplate Template { get; set; }

    public void UpdateListing(Sprite sprite, string ingredientName, int requiredCount, int currentCount)
    {
        _image.sprite = sprite;
        _nameText.SetText(ingredientName);
        _amountText.SetText($"{currentCount}/{requiredCount}");
        _greenLightImage.color = currentCount >= requiredCount ? Color.green : Color.red;
    }
}