using System;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// Manages interaction with IInteractables.
/// </summary>
public sealed class PlayerInteractor : MonoBehaviour
{
    // TODO: This event routing is dubious,
    // there should be a more explicit place
    // that stores this player state.
    public event Action ArtPieceStolen;
    public void TriggerArtStolen() { ArtPieceStolen?.Invoke(); }

    #region State Enums
    private enum InteractionState : byte
    {
        Free, Interacting
    }
    #endregion
    #region Inspector Fields
    [Tooltip("The player that this interactor is tied to.")]
    [SerializeField] private PlayerController player = null;
    [Tooltip("The canvas where interaction prompts are drawn to.")]
    [SerializeField] private GameObject interactorCanvas = null;
    [Tooltip("The text that displays the input prompt.")]
    [SerializeField] private TMP_Text promptText = null;
    [Tooltip("The image that draws the radial meter for progress.")]
    [SerializeField] private Image fillImage = null;
    [Tooltip("Defines the keyboard key that starts interaction.")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [Tooltip("The camera that defines the direction the prompt faces towards.")]
    [SerializeField] private Transform cameraTransform = null;
    [Tooltip("The transform of the prompt canvas.")]
    [SerializeField] private Transform promptTransform = null;
    [SerializeField] private Animator anim = null;
    #endregion
    #region Fields
    private InteractionState state;
    private IInteractable focusedInteractable;
    private List<IInteractable> nearbyInteractables;
    #endregion
    #region MonoBehaviour Implementation
    private void Start()
    {
        state = InteractionState.Free;
        nearbyInteractables = new List<IInteractable>();
        GameplayHUDSingleton.PlayerCanCrouch = true;
    }
    private void Update()
    {
        // Check for interaction.
        switch (state)
        {
            case InteractionState.Free:
                ScanInteractables();
                ProcessInput();
                break;
            case InteractionState.Interacting:

                break;
        }

        void ScanInteractables()
        {
            if (nearbyInteractables.Count > 0)
            {
                IInteractable nearestInteractable = null;
                float nearestDistance = float.MaxValue;
                foreach (IInteractable interactable in nearbyInteractables)
                {
                    float squaredDistance =
                        transform.position.SquaredDistanceTo(interactable.InteractionVisiblePoint);
                    if (squaredDistance < nearestDistance
                        && !Physics.Linecast(transform.position, interactable.InteractionVisiblePoint))
                    {
                        nearestInteractable = interactable;
                        nearestDistance = squaredDistance;
                    }
                }
                if (nearestInteractable != focusedInteractable)
                {
                    focusedInteractable?.OnPromptExit(player);
                    focusedInteractable = nearestInteractable;
                    focusedInteractable?.OnPromptEnter(player);
                }
            }
            else
                focusedInteractable = null;
        }
        void ProcessInput()
        {
            if (Input.GetKeyDown(interactKey)
                && focusedInteractable != null)
            {
                // Initiate interaction and listen for
                // completion of interaction.
                // run grab animation

                focusedInteractable.InteractionComplete += OnInteractionComplete;
                state = InteractionState.Interacting;
                GameplayHUDSingleton.PlayerCanCrouch = false;
                focusedInteractable.Interact();
                anim.SetBool("canInteract", true);
            }
        }

        if (focusedInteractable != null
            && focusedInteractable.PromptVisible)
        {
            interactorCanvas.SetActive(true);
            GameplayHUDSingleton.InteractionFocus = focusedInteractable;

            fillImage.fillAmount = focusedInteractable.PromptProgress;
            promptText.text = focusedInteractable.PromptMessage;
            // Update the facing direction of the prompt.
            promptTransform.forward = promptTransform.position - cameraTransform.position;
            promptTransform.position = focusedInteractable.PromptLocation;
        }
        else
        {
            interactorCanvas.SetActive(false);
            GameplayHUDSingleton.InteractionFocus = null;
            //exits and/or doesn't run animation
            anim.SetBool("canInteract", false);
        }
    }
    private void OnInteractionComplete()
    {
        GameplayHUDSingleton.PlayerCanCrouch = true;
        focusedInteractable.InteractionComplete -= OnInteractionComplete;
        state = InteractionState.Free;
    }
    #endregion
    #region Interactables Trigger Processing
    private void OnTriggerEnter(Collider other)
    {
        if (other.HasComponent(out IInteractable interactable)
            && !nearbyInteractables.Contains(interactable))
        {
            nearbyInteractables.Add(interactable);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.HasComponent(out IInteractable interactable)
            && nearbyInteractables.Contains(interactable))
        {
            nearbyInteractables.Remove(interactable);
        }
    }
    #endregion
}
