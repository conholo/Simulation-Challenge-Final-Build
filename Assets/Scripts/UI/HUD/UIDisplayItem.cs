using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDisplayItem : MonoBehaviour
{
    [SerializeField] private GameObject _testFluidDisplayObject;
    
    [SerializeField] private UIFluidContainerDisplay _fluidContainerDisplay;
    [SerializeField] private TMP_Text _heldItemNameText;
    [SerializeField] private Image _garnishImage;
    [SerializeField] private Image _fluidContainerImage;
    
    public void ClearActiveDisplays()
    {
        _fluidContainerImage.enabled = false;
        _garnishImage.enabled = false;
        _fluidContainerDisplay.Clear();
    }

    public void UpdateDisplay(GameObject obj)
    {
        ClearActiveDisplays();
        var fluidObject = obj.GetComponent<FluidObject>();

        if (fluidObject != null)
        {
            DisplayFluidContainerInformation(fluidObject);
            return;
        }
        
        var garnish = obj.GetComponent<GarnishObject>();

        if (garnish != null)
            DisplayGarnishInformation(garnish);
    }

    private void DisplayGarnishInformation(GarnishObject garnish)
    {
        _garnishImage.enabled = true;
        _garnishImage.sprite = garnish.GarnishTemplate.Sprite;
        _fluidContainerImage.enabled = false;
        _heldItemNameText.SetText(garnish.GarnishTemplate.Name);
    }

    private void DisplayFluidContainerInformation(FluidObject fluidObject)
    {
        _fluidContainerImage.enabled = true;
        _garnishImage.enabled = false;
        _fluidContainerDisplay.SetFluidContents(fluidObject.Contents, fluidObject.CurrentPercentFill, _fluidContainerImage);
        _heldItemNameText.SetText(fluidObject.name);
    }
}
