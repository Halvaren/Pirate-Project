using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefinitiveScript
{
    public class CharacterSableController : SableController
    {
        private CharacterController m_CharacterController;
        public CharacterController CharacterController
        {
            get {
                if(m_CharacterController == null) m_CharacterController = GetComponent<CharacterController>();
                return m_CharacterController;
            }
        }

        protected override IEnumerator Displacement(AnimationCurve speedCurve, float time)
        {
            GameManager.Instance.LocalPlayer.stopMovement = true;

            float elapsedTime = 0.0f;

            while(elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;
                CharacterController.Move(transform.forward * speedCurve.Evaluate(elapsedTime / time) * Time.deltaTime);
                yield return null;
            }

            GameManager.Instance.LocalPlayer.stopMovement = false;
        }

        public override void Block(bool input)
        {
            base.Block(input);

            GameManager.Instance.LocalPlayer.stopMovement = input;
        }
    }
}