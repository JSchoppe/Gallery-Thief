using System;
using UnityEngine;

/// <summary>
/// Supplies access to locations where the guards can obtain new keys.
/// </summary>
public sealed class KeyRestock : MonoBehaviour
{
    #region Singleton Fields
    private static Transform[] restockLocations;
    #endregion
    #region Singleton Properties
    /// <summary>
    /// The locations on the map where AI guards can acquire new keys.
    /// </summary>
    public static Transform[] RestockLocations
    {
        get
        {
            if (restockLocations == null)
                throw new Exception("There needs to be at least one key restock location in the scene.");
            else
                return restockLocations;
        }
    }
    #endregion

    #region MonoBehavior Implementation
    private void Start()
    {
        // Add this restock location to the statically
        // exposed locations collection.
        if (restockLocations == null)
            restockLocations = new Transform[0];
        Array.Resize(ref restockLocations, restockLocations.Length + 1);
        restockLocations[restockLocations.Length - 1] = transform;
    }
    private void OnTriggerEnter(Collider other)
    {
        // Restore keys for guards who walk through this region.
        AIGuard guard = other.GetComponent<AIGuard>();
        if (guard != null)
            guard.RestoreKeys();
    }
    #endregion
}
