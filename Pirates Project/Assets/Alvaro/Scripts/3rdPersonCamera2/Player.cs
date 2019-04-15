using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefinitiveScript
{
    [RequireComponent(typeof(MoveController))] //Es necesario que el GameObject que tiene asociado este script, también tenga el script MoveController
    [RequireComponent(typeof(CharacterAnimationController))]
    [RequireComponent(typeof(CloseCombat))]
    [RequireComponent(typeof(Shooting))]
    public class Player : MonoBehaviour
    {
        [System.Serializable]
        public class MouseInput
        {
            public Vector2 Damping; //Valor que permitirá hacer una gradación en el input recibido por el movimiento del ratón
            public Vector2 Sensitivity; //Valor que indica el nivel de sensibilidad del movimiento del ratón, es decir, como de grande será el movimiento de cámara en función del movimiento de ratón realizado
        }

        [SerializeField] MouseInput MouseControl;

        private MoveController m_MoveController; //Instancia del MoveController
        public MoveController MoveController
        {
            get{
                if(m_MoveController == null)
                {
                    m_MoveController = GetComponent<MoveController>();
                }
                return m_MoveController;
            }
        }

        private CharacterAnimationController m_CharacterAnimationController;
        public CharacterAnimationController CharacterAnimationController
        {
            get {
                if(m_CharacterAnimationController == null)
                {
                    m_CharacterAnimationController = GetComponent<CharacterAnimationController>();
                }
                return m_CharacterAnimationController;
            }
        }

        private CloseCombat m_CloseCombat;
        public CloseCombat CloseCombat
        {
            get {
                if(m_CloseCombat == null)
                {
                    m_CloseCombat = GetComponent<CloseCombat>();
                }
                return m_CloseCombat;
            }
        }

        private Shooting m_Shooting;
        public Shooting Shooting
        {
            get {
                if(m_Shooting == null)
                {
                    m_Shooting = GetComponent<Shooting>();
                }
                return m_Shooting;
            }
        }

        InputController playerInput; //Instancia del InputController
        Vector2 mouseInput; //Atributo donde se guardará los valores graduales del input del ratón hasta alcanzar el valor final

        private Transform m_CameraTransform; //Transform de la cámara
        private ThirdPersonCamera cameraScript; //Instancia del script que controla la cámara
        public Transform CameraTransform
        {
            get{
                return m_CameraTransform;
            }
            set {
                m_CameraTransform = value;
                cameraScript = m_CameraTransform.GetComponent<ThirdPersonCamera>();
            }
        }

        private bool m_MovementMode; //Sable mode = false, Gun mode = true
        public bool movementMode
        {
            get { return m_MovementMode; }
            set { m_MovementMode = value; }
        }

        private bool m_StopMovement; //Permitirá para el movimiento en los casos necesarios (inutiliza el Update)
        public bool stopMovement
        {
            get
            {
                return m_StopMovement;
            }
            set
            {
                m_StopMovement = value;
            }
        }

        private bool m_StopInput; //Permitirá para el movimiento en los casos necesarios (inutiliza el Update)
        public bool stopInput
        {
            get
            {
                return m_StopInput;
            }
            set
            {
                Camera.main.GetComponent<ThirdPersonCamera>().SetInitialized(!value);
                m_StopInput = value;
            }
        }

        public GameObject model;
        public GameObject gunObject;
        public GameObject sableObject;

        void Awake()
        {
            GameManager.Instance.LocalPlayer = this;
            playerInput = GameManager.Instance.InputController;
        }  

        void Start()
        {
            movementMode = cameraScript.movementMode = false; //Se inicializa el modo de movimiento en modo sable
            stopMovement = true;

            ChangeWeapon();
        }     

        void Update()
        {
            if(!stopInput)
            {
                if(!stopMovement)
                {
                    if(playerInput.ChangeMoveModeInput)
                    {
                        movementMode = cameraScript.movementMode = !movementMode; //Si se detecta la pulsación del botón de cambio de modo de movimiento, este será cambiado al otro modo
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
                            
                    MoveController.Move(playerInput.Vertical, playerInput.Horizontal, verDir, horDir, running && !movementMode); //Pasa el input, las direcciones de movimiento y si es correr o no

                    mouseInput.x = Mathf.Lerp(mouseInput.x, playerInput.MouseInput.x, 1f / MouseControl.Damping.x); //Calcula el valor gradual del movimiento de ratón en x para hacer un giro más natural

                    Vector3 targetDirection = playerInput.Vertical * verDir + playerInput.Horizontal * horDir; //Calcula la dirección objetivo a la que orientarse en Y que será util en el sable mode. Si no se está moviendo, será 0.

                    MoveController.Rotate(mouseInput.x, MouseControl.Sensitivity.x, targetDirection, movementMode); //Pasa el input del ratón, la sensibilidad para calcular el giro, la dirección objetivo y el modo de movimiento
                    //Si está en modo pistola, girará en función del input (gira el personaje y la cámara le sigue). Si está en modo sable, girará en función de la dirección objetivo (gira la cámara y el personaje le sigue si se está moviendo)

                    running = running && (playerInput.Vertical != 0f || playerInput.Horizontal != 0f);

                    CharacterAnimationController.MovingAnimation(playerInput.Vertical, playerInput.Horizontal, playerInput.MouseInput.x, movementMode, running);
                }

                if(!movementMode && playerInput.AttackInput) CloseCombat.ComboAttack();
                
                bool shot = playerInput.ShootingInput && Shooting.Shoot();
                Shooting.gunPrepared = CharacterAnimationController.GunAnimation(movementMode, shot);
            }
            else
            {
                CharacterAnimationController.BackToIdle();
            }
        }

        void ChangeWeapon()
        {
            sableObject.SetActive(!movementMode);
            gunObject.SetActive(movementMode);
        }

        public void MakeVisible(bool param)
        {
            model.GetComponent<SkinnedMeshRenderer>().enabled = param;
            gunObject.GetComponentInChildren<MeshRenderer>().enabled = param;
            sableObject.GetComponentInChildren<MeshRenderer>().enabled = param;
        }
    }
}