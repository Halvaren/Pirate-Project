using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace DefinitiveScript
{
    public class EnemySableController : SableController
    {
        public float minTimeBetweenAttacks;
        public float maxTimeBetweenAttacks;
        private float timeBetweenAttacks;

        public float distanceToAttack = 1f;

        private float timerBetweenAttacks = 0f;

        protected NavMeshAgent m_NavMeshAgent;
        public NavMeshAgent NavMeshAgent {
            get { 
                if(m_NavMeshAgent == null) m_NavMeshAgent = GetComponent<NavMeshAgent>();
                return m_NavMeshAgent;
            }
        }

        private EnemyBehaviour m_EnemyBehaviour;
        public EnemyBehaviour EnemyBehaviour {
            get {
                if(m_EnemyBehaviour == null) m_EnemyBehaviour = GetComponent<EnemyBehaviour>();
                return m_EnemyBehaviour;
            }
        }

        protected override IEnumerator Displacement(AnimationCurve speedCurve, float time)
        {
            float elapsedTime = 0.0f;

            while(elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;
                NavMeshAgent.Move(transform.forward * speedCurve.Evaluate(elapsedTime / time) * Time.deltaTime);
                yield return null;
            }
        }

        public bool AIAttack(bool stopBlock)
        {
            if(blocking)
            {
                Block(Random.Range(HealthController.GetCurrentStamina(), HealthController.GetTotalStamina()) > 50f);
                if(EnemyBehaviour.DistanceFromPlayer() > EnemyBehaviour.GetDetectionRadius() / 2 || stopBlock) Block(false);
            }

            if(timerBetweenAttacks == 0f)
            {
                if(Random.Range(0, HealthController.GetCurrentHealth()) < 25f)
                {
                    Block(!stopBlock);
                }
                else
                {
                    timeBetweenAttacks = Random.Range(minTimeBetweenAttacks, maxTimeBetweenAttacks);
                }
            }
            if(!blocking)
            {
                timerBetweenAttacks += Time.deltaTime;

                if(timerBetweenAttacks >= timeBetweenAttacks)
                {
                    ComboAttack();
                    timerBetweenAttacks = 0f;
                }

                return timerBetweenAttacks < timeBetweenAttacks;
            }
            return blocking;
        }

        public void ResetAIAttack()
        {
            timeBetweenAttacks = timerBetweenAttacks = 0f;
        }

        public override void StartAttack(int attackId, float time)
        {
            base.StartAttack(attackId, time);

            if(swordScript.hit) ComboAttack();
            else
            {
                if(Physics.Raycast(EnemyBehaviour.characterCenter.position, transform.forward, distanceToAttack, enemyLayerMask))
                    ComboAttack();
            }
        }

        public bool GetAttacking()
        {
            return attacking;
        }
    }
}

