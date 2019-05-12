using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyDeathBehaviour : StateMachineBehaviour 
{
    NavMeshAgent NavMeshAgent;
    EnemyBehaviour EnemyBehaviour;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        NavMeshAgent = animator.GetComponent<NavMeshAgent>();
        EnemyBehaviour = animator.GetComponent<EnemyBehaviour>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) {
        NavMeshAgent.isStopped = true;
        NavMeshAgent.ResetPath();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) {
    
    }
}
