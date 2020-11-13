using UnityEngine;

/// <summary>
/// Contains helper extensions for the Unity Component class.
/// </summary>
public static class ComponentExtensions
{
    /// <summary>
    /// Checks for a component, and outputs the component if found.
    /// </summary>
    /// <typeparam name="T">The component to search for.</typeparam>
    /// <param name="baseComponent">The component to search in.</param>
    /// <param name="foundComponent">The located component if found.</param>
    /// <returns>True if the requested component is found.</returns>
    public static bool HasComponent<T>(this Component baseComponent, out T foundComponent)
    {
        // This method is used to wrap null checks elsewhere.
        foundComponent = baseComponent.GetComponent<T>();
        return foundComponent != null;
    }
}
