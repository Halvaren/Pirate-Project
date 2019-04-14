using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCollisionDetector : MonoBehaviour
{
    public string TargetTag;

    private float damage;
    private Transform transform;

    void Awake()
    {
        transform = GetComponent<Transform>();
    }

    public void SetDamage(float param)
    {
        damage = param;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == TargetTag)
        {
            if(TargetTag == "Enemy")
            {
                other.GetComponent<DebugEnemyBehaviour>().AttackedBySword(damage, transform.forward);
            }
        }
    }
}
