﻿using UnityEngine;

/// <summary>
/// Represents an alarm system triggered by an actor walking through a laser grid.
/// </summary>
public sealed class LaserAlarm : MonoBehaviour, IAlarmSystem
{
    #region Inspector Fields
    [Header("Alarm Parameters")]
    [Tooltip("Whether the alarm is enabled initially.")]
    [SerializeField] private bool alarmEnabled = true;
    [Tooltip("Pairs of transforms representing the starts and ends of lasers.")]
    [SerializeField] private Transform[] laserEnds = null;
    #endregion
    #region Private Fields
    private LineRenderer[] renderers;
    private bool isCurrentlyAlarmed;
    #endregion
    #region Properties
    /// <summary>
    /// When the lasers are disabled they will not render or detect players.
    /// </summary>
    public bool IsEnabled
    {
        get { return alarmEnabled; }
        set
        {
            alarmEnabled = value;
            // Flush alarmed state.
            if (!value)
            {
                isCurrentlyAlarmed = false;
                foreach (LineRenderer renderer in renderers)
                    renderer.enabled = false;
            }
            else
                foreach (LineRenderer renderer in renderers)
                    renderer.enabled = true;
        }
    }
    #endregion
    #region Events
    /// <summary>
    /// Fired when this laser detects a player.
    /// </summary>
    public event AlarmTriggeredListener OnTriggered;
    #endregion
    #region MonoBehaviour Implementation
    private void Start()
    {
        // Generate the line renderers for these lasers.
        renderers = new LineRenderer[laserEnds.Length / 2];
        for (int i = 0; i < laserEnds.Length - 1; i += 2)
        {
            // Set the relevant renderer properties.
            LineRenderer renderer = AlarmSingleton.GetNewLaserRenderer();
            renderer.positionCount = 2;
            renderer.SetPosition(0, laserEnds[i].position);
            renderer.SetPosition(1, laserEnds[i + 1].position);
            renderer.material = AlarmSingleton.DefaultLaserMat;
            renderers[i / 2] = renderer;
        }
        IsEnabled = alarmEnabled;
    }
    private void Update()
    {
        if (alarmEnabled)
        {
            // Update the positioning of the renderers.
            for (int i = 0; i < laserEnds.Length - 1; i += 2)
            {
                renderers[i / 2].SetPosition(0, laserEnds[i].position);
                renderers[i / 2].SetPosition(1, laserEnds[i + 1].position);

                // Set the laser material based on the alarm state.
                // TODO should not be set every frame.
                if (isCurrentlyAlarmed)
                    renderers[i / 2].material = AlarmSingleton.AlarmedLaserMat;
                else
                    renderers[i / 2].material = AlarmSingleton.DefaultLaserMat;
            }
        }
    }
    private void FixedUpdate()
    {
        if (alarmEnabled)
        {
            // Scan for players.
            bool suspiciousActorSeen = false;
            foreach (PlayerController actor in AlarmSingleton.SuspiciousActors)
            {
                CapsuleCollider collider = actor.GetComponent<CapsuleCollider>();
                for (int i = 0; i < laserEnds.Length - 1; i += 2)
                {
                    // Get the ray for each laser.
                    Vector3 laserDirection = laserEnds[i + 1].position - laserEnds[i].position;
                    Ray laserRay = new Ray
                    {
                        origin = laserEnds[i].position,
                        direction = laserDirection
                    };
                    // Run a raycast to see if the laser crosses the player.
                    if (collider.Raycast(laserRay, out RaycastHit hit, laserDirection.magnitude))
                    {
                        // Trigger event if this is the first actor seen
                        // and the alarm is not already in a triggered state.
                        suspiciousActorSeen = true;
                        if (!isCurrentlyAlarmed)
                        {
                            isCurrentlyAlarmed = true;
                            OnTriggered?.Invoke(actor);
                        }
                        break;
                    }
                }
            }
            isCurrentlyAlarmed = suspiciousActorSeen;
        }
    }
    #endregion
    #region Gizmos Implementation
    private void OnDrawGizmos()
    {
        // Draw a line to represent each laser.
        Gizmos.color = isCurrentlyAlarmed ? Color.red : Color.green;
        for (int i = 0; i < laserEnds.Length - 1; i += 2)
            if (laserEnds[i] != null && laserEnds[i + 1] != null)
                Gizmos.DrawLine(laserEnds[i].position, laserEnds[i + 1].position);
    }
    #endregion
}
