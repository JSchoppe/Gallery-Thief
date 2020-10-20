using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractTrigger : MonoBehaviour
{
    [SerializeField] [Tooltip("The UI that changes")] GameObject promptUI;
    private PromptTextController textcontroller;
    void Start()
    {
        textcontroller = promptUI.GetComponent<PromptTextController>();
    }

    //while the player's sphere collider is colliding with any of these tags
    private void OnTriggerStay(Collider other)
    {
        switch (other.tag)
        {
            case "Stealable":
                textcontroller.OnInteractivePromptTriggered("Press F to steal", 1);
                break;
            case "Hideable":
                textcontroller.OnInteractivePromptTriggered("Press F to hide", 1);
                break;
            case "Door":
                textcontroller.OnInteractivePromptTriggered("Press F to unlock", 1);
                break;
        }
    }

    //when the player sphere collider exits another
    private void OnTriggerExit(Collider other)
    {
        textcontroller.OnInteractivePromptTriggered("", 0);
    }
}
