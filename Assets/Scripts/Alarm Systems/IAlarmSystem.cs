/// <summary>
/// Defines a listener for when an alarm is triggered by a player.
/// </summary>
/// <param name="player">The player that triggered the alarm.</param>
public delegate void AlarmTriggeredListener(PlayerController player);

/// <summary>
/// Interface for easy interaction between alarms and alarm responders.
/// </summary>
public interface IAlarmSystem
{
    /// <summary>
    /// Fired when this alarm detects a player.
    /// </summary>
    event AlarmTriggeredListener OnTriggered;
    /// <summary>
    /// Whether the alarm is currently activated.
    /// </summary>
    bool IsEnabled { get; set; }
}
