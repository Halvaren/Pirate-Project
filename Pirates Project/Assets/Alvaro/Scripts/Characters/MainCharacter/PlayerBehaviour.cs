using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace DefinitiveScript
{
    public class PlayerBehaviour : CharacterBehaviour
    {
        [System.Serializable]
        public class MouseInput
        {
            public Vector2 Damping; //Valor que permitirá hacer una gradación en el input recibido por el movimiento del ratón
            public Vector2 Sensitivity; //Valor que indica el nivel de sensibilidad del movimiento del ratón, es decir, como de grande será el movimiento de cámara en función del movimiento de ratón realizado
        }

        [SerializeField] private MouseInput m_MouseControl;
        public MouseInput MouseControl
        {
            get {
                return m_MouseControl;
            }
        }

        private InputController m_InputController; //Instancia del InputController
        public InputController InputController {
            get {
                if(m_InputController == null) m_InputController = GameManager.Instance.InputController;
                return m_InputController;
            }
        }

        private PlayerAnimatorController m_PlayerAnimatorController;
        public PlayerAnimatorController PlayerAnimatorController {
            get {
                if(m_PlayerAnimatorController == null) m_PlayerAnimatorController = GetComponent<PlayerAnimatorController>();
                return m_PlayerAnimatorController;
            }
        }

        private MoveController m_MoveController;
        public MoveController MoveController {
            get { 
                if(m_MoveController == null) m_MoveController = GetComponent<MoveController>();
                return m_MoveController;
            }
        }

        private HealthController m_HealthController;
        public HealthController HealthController {
            get {
                if(m_HealthController == null) m_HealthController = GetComponent<HealthController>();
                return m_HealthController;
            }
        }

        private Vector2 m_MouseInput; //Atributo donde se guardará los valores graduales del input del ratón hasta alcanzar el valor final
        public Vector2 mouseInput {
            get {
                return m_MouseInput;
            }
            set {
                m_MouseInput = value;
            }
        }

        private Vector2 m_MovementInput;
        public Vector2 movementInput {
            get {
                return m_MovementInput;
            }
            set {
                m_MovementInput = value;
            }
        }

        private bool m_RunningInput;
        public bool runningInput {
            get {
                return m_RunningInput;
            }
            set {
                m_RunningInput = value;
            }
        }

        private bool m_AttackInput;
        public bool attackInput {
            get {
                return m_AttackInput;
            }
            set {
                m_AttackInput = value;
            }
        }

        private bool m_ShootInput;
        public bool shootInput {
            get {
                return m_ShootInput;
            }
            set {
                m_ShootInput = value;
            }
        }

        private bool m_BlockInput;
        public bool blockInput {
            get {
                return m_BlockInput;
            }
            set {
                m_BlockInput = value;
            }
        }

        private bool m_SableMode; //Sable mode = true, Gun mode = false
        public bool sableMode {
            get {
                return m_SableMode;
            }
            set {
                m_SableMode = value;
            }
        }

        private bool m_StopMovement; //Permitirá para el movimiento en los casos necesarios (inutiliza el Update)
        public bool stopMovement {
            get {
                return m_StopMovement;
            }
            set {
                m_StopMovement = value;
            }
        }

        private bool m_StopInput; //Permitirá para el movimiento en los casos necesarios (inutiliza el Update)
        public bool stopInput {
            get {
                return m_StopInput;
            }
            set {
                m_StopInput = value;
            }
        }

        public GameObject model;
        public GameObject gunObject;
        public GameObject sableObject;

        public CinemachineVirtualCameraBase firstPersonCamera;
        public CinemachineVirtualCameraBase thirdPersonUnlockedTargetCamera;
        public CinemachineVirtualCameraBase thirdPersonLockedTargetCamera;

        void Start()
        {
            sableMode = true; //Se inicializa el modo de movimiento en modo sable
            PlayerAnimatorController.SetSableMode(sableMode);
            stopMovement = false;
            stopInput = false;

            ChangeWeapon();
        }     

        void Update()
        {
            if(alive)
            {
                if(!stopInput)
                {
                    if(InputController.ChangeMoveModeInput)
                    {
                        stopInput = true;
                        sableMode = !sableMode;

                        PlayerAnimatorController.ExitMode();
                        
                        MoveController.ResetXRotation();
                        ChangeWeapon();
                    }

                    if(!stopMovement)
                    {
                        runningInput = !HealthController.GetRunOutOfStamina() && InputController.RunningInput;

                        movementInput = new Vector2(InputController.Horizontal, InputController.Vertical);

                        float mouseInputX = Mathf.Lerp(mouseInput.x, InputController.MouseInput.x, 1f / MouseControl.Damping.x); //Calcula el valor gradual del movimiento de ratón en x para hacer un giro más natural
                        float mouseInputY = Mathf.Lerp(mouseInput.y, InputController.MouseInput.y, 1f / MouseControl.Damping.y);

                        mouseInput = new Vector2(mouseInputX, mouseInputY);
                    }
                    else
                    {
                        movementInput = Vector2.zero;
                        mouseInput = Vector2.zero;
                        runningInput = false;
                    }

                    attackInput = InputController.AttackInput;
                    shootInput = InputController.ShootingInput;
                    blockInput = InputController.BlockInput && !HealthController.GetRunOutOfStamina();
                }
                else
                {   
                    movementInput = Vector2.zero;
                    mouseInput = Vector2.zero;
                    runningInput = attackInput = shootInput = blockInput = false;
                }
            }
            else
            {
                movementInput = Vector2.zero;
                mouseInput = Vector2.zero;
                runningInput = attackInput = shootInput = blockInput = false;
            }
            /* 
            if(alive)
            {
                if(!stopInput)
                {
                    if(!stopMovement)
                    {
                        if(playerInput.Vertical == 0f && playerInput.Horizontal == 0f && playerInput.ChangeMoveModeInput)
                        {
                            MoveController.ResetXRotation();
                            movementMode = !movementMode; //Si se detecta la pulsación del botón de cambio de modo de movimiento, este será cambiado al otro modo
                            ChangeWeapon();
                        }

                        bool running = playerInput.RunningInput;

                        Vector3 verDir, horDir;
                        if(movementMode) //Si es modo pistola, las direcciones de movimiento serán las del personaje
                        {
                            verDir = transform.forward;
                            horDir = transform.right;
                        }
                        else //Si es modo sable las direcciones de movimiento corresponderán a la orientación de la cámara
                        {
                            verDir = CameraTransform.forward;
                            horDir = CameraTransform.right;
                        }
                                
                        MoveController.Move(playerInput.Vertical, playerInput.Horizontal, verDir, horDir, running); //Pasa el input, las direcciones de movimiento y si es correr o no

                        mouseInput.x = Mathf.Lerp(mouseInput.x, playerInput.MouseInput.x, 1f / MouseControl.Damping.x); //Calcula el valor gradual del movimiento de ratón en x para hacer un giro más natural
                        mouseInput.y = Mathf.Lerp(mouseInput.y, playerInput.MouseInput.y, 1f / MouseControl.Damping.y);

                        Vector3 targetDirection = playerInput.Vertical * verDir + playerInput.Horizontal * horDir; //Calcula la dirección objetivo a la que orientarse en Y que será util en el sable mode. Si no se está moviendo, será 0.

                        MoveController.YRotate(mouseInput.x, MouseControl.Sensitivity.x, targetDirection, movementMode); //Pasa el input del ratón, la sensibilidad para calcular el giro, la dirección objetivo y el modo de movimiento
                        //Si está en modo pistola, girará en función del input (gira el personaje y la cámara le sigue). Si está en modo sable, girará en función de la dirección objetivo (gira la cámara y el personaje le sigue si se está moviendo)

                        MoveController.XRotate(mouseInput.y, MouseControl.Sensitivity.y, movementMode);

                        running = running && (playerInput.Vertical != 0f || playerInput.Horizontal != 0f);

                        CharacterAnimationController.MovingAnimation(playerInput.Vertical, playerInput.Horizontal, playerInput.MouseInput.x, movementMode, running);
                    }

                    SableController.Block(playerInput.BlockInput);

                    if(!movementMode)
                    {
                        if(playerInput.AttackInput) SableController.ComboAttack();
                    }
                    
                    bool shot = playerInput.ShootingInput && GunController.Shoot();
                    GunController.gunPrepared = CharacterAnimationController.GunAnimation(movementMode, shot);
                }
                else
                {
                    CharacterAnimationController.BackToIdle();
                }
            }*/
        }

        void ChangeWeapon()
        {
            sableObject.SetActive(sableMode);
            gunObject.SetActive(!sableMode);
        }

        public void MakeVisible(bool param)
        {
            model.GetComponent<SkinnedMeshRenderer>().enabled = param;
            gunObject.GetComponentInChildren<MeshRenderer>().enabled = param;
            sableObject.GetComponentInChildren<MeshRenderer>().enabled = param;
        }

        public void ChangeToCamera(string code)
        {
            print(code);
            firstPersonCamera.m_Priority = 10;
            //thirdPersonLockedTargetCamera.m_Priority = 10;
            thirdPersonUnlockedTargetCamera.m_Priority = 10;

            switch(code)
            {
                case "first":
                    firstPersonCamera.m_Priority = 12;
                    break;
                case "thirdLocked":
                    //thirdPersonLockedTargetCamera.m_Priority = 12;
                    break;
                case "thirdUnlocked":
                    thirdPersonUnlockedTargetCamera.m_Priority = 12;
                    break;
            }
        }
    }
}