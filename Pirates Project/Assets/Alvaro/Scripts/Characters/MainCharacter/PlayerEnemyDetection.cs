using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefinitiveScript
{
    public class PlayerEnemyDetection : MonoBehaviour
    {
        public PlayerLockTargetController playerScript;

        void Start() {
            playerScript.distanceToDeactivateTargeting = GetComponent<SphereCollider>().radius;
        }

        void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.tag == "Enemy") {
                playerScript.AddEnemy(other.GetComponent<EnemyBehaviour>());
            }
        }

        void OnTriggerExit(Collider other)
        {
            if(other.gameObject.tag == "Enemy") {
                playerScript.RemoveEnemy(other.GetComponent<EnemyBehaviour>());
            }
        }
    }
}

