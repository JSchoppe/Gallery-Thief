using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteraction : MonoBehaviour, IInteractable
{
    [SerializeField] [Tooltip("The UI that changes")] GameObject promptUI = null;
    private PromptTextController textcontroller;

    private bool doorLocked; //if door is locked or not

    private void Start()
    {
        textcontroller = promptUI.GetComponent<PromptTextController>();
        doorLocked = true;
        PromptVisible = false;
        PromptMessage = "Key Needed";
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
            textcontroller.OnInteractivePromptTriggered(PromptMessage, PromptVisible);

            if (doorLocked) //&& player has key
            {
                //PromptMessage = "Unlock";
            }
            else
            {
                PromptMessage = "Lock";
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PromptVisible = false;
        textcontroller.OnInteractivePromptTriggered(PromptMessage, PromptVisible);
    }

    public void Interact()
    {
        //TODO: check if player has key to unlock door
        //TODO: unlock door
        doorLocked = false;
        //TODO: give ability to lock door
    }

    /// <summary>
    /// This is called when the player has
    /// locked/unlocked a door.
    /// </summary>
    public event Action OnInteractionComplete;
}
