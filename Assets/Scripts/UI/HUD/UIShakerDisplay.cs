using System;
using UnityEngine;
using UnityEngine.UI;

public class UIShakerDisplay : MonoBehaviour
{
    [SerializeField] private Slider _progressBar;
    [SerializeField] private ShakerBottom _shakerBottom;
    
    private void Update()
    {
        _progressBar.gameObject.SetActive(_shakerBottom.ShakeStarted);   
        _progressBar.value = _shakerBottom.PercentShaken;
    }
}