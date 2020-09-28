using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float walkingSpeed = 5f;
    [SerializeField]
    float crouchingWalkingSpeed = 2.5f;
    [SerializeField]
    float crawlingSpeed = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        // TODO Need to be able to WALK, CROUCH, and CRAWL
    }

    // Update is called once per frame
    void Update()
    {
        UpdateInputs();
    }

    /// <summary>
    /// Updates the inputs from the player
    /// </summary>
    void UpdateInputs()
    {
        // Gets direction of axises
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        
        bool crouching = Input.GetButton("Crouch");
        bool crawling = Input.GetButton("Crawl");

        // changes position of player if any direction is used
        if (horizontal > 0 || horizontal < 0 || vertical > 0 || vertical < 0)
        {
            // Changes movement speed // TODO make sure speed does not change until animation between crawl/crouch/walking is complete
            if (crouching)
            {
                // Animation Change
                this.transform.position = this.transform.position + new Vector3(horizontal, 0, vertical) * crouchingWalkingSpeed * Time.deltaTime;
                
            }
            else if (crawling)
            {
                // Animation Change
                this.transform.position = this.transform.position + new Vector3(horizontal, 0, vertical) * crawlingSpeed * Time.deltaTime;
            }
            else
            {
                // Animation Change
                this.transform.position = this.transform.position + new Vector3(horizontal, 0, vertical) * walkingSpeed * Time.deltaTime;
            }
        }
    }
}
