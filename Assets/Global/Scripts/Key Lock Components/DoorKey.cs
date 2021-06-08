using UnityEngine;

/// <summary>
/// Corresponds to a set of doors a key can unlock.
/// </summary>
public enum KeyID
{
    A, B, C, D, E, F, G, H, I, J
}

/// <summary>
/// Represents an instance of a key that can open doors on the map.
/// </summary>
public sealed class DoorKey : MonoBehaviour
{
    #region Inspector Fields
    [Tooltip("Whether this key be pickpocketed from the guard.")]
    [SerializeField] private bool isStealable = false;
    [Tooltip("The set of doors that this key opens.")]
    [SerializeField] private KeyID keyIdentity = KeyID.A;
    #endregion
    #region Properties
    /// <summary>
    /// Whether this key be pickpocketed from the guard.
    /// </summary>
    public bool IsStealable
    {
        get { return isStealable; }
        set { isStealable = value; }
    }
    /// <summary>
    /// The set of doors that this key opens.
    /// </summary>
    public KeyID KeyIdentity
    {
        get { return keyIdentity; }
        set { keyIdentity = value; }
    }
    #endregion
}
