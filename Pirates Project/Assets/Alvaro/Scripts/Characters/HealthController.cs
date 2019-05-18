using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DefinitiveScript 
{
    public class HealthController : MonoBehaviour
    {
        protected float health;
        protected float stamina;

        protected bool runOutOfStamina;
        protected bool usingStamina;

        public float initialHealth;
        public float initialStamina;

        public float recoveringStaminaSpeed = 1f;

        private ParticleSystem hitParticles;

        protected CharacterBehaviour CharacterBehaviour;
        protected SableController SableController;

        // Start is called before the first frame update
        protected void Start()
        {
            health = initialHealth;
            stamina = initialStamina;

            runOutOfStamina = false;
            usingStamina = false;

            hitParticles = GetComponentInChildren<ParticleSystem>();

            CharacterBehaviour = GetComponent<CharacterBehaviour>();
        }

        // Update is called once per frame
        protected void Update()
        {
            if(stamina < initialStamina && !usingStamina) RecoverStamina();
        }

        public virtual bool TakeDamage(float damage)
        {
            if(CharacterBehaviour.GetAlive())
            {
                health -= damage;

                if(health <= 0f)
                {    
                    CharacterBehaviour.SetAlive(false);
                    GetComponent<PlayerAnimatorController>().Die();
                }
                return health <= 0f;
            }
            return false;
        }

        public virtual void Knockback(float force, Vector3 direction, bool shot)
        {
            direction.y = 0f;
            Vector3 impact = direction.normalized * force;
            StartCoroutine(PlayKnockback(impact, 1.0f));
        }

        public void AttackedByGunParticles(Vector3 hitPoint)
        {
            hitParticles.transform.position = hitPoint;

            hitParticles.Play();
        }

        protected virtual IEnumerator PlayKnockback(Vector3 impact, float time) { yield return null; }

        public virtual bool ReduceStamina(float amount)
        {
            stamina -= amount;

            if(stamina <= 0f) 
            {
                stamina = 0f;
                runOutOfStamina = true;
            }
            return stamina <= 0f; //Devuelve true si el personaje ha perdido toda su stamina
        }

        protected virtual void RecoverStamina()
        {
            if(stamina < initialStamina)
            {
                stamina += recoveringStaminaSpeed * Time.deltaTime;
            }
            else
            {
                stamina = initialStamina;
                runOutOfStamina = false;
            }
        }

        public float GetCurrentHealth()
        {
            return health;
        }

        public float GetCurrentStamina()
        {
            return stamina;
        }

        public float GetTotalHealth()
        {
            return initialHealth;
        }

        public float GetTotalStamina()
        {
            return initialStamina;
        }

        public bool GetRunOutOfStamina()
        {
            return runOutOfStamina;
        }

        public void SetUsingStamina(bool value)
        {
            usingStamina = value;
        }
    }
}

