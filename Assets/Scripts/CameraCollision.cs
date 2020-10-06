using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script derived from https://www.youtube.com/watch?v=Uqi2jEgvVsI

public class CameraCollision : MonoBehaviour
{
    // layer that the camera should collide with
    public LayerMask collisionLayer;

    // bool if the camera is colliding
    public bool colliding = false;

    // points where the camera is right now
    public Vector3[] adjustedCameraClipPoints;
    // points where the camera should be so it isnt colldiding with anything
    public Vector3[] desiredCameraClipPoints;

    Camera camera;

    public void OnServerInitialized(Camera _camera)
    {
        camera = _camera;
        // 5 Points for the 4 camera corners and camera center
        adjustedCameraClipPoints = new Vector3[5];
        desiredCameraClipPoints = new Vector3[5];
    }

    public void UpdateCameraClipPoints(Vector3 cameraPosition, Quaternion atRotation, ref Vector3[] intoArray)
    {
        if (!camera)
        {
            return;
        }

        // clear the array
        intoArray = new Vector3[5];

        float z = camera.nearClipPlane;
        float x = Mathf.Tan(camera.fieldOfView / 3.41f) * z;
        float y = x / camera.aspect;

        // top left
        intoArray[0] = (atRotation * new Vector3(-x, y, z)) + cameraPosition; // added and rotated the point relative to the camera

        // top right
        intoArray[1] = (atRotation * new Vector3(x, y, z)) + cameraPosition;

        // bottom left
        intoArray[2] = (atRotation * new Vector3(-x, -y, z)) + cameraPosition;

        // bottom right
        intoArray[3] = (atRotation * new Vector3(x, -y, z)) + cameraPosition;

        // camera center (position)
        intoArray[4] = cameraPosition - camera.transform.forward; 

    }

    /// <summary>
    /// casts a ray from the camera points. returns true if ray collides with something
    /// </summary>
    /// <param name="clipsPoints"></param>
    /// <param name="fromPosition"></param>
    /// <returns></returns>
    bool CollisionDetectedAtClipPoints(Vector3[] clipsPoints, Vector3 fromPosition) // from position = targetpostion
    {
        for (int i = 0; i < clipsPoints.Length; i++)
        {
            Ray ray = new Ray(fromPosition, clipsPoints[i] - fromPosition);
            float rayDistance = Vector3.Distance(clipsPoints[i], fromPosition);
            if (Physics.Raycast(ray, rayDistance, collisionLayer))
            {
                return true;
            }
        }
        return false;
    }

    public float GetAdjustedDistanceWithRayFrom(Vector3 from)
    {
        float distance = -1;

        for (int i = 0; i < desiredCameraClipPoints.Length; i++)
        {
            // hit distance is the distance between where the camera point is now to where it wouldnt be colliding
            Ray ray = new Ray(from, desiredCameraClipPoints[i] - from);
            RaycastHit hit;
            // get hit info if raycast hits something
            if (Physics.Raycast(ray, out hit))
            {
                if (distance == -1)
                {
                    distance = hit.distance;
                }
                else
                {
                    if (hit.distance < distance)
                    {
                        // finds the shortest distance
                        distance = hit.distance;
                    }
                }
            }
        }

        if (distance == -1)
        {
            return 0;
        }
        else
        {
            return distance;
        }
    }

    public void CheckColliding(Vector3 targetPosition)
    {
        if (CollisionDetectedAtClipPoints(desiredCameraClipPoints, targetPosition))
        {
            colliding = true;
        }
        else
        {
            colliding = false;
        }
    }
}
