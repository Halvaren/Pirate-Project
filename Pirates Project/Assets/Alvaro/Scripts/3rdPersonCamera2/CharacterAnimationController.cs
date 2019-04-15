using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefinitiveScript
{
    public class CharacterAnimationController : MonoBehaviour
    {
        private Animator anim;

        // Start is called before the first frame update
        void Awake()
        {
            anim = GetComponent<Animator>();
        }

        public void VerticalAxisWalk(float verticalInput)
        {
            anim.SetFloat("VerticalInput", verticalInput);
        }

        public void HorizontalAxisWalk(float horizontalInput)
        {
            anim.SetFloat("HorizontalInput", horizontalInput);
        }

        public void Run(bool value)
        {
            anim.SetBool("IsRunning", value);
        }

        public void Turn(float mouseInput)
        {
            anim.SetFloat("MouseInputX", mouseInput);
        }

        public void MovingAnimation(float verticalInput, float horizontalInput, float mouseInput, bool movementMode, bool running)
        {
            if(movementMode)
            {
                VerticalAxisWalk(verticalInput);
                HorizontalAxisWalk(horizontalInput);
                Turn(mouseInput);
            }
            else
            {
                VerticalAxisWalk(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));
                Run(running);
            }
        }
    }
}

