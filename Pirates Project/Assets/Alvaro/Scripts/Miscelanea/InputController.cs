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
        public bool LockTargetInput;
        public bool RunningInput; //Guarda el valor respecto a la pulsación (manteniendo) del botón de correr
        public bool ShootingInput;
        public bool AttackInput;
        public bool BlockInput;
        public bool GrabInput;

        public bool IncreaseNumber;
        public bool DecreaseNumber;
        public bool ChangeSelectedNumberRight;
        public bool ChangeSelectedNumberLeft;
        public bool CheckNumbers;
        public bool ExitFromPuzle;

        void Update()
        {
            Vertical = Input.GetAxis("Vertical");
            Horizontal = Input.GetAxis("Horizontal");
            MouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
            ChangeMoveModeInput = Input.GetButtonDown("ChangeMoveMode");
            LockTargetInput = Input.GetButtonDown("MouseMiddleClick");
            RunningInput = Input.GetButton("Running");
            ShootingInput = Input.GetButtonDown("MouseLeftClick");
            AttackInput = Input.GetButtonDown("MouseLeftClick") || Input.GetKeyDown(KeyCode.F);
            BlockInput = Input.GetButton("MouseRightClick");
            GrabInput = Input.GetButton("MouseLeftClick");

            IncreaseNumber = Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);
            DecreaseNumber = Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow);
            ChangeSelectedNumberRight = Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow);
            ChangeSelectedNumberLeft = Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow);
            CheckNumbers = Input.GetKeyDown(KeyCode.Return);
            ExitFromPuzle = Input.GetKeyDown(KeyCode.Z);
        } 
    }
}