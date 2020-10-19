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
        if (Input.GetKey(KeyCode.W))
        {
            //rotate player
            //start walk animation
            animator.SetBool("isWalking", true);
        }
        if (Input.GetKey(KeyCode.W))
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
