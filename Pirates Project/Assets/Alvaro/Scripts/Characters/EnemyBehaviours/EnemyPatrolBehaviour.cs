using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrolBehaviour : StateMachineBehaviour
{
    public float minWaitingTime;
    public float maxWaitingTime;

    private float waitingTime;
    private float waitingTimer;
    private bool waiting;

    private bool firstPoint;

    private EnemyBehaviour enemy;
    private NavMeshAgent agent;
    private Vector3 nextDestination;

    override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        waitingTimer = 0f;

        firstPoint = true;
        nextDestination = enemy.GetNextPatrolPoint(firstPoint);
        agent.SetDestination(nextDestination);
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponent<EnemyBehaviour>();
        agent = enemy.GetNavMeshAgent();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(!waiting)
        {
            animator.SetFloat("Movement", 1.0f);

            bool running = firstPoint || enemy.IsRunning();
            animator.SetBool("Running", running);
            agent.speed = running ? enemy.runningSpeed : enemy.movingSpeed;

            if(agent.remainingDistance == 0f)
            {
                firstPoint = false;
                waiting = true;
                waitingTime = Random.Range(minWaitingTime, maxWaitingTime);
            }
        }
        else
        {
            animator.SetFloat("Movement", 0.0f);

            waitingTimer += Time.deltaTime;

            if(waitingTimer >= waitingTime)
            {
                nextDestination = enemy.GetNextPatrolPoint(false);
                agent.SetDestination(nextDestination);

                waitingTimer = 0f;
                waiting = false;
            }
        }
    }

    override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
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
