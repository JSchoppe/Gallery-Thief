using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractTrigger : MonoBehaviour, IInteractable
{
    [SerializeField] [Tooltip("The UI that changes")] GameObject promptUI;
    private PromptTextController textcontroller;
    void Start()
    {
        textcontroller = promptUI.GetComponent<PromptTextController>();
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
    /// <summary>
    /// This is called once the pickpocketing has completed
    /// or has been rejected due to position.
    /// </summary>
    public event Action OnInteractionComplete;

    public void Interact()
    {

    }


    Vector3 closetObject;
    GameObject currentInteractable;
    //while the player's sphere collider is colliding with any of these tags
    private void OnTriggerStay(Collider other)
    {   
        closetObject = other.ClosestPointOnBounds(transform.position);

        if(other.tag == "Stealable")
        {
            currentInteractable = other.gameObject;
            textcontroller.OnInteractivePromptTriggered("Steal", 1);
        }
        if (other.tag == "Hideable")
        {
            currentInteractable = other.gameObject;
            textcontroller.OnInteractivePromptTriggered("Hide", 1);
        }
        if (other.tag == "Door")
        {
            currentInteractable = other.gameObject;
            textcontroller.OnInteractivePromptTriggered("Unlock", 1);
        }
        if (other.tag == "Guard")
        {
            currentInteractable = other.gameObject;
            textcontroller.OnInteractivePromptTriggered("Pickpocket", 1);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(closetObject, 0.1f);
    }
    private void Update()
    {
        Debug.Log(closetObject);
        Debug.Log(currentInteractable);

    }

    //when the player sphere collider exits another
    private void OnTriggerExit(Collider other)
    {
        textcontroller.OnInteractivePromptTriggered("", 0);
    }
}
