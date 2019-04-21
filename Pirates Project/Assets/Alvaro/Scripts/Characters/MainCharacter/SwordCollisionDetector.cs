using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefinitiveScript 
{
    public class SwordCollisionDetector : MonoBehaviour
    {
        public string TargetTag;

        private float damage;
        private Transform transform;

        public SableController SableController;

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
            SableController.AddCollidedObject(other.gameObject);
        }
    }
}

