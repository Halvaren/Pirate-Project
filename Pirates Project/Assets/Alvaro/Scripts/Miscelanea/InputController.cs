using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefinitiveScript
{
    public class InputController : MonoBehaviour
    {
        public float Vertical; //Guarda la información del eje Vertical
        public float Horizontal; //Guarda la información del eje Horizontal
        public Vector2 MouseInput; //Guarda la información del movimiento del ratón en X y en Y
        public bool ChangeMoveModeInput; //Guarda el valor respecto a la pulsación (inicial) del botón de cambio de modo de movimiento
        public bool RunningInput; //Guarda el valor respecto a la pulsación (manteniendo) del botón de correr
        public bool ShootingInput;
        public bool AttackInput;
        public bool BlockInput;
        public bool GrabInput;

        void Update()
        {
            Vertical = Input.GetAxis("Vertical");
            Horizontal = Input.GetAxis("Horizontal");
            MouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
            ChangeMoveModeInput = Input.GetButtonDown("ChangeMoveMode");
            RunningInput = Input.GetButton("Running");
            ShootingInput = Input.GetButtonDown("MouseLeftClick");
            AttackInput = Input.GetButtonDown("MouseLeftClick");
            BlockInput = Input.GetButton("MouseRightClick");
            GrabInput = Input.GetButton("MouseLeftClick");
        } 
    }
}