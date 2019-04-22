using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefinitiveScript 
{
    public class HealthController : MonoBehaviour
    {
        private float health;
        private float stamina;

        public float initialHealth;
        public float initialStamina;

        private CharacterController m_CharacterController;
        public CharacterController CharacterController
        {
            get {
                if(m_CharacterController == null) m_CharacterController = GetComponent<CharacterController>();
                return m_CharacterController;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            health = initialHealth;
            stamina = initialStamina;
        }

        // Update is called once per frame
        void Update()
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

        private IEnumerator PlayKnockback(Vector3 impact, float time)
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

        public bool ReduceStamina(float amount)
        {
            stamina -= amount;
            return stamina <= 0f; //Devuelve true si el personaje ha muerto
        }
    }
}

