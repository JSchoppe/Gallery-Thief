using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IKeyUser
{
    [SerializeField] 
    float walkingSpeed = 5f;
    [SerializeField]
    float crouchingSpeed = 2.5f;
    [SerializeField]
    float crawlingSpeed = 1.5f;

    // this is for debug.
    [SerializeField]
    private KeyID[] startingKeys;

    /* TODO camera ease in/out
    Vector3 cameraOrigin;
    Vector3 cameraTarget;
    AnimationCurve cameraCurve;
    */

    Transform cameraArm;

    // Start is called before the first frame update
    void Start()
    {
        cameraArm = gameObject.transform.Find("CameraArm");

        keys = new List<KeyID>();
        foreach (KeyID key in startingKeys)
            keys.Add(key);
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
                this.transform.position += ((horizontal * cameraArm.right) + (vertical * cameraArm.forward)) * (crouchingSpeed * Time.deltaTime);                
            }
            else if (crawling)
            {
                // Animation Change
                this.transform.position += ((horizontal * cameraArm.right) + (vertical * cameraArm.forward)) * (crawlingSpeed * Time.deltaTime);
                Debug.Log("Crawling");
            }
            else
            {
                // Animation Change
                this.transform.position += ((horizontal * cameraArm.right) + (vertical * cameraArm.forward)) * (walkingSpeed * Time.deltaTime);
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
        this.cameraArm.transform.eulerAngles -= Vector3.up * 90;
    }

    void RotateCameraLeft()
    {
        this.cameraArm.transform.eulerAngles += Vector3.up * 90;
    }


    private List<KeyID> keys;
    public bool CheckKey(KeyDoor door)
    {
        if (keys.Contains(door.LockID))
        {
            return true;
        }
        return false;
    }
}
