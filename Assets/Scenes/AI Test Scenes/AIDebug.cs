using System.Collections.Generic;
using UnityEngine;

public class AIDebug : MonoBehaviour
{
    [SerializeField] private Material stationaryMaterial;
    [SerializeField] private Material patrollingMaterial;
    [SerializeField] private Material investigatingMaterial;
    [SerializeField] private Material chasingMaterial;

    public static Dictionary<AIBehaviorState, Material> debugMats;

    private void Start()
    {
        debugMats = new Dictionary<AIBehaviorState, Material>();
        debugMats.Add(AIBehaviorState.Stationary, stationaryMaterial);
        debugMats.Add(AIBehaviorState.Patrolling, patrollingMaterial);
        debugMats.Add(AIBehaviorState.Investigating, investigatingMaterial);
        debugMats.Add(AIBehaviorState.Chasing, chasingMaterial);
    }
}
