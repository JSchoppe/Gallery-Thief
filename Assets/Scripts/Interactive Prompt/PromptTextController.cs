using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PromptTextController : MonoBehaviour
{
    private TMP_Text promptText;
    private CanvasGroup canvasGroup;

    private void Start()
    {
        promptText = GetComponent<TMP_Text>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnInteractivePromptTriggered(string text, float opacity)
    {
        canvasGroup.alpha = opacity;
        promptText.text = text;
    }
}