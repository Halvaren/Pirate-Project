using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefinitiveScript;

public class EnemyBlockBehaviour : StateMachineBehaviour
{
    public float minTimeBlocking = 1f;
    public float maxTimeBlocking = 3f;

    private float blockingTimer;

    private EnemyBehaviour enemy;
    private EnemySableController sableController;
    private HealthController health;

    private Vector3 playerPosition;
    private float distanceFromPlayer;
    private Vector3 enemyToPlayer;

    override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        blockingTimer = 0f;
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponent<EnemyBehaviour>();
        health = animator.GetComponent<HealthController>();
        sableController = animator.GetComponent<EnemySableController>();
        
        sableController.SetBlocking(true);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerPosition = enemy.GetPlayerTransform().position;
        enemyToPlayer = playerPosition - animator.transform.position;
        enemyToPlayer.y = 0f;
        distanceFromPlayer = enemyToPlayer.magnitude;

        blockingTimer += Time.deltaTime;

        bool condition = (blockingTimer < minTimeBlocking || Random.Range(health.GetCurrentStamina(), health.GetTotalStamina()) > 0.5f * health.GetTotalStamina() && blockingTimer > minTimeBlocking);
        condition = condition && blockingTimer < maxTimeBlocking;


        if(!condition) enemy.SetStaring(); //enemy.SetStaring(!condition);
        if(!enemy.IsStaring() && condition) enemy.SetBlocking(); //enemy.SetBlocking(condition);
        sableController.SetBlocking(!enemy.IsStaring() && condition);

        if(blockingTimer >= maxTimeBlocking) blockingTimer = 0f;

        if(distanceFromPlayer > enemy.maxDistanceFromPlayer)
        {
            enemy.SetFollowing();
            //enemy.SetBlocking(false);
            sableController.SetBlocking(false);
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
