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
    
    float cameraTargetDistance = 20;
    float cameraCurrentDistance;

    [SerializeField]
    float cameraSmoothing = 50f;

    private Vector3 rayDirection;

    private void Start()
    {
        wall = LayerMask.GetMask("Wall");
        camera = GetComponent<Camera>();
        player = GameObject.FindGameObjectWithTag("Player");

        
    }

    private void Update()
    {

        rayDirection = camera.transform.position - player.transform.position;
        ray = new Ray(player.transform.position, rayDirection);


        cameraCurrentDistance = Vector3.Distance(this.transform.position, player.transform.position);

        if (cameraCurrentDistance < cameraTargetDistance)
        {
            // pulls the camera back to the start position
            cameraCurrentDistance += cameraSmoothing * Time.deltaTime;
            // finds the minimum between the two values
            cameraCurrentDistance = Mathf.Min(cameraCurrentDistance, cameraTargetDistance);
        }


        if (Physics.Raycast(ray, out hit, Mathf.Infinity, wall))
        {
            // get distance between player and hit point
            // clamp the camera to theh hit point
            float hitDistance = Vector3.Distance(hit.point, player.transform.position);

            if (cameraCurrentDistance > hitDistance)
            {
                cameraCurrentDistance = hitDistance;
            }


            Debug.DrawRay(ray.origin, ray.direction, Color.red);
            Debug.Log("hit wall");
        }

        this.transform.position = player.transform.position + ray.direction * cameraCurrentDistance;

        camera.transform.LookAt(player.transform);
    }
}
