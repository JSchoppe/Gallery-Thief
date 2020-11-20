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
    // TODO this should not be an inspector field!!!
    // Implement new input system.
    [Tooltip("The key that the player can release to stop stealing.")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [Header("Audio References")]
    [Tooltip("The audio source that will play the stealing SFX.")]
    [SerializeField] private AudioSource audioSource;
    [Tooltip("The art stealing SFX.")]
    [SerializeField] private AudioClip artStealing;
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
        audioSource = GetComponent<AudioSource>();
    }
    #endregion
    #region IInteractable - Handle Nearby Players
    // TODO these functions need to be revised if
    // multiple players are added.
    public override void OnPromptEnter(PlayerController player)
    {
        PromptMessage = $"Steal";
        PromptVisible = true;
        nearbyPlayer = player;
    }
    public override void OnPromptExit(PlayerController player)
    {
        audioSource.PlayOneShot(artStealing);
    }
    #endregion
    #region Interaction and Animation
    // TODO this is messy; should implement actual
    // animation cycle.
    public override void Interact()
    {
        nearbyPlayer.IsMovementLocked = true;
        StartCoroutine(WhileStealing());
        //audioSource.PlayOneShot(artStealing);
    }
    private IEnumerator WhileStealing()
    {
        // TODO: this string should be made
        // into an inspector field. Maybe
        // inherit this from Interaction class.
        PromptMessage = "Stealing...";
        float timeRemaining = stealTime;
        while (true)
        {
            // If the player releases the interact key
            // they can abort this stealing.
            if (!Input.GetKey(interactKey))
            {
                PromptProgress = 0f;
                PromptMessage = "Steal";
                break;
            }

            // Check the remaining time.
            timeRemaining -= Time.deltaTime;
            PromptProgress = Mathf.Clamp((stealTime - timeRemaining) / stealTime, 0f, 1f); 
            if(timeRemaining < 0.2)
            {
                if(audioSource.isPlaying == false)
                    audioSource.PlayOneShot(artStealing);
            }
            if (timeRemaining < 0f)
            {
                // TODO this is a hot fix. Player state needs to be
                // better stolen. Perhaps an objective singleton to handle
                // objective event routing.  
                FindObjectOfType<PlayerInteractor>().TriggerArtStolen();
                InteractionComplete?.Invoke(); 
                Destroy(gameObject);
                break;
            }
            yield return null;
        }
        nearbyPlayer.IsMovementLocked = false;
        InteractionComplete?.Invoke();
        
    }
    #endregion
}
