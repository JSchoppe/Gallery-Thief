using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Represents the behavior of a guard.
/// </summary>
public class AIGuard : MonoBehaviour
{
    #region Inspector Fields
    [Tooltip("The transforms of the actors being searched for in the scene.")]
    [SerializeField] private Transform[] suspiciousActors;
    [Tooltip("The agent that will be used to traverse the scene.")]
    [SerializeField] private NavMeshAgent navAgent;
    [Header("Behavior Parameters")]
    [Tooltip("What the guard is doing when the scene loads.")]
    [SerializeField] private AIBehaviorState initialBehavior = AIBehaviorState.Stationary;
    [Tooltip("AI states that this guard cannot enter.")]
    [SerializeField] private AIBehaviorState[] illegalBehaviors = null;
    [Header("Movement Parameters")]
    [Tooltip("Defines the patrol route of the guard.")]
    [SerializeField] private Transform[] patrolRoute = null;
    [Range(float.Epsilon, 10f)][Tooltip("Controls how close a guard has to get to a node before going to the next node.")]
    [SerializeField] private float patrolTolerance = 1f;
    [Tooltip("Seconds between each pathfind update.")]
    [SerializeField] private float repathInterval = 1f;
    [Header("Vision Parameters")]
    [Range(float.Epsilon, 5f)][Tooltip("The elevation of the view above the ground.")]
    [SerializeField] private float viewHeight = 1f;
    [Range(10f, 80f)][Tooltip("Controls the field of view of this guards vision.")]
    [SerializeField] private float fieldOfView = 35f;
    [Range(float.Epsilon, 100f)][Tooltip("Controls how far this guard can see ahead of itself.")]
    [SerializeField] private float viewDistance = 10f;
    #endregion


    private void OnDrawGizmosSelected()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.matrix *= Matrix4x4.Translate(Vector3.up * viewHeight);
        Gizmos.color = Color.blue;
        Gizmos.DrawFrustum(Vector3.zero, fieldOfView, viewDistance, 0, 1);
    }


}
