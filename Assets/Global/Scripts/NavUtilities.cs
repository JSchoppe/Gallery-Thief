using System;
using System.Collections.Generic;
using UnityEngine.AI;

/// <summary>
/// Contains helper functions for navigation.
/// </summary>
public static class NavUtilities
{
    #region Initialization
    private static readonly Dictionary<KeyID, int> keyMasks =
        new Dictionary<KeyID, int>()
    {
        { KeyID.A, NavMesh.GetAreaFromName("KeyA") },
        { KeyID.B, NavMesh.GetAreaFromName("KeyB") },
        { KeyID.C, NavMesh.GetAreaFromName("KeyC") },
        { KeyID.D, NavMesh.GetAreaFromName("KeyD") },
        { KeyID.E, NavMesh.GetAreaFromName("KeyE") },
        { KeyID.F, NavMesh.GetAreaFromName("KeyF") },
        { KeyID.G, NavMesh.GetAreaFromName("KeyG") },
        { KeyID.H, NavMesh.GetAreaFromName("KeyH") },
        { KeyID.I, NavMesh.GetAreaFromName("KeyI") },
        { KeyID.J, NavMesh.GetAreaFromName("KeyJ") }
    };
    #endregion
    #region Functions
    /// <summary>
    /// Converts the given key ID to it's corresponding nav mesh area.
    /// </summary>
    /// <param name="key">The key identity.</param>
    /// <returns>The value of the nav mesh area mask.</returns>
    public static int NavAreaFromKeyID(KeyID key)
    {
        if (keyMasks.ContainsKey(key))
        {
            int mask = 1;
            for (int i = 0; i < keyMasks[key]; i++)
            {
                mask *= 2;
            }
            return mask;
        }
        else
            throw new NotImplementedException();
    }
    #endregion
}
