using System;
using UnityEngine;

/// <summary>
/// Implements the interactions for locked doors in the map.
/// </summary>
public sealed class KeyDoor : MonoBehaviour, IInteractable
{
    // This event is required to notify once the
    // player has completed an interaction.
    public event Action OnInteractionComplete;

    #region Inspector Fields
    [Tooltip("The collider that stops players from entering locked doors.")]
    [SerializeField] private Collider playerBlocker = null;
    [Tooltip("The key identity that unlocks this door.")]
    [SerializeField] private KeyID doorIdentity = KeyID.A;
    [Tooltip("Controls the initial state of the door.")]
    [SerializeField] private bool startsLocked = true;
    [Header("Animation Direction")]
    [Tooltip("Place this just outside the front of the door.")]
    [SerializeField] private Transform doorFront = null;
    [Tooltip("Place this just outside the back of the door.")]
    [SerializeField] private Transform doorBack = null;
    #endregion
    #region Scene Gizmos Drawing
    private void OnDrawGizmosSelected()
    {
        // Draw the line segment through the door.
        if (doorFront != null && doorBack != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(doorFront.position, doorBack.position);
            Gizmos.DrawSphere(doorFront.position, 0.25f);
            Gizmos.DrawSphere(doorBack.position, 0.25f);
        }
    }
    #endregion
    #region Private Fields
    private IKeyUser userOpeningDoor;
    private bool doorIsBusy;
    private bool isLocked;
    private Coroutine onPromptStayRoutine;
    #endregion
    #region Properties
    /// <summary>
    /// The identifier for this lock to check against keys.
    /// </summary>
    public KeyID LockID { get { return doorIdentity; } }
    /// <summary>
    /// Whether this door is currently locked or not.
    /// </summary>
    public bool IsLocked { get; private set; }
    // Properties for the interaction:
    public bool PromptVisible { get; private set; }
    public Vector3 PromptLocation { get; private set; }
    public string PromptMessage { get; private set; }
    #endregion

    private void Start()
    {
        isLocked = startsLocked;
        playerBlocker.enabled = isLocked;
    }

    #region Triggers Implementation
    private void OnTriggerEnter(Collider other)
    {
        // Check to see if an actor holding keys has entered.
        IKeyUser user = other.gameObject.GetComponent<IKeyUser>();
        if (user != null)
        {
            // Check if they have the required key.
            if (user.CheckKey(this))
            {
                // Open the door.
                userOpeningDoor = user;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        
    }

    public void OnPromptEnter(PlayerController player)
    {
        userOpeningDoor = player;
        PromptVisible = true;
        if (player.CheckKey(this))
        {
            if (isLocked)
                PromptMessage = $"Unlock door";
            else
                PromptMessage = $"Lock door";
        }
        else
        {
            if (isLocked)
                PromptMessage = $"Requires {doorIdentity} key";
            else
                PromptVisible = false;
        }
    }
    public void OnPromptExit(PlayerController player)
    {
        PromptVisible = false;
    }

    public void Interact()
    {
        if (userOpeningDoor.CheckKey(this))
        {
            isLocked = !isLocked;
            if (isLocked)
            {
                PromptMessage = $"Unlock door";
                playerBlocker.enabled = true;
            }
            else
            {
                PromptMessage = $"Lock door";
                playerBlocker.enabled = false;
            }
        }

        OnInteractionComplete?.Invoke();
    }
    #endregion
}
