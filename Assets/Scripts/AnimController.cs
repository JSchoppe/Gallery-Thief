using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimController : MonoBehaviour
{
    private Animator animator;
    private GameObject player;
    
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

        //will switch to switches for optimization
        if (Input.GetKey(KeyCode.W))
        {
            //rotate player
            //start walk animation
            animator.SetBool("isWalking", true);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            animator.SetBool("isWalking", true);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            animator.SetBool("isWalking", true);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            animator.SetBool("isWalking", true);
        }

        else
        {
            //defaulting to idle animation when no input
            animator.SetBool("isWalking", false);

        }


    }
}
