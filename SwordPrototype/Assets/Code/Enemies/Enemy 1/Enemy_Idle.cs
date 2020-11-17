using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Idle : StateMachineBehaviour
{
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        /*
        if (Input.GetKeyDown(KeyCode.Space))
            animator.SetBool("isFollowing", true);
        */
    }

    
}
