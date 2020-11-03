using UnityEngine;
using TMPro;

/// <summary>
/// Manages interaction with IInteractables.
/// </summary>
public sealed class PlayerInteractor : MonoBehaviour
{
    #region State Enums
    private enum InteractionState : byte
    {
        OutsideRange, InsideRange, Interacting
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
    private IInteractable currentInteractable;
    #endregion
    #region MonoBehaviour Implementation
    private void Start()
    {
        state = InteractionState.OutsideRange;
    }
    private void Update()
    {
        // Check for interaction.
        if (Input.GetKeyDown(interactKey)
            && state == InteractionState.InsideRange)
        {
            // Initiate interaction and listen for
            // completion of interaction.
            currentInteractable.OnInteractionComplete += InteractionCompleteHandler;
            state = InteractionState.Interacting;
            currentInteractable.Interact();
        }
        // TODO this is kind of a hack.
        // Find a better way to do this that doesn't
        // require a check every frame.
        if (state != InteractionState.OutsideRange
            && currentInteractable.PromptVisible)
        {
            promptText.text = currentInteractable.PromptMessage;
            // Update the facing direction of the prompt.
            promptTransform.forward = promptTransform.position - cameraTransform.position;
        }
        else
            promptText.text = string.Empty;
    }
    private void InteractionCompleteHandler()
    {
        currentInteractable.OnInteractionComplete -= InteractionCompleteHandler;
        state = InteractionState.OutsideRange;
    }
    #endregion
    #region Interactables Trigger Processing
    private void OnTriggerEnter(Collider other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null)
        {
            switch (state)
            {
                case InteractionState.OutsideRange:
                    currentInteractable = interactable;
                    interactable.OnPromptEnter(player);
                    state = InteractionState.InsideRange;
                    break;
                case InteractionState.InsideRange:
                    // Prefer pickpocketing since it is a moving trigger.
                    if (interactable is PickpocketInteraction)
                    {
                        currentInteractable.OnPromptExit(player);
                        interactable.OnPromptEnter(player);
                        currentInteractable = interactable;
                    }
                    // Otherwise the existing interaction is not written over.
                    break;
                case InteractionState.Interacting:
                    // If the player is already busy in an interaction,
                    // then don't do anything with this new interactor.
                    break;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null)
        {
            // Remove focus from the interactable if
            // it has focus.
            if (state != InteractionState.Interacting
                && interactable == currentInteractable)
            {
                interactable.OnPromptExit(player);
                state = InteractionState.OutsideRange;
            }
        }
    }
    #endregion
}
