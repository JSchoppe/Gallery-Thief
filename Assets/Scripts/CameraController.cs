using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // draw raycast from screen
    // if the ray hits the ceiling, disable that mesh/opacity


    RaycastHit hit;
    Ray ray;
    Camera camera;
    GameObject player;

    MeshRenderer ceiling;

    private void Start()
    {
        camera = GetComponent<Camera>();
        player = GameObject.FindWithTag("Player");
    }

    private void Update()
    {
        CameraRotation();

        ray = camera.ScreenPointToRay(player.transform.position);

        if (Physics.Raycast(ray, out hit))
        {
            ceiling.enabled = true;
            if (hit.transform.tag == "Ceiling")
            {
                ceiling = hit.transform.GetComponent<MeshRenderer>();
                ceiling.enabled = false;
            }



            // Do something with the object that was hit by the raycast.
        }
    }

    void CameraRotation()
    {
        if (Input.GetMouseButton(1))
        {
            camera.transform.Translate(-Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"), 0.0f);
        }
    }

}







        //Camera camera;
        //GameObject player;

        //LayerMask wall;
        //RaycastHit hit;
        //Ray ray;

        //[SerializeField] [Tooltip("Distance the camera is away from the player")]
        //float cameraTargetDistance = 20;
        //[SerializeField] [Tooltip("Rate at which the camera move away from the player (Higher=Faster")]
        //float cameraSmoothing = 50f;
        //float cameraCurrentDistance;

        //private Vector3 rayDirection;

        //private void Start()
        //{
        //    wall = LayerMask.GetMask("Wall");
        //    camera = GetComponent<Camera>();
        //    player = GameObject.FindGameObjectWithTag("Player");
        //}

        //private void Update()
        //{
        //    CameraCollision();

        //    // Makes sure that the camera is always pointing at the player
        //    camera.transform.LookAt(new Vector3(player.transform.position.x, player.transform.position.y + 3f, player.transform.position.z));
        //}

        //void CameraCollision()
        //{
        //    // Vector with the direction of the ray
        //    rayDirection = camera.transform.position - player.transform.position;

        //    ray = new Ray(player.transform.position, rayDirection);

        //    // Float with the current distance between the camera and the player
        //    cameraCurrentDistance = Vector3.Distance(this.transform.position, player.transform.position);

        //    // Moves the camera away from the player if the camera can pan back
        //    if (cameraCurrentDistance < cameraTargetDistance)
        //    {
        //        cameraCurrentDistance += cameraSmoothing * Time.deltaTime;
        //        // finds the value of where the camera should be between the current pos and where the raycast hit
        //        cameraCurrentDistance = Mathf.Min(cameraCurrentDistance, cameraTargetDistance);
        //    }

        //    if (Physics.Raycast(ray, out hit, Mathf.Infinity, wall))
        //    {
        //        // gets the distance between the player and where the raycast hit // -1 so the camera doesn't clip into the wall/roof
        //        float hitDistance = Vector3.Distance(hit.point, player.transform.position) - 1f;

        //        // if the camera is further from the player than the hit point. move the camera to the hit point
        //        if (cameraCurrentDistance > hitDistance)
        //        {
        //            cameraCurrentDistance = hitDistance;
        //        }
        //        Debug.DrawRay(ray.origin, ray.direction, Color.red);
        //    }

        //    // Line where the camera is actually moved
        //    this.transform.position = player.transform.position + ray.direction * cameraCurrentDistance;
    

