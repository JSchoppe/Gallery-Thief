using UnityEngine;

/// <summary>
/// Represents a node along a path for an AI actor.
/// </summary>
public sealed class PathingNode : MonoBehaviour
{
    #region Inspector Fields
    [SerializeField] private PathingNetwork network = null;
    #endregion
    #region Pathing Properties
    /// <summary>
    /// Contains the pathing links that join to this node.
    /// </summary>
    public PathingLink[] Links { get; private set; }
    #endregion
    #region Pathing Methods
    /// <summary>
    /// Adds a link to this node.
    /// </summary>
    /// <param name="link"></param>
    public void AddLink(PathingLink link)
    {
        // Create or resize the existing links array.
        if (Links == null)
            Links = new PathingLink[] { link };
        else
        {
            PathingLink[] newArray = new PathingLink[Links.Length + 1];
            for (int i = 0; i < Links.Length; i++)
                newArray[i] = Links[i];
            newArray[Links.Length] = link;
            Links = newArray;
        }
    }
    #endregion
    #region Monobehaviour Initialization
    private void Start()
    {
        network.AddNode(this);
    }
    #endregion
    #region Gizmo Drawing Implementation
    private void OnDrawGizmos()
    {
        if (network.ShowGizmos)
        {
            Gizmos.color = network.NodesColor;
            Gizmos.DrawSphere(transform.position, network.NodesRadius);
        }
    }
    #endregion
}
