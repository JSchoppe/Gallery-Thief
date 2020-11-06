using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

/// <summary>
/// Represents the behavior of a guard.
/// </summary>
public class AIGuard : MonoBehaviour, IKeyUser
{
    // Might need to change this if the AI causes lag.
    private const float repathTime = 0.1f;

    public AudioSource audioSource;
    [SerializeField] private AudioClip[] guardFootsteps;
    #region Inspector Fields
    [Tooltip("The agent that will be used to traverse the scene.")]
    [SerializeField] private NavMeshAgent navAgent = null;
    [Tooltip("The audio source used to play the alarmed sfx.")]
    [SerializeField] private AudioSource whistleSource = null;
    [Header("Behavior Parameters")]
    [Tooltip("What the guard is doing when the scene loads.")]
    [SerializeField] private AIBehaviorState initialBehavior = AIBehaviorState.Stationary;
    [Tooltip("AI states that this guard cannot enter.")]
    [SerializeField] private AIBehaviorState[] illegalBehaviors = null;
    [Header("Movement Parameters")]
    [Tooltip("Defines the patrol route of the guard.")]
    [SerializeField] private Transform[] patrolRoute = null;
    [Range(float.Epsilon, 10f)][Tooltip("Controls how close a guard has to get to a node before going to the next node.")]
    [SerializeField] private float pathTolerance = 1f;
    [Range(float.Epsilon, 10f)][Tooltip("Controls the rate guards will pivot towards a direction when stationary.")]
    [SerializeField] private float pivotSpeed = 2f;
    [Header("Vision Parameters")]
    [Range(float.Epsilon, 5f)][Tooltip("The elevation of the view above the ground.")]
    [SerializeField] private float viewHeight = 1f;
    [Range(10f, 160f)][Tooltip("Controls the field of view of this guards vision.")]
    [SerializeField] private float fieldOfView = 35f;
    [Range(float.Epsilon, 100f)][Tooltip("Controls how far this guard can see ahead of itself.")]
    [SerializeField] private float viewDistance = 10f;
    [Range(float.Epsilon, 5f)][Tooltip("If the player gets this close the guard will be alerted.")]
    [SerializeField] private float personalSpaceRadius = 1f;
    [Header("Debug Parameters")]
    [Tooltip("Will render a given mesh with the color of the current state.")]
    [SerializeField] private bool renderDebug = true;
    [Tooltip("The renderer that displays the collider and state of the AI.")]
    [SerializeField] private Renderer debugRenderer = null;
    //animator
    [SerializeField] private Animator anim;
    #endregion
    #region Private Fields
    private AIBehaviorState currentBehavior;
    private Vector3 stationaryHome;
    private Vector3 stationaryDirection;
    private Stack<Vector3> investigationPoints;
    private Transform transformCurrentlyChasing;
    private int patrolIndex;
    // Note: current keys reflects which keys the guard
    // thinks they have (only updated when approaching a door).
    private List<KeyID> currentKeys;
    // These keys are replaced when the guard returns to security.
    private List<KeyID> stolenKeys;
    #endregion
    #region Debug Gizmos Drawing
    private void OnDrawGizmosSelected()
    {
        // Null check required here since the investigation
        // points are only used during runtime.
        if (investigationPoints != null)
        {
            // Draw invesitgation points and a line back to this guard.
            Gizmos.color = Color.red;
            foreach (Vector3 location in investigationPoints)
            {
                Gizmos.DrawSphere(location, 0.5f);
                Gizmos.DrawLine(transform.position, location);
            }
        }
        Gizmos.color = Color.blue;
        // Draw the personal space of this guard.
        Vector3[] arcPoints = new Vector3[16];
        for (int i = 0; i < 16; i++)
        {
            float angle = (i / 16f) * Mathf.PI * 2f;
            arcPoints[i] = transform.TransformPoint(new Vector3
            {
                x = Mathf.Cos(angle) * personalSpaceRadius,
                z = Mathf.Sin(angle) * personalSpaceRadius
            });
        }
        for (int i = 0; i < 15; i++)
            Gizmos.DrawLine(arcPoints[i], arcPoints[i + 1]);
        Gizmos.DrawLine(arcPoints[0], arcPoints[15]);
        // Draw the front view region for this guard.
        // This is a bit jank tbh.
        for (int i = 0; i < 15; i++)
        {
            float angle = ((i - 7) / 7f) * (fieldOfView / 2) * Mathf.Deg2Rad;
            arcPoints[i] = transform.TransformPoint(new Vector3
            {
                x = Mathf.Sin(angle) * viewDistance,
                z = Mathf.Cos(angle) * viewDistance
            });
        }
        for (int i = 0; i < 14; i++)
            Gizmos.DrawLine(arcPoints[i], arcPoints[i + 1]);
        Gizmos.DrawLine(transform.position, arcPoints[0]);
        Gizmos.DrawLine(transform.position, arcPoints[14]);
    }
    #endregion

    #region Public Properties
    /// <summary>
    /// The current behavior of this AI.
    /// </summary>
    public AIBehaviorState Behavior
    {
        // TODO: The setter here is super jank.
        get { return currentBehavior; }
        private set
        {
            // Make sure this behavior is legal for this AI.
            if (!illegalBehaviors.Contains(value))
            {
                // Initialize new state for this behavior.
                switch (value)
                {
                    case AIBehaviorState.Stationary:
                        // Walk the AI back to its home.
                        navAgent.SetDestination(stationaryHome);
                        currentBehavior = AIBehaviorState.Stationary;
                        if (renderDebug)
                            debugRenderer.material =
                                AIDebug.AIMats?[AIBehaviorState.Stationary];
                        break;
                    case AIBehaviorState.Patrolling:
                        // Start the patrol loop.
                        patrolIndex = 0;
                        navAgent.SetDestination(patrolRoute[patrolIndex].position);
                        currentBehavior = AIBehaviorState.Patrolling;
                        if (renderDebug)
                            debugRenderer.material =
                                AIDebug.AIMats?[AIBehaviorState.Patrolling];
                        break;
                    case AIBehaviorState.Investigating:
                        // If there are places to investigate:
                        if (investigationPoints.Count > 0)
                        {
                            // Start investigating the first point of interest.
                            navAgent.ResetPath();
                            navAgent.SetDestination(investigationPoints.Peek());
                            currentBehavior = AIBehaviorState.Investigating;
                            if (renderDebug)
                                debugRenderer.material =
                                    AIDebug.AIMats?[AIBehaviorState.Investigating];
                        }
                        break;
                    case AIBehaviorState.Chasing:
                        navAgent.SetDestination(transformCurrentlyChasing.position);
                        currentBehavior = AIBehaviorState.Chasing;
                        StartCoroutine(ChaseRepath());
                        if (renderDebug)
                            debugRenderer.material =
                                AIDebug.AIMats?[AIBehaviorState.Chasing];
                        break;
                    // This will throw if a new AI state is
                    // added and is not addressed here:
                    default:
                        throw new NotImplementedException();
                }
            }
        }
    }
    /// <summary>
    /// Whether this guard can respond to an alarm.
    /// </summary>
    public bool CanRespond
    {
        get
        {
            return (Behavior != AIBehaviorState.Chasing)
                && (!illegalBehaviors.Contains(AIBehaviorState.Investigating));
        }
    }
    #endregion
    #region Public Methods
    /// <summary>
    /// Gets the total distance to this guard must travel to reach the position.
    /// </summary>
    /// <param name="position">The point of interest they would be responding to.</param>
    /// <returns>The total path distance to the response point.</returns>
    public float GetResponseDistance(Vector3 position)
    {
        // Precalculate path and return length.
        NavMeshPath path = new NavMeshPath();
        if (navAgent.CalculatePath(position, path))
        {
            float length = 0f;
            for (int i = 1; i < path.corners.Length; i++)
                length += Vector3.Distance(path.corners[i], path.corners[i - 1]);
            return length;
        }
        else { return float.MaxValue; }
    }
    /// <summary>
    /// Adds a point of interest for this guard to investigate.
    /// </summary>
    /// <param name="location">The location to investigate.</param>
    public void Alert(Vector3 location)
    {
        // Do not interrupt the guard if they are chasing a player.
        // Can this guard even investigate the alert?
        if (Behavior != AIBehaviorState.Chasing &&
            !illegalBehaviors.Contains(AIBehaviorState.Investigating))
        {
            investigationPoints.Push(location);
            Behavior = AIBehaviorState.Investigating;
        }
    }
    /// <summary>
    /// Makes the guard check if they have a given key to open a door.
    /// If the guard doesn't have the key they may become suspicious or look for the key.
    /// </summary>
    /// <param name="doorKey">The type of key.</param>
    /// <returns>True if the key is present.</returns>
    public bool CheckKey(KeyDoor door)
    {
        // Does the guard think they have the key?
        if (currentKeys.Contains(door.LockID))
        {
            KeyID requiredKey = door.LockID;
            // Make sure the key hasn't been stolen.
            foreach (DoorKey key in gameObject.GetComponents<DoorKey>())
                if (key.KeyIdentity == requiredKey)
                    return true;
            // Otherwise the key has been stolen.
            currentKeys.Remove(requiredKey);
            stolenKeys.Add(requiredKey);
            // Update the pathing in navmesh.
            navAgent.areaMask -= NavUtilities.NavAreaFromKeyID(requiredKey);
            // React to the key loss with behavior:
            switch (Behavior)
            {
                case AIBehaviorState.Stationary:
                case AIBehaviorState.Patrolling:
                    investigationPoints.Push(KeyRestock.RestockLocations[0].position);
                    Behavior = AIBehaviorState.Investigating;
                    break;
                case AIBehaviorState.Investigating:
                    // Generate a cluster of suspicion points on the other side of the door.
                    investigationPoints.Push(KeyRestock.RestockLocations[0].position);
                    Behavior = AIBehaviorState.Investigating;
                    break;
            }
        }
        return false;
    }
    /// <summary>
    /// Restores all of this guards stolen keys.
    /// </summary>
    public void RestoreKeys()
    {
        // Re-enable keys on this guard.
        foreach (KeyID key in stolenKeys)
        {
            DoorKey newKey = gameObject.AddComponent<DoorKey>();
            newKey.KeyIdentity = key;
            currentKeys.Add(key);
            // Update where the AI can navigate to.
            int maskBit = NavUtilities.NavAreaFromKeyID(key);
            navAgent.areaMask = navAgent.areaMask | maskBit;
        }
        stolenKeys.Clear();
    }
    #endregion

    #region MonoBehavior Implementation
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        // Populate the inital keys that this guard has.
        stolenKeys = new List<KeyID>();
        currentKeys = new List<KeyID>();
        foreach (DoorKey key in gameObject.GetComponents<DoorKey>())
        {
            currentKeys.Add(key.KeyIdentity);
            int maskBit = NavUtilities.NavAreaFromKeyID(key.KeyIdentity);
            // Update where the AI can navigate to.
            navAgent.areaMask = navAgent.areaMask | maskBit;
        }
        investigationPoints = new Stack<Vector3>();
        // The stationary home is always the gaurds initial scene position and rotation.
        stationaryHome = transform.position;
        stationaryDirection = transform.forward;
        // Set the initial state of this guard.
        Behavior = initialBehavior;
    }
    private void Update()
    {
        // Do the update behavior specific to the current state.
        switch (Behavior)
        {
            case AIBehaviorState.Stationary:
                StationaryUpdate(); break;
            case AIBehaviorState.Patrolling:
                PatrollingUpdate(); break;
            case AIBehaviorState.Investigating:
                InvestigatingUpdate(); break;
            case AIBehaviorState.Chasing:
                ChasingUpdate(); break;
        }

        // If not chasing, check to see if any
        // players are inside the field of view.
        if (Behavior != AIBehaviorState.Chasing)
        {
            foreach (PlayerController actor in AlarmSingleton.SuspiciousActors)
            {
                // Get the direction vector from the AI to the actor.
                Vector3 actorDirection = AlarmSingleton.GetActorTorso(actor)
                    - transform.position;

                if (audioSource.isPlaying == false)
                    audioSource.PlayOneShot(guardFootsteps[Random.Range(0, guardFootsteps.Length)]);
                // Ignore elevation.
                actorDirection.y = 0f;
                // Check to see if the actor is within the personal space,
                // or if they are within the field of view.
                if (actorDirection.magnitude < personalSpaceRadius
                    ||
                    (actorDirection.magnitude < viewDistance &&
                    Vector3.Angle(actorDirection, transform.forward) < fieldOfView / 2))
                    

                {
                    // Run a linecast to make sure there isn't a
                    // wall between the guard and actor.
                    if (IsSightLineClear(AlarmSingleton.GetActorTorso(actor)))
                    {
                        // If the player is seen switch to chasing.
                        transformCurrentlyChasing = actor.transform;
                        Behavior = AIBehaviorState.Chasing;
                        // Play the whistle sound effect.
                        whistleSource.Play();
                        break;
                    }
                }
            }
        }
    }
    #region State Specific Updates
    private void StationaryUpdate()
    {
        // If the stationary guard has come to rest,
        // make sure they are facing the correct direction.
        if (!navAgent.hasPath && transform.forward != stationaryDirection)
        {
            transform.forward =
                Vector3.RotateTowards(transform.forward, stationaryDirection,
                pivotSpeed * Time.deltaTime, float.MaxValue);
        }
        anim.SetBool("isWalking", false);
    }
    private void PatrollingUpdate()
    {
        // Has the AI reached a point on their patrol?
        if (!navAgent.pathPending && 
                navAgent.remainingDistance < pathTolerance)
        {
            // Get the next point on the patrol.
            patrolIndex++;
            if (patrolIndex == patrolRoute.Length)
                patrolIndex = 0;
            // Retarget the AI towards the next patrol point.
            navAgent.SetDestination(patrolRoute[patrolIndex].position);
        }
        anim.SetBool("isWalking", false);
    }
    private void InvestigatingUpdate()
    {
        // Has a point of interest been examined?
        if (!navAgent.pathPending && 
            navAgent.remainingDistance < pathTolerance)
        {
            investigationPoints.Pop();
            // Are there more points to investigate?
            if (investigationPoints.Count > 0)
                // Investigate the next point.
                navAgent.SetDestination(investigationPoints.Peek());
            else
            {
                // Revert to default behavior: patrolling or stationary.
                if (illegalBehaviors.Contains(AIBehaviorState.Patrolling))
                    Behavior = AIBehaviorState.Stationary;
                else
                    Behavior = AIBehaviorState.Patrolling;
            }
        }
        anim.SetBool("isWalking", false);
    }
    private void ChasingUpdate()
    {
        // Look at the player that is being chased.
        transform.LookAt(new Vector3
        {
            x = transformCurrentlyChasing.position.x,
            y = transform.position.y,
            z = transformCurrentlyChasing.position.z,
        });
        if (Vector3.Distance(transformCurrentlyChasing.position, transform.position) < personalSpaceRadius)
            LevelStateSingleton.NotifyPlayerCaught(
                transformCurrentlyChasing.GetComponent<PlayerController>());
        anim.SetBool("isWalking", true);
    }
    // This is run alongside chasing
    // update but not as frequently.
    private IEnumerator ChaseRepath()
    {
        while (true)
        {
            // If the player is in sight, keep chasing them.
            if (IsSightLineClear(transformCurrentlyChasing))
                navAgent.destination = transformCurrentlyChasing.position;
            else
            {
                // TODO this is kind of jank.
                // Generate a cluster of points to investigate.
                investigationPoints.Clear();
                Vector3 direction = transformCurrentlyChasing.gameObject.GetComponent<Rigidbody>().velocity;
                direction = direction.normalized;
                investigationPoints.Push(transformCurrentlyChasing.position + direction);
                investigationPoints.Push(transformCurrentlyChasing.position);
                // Change to investigation state to continue searching for the player.
                Behavior = AIBehaviorState.Investigating;
                // Break out of coroutine loop.
                break;
            }
            // Repeat loop after some time.
            yield return new WaitForSeconds(repathTime);
        }
    }
    #endregion
    #endregion
    #region Private Helper Functions
    private bool IsSightLineClear(Vector3 end)
    {
        return !Physics.Linecast(transform.position + transform.up * viewHeight, end);
    }
    private bool IsSightLineClear(Transform end)
    {
        return IsSightLineClear(end.position);
    }
    #endregion
}
