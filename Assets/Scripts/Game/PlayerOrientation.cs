using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(NetworkGamePlayer))]
public class PlayerOrientation : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private NavMeshAgent _agent;

    int walkingState = 0; 

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        Vector3 moveDirection = _agent.steeringTarget - _agent.transform.position;

        //Debug.Log("++++++++++ ANIMATOR: " + animator.runtimeAnimatorController);
        //Debug.Log(moveDirection);
        animator.SetInteger("walkingState", walkingState); 

    }
}
