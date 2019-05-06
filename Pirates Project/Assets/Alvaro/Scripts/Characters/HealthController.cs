using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefinitiveScript 
{
    public class HealthController : MonoBehaviour
    {
        protected float health;
        protected float stamina;

        protected bool runOutOfStamina;

        public float initialHealth;
        public float initialStamina;

        public float recoveringStaminaSpeed = 0.5f;

        private ParticleSystem hitParticles;

        // Start is called before the first frame update
        protected void Start()
        {
            health = initialHealth;
            stamina = initialStamina;

            runOutOfStamina = false;

            hitParticles = GetComponentInChildren<ParticleSystem>();
        }

        // Update is called once per frame
        protected void Update()
        {
            if(runOutOfStamina) RecoverStamina();
        }

        public bool TakeDamage(float damage)
        {
            health -= damage;

            if(health <= 0f)
            {    
                GetComponent<CharacterBehaviour>().SetAlive(false);
                GetComponent<CharacterAnimationController>().Die();
            }

            return health <= 0f; //Devuelve true si el personaje ha muerto
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

        public bool ReduceStamina(float amount)
        {
            stamina -= amount;

            if(stamina <= 0f) 
            {
                stamina = 0f;
                runOutOfStamina = true;
            }
            return stamina <= 0f; //Devuelve true si el personaje ha muerto
        }

        protected void RecoverStamina()
        {
            if(stamina < initialStamina)
            {
                stamina += recoveringStaminaSpeed * Time.deltaTime;
            }
            stamina = initialStamina;
            runOutOfStamina = false;
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
    }
}

