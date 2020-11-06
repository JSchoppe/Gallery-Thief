using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Singleton class that manages the player HUD state.
/// </summary>
public sealed class GameplayHUDSingleton : MonoBehaviour
{
    #region Inspector Fields
    [Tooltip("The text linked to the stolen items objective.")]
    [SerializeField] private Text stolenItemsText = null;
    [Tooltip("The icon linked to the crouch affordance.")]
    [SerializeField] private Image crouchIcon = null;
    [Tooltip("The icon linked to the hide affordance.")]
    [SerializeField] private Image hideIcon = null;
    [Tooltip("The icon linked to the steal affordance.")]
    [SerializeField] private Image stealIcon = null;
    [Tooltip("The color applied to the albedo of active icons.")]
    [SerializeField] private Color activeColor = Color.white;
    [Tooltip("The color applied to the albedo of inactive icons.")]
    [SerializeField] private Color inactiveColor = new Color(1f, 1f, 1f, 0.5f);
    #endregion
    // TODO The HUD as a singleton does not support multiplayer.
    private static GameplayHUDSingleton instance;
    #region Singleton State Fields
    private static int stolenItemsNeeded;
    private static int stolenItemsObtained;
    #endregion
    #region Singleton Initialization
    private void Start()
    {
        instance = this;
        UpdateStolenItemsText();
    }
    #endregion
    #region Singleton Accessors
    /// <summary>
    /// Whether the crouch prompt is focused.
    /// </summary>
    public static bool PlayerCanCrouch
    {
        set { instance.SetCrouchFocus(value); }
    }
    /// <summary>
    /// Which interaction the interact key is focused on (can be null).
    /// </summary>
    public static IInteractable InteractionFocus
    {
        set { instance.SetInteractionFocus(value); }
    }
    /// <summary>
    /// How many items are needed for the stolen item objective (only drives UI).
    /// </summary>
    public static int StolenItemsNeeded
    {
        set
        {
            stolenItemsNeeded = value;
            instance?.UpdateStolenItemsText();
        }
    }
    /// <summary>
    /// How many items have been gotten for the stolen item objective (only drives UI).
    /// </summary>
    public static int StolenItemsObtained
    {
        set
        {
            stolenItemsObtained = value;
            instance?.UpdateStolenItemsText();
        }
    }

    //Waypoint
    public static void WaypointHUD()
    {
        //escape through entry hatch
    }
    #endregion
    #region UI Binding
    private void SetCrouchFocus(bool canCrouch)
    {
        if (canCrouch)
            crouchIcon.color = activeColor;
        else
            crouchIcon.color = inactiveColor;
    }
    private void SetInteractionFocus(IInteractable focusedInteraction)
    {
        // TODO this is kind of gross.
        // Use a dictionary or enum or anything else.
        if (focusedInteraction is null)
        {
            hideIcon.color = inactiveColor;
            stealIcon.color = inactiveColor;
        }
        else if (focusedInteraction is HideInteraction)
        {
            hideIcon.color = activeColor;
            stealIcon.color = inactiveColor;
        }
        else if (focusedInteraction is StealInteraction)
        {
            hideIcon.color = inactiveColor;
            stealIcon.color = activeColor;
        }
    }
    public void UpdateStolenItemsText()
    {
        stolenItemsText.text = $"{stolenItemsObtained}/{stolenItemsNeeded}";
    }

    
    #endregion
}
