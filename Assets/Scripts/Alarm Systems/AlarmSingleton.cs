using UnityEngine;

public sealed class AlarmSingleton : MonoBehaviour
{

    [SerializeField] private PlayerController[] suspiciousActors = null;
    [SerializeField] private GameObject laserLineRendererObject = null;

    public static PlayerController[] SuspiciousActors { get; private set; }
    private static GameObject laserLineRendererPrefab;
    private static Transform laserHolderTransform;

    private void Start()
    {
        laserHolderTransform = transform;
        SuspiciousActors = suspiciousActors;
        laserLineRendererPrefab = laserLineRendererObject;
    }

    public static LineRenderer GetNewLaserRenderer()
    {
        GameObject newRendererBase = Instantiate(laserLineRendererPrefab);
        newRendererBase.transform.parent = laserHolderTransform;
        return newRendererBase.GetComponent<LineRenderer>();
    }

    // TODO this is a hack. Should be on player controller.
    // Put here to avoid conflicts.
    public static Vector3 GetActorTorso(PlayerController actor)
    {
        return actor.transform.position + Vector3.up;
    }
}
