using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefinitiveScript {
    
    public class PlayerSableController : SableController
    {
        //Instancia del componente que controla las animaciones del jugador
        private PlayerAnimatorController m_PlayerAnimatorController;
        public PlayerAnimatorController PlayerAnimatorController
        {
            get {
                if(m_PlayerAnimatorController == null) m_PlayerAnimatorController = GetComponent<PlayerAnimatorController>();
                return m_PlayerAnimatorController;
            }
        }

        //Instancia del CharacterController para poder desplazar al jugador
        private CharacterController m_CharacterController;
        public CharacterController CharacterController
        {
            get {
                if(m_CharacterController == null) m_CharacterController = GetComponent<CharacterController>();
                return m_CharacterController;
            }
        }

        protected void Update()
        {       
            if(blocking) //Si se está bloqueando, el HealthController deberá reducir la stamina progresivamente
            {
                blocking = !HealthController.ReduceStamina(reducingStaminaSpeed * Time.deltaTime); //Devolverá un true y detendrá el bloqueo si la stamina se acaba
            }

            PlayerAnimatorController.SetBlocking(blocking); //Se notifica al AnimatorController de los posibles cambios
        }

        //Este método es el que inicia el siguiente ataque del combo si es llamado desde fuera y se puede encadenar otro ataque
        public override void ComboAttack()
        {
            if(chaining && comboCount < 3) //Si se puede encadenar otro golpe y el número de golpes encadenados no ha alcanzado su máximo
            {
                chaining = false; //Ya no se puede encadenar otro golpe (StartAttack lo volverá a reinicializar)
                nextAttack = true; //Se ejecutará el siguiente ataque en el método FinishAttack

                attacking = true; //El jugador está atacando
                PlayerAnimatorController.SetAttacking(true);

                if(comboCount == 0) PlayerAnimatorController.Attack(); //Si el combo aún no había empezado, se ejecuta la animación del primer ataque
                comboCount++; //Se incrementa el número de ataques combinados
            }
        }

        //Reinicializa todas las variables necesarias cuando un ataque se acaba, ya haya sido interrumpido o haya llegado a su fin
        protected override void CancelAttack()
        {
            base.CancelAttack(); //Se realiza lo mismo que hace la clase SableController

            PlayerAnimatorController.SetAttacking(false); //Y se añade que el PlayerAnimatorController necesita saber que el ataque se ha acabado
        }

        //En función de si se ha encadenado otro golpe o no, se realizará una cosa u otra
        public override void FinishAttack(int attackId)
        {
            if(nextAttack) PlayerAnimatorController.Attack(); //Si se debe realizar otro ataque, se inicia la animación
            else CancelAttack(); //Si no, se reinicializa todo
        }

        //Método que ejecutará las acciones necesarias cuando la espada del que lleva este script es golpeada
        public override void HitOnSword(Vector3 hitDirection) 
        {
            if(blocking) //Si se ha golpeado en la espada estando bloqueando
            { 
                //Se reduce la stamina cierta cantidad
                if(HealthController.ReduceStamina(10f)) //Y se ha llegado a 0
                {
                    PlayerAnimatorController.Disarm(); //Se realiza el desarme
                    HealthController.Knockback(5f, hitDirection, false); //Mediante el HealthController, se hace un retroceso

                    blocking = false; //Se deja de bloquear
                    PlayerAnimatorController.SetBlocking(false);
                }
                else //Si la stamina no ha llegado a 0
                {
                    PlayerAnimatorController.HitOnSword(); //Se realiza una animación
                    HealthController.Knockback(5f, hitDirection, false); //Y un cierto retroceso
                }
            }
            else if(attacking) //Si se ha golpeado en la espada estando atacando
            {
                HealthController.ReduceStamina(10f); //Se reduce la stamina
                PlayerAnimatorController.Disarm(); //Se realiza el desarme
                HealthController.Knockback(5f, hitDirection, false); //Y un cierto retroceso

                CancelAttack(); //Se cancela el ataque
            }
        }

        //Método que ejecutará las acciones necesarias cuando el cuerpo del que lleva este script es golpeado
        public override void HitOnBody(Vector3 hitDirection)
        {
            HealthController.Knockback(2.5f, hitDirection, false); //Se hace un retroceso

            if(!HealthController.TakeDamage(damage)) //Se le produce un daño y si no se muere
                PlayerAnimatorController.HitOnBody(); //Se ejecuta la animación de golpe en el cuerpo

            //Independientemente del estado (atacando, bloqueando o normal), se cancelan el bloque y el ataque
            blocking = false;
            PlayerAnimatorController.SetBlocking(false);
            CancelAttack();
        }

        //Realiza un desplazamiento durante un tiempo en función de una curva que modula la velocidad del desplazamiento
        protected override IEnumerator Displacement(AnimationCurve speedCurve, float time)
        {
            float elapsedTime = 0.0f;

            while(elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;

                //La velocidad del movimiento viene dada por la curva de animación recibida por parámetro
                CharacterController.Move(transform.forward * speedCurve.Evaluate(elapsedTime / time) * Time.deltaTime); 

                yield return null;
            }
        }
    }
}
