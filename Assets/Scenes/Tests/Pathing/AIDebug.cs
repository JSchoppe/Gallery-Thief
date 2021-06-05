using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains static references to AI debug assets.
/// </summary>
public sealed class AIDebug : MonoBehaviour
{
    #region Inspector Fields
    [Tooltip("The material applied to the debug mesh when a guard is stationary.")]
    [SerializeField] private Material stationaryMaterial = null;
    [Tooltip("The material applied to the debug mesh when a guard is patrolling.")]
    [SerializeField] private Material patrollingMaterial = null;
    [Tooltip("The material applied to the debug mesh when a guard is investigating.")]
    [SerializeField] private Material investigatingMaterial = null;
    [Tooltip("The material applied to the debug mesh when a guard is chasing.")]
    [SerializeField] private Material chasingMaterial = null;
    #endregion
    #region Static Properties
    /// <summary>
    /// Contains a debug material for each AI behavior state.
    /// </summary>
    public static Dictionary<AIBehaviorState, Material> AIMats { get; private set; }
    #endregion

    #region Static Properties Initialization
    private void Start()
    {
        // Post the inspector fields to statically accessible properties.
        AIMats = new Dictionary<AIBehaviorState, Material>
        {
            { AIBehaviorState.Stationary, stationaryMaterial },
            { AIBehaviorState.Patrolling, patrollingMaterial },
            { AIBehaviorState.Investigating, investigatingMaterial },
            { AIBehaviorState.Chasing, chasingMaterial }
        };
    }
    #endregion
}
