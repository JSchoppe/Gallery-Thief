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

    // this is for debug.
    [SerializeField]
    private KeyID[] startingKeys;

    /* TODO camera ease in/out
    Vector3 cameraOrigin;
    Vector3 cameraTarget;
    AnimationCurve cameraCurve;
    */

    Transform cameraArm;
    
    void Start()
    {
        cameraArm = gameObject.transform.Find("CameraArm");

        keys = new List<KeyID>();
        foreach (KeyID key in startingKeys)
            keys.Add(key);
    }
    
    void Update()
    {
        UpdateCameraLoc();
        Interact();
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
            // Changes movement speed 
            // TODO make sure speed does not change until animation between crawl/crouch/walking is complete
            if (crouching)
            {
                // Animation Change
                this.transform.position += ((horizontal * cameraArm.right) + (vertical * cameraArm.forward)) * (crouchingSpeed * Time.deltaTime);
                playerCollider.height = 8f;
                playerCollider.center = new Vector3(playerCollider.center.x, 3f, playerCollider.center.z);
            }
            else if (crawling)
            {
                // Animation Change
                this.transform.position += ((horizontal * cameraArm.right) + (vertical * cameraArm.forward)) * (crawlingSpeed * Time.deltaTime);
                playerCollider.height = 1f;
                playerCollider.center = new Vector3(playerCollider.center.x, 2f, playerCollider.center.z);
            }
            else
            {
                // Animation Change
                this.transform.position += ((horizontal * cameraArm.right) + (vertical * cameraArm.forward)) * (walkingSpeed * Time.deltaTime);
                playerCollider.height = 12f;
                playerCollider.center = new Vector3(playerCollider.center.x , 5.5f, playerCollider.center.z);
            }

            // Makes sure the player faces the way it's moving
            lookRotation = Input.GetAxis("Horizontal") * cameraArm.right + Input.GetAxis("Vertical") * cameraArm.forward;
            this.mesh.rotation = Quaternion.RotateTowards(this.mesh.rotation, Quaternion.LookRotation(lookRotation), playerTurnSpeed * Time.deltaTime); 
        }
    }

    void UpdateCameraLoc()
    {
        if (Input.GetButtonDown("CameraRight"))
        {
            // Rotate Camera Right
            StartCoroutine("RotateCameraRight");
        }
        else if (Input.GetButtonDown("CameraLeft"))
        {
            // Rotate Camera Left
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

    void Interact()
    {
        /*
        if (Input.GetButtonDown("Interact"))
        {
            canMove = false;
        }*/
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

    public void GrantKey(KeyID key)
    {
        keys.Add(key);
    }
}
