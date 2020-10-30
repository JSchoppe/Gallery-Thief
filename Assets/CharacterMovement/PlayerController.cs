﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IKeyUser
{
    [SerializeField] 
    float walkingSpeed = 100f;
    [SerializeField]
    float crouchingSpeed = 2.5f;
    [SerializeField]
    float crawlingSpeed = 1.5f;
    [SerializeField] [Tooltip("How fast the player mesh rotates when changing forward directions")]
    float playerTurnSpeed = 400f;

    /// <summary> if the player can move </summary>
    bool canMove = true;
    /// <summary> forward direction of the player </summary>
    Vector3 lookRotation;

    [SerializeField] [Tooltip("Transform with the player mesh renderer and mesh filter")]
    Transform mesh;
    [SerializeField] [Tooltip("Capsule Collider attached to the player. Used to change the player's collision height when crouching/crawling")]
    CapsuleCollider playerCollider;
    [SerializeField] [Tooltip("Player's Rigidbody")]
    Rigidbody rb;
    [SerializeField]
    Transform CinemachineCamera;

    // this is for debug.
    [SerializeField]
    private KeyID[] startingKeys;
    
    void Start()
    {
        keys = new List<KeyID>();
        foreach (KeyID key in startingKeys)
            keys.Add(key);
    }
    
    void Update()
    {
        CameraZoom();
    }

    private void FixedUpdate()
    {
        UpdateMovement();
    }

    /// <summary> Updates the inputs from the player </summary>
    void UpdateMovement()
    {
        // Gets direction of axises
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Input variables for Crouching and Crawling
        bool crouching = Input.GetButton("Crouch");
        bool crawling = Input.GetButton("Crawl");

        // changes position of player if any direction is used
        if (horizontal > 0 || horizontal < 0 || vertical > 0 || vertical < 0)
        {
            // Changes movement speed and player collider
            if (crouching)
            {
                rb.velocity = ((transform.forward * vertical) + (transform.right * horizontal)) * crouchingSpeed * Time.fixedDeltaTime;
                playerCollider.height = 8f;
                playerCollider.center = new Vector3(playerCollider.center.x, 3f, playerCollider.center.z);
            }
            else
            {
                rb.velocity = (((new Vector3(CinemachineCamera.forward.x, 0, CinemachineCamera.forward.z)).normalized * vertical) + (CinemachineCamera.right * horizontal)) * walkingSpeed * Time.fixedDeltaTime;
                playerCollider.height = 12f;
                playerCollider.center = new Vector3(playerCollider.center.x , 5.5f, playerCollider.center.z);
            }

            // Makes sure the player faces the way it's moving
            

            lookRotation = (CinemachineCamera.forward * vertical) + (CinemachineCamera.right * horizontal);
            //OG code
            //this.mesh.rotation = Quaternion.RotateTowards(this.mesh.rotation, Quaternion.LookRotation(lookRotation), playerTurnSpeed * Time.deltaTime);
            //new code, turns 
            //this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, Quaternion.LookRotation(lookRotation), playerTurnSpeed * Time.deltaTime);
            
        }
    }

    void CameraZoom()
    {
        Debug.Log(Input.GetAxis("Mouse ScrollWheel"));
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            // move camera closer
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            // move camera away
        }
    }

    //void UpdateCameraLoc()
    //{
    //    if (Input.GetButtonDown("CameraRight"))
    //    {
    //        // Rotate Camera Right
    //        StartCoroutine("RotateCameraRight");
    //    }
    //    else if (Input.GetButtonDown("CameraLeft"))
    //    {
    //        // Rotate Camera Left
    //        StartCoroutine("RotateCameraLeft");
    //    }
    //}

    //void RotateCameraRight()
    //{
    //   // this.cameraArm.transform.eulerAngles -= Vector3.up * 90;
    //}

    //void RotateCameraLeft()
    //{
    //   // this.cameraArm.transform.eulerAngles += Vector3.up * 90;
    //}

    private List<KeyID> keys;
    public bool CheckKey(KeyDoor door)
    {
        if (keys.Contains(door.LockID))
        {
            return true;
        }
        return false;
    }

    public void GrantKey(KeyID key)
    {
        keys.Add(key);
    }
}
