using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// cast ray from each 4 corners and center of camera
// if any of these rays hit a wall, move the camera closer to the player
// move the camera closer down camera arm if any points are casting to a wall


public class CameraController : MonoBehaviour
{
    Camera camera;
    GameObject player;

    LayerMask wall;
    RaycastHit hit;
    Ray ray;
    
    [SerializeField] [Tooltip("Distance the camera is away from the player")]
    float cameraTargetDistance = 20;
    [SerializeField] [Tooltip("Rate at which the camera move away from the player (Higher=Faster")]
    float cameraSmoothing = 50f;
    float cameraCurrentDistance;

    private Vector3 rayDirection;

    private void Start()
    {
        wall = LayerMask.GetMask("Wall");
        camera = GetComponent<Camera>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        CameraCollision();

        // Makes sure that the camera is always pointing at the player
        camera.transform.LookAt(new Vector3(player.transform.position.x, player.transform.position.y + 3f, player.transform.position.z));
    }

    void CameraCollision()
    {
        // Vector with the direction of the ray
        rayDirection = camera.transform.position - player.transform.position;

        ray = new Ray(player.transform.position, rayDirection);

        // Float with the current distance between the camera and the player
        cameraCurrentDistance = Vector3.Distance(this.transform.position, player.transform.position);

        // Moves the camera away from the player if the camera can pan back
        if (cameraCurrentDistance < cameraTargetDistance)
        {
            cameraCurrentDistance += cameraSmoothing * Time.deltaTime;
            // finds the value of where the camera should be between the current pos and where the raycast hit
            cameraCurrentDistance = Mathf.Min(cameraCurrentDistance, cameraTargetDistance);
        }
        
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, wall))
        {
            // gets the distance between the player and where the raycast hit // -1 so the camera doesn't clip into the wall/roof
            float hitDistance = Vector3.Distance(hit.point, player.transform.position) - 1f;

            // if the camera is further from the player than the hit point. move the camera to the hit point
            if (cameraCurrentDistance > hitDistance)
            {
                cameraCurrentDistance = hitDistance;
            }
            Debug.DrawRay(ray.origin, ray.direction, Color.red);
            Debug.Log("hit wall");
        }

        // Line where the camera is actually moved
        this.transform.position = player.transform.position + ray.direction * cameraCurrentDistance;
    }
}
