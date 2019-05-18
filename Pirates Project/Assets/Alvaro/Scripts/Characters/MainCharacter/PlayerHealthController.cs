using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefinitiveScript 
{
    public class PlayerHealthController : HealthController
    {
        protected CharacterController m_CharacterController;
        public CharacterController CharacterController
        {
            get {
                if(m_CharacterController == null) m_CharacterController = GetComponent<CharacterController>();
                return m_CharacterController;
            }
        }

        public PlayerUIController PlayerUIController;

        protected override IEnumerator PlayKnockback(Vector3 impact, float time)
        {
            float elapsedTime = 0.0f;

            while(elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;
                CharacterController.Move(impact * Time.deltaTime);
                impact = Vector3.Lerp(impact, Vector3.zero, elapsedTime / time);
                yield return null;
            }
        }

        public override bool TakeDamage(float damage)
        {
            if(CharacterBehaviour.GetAlive())
            {
                health -= damage;

                if(health <= 0f)
                {    
                    CharacterBehaviour.SetAlive(false);
                    GetComponent<PlayerAnimatorController>().Die();
                }

                PlayerUIController.UpdateHealthBar(health <= 0f ? 0f : health);

                return health <= 0f;
            }
            return false;
        }

        public override bool ReduceStamina(float amount)
        {
            stamina -= amount;

            if(stamina <= 0f) 
            {
                stamina = 0f;
                runOutOfStamina = true;
            }

            PlayerUIController.UpdateStaminaBar(stamina <= 0f ? 0f: stamina);

            return stamina <= 0f; //Devuelve true si el personaje ha perdido toda su stamina
        }

        protected override void RecoverStamina()
        {
            base.RecoverStamina();

            PlayerUIController.UpdateStaminaBar(stamina);
        }
    }
}