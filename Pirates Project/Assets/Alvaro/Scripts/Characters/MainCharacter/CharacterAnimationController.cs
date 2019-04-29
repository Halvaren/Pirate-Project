using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefinitiveScript
{
    public class CharacterAnimationController : MonoBehaviour
    {
        private Animator anim;

        private SableController m_SableController;
        public SableController SableController
        {
            get {
                if(m_SableController == null) m_SableController = GetComponent<SableController>();
                return m_SableController;
            }
        }

        // Start is called before the first frame update
        void Awake()
        {
            anim = GetComponent<Animator>();
        }

        //Métodos que interaccionan con el Animator

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

        public void Hit()
        {
            anim.SetTrigger("Hit");
        }

        public void Block(bool value)
        {
            anim.SetBool("Block", value);
        }

        public void HitOnSword()
        {
            anim.SetTrigger("HitOnSword");
        }

        public void Disarm()
        {
            anim.SetTrigger("Disarmed");
        }

        //Métodos que son llamados desde fuera

        public void MovingAnimation(float verticalInput, float horizontalInput, float mouseInput, bool movementMode, bool running = false)
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

        public void MovingAnimation(bool move, bool running)
        {
            MovingAnimation(move ? 1f : 0f, move ? 1f : 0f, 0f, false, move ? running : move);
        }

        public void MovingAnimation(float velocity, bool running)
        {
            MovingAnimation(velocity, velocity, 0f, false, velocity > 0f ? running : false);
        }

        public bool GunAnimation(bool aiming, bool shoot)
        {
            Aim(aiming);
            AnimatorStateInfo currentState = anim.GetCurrentAnimatorStateInfo(0);
            if(aiming && currentState.IsName("AimingIdle"))
            {
                if(shoot) Shoot();
                return true;
            }
            return false;
        }

        //Métodos de eventos de animaciones

        public void StartAttackEvent(int attackId)
        {
            SableController.StartAttack(attackId, anim.GetNextAnimatorClipInfo(0)[0].clip.length);
        }

        public void EnableSwordColliderEvent()
        {
            SableController.EnableSwordCollider();
        }

        public void DisableSwordColliderEvent()
        {
            SableController.DisableSwordCollider();
        }

        public void FinishAttackEvent(int attackId)
        {
            SableController.FinishAttack(attackId);
        }
    }
}

