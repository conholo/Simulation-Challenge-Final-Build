using System.Globalization;
using TMPro;
using UnityEngine;


public class UIFluidContentsDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text _fluidNameText;
    [SerializeField] private TMP_Text _fluidAmountText;

    public void Initialize(string fluidName, float fluidPercent)
    {
        _fluidNameText.SetText(fluidName);
        _fluidAmountText.SetText(fluidPercent.ToString(CultureInfo.InvariantCulture));
    }
}