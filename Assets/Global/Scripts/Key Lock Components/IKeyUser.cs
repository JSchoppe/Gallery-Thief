using UnityEngine;

/// <summary>
/// Interface for all actors in the scene that can use doors.
/// </summary>
public interface IKeyUser
{
    /// <summary>
    /// Provides a method for the key door to check whether a user has the required key.
    /// </summary>
    /// <param name="door">The door sends itself as a parameter.</param>
    /// <returns>True if the key user has the required key.</returns>
    bool CheckKey(KeyDoor door);
    /// <summary>
    /// Exposes the transform of the key user.
    /// </summary>
    Transform transform { get; }
}
