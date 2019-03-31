using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefinitiveScript
{
    public class CharacterAnimationController : MonoBehaviour
    {
        private Animator anim;

        private Player m_LocalPlayer;
        public Player LocalPlayer {
            get {
                if(m_LocalPlayer == null) m_LocalPlayer = GameManager.Instance.LocalPlayer;
                return m_LocalPlayer;
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

        //Métodos que son llamados desde fuera

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

        public bool GunAnimation(bool aiming, bool shoot)
        {
            Aim(aiming);
            AnimatorStateInfo currentState = anim.GetCurrentAnimatorStateInfo(0);
            if(aiming && currentState.IsName("AimingIdle") && shoot)
            {
                Shoot();
                return true;
            }
            return false;
        }

        //Métodos de eventos de animaciones

        public void StartAttackEvent(int attackId)
        {
            LocalPlayer.CloseCombat.StartAttack(attackId, anim.GetNextAnimatorClipInfo(0)[0].clip.length);
        }

        public void EnableSwordColliderEvent()
        {
            LocalPlayer.CloseCombat.EnableSwordCollider();
        }

        public void DisableSwordColliderEvent()
        {
            LocalPlayer.CloseCombat.DisableSwordCollider();
        }

        public void FinishAttackEvent(int attackId)
        {
            LocalPlayer.CloseCombat.FinishAttack(attackId);
        }
    }
}

