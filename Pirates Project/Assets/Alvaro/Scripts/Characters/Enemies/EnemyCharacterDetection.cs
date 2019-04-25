using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefinitiveScript 
{
    public class EnemyCharacterDetection : MonoBehaviour
    {
        [HideInInspector] public EnemyBehaviour enemyScript;

        void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.tag == "Player") {
                enemyScript.CharacterDetection(true);
            }
        }

        void OnTriggerStay(Collider other)
        {
            if(other.gameObject.tag == "Player") {
                enemyScript.CharacterDetection(true);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if(other.gameObject.tag == "Player") {
                enemyScript.CharacterDetection(false);
            }
        }
    }
}

