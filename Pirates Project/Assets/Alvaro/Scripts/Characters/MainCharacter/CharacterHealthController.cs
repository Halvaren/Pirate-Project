using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefinitiveScript 
{
    public class CharacterHealthController : HealthController
    {
        protected CharacterController m_CharacterController;
        public CharacterController CharacterController
        {
            get {
                if(m_CharacterController == null) m_CharacterController = GetComponent<CharacterController>();
                return m_CharacterController;
            }
        }

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
    }
}