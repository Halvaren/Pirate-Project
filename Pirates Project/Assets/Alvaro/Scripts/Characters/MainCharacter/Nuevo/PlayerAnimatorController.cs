using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefinitiveScript
{
    public class PlayerAnimatorController : MonoBehaviour
    {
        private Animator m_Animator;
        public Animator Animator {
            get {
                if(m_Animator == null) m_Animator = GetComponent<Animator>();
                return m_Animator;
            }
        }

        private PlayerSableController m_PlayerSableController;
        public PlayerSableController PlayerSableController {
            get {
                if(m_PlayerSableController == null) m_PlayerSableController = GetComponent<PlayerSableController>();
                return m_PlayerSableController;
            }
        }

        private HealthController m_HealthController;
        public HealthController HealthController {
            get {
                if(m_HealthController == null) m_HealthController = GetComponent<HealthController>();
                return m_HealthController;
            }
        }

        private float verticalMovement;
        private float horizontalMovement;

        private bool movement;
        private bool running;
        private bool attacking;
        private bool blocking;
        private bool turningLeft;
        private bool turningRight;
        private bool sableMode;
        private bool lockedTarget;

        void Update()
        {
            Animator.SetFloat("VerticalMovement", verticalMovement);
            Animator.SetFloat("HorizontalMovement", horizontalMovement);

            Animator.SetBool("Movement", movement);
            Animator.SetBool("Running", running);
            Animator.SetBool("Attacking", attacking);
            Animator.SetBool("Blocking", blocking);
            Animator.SetBool("SableMode", sableMode);
            Animator.SetBool("GunMode", !sableMode);
            Animator.SetBool("TurnLeft", turningLeft);
            Animator.SetBool("TurnRight", turningRight);
            Animator.SetBool("LockedTarget", lockedTarget);

            HealthController.SetUsingStamina((movement && running) || ((verticalMovement != 0f || horizontalMovement != 0f) && running) || blocking);
        }

        public void Shoot()
        {
            Animator.SetTrigger("Shoot");
        }

        public void Die()
        {
            Animator.SetTrigger("Die");
        }

        public void Disappear()
        {
            StartCoroutine(FadeOut(1.0f));
        }

        private IEnumerator FadeOut(float time)
        {
            float elapsedTime = 0.0f;
            Vector3 newPosition = transform.position;

            while(elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;

                newPosition.y -= Time.deltaTime;
                transform.position = newPosition;

                yield return null;
            }

            //Destroy(gameObject); //?
        }

        public void HitOnBody()
        {
            Animator.SetTrigger("HitOnBody");
        }

        public void HitOnSword()
        {
            Animator.SetTrigger("HitOnSword");
        }

        public void Disarm()
        {
            Animator.SetTrigger("SwordKnockback");
        }

        public void Attack()
        {
            Animator.SetTrigger("Attack");
        }

        public void ExitMode()
        {
            Animator.SetTrigger("ChangeMode");
        }

        public void ChangeMode()
        {
            if(!sableMode) ActiveSableMode();
            else ActiveGunMode();
        }
        

        public float GetVerticalMovement()
        {
            return verticalMovement;
        }

        public float GetHorizontalMovement()
        {
            return horizontalMovement;
        }

        public bool IsMoving()
        {
            return movement;
        }

        public bool IsRunning()
        {
            return running;
        }

        public bool IsAttacking()
        {
            return attacking;
        }

        public bool IsBlocking()
        {
            return blocking;
        }

        public bool IsTurningLeft()
        {
            return turningLeft;
        }

        public bool IsTurningRight()
        {
            return turningRight;
        }

        public bool IsSableMode()
        {
            return sableMode;
        }

        public bool IsGunMode()
        {
            return !sableMode;
        }

        public bool IsTargetLocked()
        {
            return lockedTarget;
        }


        public void SetVerticalMovement(float value)
        {
            verticalMovement = value;
        }

        public void SetHorizontalMovement(float value)
        {
            horizontalMovement = value;
        }

        public void SetMoving(bool value)
        {
            movement = value;
        }

        public void SetRunning(bool value)
        {
            running = value;
        }
        
        public void SetAttacking(bool value)
        {
            attacking = value;
        }

        public void SetBlocking(bool value)
        {
            blocking = value;
        }

        public void SetTurningLeft(bool value)
        {
            turningLeft = value;
        }

        public void SetTurningRight(bool value)
        {
            turningRight = value;
        }

        public void SetSableMode(bool value)
        {
            sableMode = value;
        }

        public void ActiveSableMode()
        {
            sableMode = true;
        }

        public void ActiveGunMode()
        {
            sableMode = false;
        }

        public void SetLockedTarget(bool value)
        {
            lockedTarget = value;
        }
    }
}

