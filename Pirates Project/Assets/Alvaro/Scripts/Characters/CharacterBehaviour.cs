using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefinitiveScript 
{
    public class CharacterBehaviour : MonoBehaviour
    {
        public Transform characterCenter;
        
        protected bool alive = true;

        public bool GetAlive()
        {
            return alive;
        }

        public void SetAlive(bool param)
        {
            alive = param; 
        }
    }
}

