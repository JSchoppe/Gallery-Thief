using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealInteraction : MonoBehaviour, IInteractable
{
    [SerializeField] [Tooltip("The UI that changes")] GameObject promptUI = null;
    private PromptTextController textcontroller;

    private void Start()
    {
        textcontroller = promptUI.GetComponent<PromptTextController>();
        PromptVisible = false;
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

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            PromptVisible = true;
        }
        textcontroller.OnInteractivePromptTriggered(PromptMessage, PromptVisible);
    }

    private void OnTriggerExit(Collider other)
    {
        PromptVisible = false;
        textcontroller.OnInteractivePromptTriggered(PromptMessage, PromptVisible);
    }

    public void Interact()
    {
        //TODO: play stealing animation here
        //TODO: add stolen item to inventory
        //TODO: hide gameobject that was stolen
    }

    /// <summary>
    /// This is called once the art piece
    /// has been stolen.
    /// </summary>
    public event Action OnInteractionComplete;
}
