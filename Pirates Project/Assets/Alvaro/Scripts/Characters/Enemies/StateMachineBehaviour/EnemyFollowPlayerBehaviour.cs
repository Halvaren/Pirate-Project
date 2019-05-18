using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyFollowPlayerBehaviour : StateMachineBehaviour
{
    private EnemyBehaviour enemy;
    private NavMeshAgent agent;

    private Vector3 playerPosition;
    private float distanceFromPlayer;
    private Vector3 enemyToPlayer;

    override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponent<EnemyBehaviour>();
        agent = enemy.GetNavMeshAgent();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerPosition = enemy.GetPlayerTransform().position;
        agent.SetDestination(playerPosition);
        agent.speed = enemy.runningSpeed;

        enemyToPlayer = playerPosition - animator.transform.position;
        enemyToPlayer.y = 0f;
        distanceFromPlayer = enemyToPlayer.magnitude;

        if(distanceFromPlayer < enemy.maxDistanceFromPlayer)
        {
            agent.isStopped = true;
            agent.ResetPath();
            
            //enemy.SetFollowing(false);
            enemy.SetStaring();
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    /* 
    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }*/
}
