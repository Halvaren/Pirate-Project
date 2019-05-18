using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefinitiveScript
{
    public class PlayerMoveController : MonoBehaviour
    {
        public float movingSpeed = 5.0f; //Velocidad de movimiento normal
        public float runningSpeed = 10.0f; //Velocidad de movimiento corriendo
        public float rotationSpeed = 20.0f; //Velocidad de giro
        public float gravity = 20.0f; //Gravedad que afecta al personaje
        public float limitXAngle;

        public float reducingStaminaSpeed = 5f;

        private CharacterController controller; //Instancia del CharacterController
        private HealthController health;

        void Awake()
        {
            controller = GetComponent<CharacterController>();
            health = GetComponent<HealthController>();
        }

        public void Move(float verInput, float horInput, Vector3 verDir, Vector3 horDir, bool running)
        {
            if(running)
            {
                print("hola");
                running = !health.ReduceStamina(reducingStaminaSpeed * Time.deltaTime);
            }

            float speed = running ? runningSpeed : movingSpeed; //Se decide que velocidad emplear
            Vector2 movement = new Vector2 (verInput * speed, horInput * speed); //Se calcula la cantidad de movimiento
            Vector3 newPosition = verDir * movement.x * Time.deltaTime 
                                + horDir * movement.y * Time.deltaTime; //Se determina la nueva posición en función de la dirección de movimiento, la cantidad de movimiento y del tiempo transcurrido

            newPosition.y = newPosition.y - (gravity * Time.deltaTime); //En la componente Y se ve afectado por la gravedad

            controller.Move(newPosition);
        }

        /*public void YRotate(float mouseInput, float mouseSensitivity, Vector3 targetDirection, bool movementMode)
        {
            if(movementMode) //Gun mode
            {
                transform.Rotate(Vector3.up * mouseInput * mouseSensitivity, Space.World); //Si se está en modo pistola, gira sobre el eje Y en función del input recibido
            }
            else //Sable mode
            {
                Vector3 lookDirection = targetDirection.normalized; //Si se está en modo sable, se utiliza la dirección objetivo para girar al personaje hacia esa dirección
                if(lookDirection != Vector3.zero)
                {
                    Quaternion newRotation = Quaternion.LookRotation(lookDirection, transform.up);
                    float differenceRotation = newRotation.eulerAngles.y - transform.eulerAngles.y;
                    float eulerY = newRotation.eulerAngles.y;
                    
                    Vector3 euler = new Vector3(transform.eulerAngles.x, eulerY, transform.eulerAngles.z);
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(euler), rotationSpeed * Time.deltaTime); //Se realiza un giro interpolado
                }
            }
        }*/

        public void GunRotate(Vector2 mouseInput, Vector2 mouseSensitivity)
        {
            transform.Rotate(Vector3.up * mouseInput.x * mouseSensitivity.x, Space.World);        

            transform.Rotate(Vector3.right * -mouseInput.y * mouseSensitivity.y);

            Vector3 eulerAngles = transform.localEulerAngles;
            eulerAngles.z = 0f;

            eulerAngles.x = (eulerAngles.x > 180) ? eulerAngles.x - 360 : eulerAngles.x;

            if(eulerAngles.x > limitXAngle)
            {
                eulerAngles.x = limitXAngle;
            } 
            else if(eulerAngles.x < -limitXAngle)
            {
                eulerAngles.x = -limitXAngle;
            }

            transform.localEulerAngles = eulerAngles;
        }

        public void SableRotate(Vector3 targetDirection)
        {
            Vector3 lookDirection = targetDirection.normalized; //Si se está en modo sable, se utiliza la dirección objetivo para girar al personaje hacia esa dirección
            if(lookDirection != Vector3.zero)
            {
                Quaternion newRotation = Quaternion.LookRotation(lookDirection, transform.up);
                float differenceRotation = newRotation.eulerAngles.y - transform.eulerAngles.y;
                float eulerY = newRotation.eulerAngles.y;
                
                Vector3 euler = new Vector3(transform.eulerAngles.x, eulerY, transform.eulerAngles.z);
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(euler), rotationSpeed * Time.deltaTime); //Se realiza un giro interpolado
            }
        }

        public void ResetXRotation()
        {
            Vector3 eulerAngles = transform.localEulerAngles;
            eulerAngles.x = 0f;
            transform.localEulerAngles = eulerAngles;
        }
    }
}