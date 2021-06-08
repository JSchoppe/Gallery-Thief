using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public sealed class PromptTextController : MonoBehaviour
{
    private TMP_Text promptText;
    Vector3 promptPosition;

    private void Start()
    {
        promptText = GetComponent<TMP_Text>();
        promptPosition = transform.position;
    }

    public void OnInteractivePromptTriggered(string text, bool visible)
    {
        gameObject.SetActive(visible);
        promptText.text = text;
    }
}