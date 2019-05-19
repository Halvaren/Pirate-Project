using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlayerDetection : MonoBehaviour
{
    [HideInInspector] public EnemyBehaviour enemyScript;
    [HideInInspector] public bool sphereCollider;

    void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>() != null;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player") {
            enemyScript.CharacterDetection(sphereCollider, !sphereCollider);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Player" && sphereCollider) {
            enemyScript.CharacterDetection(true, false);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player" && sphereCollider) {
            enemyScript.CharacterDetection(false, false);
        }
    }
}


