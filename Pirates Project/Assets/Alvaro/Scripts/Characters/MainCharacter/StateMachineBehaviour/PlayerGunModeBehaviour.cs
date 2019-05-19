using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefinitiveScript
{
    public class PlayerGunModeBehaviour : StateMachineBehaviour
    {
        private PlayerAnimatorController PlayerAnimatorController;
        private PlayerBehaviour PlayerBehaviour;
        private PlayerMoveController MoveController;
        private PlayerGunController GunController;

        private Vector3 verticalDirection;
        private Vector3 horizontalDirection;

        override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
        {
            //PlayerBehaviour = animator.GetComponent<PlayerBehaviour>();
            
            //PlayerBehaviour.ChangeToCamera("first");
        }

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            PlayerAnimatorController = animator.GetComponent<PlayerAnimatorController>();
            PlayerBehaviour = animator.GetComponent<PlayerBehaviour>();
            MoveController = animator.GetComponent<PlayerMoveController>();
            GunController = animator.GetComponent<PlayerGunController>();

            PlayerBehaviour.stopInput = stateInfo.IsName("ExitingGunMode");
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            GunController.gunPrepared = !stateInfo.IsName("PrepareGun") && !stateInfo.IsName("PreparingGunHit");
            GunController.gunPrepared = GunController.gunPrepared && !stateInfo.IsName("Shoot") && !stateInfo.IsName("ShootHit");

            if(GunController.gunPrepared)
            {
                verticalDirection = animator.transform.forward;
                horizontalDirection = animator.transform.right;

                Vector2 movementInput = PlayerBehaviour.movementInput;
                movementInput = movementInput.normalized;
                bool running = PlayerBehaviour.runningInput;
                bool shoot = PlayerBehaviour.shootInput;

                MoveController.Move(movementInput.y, movementInput.x, verticalDirection, horizontalDirection, running);

                Vector2 mouseInput = PlayerBehaviour.mouseInput;
                Vector2 mouseSensitivity = PlayerBehaviour.MouseControl.Sensitivity;
                
                MoveController.GunRotate(mouseInput, mouseSensitivity);

                PlayerAnimatorController.SetVerticalMovement(movementInput.y);
                PlayerAnimatorController.SetHorizontalMovement(movementInput.x);
                PlayerAnimatorController.SetRunning(running);

                PlayerAnimatorController.SetTurningLeft(mouseInput.x < -0.1f);
                PlayerAnimatorController.SetTurningRight(mouseInput.x > 0.1f);

                if(shoot && GunController.Shoot()) PlayerAnimatorController.Shoot();
            }
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            
        }

        override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
        {
            GunController.gunPrepared = false;
        }
    }
}
