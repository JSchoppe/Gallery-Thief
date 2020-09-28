using System.Collections;
using UnityEngine;

/// <summary>
/// Represents the behavior of a guard.
/// </summary>
public class AIGuard : MonoBehaviour
{
    #region Inspector Fields
    [Tooltip("The pathing network that this AI is bound to.")]
    [SerializeField] private PathingNetwork network = null;
    [Header("Behavior Parameters")]
    [Tooltip("What the guard is doing when the scene loads.")]
    [SerializeField] private AIBehaviorState initialState = AIBehaviorState.Stationary;
    [Tooltip("AI states that this guard cannot enter.")]
    [SerializeField] private AIBehaviorState[] illegalStates = null;
    [Tooltip("Defines the patrol route of the guard.")]
    [SerializeField] private PathingNode[] patrolRoute = null;
    [Header("Movement Parameters")]
    [Tooltip("Controls how quickly the guard can start running.")]
    [SerializeField] private float acceleration = 10f;
    [Tooltip("Controls the max running speed of the guard.")]
    [SerializeField] private float speedLimit = 2.5f;
    [Tooltip("Controls how close a guard has to get to a node before going to the next node.")]
    [SerializeField] private float pathTolerance = 1.0f;
    #endregion

    private Rigidbody body;

    private void Start()
    {
        body = GetComponent<Rigidbody>();
        // StartCoroutine(PathFindAfterInit());
    }

    /*
    private IEnumerator PathFindAfterInit()
    {
        while (true)
        {
            yield return new WaitForSeconds(3);

            PathingNode fromNode = network.FindNodeNear(transform.position);
            PathingNode toNode = network.FindNodeNear(target.position);

            PathingNode[] path = network.FindPath(fromNode, toNode);

            int pathIndex = 0;
            while (pathIndex < path.Length)
            {
                if (body.velocity.magnitude < speedLimit)
                    body.AddForce((path[pathIndex].transform.position - transform.position).normalized * acceleration * Time.deltaTime, ForceMode.Impulse);
                if (Vector3.Distance(transform.position, path[pathIndex].transform.position) < pathTolerance)
                    pathIndex++;
                yield return null;
            }
        }
    }
    */

    private void Update()
    {
        
    }
}
