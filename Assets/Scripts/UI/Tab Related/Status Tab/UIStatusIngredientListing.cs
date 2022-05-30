using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIStatusIngredientListing : MonoBehaviour
{
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _percentText;
    [SerializeField] private Image _image;
    [SerializeField] private Image _greenLightImage;
    [SerializeField] private Slider _ingredientAmountProgress;
    public FluidIngredientTemplate Template { get; set; }

    public void UpdateListing(Sprite sprite, string ingredientName, float currentPercent, float requiredPercent)
    {
        _image.sprite = sprite;
        _nameText.SetText(ingredientName);
        
        var percentComplete = currentPercent / requiredPercent * 100.0f;
        _ingredientAmountProgress.value = currentPercent / requiredPercent;
        var isDone = Mathf.Abs(100.0f - percentComplete) <= 25.0f;
        _percentText.SetText(percentComplete.ToString("n2"));
        _greenLightImage.color = isDone ? Color.green : Color.red;
    }
}