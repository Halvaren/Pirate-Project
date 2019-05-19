using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace DefinitiveScript 
{
    public class EnemyHealthController : HealthController
    {
        protected NavMeshAgent m_NavMeshAgent;
        public NavMeshAgent NavMeshAgent
        {
            get {
                if(m_NavMeshAgent == null) m_NavMeshAgent = GetComponent<NavMeshAgent>();
                return m_NavMeshAgent;
            }
        }

        private EnemyUIController m_EnemyUIController;
        public EnemyUIController EnemyUIController
        {
            get {
                if(m_EnemyUIController == null) m_EnemyUIController = GetComponent<EnemyUIController>();
                return m_EnemyUIController;
            }
        }

        protected override IEnumerator PlayKnockback(Vector3 impact, float time)
        {
            float elapsedTime = 0.0f;

            while(elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;
                NavMeshAgent.Move(impact * Time.deltaTime);
                impact = Vector3.Lerp(impact, Vector3.zero, elapsedTime / time);
                yield return null;
            }
        }

        public override void Knockback(float force, Vector3 direction, bool shot)
        {
            base.Knockback(force, direction, shot);

            //GetComponent<EnemyBehaviour>().ReactToAttack(shot);
        }

        public override bool TakeDamage(float damage)
        {
            if(CharacterBehaviour.GetAlive()) {
                health -= damage;
                EnemyUIController.ChangeHealthBarValue(-damage);

                if(health <= 0f)
                {    
                    EnemyUIController.EnableUI(false);
                    GetComponent<CharacterBehaviour>().SetAlive(false);
                    GetComponent<EnemyBehaviour>().Die();
                }
                return health <= 0f;
            }
            return false;
        }
    }
}
