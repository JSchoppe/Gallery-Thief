using UnityEngine;

/// <summary>
/// Contains helper extensions for the Unity Vector3 class.
/// </summary>
public static class Vector3Extensions
{
    /// <summary>
    /// Returns the squared distance between two points.
    /// This is a fast method for distance comparison.
    /// </summary>
    /// <param name="start">The start position.</param>
    /// <param name="end">The end position.</param>
    /// <returns>The distance from point to be point squared.</returns>
    public static float SquaredDistanceTo(this Vector3 start, Vector3 end)
    {
        float deltaX = Mathf.Abs(start.x - end.x);
        float deltaY = Mathf.Abs(start.y - end.y);
        float deltaZ = Mathf.Abs(start.z - end.z);
        return deltaX * deltaX * deltaY * deltaY * deltaZ * deltaZ;
    }
}
