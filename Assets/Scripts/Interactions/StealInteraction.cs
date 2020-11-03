using System;
using UnityEngine;

public sealed class StealInteraction : MonoBehaviour, IInteractable
{
    private void Start()
    {
        PromptVisible = true;
        PromptMessage = "Steal";
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
        //TODO: play stealing animation here
        //TODO: add stolen item to inventory
        //TODO: hide gameobject that was stolen
    }

    /// <summary>
    /// This is called once the art piece has been stolen.
    /// </summary>
    public event Action OnInteractionComplete;
}
