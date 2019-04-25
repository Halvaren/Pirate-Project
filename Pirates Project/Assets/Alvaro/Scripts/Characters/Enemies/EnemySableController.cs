using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace DefinitiveScript
{
    public class EnemySableController : SableController
    {
        protected NavMeshAgent m_NavMeshAgent;
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
    }
}

