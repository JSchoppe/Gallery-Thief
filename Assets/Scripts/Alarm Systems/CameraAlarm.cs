using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents an alarm system triggered by an actor walking through a camera frustum.
/// </summary>
public sealed class CameraAlarm : MonoBehaviour, IAlarmSystem
{
    #region Inspector Fields
    [Header("Alarm Parameters")]
    [Tooltip("Whether the alarm is enabled initially.")]
    [SerializeField] private bool alarmEnabled = true;
    [Header("Camera Parameters")]
    [Range(float.Epsilon, 80f)][Tooltip("The FOV of the security camera.")]
    [SerializeField] private float fieldOfView = 40f;
    [Range(1f, 100f)][Tooltip("The max distance the camera can see.")]
    [SerializeField] private float maxViewDistance = 5f;
    [Header("Optional: Camera Animation")]
    [Range(0f, 360f)][Tooltip("Degrees per second rotation of the camera.")]
    [SerializeField] private float panSpeed = 30f;
    [Tooltip("Defines the target focal points to rotate between.")]
    [SerializeField] private Transform[] keyFocalPoints = null;
    [Tooltip("Defines the wait time at each focal point.")]
    [SerializeField] private float[] keyPointsHoldTime = null;
    private void OnValidate()
    {
        for (int i = 0; i < keyPointsHoldTime.Length; i++)
            if (keyPointsHoldTime[i] < 0)
                keyPointsHoldTime[i] = 0;
    }
    #endregion
    #region Private Fields
    // Event state.
    private bool isCurrentlyAlarmed;
    // Animation state.
    private bool doesAnimate;
    private float cycleTimeElapsed;
    // Precalculated animation data.
    private Vector3[] keyRotations;
    private float[] keyTimes;
    private float totalTime;
    #endregion
    #region Properties
    /// <summary>
    /// When the camera is disabled it will neither animate nor detect players.
    /// </summary>
    public bool IsEnabled
    {
        get { return alarmEnabled; }
        set
        {
            alarmEnabled = value;
            // Flush alarmed state.
            if (!value)
                isCurrentlyAlarmed = false;
        }
    }
    #endregion
    #region Events
    /// <summary>
    /// Fired when this camera detects a player.
    /// </summary>
    public event AlarmTriggeredListener OnTriggered;
    #endregion
    #region MonoBehaviour Implementation
    private void Start()
    {
        // Initialize animation state if necessary.
        doesAnimate = (keyFocalPoints.Length > 0);
        if (doesAnimate)
        {
            // Parse angle keyframes for the entire cycle.
            List<Vector3> rotations = new List<Vector3>();
            for (int i = 0; i < keyFocalPoints.Length; i++)
            {
                Vector3 direction = (keyFocalPoints[i].position - transform.position).normalized;
                rotations.Add(direction);
                // If there is a wait time duplicate the keyframe.
                if (keyPointsHoldTime.Length > i && keyPointsHoldTime[i] > 0)
                    rotations.Add(direction);
            }
            // Link the end back to the start.
            rotations.Add((keyFocalPoints[0].position - transform.position).normalized);

            // Calculate the key times for every angle.
            List<float> times = new List<float>();
            float timeAccumulator = 0;
            int j = 0;
            for (int i = 0; i < rotations.Count; i++)
            {
                times.Add(timeAccumulator);
                if (i < rotations.Count - 1)
                {
                    if (rotations[i] == rotations[i + 1])
                    {
                        timeAccumulator += keyPointsHoldTime[j];
                        j++;
                    }
                    else
                        timeAccumulator += Vector3.Angle(rotations[i], rotations[i + 1]) / panSpeed;
                }
                else
                    timeAccumulator += Vector3.Angle(rotations[i], rotations[0]) / panSpeed;
            }
            times.Add(timeAccumulator);
            
            // Save precalculated data.
            totalTime = timeAccumulator;
            keyRotations = rotations.ToArray();
            keyTimes = times.ToArray();

            // Clean up scene components for designers.
            foreach (Transform transform in keyFocalPoints)
                Destroy(transform.gameObject);
            // Start at the first keyframe.
            cycleTimeElapsed = 0;
        }
    }
    private void FixedUpdate()
    {
        // If the alarm is enabled:
        if (IsEnabled)
        {
            // Animate if animation was specified.
            if (doesAnimate)
                Animate();
            // Scan the are for players.
            Scan();
        }
    }
    #endregion
    #region Camera Animation Function
    private void Animate()
    {
        // Update cycle time.
        cycleTimeElapsed += Time.fixedDeltaTime;
        cycleTimeElapsed %= totalTime;
        // Find the current location in the keyframes.
        for (int i = 0; i < keyTimes.Length; i++)
        {
            if (cycleTimeElapsed < keyTimes[i])
            {
                float subInterpolant = Mathf.InverseLerp(keyTimes[i - 1], keyTimes[i], cycleTimeElapsed);
                // Apply camera rotation.
                transform.LookAt(transform.position + Vector3.Slerp(keyRotations[i - 1], keyRotations[i], subInterpolant));
                break;
            }
        }
    }
    #endregion
    #region Scan Detection Function
    private void Scan()
    {
        bool suspiciousActorSeen = false;
        // Check each player:
        foreach (PlayerController actor in AlarmSingleton.SuspiciousActors)
        {
            Vector3 actorDirection = AlarmSingleton.GetActorTorso(actor) - transform.position;
            // Check to see if the actor is in the field of view and range of the camera.
            if (Vector3.Angle(actorDirection, transform.forward) < fieldOfView / 2f
                && Vector3.Project(actorDirection, transform.forward).magnitude < maxViewDistance)
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
        isCurrentlyAlarmed = suspiciousActorSeen;
    }
    #endregion
    #region Gizmos Implementation
    private void OnDrawGizmos()
    {
        Gizmos.color = isCurrentlyAlarmed ? Color.red : Color.green;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawFrustum(Vector3.zero, fieldOfView, maxViewDistance, 1, 1);
    }
    #endregion
}
