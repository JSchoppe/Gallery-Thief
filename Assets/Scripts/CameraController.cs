using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    CameraCollision collision = new CameraCollision();

    // Start is called before the first frame update
    void Start()
    {
        collision.Initialize(Camera.main);
        collision.UpdateCameraClipPoints(transform.position, transform.rotation, ref collision.adjustedCameraClipPoints);
        //collision.UpdateCameraClipPoints(destination/*where camera is suppose to be*/, transform.rotation, ref collision.desiredCameraClipPoints);
    }

    // Update is called once per frame
    void Update()
    {
        if (collision.colliding)
        {

        }
        else
        {

        }
    }

    private void FixedUpdate()
    {
        collision.UpdateCameraClipPoints(transform.position, transform.rotation, ref collision.adjustedCameraClipPoints);
        //collision.UpdateCameraClipPoints(destination/*where camera is suppose to be*/, transform.rotation, ref collision.desiredCameraClipPoints);

        //collision.CheckColliding(targetPosition);
    }
}
