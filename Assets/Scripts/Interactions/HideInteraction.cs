using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// An interaction where the player hides in a static location.
/// </summary>
public sealed class HideInteraction : MonoBehaviour, IInteractable
{
    #region Events
    /// <summary>
    /// This is called once the player is done hiding.
    /// </summary>
    public event Action OnInteractionComplete;
    #endregion
    #region Inspector Fields
    [Tooltip("The key to press to exit hiding.")]
    [SerializeField] private KeyCode exitHidingKey = KeyCode.E;
    [Range(0.2f, 5f)][Tooltip("Controls the rate at which the enter-exit animations play.")]
    [SerializeField] private float enterExitHideSpeed = 1f;
    [Range(0.5f, 5f)][Tooltip("Controls how far the player is ejected from the hiding spot.")]
    [SerializeField] private float exitDistance = 1f;
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
        PromptLocation = transform.position + Vector3.up;
    }
    #endregion
    #region IInteractable - Handle Nearby Players
    // TODO these functions need to be revised if
    // multiple players are added.
    public void OnPromptEnter(PlayerController player)
    {
        PromptMessage = "Hide";
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
        PromptMessage = string.Empty;
        nearbyPlayer.IsHiding = true;
        StartCoroutine(EnterHiding());
    }
    private IEnumerator EnterHiding()
    {
        animatedPosition = nearbyPlayer.transform.position;
        animatedDirection = (transform.position - nearbyPlayer.transform.position).normalized;
        while (true)
        {
            float travel = Time.deltaTime * enterExitHideSpeed;
            if (travel < Vector3.Distance(animatedPosition, transform.position))
            {
                animatedPosition += travel * animatedDirection;
                nearbyPlayer.transform.position = animatedPosition;
            }
            else
            {
                animatedPosition = transform.position;
                nearbyPlayer.transform.position = animatedPosition;
                break;
            }
            yield return null;
        }
        PromptMessage = "Leave";
        StartCoroutine(WhileHiding());
    }
    private IEnumerator WhileHiding()
    {
        while (true)
        {
            if (Input.GetKeyDown(exitHidingKey))
                break;
            yield return null;
        }
        PromptMessage = string.Empty;
        StartCoroutine(ExitHiding());
    }
    private IEnumerator ExitHiding()
    {
        animatedPosition = transform.position;
        animatedDirection = transform.forward;
        while (true)
        {
            float travel = Time.deltaTime * enterExitHideSpeed;
            if (travel < Vector3.Distance(animatedPosition, transform.position + transform.forward * exitDistance))
            {
                animatedPosition += travel * animatedDirection;
                nearbyPlayer.transform.position = animatedPosition;
            }
            else
            {
                animatedPosition = transform.position + transform.forward * exitDistance;
                nearbyPlayer.transform.position = animatedPosition;
                break;
            }
            yield return null;
        }
        PromptMessage = "Hide";
        nearbyPlayer.IsHiding = false;
        OnInteractionComplete?.Invoke();
    }
    #endregion
}
