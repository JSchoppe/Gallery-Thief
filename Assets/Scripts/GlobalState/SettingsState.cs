using System;

public static class SettingsState
{
    #region Fields
    public static bool hasInitialized = false;
    #endregion
    #region Events
    public static event Action<float> VolumeChanged;
    public static event Action<float> SensitivityRevolveChanged;
    public static event Action<float> SensitivityPitchChanged;
    public static event Action<float> SensitivityZoomChanged;
    public static event Action<bool> InvertRevolveChanged;
    public static event Action<bool> InvertPitchChanged;
    public static event Action<bool> InvertZoomChanged;
    #endregion
    #region Accessors
    private static float volume;
    public static float Volume
    {
        get { return volume; }
        set
        {
            volume = value;
            VolumeChanged?.Invoke(volume);
        }
    }
    private static float sensitivityRevolve;
    public static float SensitivityRevolve
    {
        get { return sensitivityRevolve; }
        set
        {
            sensitivityRevolve = value;
            SensitivityRevolveChanged?.Invoke(sensitivityRevolve);
        }
    }
    private static float sensitivityPitch;
    public static float SensitivityPitch
    {
        get { return sensitivityPitch; }
        set
        {
            sensitivityPitch = value;
            SensitivityPitchChanged?.Invoke(sensitivityPitch);
        }
    }
    private static float sensitivityZoom;
    public static float SensitivityZoom
    {
        get { return sensitivityZoom; }
        set
        {
            sensitivityZoom = value;
            SensitivityZoomChanged?.Invoke(sensitivityZoom);
        }
    }
    private static bool invertRevolve;
    public static bool InvertRevolve
    {
        get { return invertRevolve; }
        set
        {
            invertRevolve = value;
            InvertRevolveChanged?.Invoke(invertRevolve);
        }
    }
    private static bool invertPitch;
    public static bool InvertPitch
    {
        get { return invertPitch; }
        set
        {
            invertPitch = value;
            InvertPitchChanged?.Invoke(invertPitch);
        }
    }
    private static bool invertZoom;
    public static bool InvertZoom
    {
        get { return invertZoom; }
        set
        {
            invertZoom = value;
            InvertZoomChanged?.Invoke(invertZoom);
        }
    }
    #endregion
}
