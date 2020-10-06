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
    [Tooltip("The players to look for in the lasers.")]
    [SerializeField] private PlayerController[] suspiciousActors = null;
    [Tooltip("The object containing the line renderer that will be instantiated.")]
    [SerializeField] private GameObject laserRendererPrefab = null;
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
            // Create a new game object.
            GameObject gameObject = Instantiate(laserRendererPrefab);
            gameObject.transform.parent = transform;
            // Set the relevant renderer properties.
            LineRenderer renderer = gameObject.GetComponent<LineRenderer>();
            renderer.positionCount = 2;
            renderer.SetPosition(0, laserEnds[i].position);
            renderer.SetPosition(1, laserEnds[i + 1].position);
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
            }
        }
    }
    private void FixedUpdate()
    {
        if (alarmEnabled)
        {
            // Scan for players.
            bool suspiciousActorSeen = false;
            foreach (PlayerController actor in suspiciousActors)
            {
                for (int i = 0; i < laserEnds.Length - 1; i += 2)
                {
                    // TODO: Make a utility method for this somewhere else.
                    Vector3 start = laserEnds[i].position;
                    Vector3 end = laserEnds[i + 1].position;
                    Vector3 player = actor.transform.position;
                    Vector3 closestPoint = Vector3.Project(player - start, end - start) + start;
                    closestPoint.x = Mathf.Clamp(closestPoint.x, Mathf.Min(start.x, end.x), Mathf.Max(start.x, end.x));
                    closestPoint.y = Mathf.Clamp(closestPoint.y, Mathf.Min(start.y, end.y), Mathf.Max(start.y, end.y));
                    closestPoint.z = Mathf.Clamp(closestPoint.z, Mathf.Min(start.z, end.z), Mathf.Max(start.z, end.z));
                    // TODO: PlayerController needs a capsule width property to check against here.
                    if (Vector3.Distance(player, closestPoint) < 0.5f)
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
