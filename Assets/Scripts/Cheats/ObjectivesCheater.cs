using UnityEngine;

/// <summary>
/// Cheater class that allows the tester to progress objectives
/// currently active in an objective manager.
/// </summary>
public sealed class ObjectivesCheater : MonoBehaviour
{
    #region Inspector Fields
    [Tooltip("The manager that cheating will be applied to.")]
    [SerializeField] private ObjectiveManager toCheat = null;
    [Tooltip("The key that will increment objective progress.")]
    [SerializeField] private KeyCode incrementProgressKey = KeyCode.Alpha1;
    [Tooltip("The key that will decrement objective progress.")]
    [SerializeField] private KeyCode decrementProgressKey = KeyCode.Alpha2;
    #endregion
    #region Register Input
    private void Update()
    {
        if (Input.GetKeyDown(incrementProgressKey))
            toCheat.IncrementProgress();
        else if (Input.GetKeyDown(decrementProgressKey))
            toCheat.DecrementProgress();
    }
    #endregion
}
