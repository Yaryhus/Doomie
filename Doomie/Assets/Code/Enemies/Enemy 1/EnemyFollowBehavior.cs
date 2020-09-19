using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFollowBehavior : StateMachineBehaviour
{
    /*
    
    Transform owner;
    Enemy enemigo;
    Transform playerPos;
    [SerializeField]
    float speed;
    PlayerManager playerManager;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerManager = PlayerManager.instance;

        enemigo = animator.GetComponentInParent<Enemy>();
        owner = enemigo.GetComponentInParent<Transform>();
        playerPos = playerManager.Player.transform;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        owner.position = Vector3.MoveTowards(owner.transform.position, playerPos.position, speed * Time.deltaTime);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
    
    */

}
