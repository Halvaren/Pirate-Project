using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefinitiveScript {

    public class CursorController : MonoBehaviour
    {
        public void LockCursor(){
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void UnlockCursor()
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}

