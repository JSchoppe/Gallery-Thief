using UnityEngine;

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
    [Tooltip("Interpolant speed that this laser re-extends at.")]
    [SerializeField] private float restoreSpeed = 5f;
    #endregion
    #region Private Fields
    private LineRenderer[] renderers;
    private bool isCurrentlyAlarmed;
    private float[] laserInterpolants;
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
        laserInterpolants = new float[renderers.Length];
        for (int i = 0; i < laserEnds.Length - 1; i += 2)
        {
            // Set the relevant renderer properties.
            LineRenderer renderer = AlarmSingleton.GetNewLaserRenderer();
            renderer.positionCount = 2;
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
                renderers[i / 2].SetPosition(1, Vector3.Lerp(laserEnds[i].position, laserEnds[i + 1].position, laserInterpolants[i / 2]));
            }
        }
    }
    private void FixedUpdate()
    {
        if (alarmEnabled)
        {
            // Extends lasers back to full length.
            for (int i = 0; i < laserInterpolants.Length; i++)
                if (laserInterpolants[i] < 1f)
                    laserInterpolants[i] = Mathf.Min(1f,
                        laserInterpolants[i] + restoreSpeed * Time.fixedDeltaTime);
            // Scan for players.
            bool suspiciousActorSeen = false;
            for (int i = 0; i < laserEnds.Length - 1; i += 2)
            {
                // Get the ray for each laser.
                Vector3 laserDirection = Vector3.Lerp(laserEnds[i].position, laserEnds[i + 1].position,
                    laserInterpolants[i / 2]);
                Ray laserRay = new Ray
                {
                    origin = laserEnds[i].position,
                    direction = laserDirection - laserEnds[i].position
                };
                // Check the ray against each player.
                foreach (PlayerController actor in AlarmSingleton.SuspiciousActors)
                {
                    CapsuleCollider collider = actor.GetComponent<CapsuleCollider>();
                    // Run a raycast to see if the laser crosses the player.
                    if (collider.Raycast(laserRay, out RaycastHit hit, laserDirection.magnitude))
                    {
                        // Update the laser interpolant to reflect being intersected by the player.
                        laserInterpolants[i / 2] = Vector3.Distance(hit.point, laserEnds[i].position)
                            / Vector3.Distance(laserEnds[i].position, laserEnds[i + 1].position);

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
        if (alarmEnabled)
        {
            Gizmos.color = isCurrentlyAlarmed ? Color.red : Color.green;
            for (int i = 0; i < laserEnds.Length - 1; i += 2)
                if (laserEnds[i] != null && laserEnds[i + 1] != null)
                    Gizmos.DrawLine(laserEnds[i].position, laserEnds[i + 1].position);
        }
    }
    #endregion
}
