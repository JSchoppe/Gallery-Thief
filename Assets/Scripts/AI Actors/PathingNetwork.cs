using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
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
    #region Exposed Properties
    /// <summary>
    /// The nodes that make up this pathing network.
    /// </summary>
    public PathingNode[] Nodes { get; private set; }
    #endregion
    #region Exposed Methods
    /// <summary>
    /// Registers a new node to this network.
    /// </summary>
    /// <param name="node">The pathing node to be added.</param>
    public void AddNode(PathingNode node)
    {
        // Create or resize the existing links array.
        if (Nodes == null)
            Nodes = new PathingNode[] { node };
        else
        {
            PathingNode[] newArray = new PathingNode[Nodes.Length + 1];
            for (int i = 0; i < Nodes.Length; i++)
                newArray[i] = Nodes[i];
            newArray[Nodes.Length] = node;
            Nodes = newArray;
        }
    }
    /// <summary>
    /// Finds the pathing node that is closest to a given set off coordinates.
    /// </summary>
    /// <param name="position">The coordinates to look around.</param>
    /// <returns>The nearest node in the network to the position. Returns null if there are no nodes.</returns>
    public PathingNode FindNodeNear(Vector3 position)
    {
        // Compare distances to find the closest node.
        PathingNode nearestNode = null;
        float nearestDistance = float.MaxValue;
        foreach (PathingNode node in Nodes)
        {
            float distance = Vector3.Distance(node.transform.position, position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestNode = node;
            }
        }
        return nearestNode;
    }
    /// <summary>
    /// Attempts to find the shortest path to get from one node to another.
    /// </summary>
    /// <param name="start">The starting node.</param>
    /// <param name="end">The ending node.</param>
    /// <param name="path">Where to return the path to if found.</param>
    /// <returns>True if a path was found.</returns>
    public bool TryFindPath(PathingNode start, PathingNode end, out PathingNode[] path)
    {
        // If the AI is already at the nearest node,
        // go directly to it.
        if (start == end)
        {
            path = new PathingNode[] { start };
            return true;
        }

        // Clear previous pathfinding state.
        foreach (PathingNode node in Nodes)
            node.ResetPathfindingProps();
        // Initialize the collection for A*.
        List<PathingNode> openNodes = new List<PathingNode>();
        // Initialize the starting node.
        openNodes.Add(start);
        start.PathParent = null;
        start.TravelG = 0f;
        start.HeuristicH = CalculateHeuristic(start, end);

        // Start the A* algorithm.
        while (openNodes.Count > 0)
        {
            // Find the best f score in the open nodes.
            PathingNode current = openNodes[0];
            for (int i = 1; i < openNodes.Count; i++)
                if (openNodes[i].EstimateF < current.EstimateF)
                    current = openNodes[i];

            // Return the path if the end has been found.
            if (current == end)
            {
                path = UnwindPath(current);
                return true;
            }

            openNodes.Remove(current);
            // For each candidate successor path:
            foreach (PathingLink link in current.Links)
            {
                // Is this pathway currently open?
                if ((current == link.StartNode && link.IsForwardsOpen) 
                    || (current == link.EndNode && link.IsBackwardsOpen))
                {
                    // Get the opposing node.
                    PathingNode other = (current == link.StartNode) ? link.EndNode : link.StartNode;
                    // Calculate the new travel distance to this node.
                    float newTravelG = current.TravelG + link.Length;
                    if (newTravelG < other.TravelG)
                    {
                        // If this is a new best path to this node,
                        // add it to the open nodes and calculate the heuristic.
                        other.TravelG = newTravelG;
                        other.PathParent = current;
                        if (!openNodes.Contains(other))
                        {
                            other.HeuristicH = CalculateHeuristic(other, end);
                            openNodes.Add(other);
                        }
                    }
                }
            }
        }
        // A* pathfinding failed to find a path.
        path = new PathingNode[0];
        return false;
    }
    #endregion
    #region A* Functions
    private float CalculateHeuristic(PathingNode node, PathingNode end)
    {
        // The heuristic is the distance in 3D space.
        return Vector3.Distance(node.transform.position, end.transform.position);
    }
    private PathingNode[] UnwindPath(PathingNode node)
    {
        // Unwind the path using the PathParent property.
        Stack<PathingNode> path = new Stack<PathingNode>();
        path.Push(node);
        while (node.PathParent != null)
        {
            node = node.PathParent;
            path.Push(node);
        }
        // Return the path found.
        return path.ToArray();
    }
    #endregion
}
