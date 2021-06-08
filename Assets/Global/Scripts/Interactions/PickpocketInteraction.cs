using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// An interaction where a player can steal a key from a guard.
/// </summary>
public sealed class PickpocketInteraction : Interaction
{
    #region Events
    /// <summary>
    /// This is called once the pickpocketing has completed
    /// or has been rejected due to position.
    /// </summary>
    public override event Action InteractionComplete;
    #endregion
    #region Inspector Fields
    [Header("Pickpocketing Parameters")]
    [Range(5f, 90f)][Tooltip("Controls the angle leniency that the player can steal keys.")]
    [SerializeField] private float stealAngle = 30f;
    [Range(1f, 10f)][Tooltip("Controls the distance leniency that the player can steal keys.")]
    [SerializeField] private float stealDistance = 4f;
    #endregion
    #region Fields (Animation State)
    private PlayerController nearbyPlayer;
    private Coroutine validate;
    #endregion
    #region Gizmos Implementation
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        // Draw an arc to demonstrate the interaction area.
        Vector3 backCenter = -transform.forward;
        Vector3 backRight = Vector3.RotateTowards(backCenter,
            transform.right, stealAngle * Mathf.Deg2Rad, float.MaxValue);
        Vector3 backLeft = Vector3.RotateTowards(backCenter,
            -transform.right, stealAngle * Mathf.Deg2Rad, float.MaxValue);
        backCenter = backCenter * stealDistance + transform.position;
        backRight = backRight * stealDistance + transform.position;
        backLeft = backLeft * stealDistance + transform.position;
        Gizmos.DrawLine(transform.position, backLeft);
        Gizmos.DrawLine(backLeft, backCenter);
        Gizmos.DrawLine(backCenter, backRight);
        Gizmos.DrawLine(backRight, transform.position);
    }
    #endregion
    #region State Initialization
    public AudioSource audioSource;
    [SerializeField] private AudioClip keySteal = null;

    private void Start()
    {
        PromptVisible = false;
        PromptMessage = "Steal Key";
        PromptLocation = Vector3.zero;
        audioSource = GetComponent<AudioSource>();
    }
    #endregion
    #region IInteractable - Handle Nearby Players
    public override void OnPromptEnter(PlayerController player)
    {
        // TODO: this is not multiplayer safe.
        nearbyPlayer = player;
        validate = StartCoroutine(ValidateInteraction());
    }
    public override void OnPromptExit(PlayerController player)
    {
        StopCoroutine(validate);
        PromptVisible = false;
    }
    #endregion
    #region Proximity Checking
    private IEnumerator ValidateInteraction()
    {
        // On every frame:
        while (true)
        {
            // Is the player within the steal arc?
            Vector3 playerPosition = nearbyPlayer.transform.position;
            if (Vector3.Distance(playerPosition, transform.position) < stealDistance
                && Vector3.Angle(-transform.forward, playerPosition - transform.position) < stealAngle)
            {
                // TODO this feels very inneficient!!!
                bool stealableKeyExists = false;
                foreach (DoorKey key in gameObject.GetComponents<DoorKey>())
                {
                    if (key.IsStealable)
                    {
                        stealableKeyExists = true;
                        break;
                    }
                }
                if (stealableKeyExists)
                {
                    PromptVisible = true;
                    PromptLocation = transform.position;
                }
                else
                    PromptVisible = false;
            }
            else
                PromptVisible = false;
            // Wait for next update loop.
            yield return null;
        }
    }
    #endregion
    #region Interaction and Animation
    // This is called when the player
    // pressed the interact button
    public override void Interact()
    {
        // If the player is currently in the prompt range,
        // then start stealing the key.
        if (PromptVisible)
            StartCoroutine(StealKeyAnimation());
        // Otherwise reject this interaction attempt.
        else
            InteractionComplete?.Invoke();
    }
    private IEnumerator StealKeyAnimation()
    {
        // Do animation procedure towards guard to steal key.
        while(true)
        {
            // TODO replace this dummy animation.
            yield return new WaitForSeconds(1.0f);
            break;
        }
        // TODO; like above, seems inefficient.
        // Steal the first available key.
        foreach (DoorKey key in gameObject.GetComponents<DoorKey>())
        {
            if (key.IsStealable)
            {
                // Grant the key to the player.
                audioSource.PlayOneShot(keySteal);
                nearbyPlayer.GrantKey(key.KeyIdentity);
                Destroy(key);
                break;
            }
        }
        // Release interaction state.
        InteractionComplete?.Invoke();
    }
    #endregion
}
