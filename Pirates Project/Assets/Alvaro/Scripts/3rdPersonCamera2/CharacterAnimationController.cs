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

        public void Walk(bool value)
        {
            anim.SetBool("IsWalking", value);
        }

        public void Run(bool value)
        {
            anim.SetBool("IsRunning", value);
        }

        public void SideMoving(bool value)
        {
            anim.SetBool("IsSideMoving", value);
        }

        public void BackwardMoving(bool value)
        {
            anim.SetBool("IsBackwardMoving", value);
        }

        public void MovingAnimation(float verticalInput, float horizontalInput, bool movementMode, bool running)
        {
            if(movementMode)
            {
                bool frontWalking = verticalInput < 0f;
            }
            else
            {
                bool walking = verticalInput != 0f || horizontalInput != 0f;
                
                if(running) Run(walking);
                else Walk(walking);
            }
        }
    }
}

