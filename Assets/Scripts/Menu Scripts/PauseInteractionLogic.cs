using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Implements logic for the pause UI and exposes events for pause state.
/// </summary>
public sealed class PauseInteractionLogic : MonoBehaviour
{
    #region Events
    /// <summary>
    /// This is called every time the game is paused.
    /// </summary>
    public static event Action GamePaused;
    /// <summary>
    /// This is called every time the game is resumed.
    /// </summary>
    public static event Action GameResumed;
    #endregion
    #region Inspector Fields
    [Tooltip("The top level panel for the pause screen.")]
    [SerializeField] private GameObject gameMenuPanel = null;
    [Tooltip("The panel for the settings screen.")]
    [SerializeField] private GameObject settingsMenuPanel = null;
    #endregion
    #region Private Fields
    private bool isPaused;
    #endregion
    #region MonoBehavior Implementation
    private void Start()
    {
        isPaused = false;
        gameMenuPanel.SetActive(false);
    }
    private void Update()
    {
        // TODO: potentially replace with better input.
        if (Input.GetKeyDown(KeyCode.P))
        {
            isPaused = !isPaused;
            if (isPaused)
                Pause();
            else
                Resume();
        }
    }
    private void OnDestroy()
    {
        // Clear out listeners so they can be
        // properly garbage collected.
        GamePaused = null;
        GameResumed = null;
    }
    #endregion
    #region UI Binding
    public void OnResumePressed() { Resume(); }

    public void OnSettingsPressed()
    {
        settingsMenuPanel.SetActive(true);
        gameMenuPanel.SetActive(false);
    }

    public void OnSettingsBackPressed()
    {
        settingsMenuPanel.SetActive(false);
        gameMenuPanel.SetActive(true);
    }

    public void OnQuitPressed()
    {
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene(0);
    }
    #endregion
    #region Resume/Pause Methods
    private void Resume()
    {
        gameMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        GameResumed?.Invoke();
    }
    private void Pause()
    {
        gameMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        GamePaused?.Invoke();
    }
    #endregion
}
