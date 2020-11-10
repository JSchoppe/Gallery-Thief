using System;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Manages interaction with IInteractables.
/// </summary>
public sealed class PlayerInteractor : MonoBehaviour
{
    // TODO: This event routing is dubious,
    // there should be a more explicit place
    // that stores this player state.
    public event Action ArtPieceStolen;

    #region State Enums
    private enum InteractionState : byte
    {
        Free, Interacting
    }
    #endregion
    #region Inspector Fields
    [Tooltip("The player that this interactor is tied to.")]
    [SerializeField] private PlayerController player = null;
    [Tooltip("The text that displays the input prompt.")]
    [SerializeField] private TMP_Text promptText = null;
    [Tooltip("Defines the keyboard key that starts interaction.")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    // TODO this should be accomplished via parenting relationships in
    // the scene; not done programatically.
    [Tooltip("The camera that defines the direction the prompt faces towards.")]
    [SerializeField] private Transform cameraTransform = null;
    [Tooltip("The transform of the prompt canvas.")]
    [SerializeField] private Transform promptTransform = null;
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
                if (focusedInteractable is StealInteraction)
                {
                    focusedInteractable.InteractionComplete += () =>
                    {
                        // TODO: this is a hotfix, see comment at top.
                        ArtPieceStolen?.Invoke();
                        OnInteractionComplete();
                    };
                }
                else
                    focusedInteractable.InteractionComplete += OnInteractionComplete;
                state = InteractionState.Interacting;
                GameplayHUDSingleton.PlayerCanCrouch = false;
                focusedInteractable.Interact();
            }
        }

        if (focusedInteractable != null
            && focusedInteractable.PromptVisible)
        {
            GameplayHUDSingleton.InteractionFocus = focusedInteractable;

            promptText.text = focusedInteractable.PromptMessage;
            // Update the facing direction of the prompt.
            promptTransform.forward = promptTransform.position - cameraTransform.position;
            promptTransform.position = focusedInteractable.PromptLocation;
        }
        else
        {
            GameplayHUDSingleton.InteractionFocus = null;
            promptText.text = string.Empty;
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
