using System.Collections.Generic;
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
    /// Finds the shortest path to get from one node to another.
    /// </summary>
    /// <param name="start">The starting node.</param>
    /// <param name="end">The ending node.</param>
    /// <returns>An array of nodes that from start to end, or null if no paths are found.</returns>
    public PathingNode[] FindPath(PathingNode start, PathingNode end)
    {
        // When a path from start to end is found it will
        // be stored here (while the algorithm continues
        // to search for better options).
        PathingNode[] bestPath = null;

        // Define variables to hold the current state of the
        // recursion algorithm's traversal of the network.
        float currentDistance = 0;
        Stack<PathingNode> currentPath = new Stack<PathingNode>();

        // Keep track of the shortest distance taken to get to
        // each node. This ensures that infinite loops will be
        // short-ciruited, and that the fastest path is found.
        Dictionary<PathingNode, float> minDistanceToReach =
            new Dictionary<PathingNode, float>();
        foreach (PathingNode node in Nodes)
            minDistanceToReach.Add(node, float.MaxValue);

        // Start the recursive searching of the network.
        // Search is done from end to start, so that the
        // stack does not need to be reversed in the
        // returned array.
        currentPath.Push(end);
        Traverse(end);
        // Return the results.
        return bestPath;

        // Recursion Implementation:
        void Traverse(PathingNode fromNode)
        {
            // Since we decided to step here, we know that
            // this is the quickest we've ever gotten to
            // this node. Keep track of that.
            minDistanceToReach[fromNode] = currentDistance;

            // If we found a path, save it as the best path.
            // Do not continue searching past the target node.
            if (fromNode == start)
                bestPath = currentPath.ToArray();
            // Otherwise look at the other links in this node.
            else
            {
                foreach (PathingLink link in fromNode.Links)
                {
                    // Is the link open to the next node?
                    bool directionIsForwards = (fromNode == link.EndNode);
                    PathingNode toNode = directionIsForwards ? link.StartNode : link.EndNode;
                    if ((directionIsForwards && link.IsForwardsOpen)
                        || (!directionIsForwards && link.IsBackwardsOpen))
                    {
                        // Calculate the distance to travel to the next node.
                        float addedDistance = Vector3.Distance(
                            link.StartNode.transform.position, link.EndNode.transform.position);
                        // Add that distance.
                        currentDistance += addedDistance;
                        // Will that get us to this node faster than ever before?
                        if (currentDistance < minDistanceToReach[toNode])
                        {
                            // If so travel to that node.
                            currentPath.Push(toNode);
                            // Repeat recursion.
                            Traverse(toNode);
                            // Traverse back after exploring that tree of options.
                            currentPath.Pop();
                        }
                        currentDistance -= addedDistance;
                    }
                }
            }
        }
    }
    #endregion
}
