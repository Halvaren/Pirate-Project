using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefinitiveScript 
{
    public class HealthController : MonoBehaviour
    {
        protected float health;
        protected float stamina;

        public float initialHealth;
        public float initialStamina;

        // Start is called before the first frame update
        protected void Start()
        {
            health = initialHealth;
            stamina = initialStamina;
        }

        // Update is called once per frame
        protected void Update()
        {
            
        }

        public bool TakeDamage(float damage)
        {
            health -= damage;
            return health <= 0f; //Devuelve true si el personaje ha muerto
        }

        public void Knockback(float force, Vector3 direction)
        {
            direction.y = 0f;
            Vector3 impact = direction.normalized * force;
            StartCoroutine(PlayKnockback(impact, 1.0f));
        }

        protected virtual IEnumerator PlayKnockback(Vector3 impact, float time) { yield return null; }

        public bool ReduceStamina(float amount)
        {
            stamina -= amount;
            return stamina <= 0f; //Devuelve true si el personaje ha muerto
        }
    }
}

