using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefinitiveScript 
{
    public class PlayerLockUnlockTargetBehaviour : StateMachineBehaviour
    {
        PlayerAnimatorController PlayerAnimatorController;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            PlayerAnimatorController = animator.GetComponent<PlayerAnimatorController>();

            PlayerAnimatorController.SetAttacking(false);
            PlayerAnimatorController.SetBlocking(false);
            PlayerAnimatorController.SetLockedTarget();
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            
        }
    }
}
