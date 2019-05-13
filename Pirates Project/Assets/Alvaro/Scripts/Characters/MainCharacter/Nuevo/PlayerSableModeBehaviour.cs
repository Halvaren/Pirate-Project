using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefinitiveScript 
{
    public class PlayerSableModeBehaviour : StateMachineBehaviour
    {
        private PlayerAnimatorController PlayerAnimatorController;
        private PlayerBehaviour PlayerBehaviour;
        private MoveController MoveController;
        private PlayerSableController SableController;
        private Transform CameraTransform;

        private Vector3 verticalDirection;
        private Vector3 horizontalDirection;

        override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
        {
            //PlayerBehaviour = animator.GetComponent<PlayerBehaviour>();
            //PlayerBehaviour.ChangeToCamera("thirdUnlocked");
        }

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            PlayerBehaviour = animator.GetComponent<PlayerBehaviour>();
            PlayerAnimatorController = animator.GetComponent<PlayerAnimatorController>();
            MoveController = animator.GetComponent<MoveController>();
            SableController = animator.GetComponent<PlayerSableController>();

            CameraTransform = Camera.main.transform;

            PlayerBehaviour.stopInput = stateInfo.IsName("ExitingSableMode");
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            bool attack = PlayerBehaviour.attackInput && !PlayerAnimatorController.IsBlocking();
            bool block = PlayerBehaviour.blockInput && !PlayerAnimatorController.IsAttacking();

            if(!PlayerAnimatorController.IsBlocking() && !PlayerAnimatorController.IsAttacking())
            {
                verticalDirection = CameraTransform.forward;
                horizontalDirection = CameraTransform.right;

                Vector2 movementInput = PlayerBehaviour.movementInput;
                bool running = PlayerBehaviour.runningInput;

                MoveController.Move(movementInput.y, movementInput.x, verticalDirection, horizontalDirection, running);

                Vector3 targetDirection = movementInput.y * verticalDirection + movementInput.x * horizontalDirection;
                Vector2 mouseInput = PlayerBehaviour.mouseInput;

                MoveController.SableRotate(targetDirection);

                PlayerAnimatorController.SetVerticalMovement(movementInput.y);
                PlayerAnimatorController.SetHorizontalMovement(movementInput.x);
                
                PlayerAnimatorController.SetMoving(movementInput.x != 0f || movementInput.y != 0f);
                PlayerAnimatorController.SetRunning(running);

                PlayerAnimatorController.SetTurningLeft(mouseInput.x < -0.1f);
                PlayerAnimatorController.SetTurningRight(mouseInput.x > 0.1f);
            }
            
            if(attack) SableController.ComboAttack();
            SableController.SetBlocking(block);
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            
        }

        override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
        {
            
        }
    }
}