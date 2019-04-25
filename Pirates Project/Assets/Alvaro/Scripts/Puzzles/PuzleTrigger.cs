using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefinitiveScript
{
    public class PuzleTrigger : MonoBehaviour
    {
        public VisualPuzle visualPuzle;
        public Puzle nextPuzle;

        void OnTriggerStay(Collider other)
        {
            Player player = other.GetComponent<Player>();
            if(other.gameObject.tag == "Player" && Input.GetKeyDown(KeyCode.Space) && !player.movementMode)
            {
                if(!visualPuzle.GetEndedPuzle())
                {  
                    visualPuzle.IntroducePuzle(player);
                }
                else if(!nextPuzle.GetEndedPuzle())
                {
                    nextPuzle.IntroducePuzle(player);
                }
            }
        }
    }
}
