using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// An interaction where a player can steal a key from a guard.
/// </summary>
public sealed class PickpocketInteraction : MonoBehaviour, IInteractable
{
    #region Inspector Fields
    [Range(5f, 90f)][Tooltip("Controls the angle leniency that the player can steal keys.")]
    [SerializeField] private float stealAngle = 30f;
    [Range(1f, 10f)][Tooltip("Controls the distance leniency that the player can steal keys.")]
    [SerializeField] private float stealDistance = 4f;
    #endregion
    #region Private Fields
    private PlayerController nearbyPlayer;
    private Coroutine validate;
    #endregion
    #region Gizmos Implementation
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        // Draw a primitive arc to demonstrate the interaction area.
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
    private void Start()
    {
        PromptVisible = false;
        PromptMessage = "Steal Key";
        PromptLocation = Vector3.zero;
    }
    #endregion
    #region IInteractable Piping
    public bool PromptVisible { get; private set; }
    public Vector3 PromptLocation { get; private set; }
    public string PromptMessage { get; private set; }
    public void OnPromptEnter(PlayerController player)
    {
        // TODO: this is not multiplayer safe.
        nearbyPlayer = player;
        validate = StartCoroutine(ValidateInteraction());
    }
    public void OnPromptExit(PlayerController player)
    {
        StopCoroutine(validate);
        PromptVisible = false;
    }
    /// <summary>
    /// This is called once the pickpocketing has completed
    /// or has been rejected due to position.
    /// </summary>
    public event Action OnInteractionComplete;
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
    #region IInteraction
    // This is called when the player
    // pressed the interact button
    public void Interact()
    {
        // If the player is currently in the prompt range,
        // then start stealing the key.
        if (PromptVisible)
            StartCoroutine(StealKeyAnimation());
        // Otherwise reject this interaction attempt.
        else
            OnInteractionComplete?.Invoke();
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
                nearbyPlayer.GrantKey(key.KeyIdentity);
                Destroy(key);
                break;
            }
        }
        // Release interaction state.
        OnInteractionComplete?.Invoke();
    }
    #endregion
}
