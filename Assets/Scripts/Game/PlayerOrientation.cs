using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(NetworkGamePlayer))]
public class PlayerOrientation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer sprite;

    private NavMeshAgent _agent;
    private Vector3 myScale; 

    int walkingState = 0; 

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        myScale = transform.localScale;
    }

    void Update()
    {
        Vector3 moveDirection = _agent.steeringTarget - _agent.transform.position;

        if(moveDirection.x < 0)
        {
            if (moveDirection.z < moveDirection.x)
            {
                walkingState = 2;
            }
            else
            {
                sprite.flipX = true;
                //myScale.x = -Math.Abs(myScale.x);
                walkingState = 1;
            }
        }
        else if(moveDirection.x > 0)
        {
            if (moveDirection.z > moveDirection.x)
            {
                walkingState = 3;
            }
            else
            {
                sprite.flipX = false;
                //myScale.x = Math.Abs(myScale.x); 
                walkingState = 1; 
            }
        }
        else
        {
            walkingState = 0; 
        }
        
        
        



        //Debug.Log("++++++++++ ANIMATOR: " + animator.runtimeAnimatorController);
        //Debug.Log(moveDirection);
        transform.localScale = myScale;
        animator.SetInteger("walkingState", walkingState); 

    }
}
