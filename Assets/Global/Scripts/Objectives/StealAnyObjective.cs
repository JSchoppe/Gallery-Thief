using System;
using UnityEngine;

/// <summary>
/// Objective requiring the player to steal a number of art pieces.
/// </summary>
public sealed class StealAnyObjective : Objective, IProgressCheater
{
    #region Event Piping
    /// <summary>
    /// Called when the number of items have been stolen.
    /// </summary>
    public override event Action ObjectiveComplete;
    #endregion
    #region Inspector Fields
    [Header("Objective Parameters")]
    [Tooltip("The number of items that must be stolen.")]
    [SerializeField] private int amountToSteal = 1;
    [Tooltip("The interactor that will report the stealing.")]
    [SerializeField] private PlayerInteractor interactor = null;
    private void OnValidate()
    {
        amountToSteal = Mathf.Clamp(amountToSteal, 1, int.MaxValue);
    }
    #endregion
    #region Fields (Objective State)
    private int totalStolen;
    #endregion
    #region Objective Tracking
    public override void OnObjectiveEnabled()
    {
        totalStolen = 0;
        interactor.ArtPieceStolen += OnArtPieceStolen;
        // TODO use of singleton here restricts multiplayer.
        GameplayHUDSingleton.StolenItemsObtained = totalStolen;
        GameplayHUDSingleton.StolenItemsNeeded = amountToSteal;
    }
    private void OnArtPieceStolen()
    {
        totalStolen++;
        GameplayHUDSingleton.StolenItemsObtained = totalStolen;
        if (totalStolen >= amountToSteal)
            ObjectiveComplete?.Invoke();
    }
    #endregion
    #region Cheating Implementation
    public void IncrementProgress()
    {
        totalStolen++;
        GameplayHUDSingleton.StolenItemsObtained = totalStolen;
        if (totalStolen >= amountToSteal)
            ObjectiveComplete?.Invoke();
    }
    public void DecrementProgress()
    {
        totalStolen--;
        GameplayHUDSingleton.StolenItemsObtained = totalStolen;
    }
    #endregion
}
