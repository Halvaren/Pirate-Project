using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefinitiveScript 
{
    public class SwordCollisionDetector : MonoBehaviour
    {
        public bool hit;
        public SableController SableController;

        void OnTriggerEnter(Collider other)
        {
            SableController.AddCollidedObject(other.gameObject);
            hit = true;
        }
    }
}

