using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Game.Boot
{
    public class BootLoadingView : MonoBehaviour
    {
        [Header("Loading")]
        [SerializeField] private Slider progressSlider;
        [SerializeField] private TMP_Text percentText;
        [SerializeField] private TMP_Text messageText;

        [Header("Error")]
        [SerializeField] private GameObject errorPanel;
        [SerializeField] private TMP_Text errorText;
        [SerializeField] private Button retryButton;


        public void SetMessage(string message)
        {
            if(message != null)
            {
                messageText.text = message;
            }
        }
        public void SetProgress(float value)
        {
            value = Mathf.Clamp01(value);
            if (progressSlider != null)
            {
                progressSlider.value = value;
            }
            if (percentText != null)
            {
                percentText.text = $"{Mathf.RoundToInt(value * 100)}%";
            }
        }
        
        public void ShowError(string errorMessage, System.Action onRetry)
        {
            if(errorPanel!=null)
            {
                errorPanel.SetActive(true);
            }
            if(errorText != null)
            {
                errorText.text = errorMessage;
            }
            if(retryButton != null)
            {
                retryButton.onClick.RemoveAllListeners();
                retryButton.onClick.AddListener(() => onRetry?.Invoke());
            }
        }
        public void HideError()
        {
            if (errorPanel != null)
            {
                errorPanel.SetActive(false);
            }
            if(retryButton!=null)
            {
                retryButton.onClick.RemoveAllListeners();
            }
        }
    }
}