using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Contains high level state for the current gameplay.
/// </summary>
public sealed class LevelStateSingleton : MonoBehaviour
{
    /// <summary>
    /// This event is called once a player is caught.
    /// </summary>
    public static event Action OnPlayerCaught;

    private void Start()
    {
        // Post important scene instances statically here.
    }

    public static void NotifyPlayerCaught(PlayerController player)
    {
        OnPlayerCaught?.Invoke();
        // TODO: Replace with better caught sequence.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
