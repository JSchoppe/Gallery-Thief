using UnityEngine;

/// <summary>
/// Represents a node along a path for an AI actor.
/// </summary>
public sealed class PathingNode : MonoBehaviour
{
    #region Inspector Fields
    [SerializeField] private PathingNetwork network = null;
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
