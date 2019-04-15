using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefinitiveScript
{
    public class CharacterAnimationController : MonoBehaviour
    {
        private Animator anim;

        // Start is called before the first frame update
        void Awake()
        {
            anim = GetComponent<Animator>();
        }

        public void VerticalAxisWalk(float verticalInput)
        {
            anim.SetFloat("VerticalInput", verticalInput);
        }

        public void HorizontalAxisWalk(float horizontalInput)
        {
            anim.SetFloat("HorizontalInput", horizontalInput);
        }

        public void Run(bool value)
        {
            anim.SetBool("IsRunning", value);
        }

        public void Turn(float mouseInput)
        {
            anim.SetFloat("MouseInputX", mouseInput);
        }

<<<<<<< HEAD
        public void Aim(bool value)
        {
            anim.SetBool("IsAiming", value);
        }

        public void Shoot()
        {
            anim.SetTrigger("Shoot");
        }

        public void Attack()
        {
            anim.SetTrigger("Attack");
        }

        public void StopAttack()
        {
            anim.SetTrigger("StopAttack");
        }

        public void BackToIdle()
        {
            anim.Play("Idle");
        }

        //Métodos que son llamados desde fuera

=======
>>>>>>> d4c2d9e58e6c386123d43a2a89221c1c7057c5a7
        public void MovingAnimation(float verticalInput, float horizontalInput, float mouseInput, bool movementMode, bool running)
        {
            if(movementMode)
            {
                VerticalAxisWalk(verticalInput);
                HorizontalAxisWalk(horizontalInput);
                Turn(mouseInput);
            }
            else
            {
                VerticalAxisWalk(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));
                Run(running);
            }
        }
    }
}

