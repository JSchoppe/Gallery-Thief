using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    event Action OnInteractionComplete;
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
}
