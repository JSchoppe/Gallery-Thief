using System;
using UnityEngine;

#region Interaction Interfaces
/// <summary>
/// Abstracts interaction between players and interactable items.
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// Called when the player enters this interactable's range.
    /// </summary>
    void OnPromptEnter(PlayerController player);
    /// <summary>
    /// Called when the player leaves this interactable's range.
    /// </summary>
    void OnPromptExit(PlayerController player);
    /// <summary>
    /// Called when the player presses the interact button.
    /// </summary>
    void Interact();
    /// <summary>
    /// Notifies listeners that the interaction is complete.
    /// </summary>
    event Action InteractionComplete;
    /// <summary>
    /// Whether the interactable wants to telegraph a message to the player.
    /// </summary>
    bool PromptVisible { get; }
    /// <summary>
    /// The contextual position in 3D space for the prompt to point at.
    /// </summary>
    Vector3 PromptLocation { get; }
    /// <summary>
    /// The prompt text to print.
    /// </summary>
    string PromptMessage { get; }
    /// <summary>
    /// To interact a linecast must clear between the player and this point.
    /// </summary>
    Vector3 InteractionVisiblePoint { get; }
}
#endregion

/// <summary>
/// Implements core functionality for all interactions.
/// </summary>
public abstract class Interaction : MonoBehaviour, IInteractable
{
    #region Event Piping
    /// <summary>
    /// This event should be raised when control
    /// is released from the interaction.
    /// </summary>
    public abstract event Action InteractionComplete;
    #endregion
    #region Inspector Fields
    [Header("Interaction Parameters")]
    [Tooltip("This local transform should yield a point that is not occluded by interactable mesh components.")]
    [SerializeField] protected Transform interactionVisibleTransform = null;
    #endregion
    #region Prompt Properties
    /// <summary>
    /// Whether the interaction prompt should currently be shown.
    /// </summary>
    public virtual bool PromptVisible { get; protected set; }
    /// <summary>
    /// The current string for the interaction prompt.
    /// </summary>
    public virtual string PromptMessage { get; protected set; }
    /// <summary>
    /// The contextual location of the interaction prompt.
    /// </summary>
    public virtual Vector3 PromptLocation { get; protected set; }
    /// <summary>
    /// Point in 3D space that should be linecast clear for interaction to take place.
    /// </summary>
    public virtual Vector3 InteractionVisiblePoint
    {
        get { return interactionVisibleTransform.position; }
    }
    #endregion
    #region Optional Subclass Methods
    /// <summary>
    /// Reacts to a player entering interaction distance.
    /// </summary>
    /// <param name="player">The player that entered the interaction distance.</param>
    public virtual void OnPromptEnter(PlayerController player) { }
    /// <summary>
    /// Reacts to a player exiting interaction distance.
    /// </summary>
    /// <param name="player">The player that exited the interaction distance.</param>
    public virtual void OnPromptExit(PlayerController player) { }
    #endregion
    #region Required Subclass Methods
    /// <summary>
    /// Initiates interaction, InteractionComplete should be called once
    /// the interaction is completed or otherwise rejected.
    /// </summary>
    public abstract void Interact();
    #endregion
}
