using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace DefinitiveScript
{
    public class EnemySableController : SableController
    {
        private EnemyBehaviour m_EnemyBehaviour;
        public EnemyBehaviour EnemyBehaviour {
            get {
                if(m_EnemyBehaviour == null) m_EnemyBehaviour = GetComponent<EnemyBehaviour>();
                return m_EnemyBehaviour;
            }
        }

        private NavMeshAgent m_NavMeshAgent;
        public NavMeshAgent NavMeshAgent {
            get {
                if(m_NavMeshAgent == null) m_NavMeshAgent = GetComponent<NavMeshAgent>();
                return m_NavMeshAgent;
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

        public void SetAttacking(bool value)
        {
            attacking = value;
        }

        public void SetBlocking(bool value)
        {
            blocking = value;
        }

        public override void HitOnSword(Vector3 hitDirection)
        {
            if(blocking)
            {
                if(HealthController.ReduceStamina(10f))
                {
                    EnemyBehaviour.Disarm();
                    HealthController.Knockback(5f, hitDirection, false);
                    blocking = false;
                    EnemyBehaviour.SetBlocking(false);
                    EnemyBehaviour.SetStaring(true);
                }
                else
                {
                    EnemyBehaviour.HitOnSword();
                    HealthController.Knockback(5f, hitDirection, false);
                }
            }
            else if(attacking)
            {
                HealthController.ReduceStamina(10f);
                EnemyBehaviour.Disarm();
                HealthController.Knockback(5f, hitDirection, false);

                CancelAttack();
            }
        }

        public override void HitOnBody(Vector3 hitDirection)
        {
            CancelAttack();

            HealthController.Knockback(2.5f, hitDirection, false);

            if(HealthController.TakeDamage(Damage))
            {
                print("sa morío");
            }
            else
            {
                EnemyBehaviour.HitOnBody();
            }
        }

        public override void ComboAttack()
        {
            if(chaining && comboCount < 3)
            {
                chaining = false;
                nextAttack = true;

                attacking = true;
                EnemyBehaviour.SetAttacking(true);

                if(comboCount == 0) EnemyBehaviour.Attack();
                comboCount++;
            }
        }

        public override void FinishAttack(int attackId)
        {
            if(nextAttack)
            {
                EnemyBehaviour.Attack();
            }
            else
            {
                CancelAttack();
            }
        }

        protected override void CancelAttack()
        {
            chaining = true;
            nextAttack = false;

            DisableSwordCollider();
            comboCount = 0;

            attacking = false;
            EnemyBehaviour.SetAttacking(false);
            EnemyBehaviour.SetStaring(true);
        }

        public bool GetChaining()
        {
            return chaining && comboCount != 0;
        }

        public bool GetHit()
        {
            return swordScript.hit;
        }
    }
}

