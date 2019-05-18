using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace DefinitiveScript
{
    public class EnemySableController : SableController
    {
        //Instancia del EnemyBehaviour, el cual controla los parámetros del Animator
        private EnemyBehaviour m_EnemyBehaviour;
        public EnemyBehaviour EnemyBehaviour {
            get {
                if(m_EnemyBehaviour == null) m_EnemyBehaviour = GetComponent<EnemyBehaviour>();
                return m_EnemyBehaviour;
            }
        }

        //Instancia del NavMeshAgent para poder desplazar al enemigo
        private NavMeshAgent m_NavMeshAgent;
        public NavMeshAgent NavMeshAgent {
            get {
                if(m_NavMeshAgent == null) m_NavMeshAgent = GetComponent<NavMeshAgent>();
                return m_NavMeshAgent;
            }
        }

        protected void Update()
        {       
            if(blocking) //Si se está bloqueando, el HealthController deberá reducir la stamina progresivamente
            {
                blocking = !HealthController.ReduceStamina(reducingStaminaSpeed * Time.deltaTime); //Devolverá un true y detendrá el bloqueo si la stamina se acaba
            }
            HealthController.SetUsingStamina(blocking);
        }

        //Este método es el que inicia el siguiente ataque del combo si es llamado desde fuera y se puede encadenar otro ataque
        public override void ComboAttack()
        {
            if(chaining && comboCount < 3) //Si se puede encadenar otro golpe y el número de golpes encadenados no ha alcanzado su máximo
            {
                chaining = false; //Ya no se puede encadenar otro golpe (StartAttack lo volverá a reinicializar)
                nextAttack = true; //Se ejecutará el siguiente ataque en el método FinishAttack

                attacking = true; //El enemigo está atacando
                EnemyBehaviour.SetAttacking();

                if(comboCount == 0) EnemyBehaviour.Attack(); //Si el combo aún no había empezado, se ejecuta la animación del primer ataque
                comboCount++; //Se incrementa el número de ataques combinados
            }
        }

        //Reinicializa todas las variables necesarias cuando un ataque se acaba, ya haya sido interrumpido o haya llegado a su fin
        protected override void CancelAttack()
        {
            base.CancelAttack(); //Se realiza lo mismo que hace la clase SableController

            //EnemyBehaviour.SetAttacking(false); //Y se añade el paso al comportamiento de fijación al jugador
            EnemyBehaviour.SetStaring();
        }

        //En función de si se ha encadenado otro golpe o no, se realizará una cosa u otra
        public override void FinishAttack(int attackId)
        {
            if(nextAttack)
            {
                EnemyBehaviour.Attack(); //Si se debe realizar otro ataque, se inicia la animación
            }
            else
            {
                CancelAttack(); //Si no, se reinicializa todo
            }
        }

        //Método que ejecutará las acciones necesarias cuando la espada del que lleva este script es golpeada
        public override void HitOnSword(Vector3 hitDirection)
        {
            if(blocking) //Si se ha golpeado en la espada estando bloqueando
            {
                //Se reduce la stamina cierta cantidad
                if(HealthController.ReduceStamina(10f)) //Y se ha llegado a 0
                {
                    EnemyBehaviour.Disarm(); //Se realiza el desarme
                    HealthController.Knockback(5f, hitDirection, false); //Mediante el HealthController, se hace un retroceso

                    blocking = false; //Se deja de bloquear
                    //EnemyBehaviour.SetBlocking(false);
                    EnemyBehaviour.SetStaring(); //Y se pasa a fijar al jugador
                }
                else //Si la stamina no ha llegado a 0
                {
                    EnemyBehaviour.HitOnSword(); //Se realiza una animación
                    HealthController.Knockback(5f, hitDirection, false); //Y un cierto retroceso
                }
            }
            else if(attacking) //Si se ha golpeado en la espada estando atacando
            {
                HealthController.ReduceStamina(10f); //Se reduce la stamina
                EnemyBehaviour.Disarm(); //Se realiza el desarme
                HealthController.Knockback(5f, hitDirection, false); //Y un cierto retroceso

                CancelAttack(); //Se cancela el ataque
            }
        }

        //Método que ejecutará las acciones necesarias cuando el cuerpo del que lleva este script es golpeado
        public override void HitOnBody(Vector3 hitDirection)
        {
            //Independientemente del estado (atacando, bloqueando o normal), se cancelan el bloque y el ataque
            blocking = false;
            //EnemyBehaviour.SetBlocking(false);
            CancelAttack();

            HealthController.Knockback(2.5f, hitDirection, false); //Se hace un retroceso

            if(!HealthController.TakeDamage(damage)) //Se le produce un daño y si no se muere
                EnemyBehaviour.HitOnBody(); //Se ejecuta la animación de golpe en el cuerpo
        }

        //Realiza un desplazamiento durante un tiempo en función de una curva que modula la velocidad del desplazamiento
        protected override IEnumerator Displacement(AnimationCurve speedCurve, float time)
        {
            float elapsedTime = 0.0f;

            while(elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;

                //La velocidad del movimiento viene dada por la curva de animación recibida por parámetro
                NavMeshAgent.Move(transform.forward * speedCurve.Evaluate(elapsedTime / time) * Time.deltaTime);
                
                yield return null;
            }
        }
    }
}

