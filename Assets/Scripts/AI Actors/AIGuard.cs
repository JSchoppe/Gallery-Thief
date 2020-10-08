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


    private Dictionary<KeyID, bool> isKeyStolen;

    private enum StationarySubState : byte
    {
        ReturningHome, AtHome
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.matrix *= Matrix4x4.Translate(Vector3.up * viewHeight);
        Gizmos.color = Color.blue;
        Gizmos.DrawFrustum(Vector3.zero, fieldOfView, viewDistance, 0, 1);
    }


    private Vector3 stationaryHome;

    private AIBehaviorState currentBehavior;
    /// <summary>
    /// The current behavior of this AI.
    /// </summary>
    public AIBehaviorState Behavior
    {
        get { return currentBehavior; }
        set
        {
            if (!illegalBehaviors.Contains(value)
                && value != currentBehavior)
            {
                currentBehavior = value;
                switch (value)
                {
                    case AIBehaviorState.Stationary:
                        stationaryState = StationarySubState.ReturningHome;
                        break;
                    case AIBehaviorState.Patrolling:
                        break;
                    case AIBehaviorState.Investigating:
                        break;
                    case AIBehaviorState.Chasing:
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }
    }



    private StationarySubState stationaryState;

    private Rigidbody body;

    private float pathFindingTimer = 0f;

    [SerializeField] private Transform target;

    private void Start()
    {
        Behavior = initialBehavior;
        stationaryHome = transform.position;
        body = GetComponent<Rigidbody>();

        isKeyStolen = new Dictionary<KeyID, bool>();
        foreach (DoorKey key in gameObject.GetComponents<DoorKey>())
        {
            isKeyStolen.Add(key.KeyIdentity, false);
            navAgent.areaMask += NavUtilities.NavAreaFromKeyID(key.KeyIdentity);
        }

        navAgent.SetDestination(target.position);
    }

    private void Update()
    {
        switch (Behavior)
        {
            case AIBehaviorState.Stationary:
                StationaryBehavior();
                break;
            case AIBehaviorState.Patrolling:
                break;
            case AIBehaviorState.Investigating:
                break;
            case AIBehaviorState.Chasing:
                break;
            default:
                throw new NotImplementedException();
        }
    }

    private void StationaryBehavior()
    {
        switch (stationaryState)
        {
            case StationarySubState.ReturningHome:

                break;
            case StationarySubState.AtHome:

                break;
            default:
                throw new NotImplementedException();
        }

    }

    private IEnumerator PathFindAfterInit()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);

        }
    }
}
