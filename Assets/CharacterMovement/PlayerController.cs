﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] 
    float walkingSpeed = 5f;
    [SerializeField]
    float crouchingSpeed = 2.5f;
    [SerializeField]
    float crawlingSpeed = 1.5f;

    /* TODO camera ease in/out
    Vector3 cameraOrigin;
    Vector3 cameraTarget;
    AnimationCurve cameraCurve;
    */

    Transform cameraArm;
    Camera camera;

    // Start is called before the first frame update
    void Start()
    {
        cameraArm = gameObject.transform.Find("CameraArm");
        camera = GetComponent<Camera>();

        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(cameraArm.position);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCameraLoc();
    }

    private void FixedUpdate()
    {
        UpdateMovement();
    }

    /// <summary>
    /// Updates the inputs from the player
    /// </summary>
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
            // Changes movement speed 
            // TODO make sure speed does not change until animation between crawl/crouch/walking is complete
            if (crouching)
            {
                // Animation Change
                this.transform.position = this.transform.position + new Vector3(horizontal, 0, vertical) * crouchingSpeed * Time.deltaTime;
                Debug.Log("Crouching");
                
            }
            else if (crawling)
            {
                // Animation Change
                this.transform.position = this.transform.position + new Vector3(horizontal, 0, vertical) * crawlingSpeed * Time.deltaTime;
                Debug.Log("Crawling");
            }
            else
            {
                // Animation Change
                this.transform.position = this.transform.position + new Vector3(horizontal, 0, vertical) * walkingSpeed * Time.deltaTime;
            }
        }
    }

    void UpdateCameraLoc()
    {
        if (Input.GetButtonDown("CameraRight"))
        {
            // Rotate Camera Right
            Debug.Log("Camera Right");
            StartCoroutine("RotateCameraRight");
        }
        else if (Input.GetButtonDown("CameraLeft"))
        {
            // Rotate Camera Left
            Debug.Log("Camera Left");
            StartCoroutine("RotateCameraLeft");
        }
    }

    void RotateCameraRight()
    {
        this.cameraArm.transform.eulerAngles += Vector3.up * 90;
    }

    void RotateCameraLeft()
    {
        this.cameraArm.transform.eulerAngles -= Vector3.up * 90;
    }
}
