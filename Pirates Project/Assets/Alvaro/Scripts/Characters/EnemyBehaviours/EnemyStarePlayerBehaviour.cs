using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DefinitiveScript;

public class EnemyStarePlayerBehaviour : StateMachineBehaviour
{
    public float rotationSpeed = 12f;
    public float turningSpeed = 30f;
    public LayerMask enemyLayer;
    public float characterCenterOffsetMagnitude = 0.1f;

    public float timeToAttack = 2f;
    private float timerToAttack;

    private EnemyBehaviour enemy;
    private NavMeshAgent agent;
    private HealthController health;

    private Vector3 playerPosition;
    private float distanceFromPlayer;
    private Vector3 enemyToPlayer;

    override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        timerToAttack = 0f;
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponent<EnemyBehaviour>();
        health = animator.GetComponent<HealthController>();
        agent = enemy.GetNavMeshAgent();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerPosition = enemy.GetPlayerTransform().position;
        enemyToPlayer = playerPosition - animator.transform.position;
        enemyToPlayer.y = 0f;
        distanceFromPlayer = enemyToPlayer.magnitude;

        agent.speed = enemy.movingSpeed;

        Quaternion rotation = Quaternion.LookRotation(enemyToPlayer);
        animator.transform.rotation = Quaternion.Slerp(animator.transform.rotation, rotation, rotationSpeed * Time.deltaTime);

        Vector3 characterCenterOffset = enemy.characterCenter.TransformDirection(Vector3.right * characterCenterOffsetMagnitude);
        if(Physics.Raycast(enemy.characterCenter.position + characterCenterOffset, enemyToPlayer, distanceFromPlayer, enemyLayer))
        {
            if(Physics.Raycast(enemy.characterCenter.position - characterCenterOffset, enemyToPlayer, distanceFromPlayer, enemyLayer))
            {
                animator.transform.RotateAround(playerPosition, Vector3.up, -turningSpeed * Time.deltaTime);
                animator.SetFloat("HorizontalMovement", 1.0f);
            }
            animator.transform.RotateAround(playerPosition, Vector3.up, turningSpeed * Time.deltaTime);
            animator.SetFloat("HorizontalMovement", -1.0f);
        }
        else
        {
            if(Physics.Raycast(enemy.characterCenter.position - characterCenterOffset, enemyToPlayer, distanceFromPlayer, enemyLayer))
            {
                animator.transform.RotateAround(playerPosition, Vector3.up, -turningSpeed * Time.deltaTime);
                animator.SetFloat("HorizontalMovement", 1.0f);
            }
            else
            {
                animator.SetFloat("HorizontalMovement", 0.0f);
                timerToAttack += Time.deltaTime;
            }
        }

        if(distanceFromPlayer > enemy.maxDistanceFromPlayer)
        {
            enemy.SetFollowing(true);
            enemy.SetStaring(false);
        }

        if(timerToAttack >= timeToAttack)
        {
            if(enemy.CanAttack())
            {
                timerToAttack = 0f;
                if(!health.GetRunOutOfStamina() && Random.Range(0, health.GetCurrentStamina()) > 0.7 * health.GetTotalStamina())
                {
                    enemy.SetBlocking(true);
                    enemy.SetStaring(false);
                }
                else if(enemy.CanAttack())
                {
                    enemy.SetStaring(false);
                    enemy.ComboAttack();
                }
            }
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
