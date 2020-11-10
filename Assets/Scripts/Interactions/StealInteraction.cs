using System;
using System.Collections;
using UnityEngine;

// TODO this class is highly derivative
// of hide interaction, maybe use inheritence.

/// <summary>
/// An interaction where the player steals an art piece.
/// After stealing the piece is removed from the scene.
/// </summary>
public sealed class StealInteraction : Interaction
{
    #region Events
    /// <summary>
    /// This is called once the player is done stealing.
    /// </summary>
    public override event Action InteractionComplete;
    #endregion
    #region Inspector Fields
    [Header("Stealing Parameters")]
    [Range(0, 10)][Tooltip("The number of seconds required to steal this piece.")]
    [SerializeField] private int stealTime = 5;
    [Header("Audio References")]
    [Tooltip("The audio source that will play the stealing SFX.")]
    [SerializeField] private AudioSource audioSource = null;
    [Tooltip("The art stealing SFX.")]
    [SerializeField] private AudioClip artStealing = null;
    #endregion
    #region Interaction Property Overrides
    public override Vector3 PromptLocation
    {
        get { return interactionVisibleTransform.position; }
    }
    #endregion
    #region Fields (Player and Animation State)
    private PlayerController nearbyPlayer;
    #endregion
    #region Initialization
    private void Start()
    {
        // TODO actually use this when rendering the prompt.
        PromptLocation = transform.forward + Vector3.up;
    }
    #endregion
    #region IInteractable - Handle Nearby Players
    // TODO these functions need to be revised if
    // multiple players are added.
    public override void OnPromptEnter(PlayerController player)
    {
        PromptMessage = $"Steal ({stealTime}s)";
        PromptVisible = true;
        nearbyPlayer = player;
    }
    public override void OnPromptExit(PlayerController player)
    {

    }
    #endregion
    #region Interaction and Animation
    // TODO this is messy; should implement actual
    // animation cycle.
    public override void Interact()
    {
        nearbyPlayer.IsMovementLocked = true;
        StartCoroutine(WhileStealing());
        audioSource.PlayOneShot(artStealing);
    }
    private IEnumerator WhileStealing()
    {
        float timeRemaining = stealTime;
        while (true)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining < 0f)
                break;
            PromptMessage = $"Stealing... ({Mathf.CeilToInt(timeRemaining)}s)";
            yield return null;
        }
        PromptMessage = string.Empty;
        nearbyPlayer.IsMovementLocked = false;
        nearbyPlayer.PaintingsStolen++;
        InteractionComplete?.Invoke();
        Destroy(gameObject);
    }
    #endregion
}
