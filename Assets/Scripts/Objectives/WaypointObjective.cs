using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Simple waypoint objective that can be triggered by any player.
/// </summary>
public sealed class WaypointObjective : Objective
{
    #region Events
    /// <summary>
    /// Called once the player gets close to the waypoint.
    /// </summary>
    public override event Action ObjectiveComplete;
    #endregion
    // TODO this breaks if there are multiple waypoints.
    // Make this an instantiation of a prefab.
    #region Inspector Fields
    [Tooltip("The arrow that will point the player towards the waypoint.")]
    [SerializeField] private GameObject waypointArrow = null;
    [Tooltip("The players to check for distance to the waypoint.")]
    [SerializeField] private Transform[] playersToTriggerWaypoint = null;
    [Header("Waypoint Parameters")]
    [Tooltip("How high the arrow appears above the player's head.")]
    [SerializeField] private float arrowHeight = 3f;
    [Tooltip("Holds the location of the waypoint itself.")]
    [SerializeField] private Transform waypoint = null;
    [Tooltip("How close the player must get to trigger this waypoint.")]
    [SerializeField] private float radius = 2f;
    #endregion
    #region Inspector Functions
    private void OnValidate()
    {
        // Clamp inspector fields to avoid bad logic.
        arrowHeight = Mathf.Clamp(arrowHeight, 0f, float.MaxValue);
        radius = Mathf.Clamp(radius, 0.1f, float.MaxValue);
    }
    private void OnDrawGizmosSelected()
    {
        // Draw an preview at the waypoint position.
        Gizmos.color = Color.yellow;
        if (waypoint != null)
            Gizmos.DrawWireSphere(waypoint.position, radius);
    }
    #endregion
    #region Objective Start
    public override void OnObjectiveEnabled()
    {
        // Start checking for player proximity
        // and drawing the guiding arrow.
        StartCoroutine(CheckForPlayerInWaypoint());
    }
    #endregion
    #region Objective Update Loop
    private IEnumerator CheckForPlayerInWaypoint()
    {
        while (true)
        {
            UpdateArrow();
            // Check for any player in waypoint proximity.
            bool playerIsAtWaypoint = false;
            foreach (Transform player in playersToTriggerWaypoint)
            {
                if (Vector3.Distance(player.position, waypoint.position) < radius)
                {
                    playerIsAtWaypoint = true;
                    break;
                }
            }
            if (playerIsAtWaypoint)
                break;
            else
                yield return null;
        }
        ObjectiveComplete?.Invoke();
    }
    private void UpdateArrow()
    {
        // TODO this is not player specific; just grabs the
        // first player.
        waypointArrow.transform.position =
            playersToTriggerWaypoint[0].position + Vector3.up * arrowHeight;
        // Face the arrow towards the waypoint location.
        waypointArrow.transform.LookAt(waypoint);
    }
    #endregion
}
