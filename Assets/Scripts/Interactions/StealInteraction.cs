using System;
using System.Collections;
using UnityEngine;

// TODO this class is highly derivative
// of hide interaction, maybe use inheritence.

/// <summary>
/// An interaction where the player steals an art piece.
/// After stealing the piece is removed from the scene.
/// </summary>
public sealed class StealInteraction : MonoBehaviour, IInteractable
{
    #region Events
    /// <summary>
    /// This is called once the player is done stealing.
    /// </summary>
    public event Action OnInteractionComplete;
    #endregion
    #region Inspector Fields
    [Range(0, 10)][Tooltip("The number of seconds required to steal this piece.")]
    [SerializeField] private int stealTime = 5;
    #endregion
    #region Fields (Player and Animation State)
    private PlayerController nearbyPlayer;
    private Vector3 animatedPosition;
    private Vector3 animatedDirection;
    #endregion
    #region IInteractable Properties
    public bool PromptVisible { get; private set; }
    public Vector3 PromptLocation { get; private set; }
    public string PromptMessage { get; private set; }
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
    public void OnPromptEnter(PlayerController player)
    {
        PromptMessage = $"Steal ({stealTime}s)";
        PromptVisible = true;
        nearbyPlayer = player;
    }
    public void OnPromptExit(PlayerController player)
    {

    }
    #endregion
    #region Interaction and Animation Definition
    // TODO this is messy; should implement actual
    // animation cycle.
    public void Interact()
    {
        nearbyPlayer.IsMovementLocked = true;
        StartCoroutine(WhileStealing());
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
        OnInteractionComplete?.Invoke();
        Destroy(gameObject);
    }
    #endregion
}
