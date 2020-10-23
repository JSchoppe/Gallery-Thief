using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimController : MonoBehaviour
{
    private Animator animator;
    private GameObject player;
    //IEnum for player state standing, crouching etc


    //WHEN ADDING TO MESH NOT PLAYER GAMEOBJECT
    //PLACE ANIMATED MESH INSIDE EMPTY MESH GAMEOBJECT AND THIS SCRIPT ONTO EMPTY AS A COMPONENT
    void Start()
    {
        animator = GetComponent<Animator>();
        player = GetComponent<GameObject>();
    }
    void Update()
    {
        Walk();
        
    }

    void Walk()
    {

        
        //set in states after crouching and crawling is implemented

        //IEnum = standing
        if (Input.GetButton("Horizontal") || Input.GetButton("Vertical")) //continuous input
        {
            animator.SetBool("isWalking", true);
        }

        else
        {
            //defaulting to idle animation when no input
            animator.SetBool("isWalking", false);

        }
        //set IEnums for crouching and crawling for later


    }
}
