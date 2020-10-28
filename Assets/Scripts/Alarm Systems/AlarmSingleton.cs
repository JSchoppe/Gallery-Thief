using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Stores singleton data related to the alarm mechanisms.
/// </summary>
public sealed class AlarmSingleton : MonoBehaviour
{
    [SerializeField] private PlayerController[] suspiciousActors = null;
    [SerializeField] private GameObject laserLineRendererObject = null;
    [SerializeField] private Material defaultLaserMat = null;
    [SerializeField] private Material alarmedLaserMat = null;

    public static PlayerController[] SuspiciousActors { get; private set; }
    private static GameObject laserLineRendererPrefab;
    private static Transform laserHolderTransform;

    public static Material DefaultLaserMat { get; private set; }
    public static Material AlarmedLaserMat { get; private set; }

    private static List<AIGuard> guards;

    private void Start()
    {
        laserHolderTransform = transform;
        SuspiciousActors = suspiciousActors;
        laserLineRendererPrefab = laserLineRendererObject;

        DefaultLaserMat = defaultLaserMat;
        AlarmedLaserMat = alarmedLaserMat;

        // TODO this feels kinda jank the way this is setup.
        foreach (IAlarmSystem system in FindObjectsOfType<MonoBehaviour>().OfType<IAlarmSystem>())
            system.OnTriggered += AlertNearestGuard;
        guards = new List<AIGuard>();
        foreach (AIGuard guard in FindObjectsOfType<AIGuard>())
            guards.Add(guard);
    }

    private void AlertNearestGuard(PlayerController player)
    {
        // Look for closest guard that can respond.
        AIGuard nearestAvailable = null;
        float nearestDistance = float.MaxValue;
        foreach (AIGuard guard in guards)
        {
            if (guard.CanRespond)
            {
                // TODO this is expensive! Might cause performance issues.
                // Maybe use Jobs to multithread this?
                float distance = guard.GetResponseDistance(player.transform.position);
                if (distance < nearestDistance)
                {
                    nearestAvailable = guard;
                    nearestDistance = distance;
                }
            }
        }
        // If a guard can respond, tap that guard to investigate.
        if (nearestAvailable != null)
            nearestAvailable.Alert(player.transform.position);
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
