using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BootUI : MonoBehaviour
{
    [SerializeField] private Slider progressSlider;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text percentText;
    
    public void SetProgress(float value)
    {
        value = Mathf.Clamp01(value);
        if(progressSlider != null )
        {
            progressSlider.value = value;
        }
        if(percentText != null)
        {
            percentText.text = $"{Mathf.RoundToInt(value * 100)}%";
        }
    }
    public void SetText(string text)
    {
        if (*)
        {
            
        }
    }
}
