using UnityEngine;

/// <summary>
/// Represents a link between two nodes that AI can travel between.
/// </summary>
public sealed class PathingLink : MonoBehaviour
{
    #region Inspector Fields
    [SerializeField] private PathingNetwork network = null;
    [SerializeField] private PathingNode start = null;
    [SerializeField] private PathingNode end = null;
    [SerializeField] private bool forwardsOpen = true;
    [SerializeField] private bool backwardsOpen = true;
    #endregion
    #region Accessors and Setters
    /// <summary>
    /// When true AI actors can travel from the start node to the end node.
    /// </summary>
    public bool IsForwardsOpen
    {
        get { return forwardsOpen; }
        set { forwardsOpen = value; }
    }
    /// <summary>
    /// When true AI actors can travel from the end node to the start node.
    /// </summary>
    public bool IsBackwardsOpen
    {
        get { return backwardsOpen; }
        set { backwardsOpen = value; }
    }
    /// <summary>
    /// The start node of this link.
    /// </summary>
    public PathingNode StartNode { get { return start; } }
    /// <summary>
    /// The end node of this link.
    /// </summary>
    public PathingNode EndNode { get { return end; } }
    /// <summary>
    /// The distance between the start and end nodes of this link.
    /// </summary>
    public float Length
    {
        get
        {
            return Vector3.Distance(StartNode.transform.position, EndNode.transform.position);
        }
    }
    #endregion
    #region Monobehaviour Initialization
    private void Start()
    {
        start.AddLink(this);
        end.AddLink(this);
    }
    #endregion
    #region Gizmo Drawing Implementation
    private void OnDrawGizmos()
    {
        if (network.ShowGizmos)
        {
            Vector3 direction = end.transform.position - start.transform.position;

            // Get the offset to reach the vector traveling along the tangents of the spheres.
            Vector3 localRight = Vector3.Cross(direction, Vector3.up).normalized * network.NodesRadius;

            // Draw the forwards gizmos.
            Gizmos.color = forwardsOpen ? network.ActivePathsColor : network.DisabledPathsColor;
            Gizmos.DrawLine(start.transform.position + localRight, end.transform.position + localRight);
            for (int i = 1; i < direction.magnitude; i++)
            {
                Vector3 locationAlong = start.transform.position + direction.normalized * i + localRight;
                Vector3 arrowBaseAlong = locationAlong - direction.normalized * network.NodesRadius;
                Gizmos.DrawLine(locationAlong, arrowBaseAlong + 0.5f * localRight);
                Gizmos.DrawLine(locationAlong, arrowBaseAlong - 0.5f * localRight);
            }

            // Draw the backwards gizmos.
            Gizmos.color = backwardsOpen ? network.ActivePathsColor : network.DisabledPathsColor;
            Gizmos.DrawLine(start.transform.position - localRight, end.transform.position - localRight);
            for (int i = 1; i < direction.magnitude; i++)
            {
                Vector3 locationAlong = end.transform.position - direction.normalized * i - localRight;
                Vector3 arrowBaseAlong = locationAlong + direction.normalized * network.NodesRadius;
                Gizmos.DrawLine(locationAlong, arrowBaseAlong + 0.5f * localRight);
                Gizmos.DrawLine(locationAlong, arrowBaseAlong - 0.5f * localRight);
            }
        }
    }
    #endregion
}
