using UnityEngine;

/// <summary>
/// Represents a network of nodes that AI actors can traverse.
/// </summary>
public sealed class PathingNetwork : MonoBehaviour
{
    #region Inspector Fields
    [Header("Debug Settings")]
    [Tooltip("Enables the gizmos in the scene.")]
    [SerializeField] private bool showGizmos = true;
    [Range(0.1f, 2f)][Tooltip("The size of the gizmo nodes.")]
    [SerializeField] private float nodesRadius = 0.2f;
    [Tooltip("The color of active gizmo paths.")]
    [SerializeField] private Color activePathsColor = Color.green;
    [Tooltip("The color of inactive gizmo paths.")]
    [SerializeField] private Color disabledPathsColor = Color.red;
    [Tooltip("The color of gizmo nodes.")]
    [SerializeField] private Color nodesColor = Color.white;
    #endregion
    #region Gizmos Settings Accessors
    public bool ShowGizmos { get { return showGizmos; } }
    public float NodesRadius { get { return nodesRadius; } }
    public Color ActivePathsColor { get { return activePathsColor; } }
    public Color DisabledPathsColor { get { return disabledPathsColor; } }
    public Color NodesColor { get { return nodesColor; } }
    #endregion

    // TODO Implement path finding functionality.
}
