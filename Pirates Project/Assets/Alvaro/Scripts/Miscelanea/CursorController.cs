using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefinitiveScript {

    public class CursorController : MonoBehaviour
    {
        public void LockCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void UnlockCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public bool LockedCursor()
        {
            return Cursor.lockState == CursorLockMode.Locked;
        }
    }
}

