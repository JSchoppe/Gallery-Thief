using System;
using UnityEngine;

public class HideInteraction : MonoBehaviour, IInteractable
{
    private void Start()
    {
        PromptVisible = true;
        PromptMessage = "Hide";
        PromptLocation = Vector3.zero;
    }

    public bool PromptVisible { get; private set; }
    public Vector3 PromptLocation { get; private set; }
    public string PromptMessage { get; private set; }
    public void OnPromptEnter(PlayerController player)
    {

    }
    public void OnPromptExit(PlayerController player)
    {

    }

    public void Interact()
    {
        //TODO: play animation here
        //TODO: player not visible to guards
    }

    /// <summary>
    /// This is called once the player is hiding.
    /// </summary>
    public event Action OnInteractionComplete;
}
