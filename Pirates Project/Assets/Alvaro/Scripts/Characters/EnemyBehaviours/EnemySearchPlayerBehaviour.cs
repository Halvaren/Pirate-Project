using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySearchPlayerBehaviour : StateMachineBehaviour
{
    private EnemyBehaviour enemy;
    private NavMeshAgent agent;

    private Vector3 lastPlayerPosition;

    override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        enemy = animator.GetComponent<EnemyBehaviour>();

        lastPlayerPosition = enemy.GetPlayerTransform().position;
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponent<EnemyBehaviour>();
        agent = enemy.GetNavMeshAgent();

        agent.SetDestination(lastPlayerPosition);
        agent.speed = enemy.runningSpeed;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(agent.remainingDistance == 0f)
        {
            //enemy.SetSearching(false);
            enemy.StartRunningPatrol();
            enemy.SetPatrolling();
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
