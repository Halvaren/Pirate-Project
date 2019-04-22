using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefinitiveScript
{
    public class EnemyBehaviour : MonoBehaviour
    {
        private SableController SableController;

        void Awake()
        {
            SableController = GetComponent<SableController>();
        }

        void Update()
        {
            SableController.Block(true);
        }

        /*public Material InitialMaterial;
        public Material KnockbackMaterial;
        public float KnockbackTime = 0.5f;
        //public float knockbackForce = 10f;
        private bool knockback;

        public float InitialHealth = 100f;
        private float health;

        private MeshRenderer meshRenderer;
        private Rigidbody rigidbody;
        private ParticleSystem hitParticles;

        // Start is called before the first frame update
        void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            hitParticles = GetComponentInChildren<ParticleSystem>();
            InitialMaterial = meshRenderer.material;

            rigidbody = GetComponent<Rigidbody>();
        }

        void Start()
        {
            health = InitialHealth;
            knockback = false;
        }

        public void AttackedBySword(float damage, Vector3 direction)
        {
            if(!knockback)
            {
                health -= damage;
                if(health <= 0)
                {
                    Destroy(gameObject);
                }
                else
                {
                    StartCoroutine(Knockback(KnockbackTime, direction, damage/4));
                }
            }
            
        }

        public void AttackedByGun(float damage, Vector3 direction, Vector3 hitPoint)
        {
            if(!knockback)
            {
                hitParticles.transform.position = hitPoint;

                hitParticles.Play();

                health -= damage;
                if(health <= 0)
                {
                    Destroy(gameObject);
                }
                else
                {
                    StartCoroutine(Knockback(KnockbackTime, direction, damage/4));
                }
            }
            
        }

        IEnumerator Knockback(float time, Vector3 direction, float knockbackForce)
        {
            meshRenderer.material = KnockbackMaterial;
            knockback = true;

            rigidbody.AddForce(direction.normalized * knockbackForce, ForceMode.Impulse);

            yield return new WaitForSeconds(time);

            meshRenderer.material = InitialMaterial;
            knockback = false;
        }*/
    }
}