using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefinitiveScript {
    public class SableController : MonoBehaviour
    {
        public Transform swordTransform; //Transform del sable
        protected MeshCollider swordCollider; //Collider del sable
        protected SwordCollisionDetector swordScript; //Script componente del sable que indica cuando colisiona con algo

        [SerializeField] AnimationCurve[] attackMovementSpeed; //Curvas de animación para determinar la fluctuación del movimiento en los desplazamientos del ataque

        protected int comboCount; //Número de ataques combinados en el combo

        protected bool chaining; //Determina si se puede encadenar otro ataque
        protected bool nextAttack; //Determina si se ha encadenado otro ataque y, por tanto, si se debe ejecutar

        public float damage = 10f; //Daño que causa el ataque con sable

        protected bool attacking; //Está atacando
        protected bool blocking; //Está bloqueando

        public float reducingStaminaSpeed = 1f; //Velocidad con la que se reduce la stamina mientras se bloquea

        //Instancia del componente HealthController para reducir vida y stamina cuando sea necesario
        protected HealthController m_HealthController;
        public HealthController HealthController
        {
            get {
                if(m_HealthController == null) m_HealthController = GetComponent<HealthController>();
                return m_HealthController;
            }
        }

        //Lista que se llenará con aquellos componentes SableController con los que se colisione mientras el collider del sable esté activo
        //Un SableController puede ser añadido a la lista si se colisiona con el cuerpo del otro character o con la espada
        //Dependiendo del orden de las colisiones y el estado del otro character, se realizarán unas acciones u otras
        protected List<SableController> collidedEnemies; 

        public LayerMask enemyLayerMask; //Determina qué es para este character un enemigo

        //Variable que almacenará la corutina de desplazamiento durante el ataque para poder detenerla en caso de ser necesario
        protected Coroutine displacementCoroutine; 

        //Métodos del monobehaviour

        protected void Awake()
        {
            swordCollider = swordTransform.GetComponentInChildren<MeshCollider>();
            swordCollider.enabled = false;

            swordScript = swordTransform.GetComponentInChildren<SwordCollisionDetector>();

            //El script que controla la detección de colisiones de la espada necesita de una referencia a este script 
            //para poder ejecutar el método AddCollidedObject
            swordScript.SableController = this; 
        }

        protected void Start()
        {
            chaining = true;
            nextAttack = false;
            attacking = false;
            comboCount = 0;
             
            collidedEnemies = new List<SableController>();
        }

        //Métodos varios

        //Método que se ejecuta como parte de un evento de animación en cada una de las animaciones de ataque
        //Permite poder encadenar el siguiente ataque y también inicia el desplazamiento que sufre el Character durante el ataque
        public void StartAttack(int attackId) 
        {
            chaining = true; //Al llamarse al método ComboAttack, si el número de ataques realizados aún no es el máximo, permitirá volver a ejecutar otro ataque
            nextAttack = false; //Lo reinicializa a false, de manera que si no se vuelve a llamar al método ComboAttack antes de que se ejecute FinishAttack, se considerará que el combo se ha acabado

            float time = GetComponent<Animator>().GetNextAnimatorClipInfo(0)[0].clip.length; //Se necesita saber el tiempo que durará la animación

            displacementCoroutine = StartCoroutine(Displacement(attackMovementSpeed[attackId - 1], time)); //Inicia la corutina de desplazamiento y la guarda en una variable en caso de necesitar detenerla
        }

        //Activa el collider de la espada en un momento concreto de la animación
        public void EnableSwordCollider()
        {
            swordCollider.enabled = true;
            swordScript.hit = false; //Será útil para la IA del enemigo con sable, ya que sí ha golpeado, deberá encadenar otro golpe
        }
        
        //Desactiva el collider de la espada en un momento concreto de la animación
        public void DisableSwordCollider()
        {
            swordCollider.enabled = false;

            collidedEnemies.Clear(); //Vacía la lista de enemigos golpeados
        }

        //Este método será llamado desde el script que controla las colisiones de la espada cada vez que colisiona con aquello que esté en la layer de enemigo
        public void AddCollidedObject(GameObject other)
        {
            if(((1 << other.layer) | enemyLayerMask) == enemyLayerMask) //Lo detectado es el cuerpo del enemigo
            {
                //Si se ha detectado un cuerpo de enemigo, cabe plantearse si dicho enemigo lleva una espada o no
                //Se toma su CharacterBehaviour que es la clase padre tanto del PlayerBehaviour (el jugador) como del EnemyBehaviour (cualquier enemigo)
                CharacterBehaviour enemy = other.GetComponent<CharacterBehaviour>();

                //Se calculan las direcciones de los contrincantes en el momento de la colisión
                Vector3 characterForward = transform.TransformDirection(Vector3.forward);
                Vector3 enemyForward = enemy.transform.TransformDirection(Vector3.forward);

                //Se toma también el SableController del enemigo en cuestión
                SableController enemySableController = enemy.GetComponent<SableController>();

                //Si el enemigo es el jugador y está en modo sable, significa que porta la espada. Si el enemigo es efectivamente un enemigo y es un enemigo con sable, lleva espada
                if((enemy is PlayerBehaviour && ((PlayerBehaviour) enemy).sableMode) || (enemy is EnemyBehaviour && ((EnemyBehaviour) enemy).sableEnemy))
                {
                    bool onList = false;
                    for(int i = 0; i < collidedEnemies.Count; i++) //Se comprueba si ya ha sido detectado con anterioridad (si se ha detectado con anterioridad, es que se ha detectado su espada)
                    {
                        if(enemySableController == collidedEnemies[i])
                        {
                            onList = true;
                            break;
                        }
                    }

                    if(onList) //Si está en la lista
                    {
                        if(!enemySableController.GetBlocking() && !enemySableController.GetAttacking()) //Y además no está bloqueando o atacando
                        {
                            enemySableController.HitOnBody(characterForward); //Debe recibir un golpe
                        }
                        //En cualquier otro caso, cuando se detectó la espada antes que el cuerpo, se habrá golpeado la espada
                    }
                    else //Si no está en la lista
                    {
                        if(enemySableController.GetBlocking() || (enemySableController.GetAttacking() && enemySableController.SwordColliderEnable())) //Si está bloqueando o está atacando con la collider de la espada ya activa
                        {
                            //Se calcula su ángulo entre las direcciones
                            float angle = Vector3.Angle(characterForward, enemyForward);

                            if(angle > 110f) //Si el ángulo es mayor de 110 quiere decir que están bastante encarados el uno al otro, por lo que se puede considerar bloqueado el golpe
                            {
                                enemySableController.HitOnSword(characterForward); //Golpea en la espada del enemigo
                                HitOnSword(enemyForward); //Y él también se ve afectado
                            }
                            else //Está atacando al enemigo por un flanco por el cual no se está protegiendo
                            {
                                enemySableController.HitOnBody(characterForward); //Golpea en el cuerpo del enemigo
                            }
                        }
                        else //Si en general no se está protegiendo o atacando y ha atacado en el cuerpo antes de golpear en la espada
                        {
                            enemySableController.HitOnBody(characterForward); //Golpea en el cuerpo del enemigo
                        }

                        //Ahora, como no está en la lista, hay que meterlo
                        collidedEnemies.Add(enemySableController);
                    }
                }
                else //Si, independientemente de si es el jugador o un enemigo, no lleva la espada
                {
                    enemySableController.HitOnBody(characterForward); //Se golpea en el cuerpo del enemigo
                }
            }
            else //El objeto con el que se ha colisionado es una espada
            {
                //En este caso no cabe plantearse si el contrario lleva una espada o no, porque se ha colisionado directamente con la espada

                SableController enemy = other.GetComponent<SwordCollisionDetector>().SableController; //Se puede acceder igualmente a su SableController

                //Se calculan las direcciones de los contrincantes en el momento de la colisión
                Vector3 characterForward = transform.TransformDirection(Vector3.forward);
                Vector3 enemyForward = enemy.transform.TransformDirection(Vector3.forward);
                
                bool onList = false;
                for(int i = 0; i < collidedEnemies.Count; i++) //Se comprueba si ya ha sido detectado con anterioridad (en principio, habrá sido detectado su cuerpo)
                {
                    if(enemy == collidedEnemies[i])
                    {
                        onList = true;
                        break;
                    }
                }

                if(!onList) //De la espada sólo nos interesa si es lo primero con lo que se colisiona del enemigo
                {
                    if(enemy.GetBlocking() || enemy.GetAttacking()) //Si se choca con la espada estando el enemigo bloqueando o atacando, deberá retroceder
                    {
                        enemy.HitOnSword(characterForward); //Se golpea en la espada del enemigo
                        HitOnSword(enemyForward); //El que lleva este script también se ve afectado
                    }
                    //En cualquier otro caso, no se debe hacer nada si no se colisiona con el cuerpo

                    //Pero sí se debe meter en la lista, si no lo está
                    collidedEnemies.Add(enemy);
                }
            }
        }

        //Getters y setters

        public bool GetBlocking() { return blocking; }
        
        public void SetBlocking(bool param) { blocking = param; }

        public bool GetAttacking() { return attacking; }

        public void SetAttacking(bool value) { attacking = value; }

        public bool GetChaining() { return chaining && comboCount != 0; }

        public bool GetHit() { return swordScript.hit; }

        public bool SwordColliderEnable() { return swordCollider.enabled; }

        //Métodos que se sobreescribirán - Cada uno de estos métodos son sobreescritos de maneras distintas si se trata del jugador o el enemigo

        //Método que ejecutará las acciones necesarias cuando la espada del que lleva este script es golpeada
        public virtual void HitOnSword(Vector3 hitDirection) {}

        //Método que ejecutará las acciones necesarias cuando el cuerpo del que lleva este script es golpeado
        public virtual void HitOnBody(Vector3 hitDirection) {}

        //En función de si se ha encadenado otro golpe o no, se realizará una cosa u otra
        public virtual void FinishAttack(int attackId) {}

        //Realiza un desplazamiento durante un tiempo en función de una curva que modula la velocidad del desplazamiento
        protected virtual IEnumerator Displacement(AnimationCurve speedCurve, float time) { yield return null; }

        //Reinicializa todas las variables necesarias cuando un ataque se acaba, ya haya sido interrumpido o haya llegado a su fin
        protected virtual void CancelAttack() 
        {
            chaining = true; //Se podrá volver iniciar otro ataque
            nextAttack = false;

            swordScript.hit = false;

            DisableSwordCollider(); //Esto también vaciará la lista de objetos colisionados
            comboCount = 0;

            if(displacementCoroutine != null) StopCoroutine(displacementCoroutine); //Se detiene el desplazamiento si aún está activo

            attacking = false; //El ataque será considerado finalizado
        }

        //Este método es el que inicia el siguiente ataque del combo si es llamado desde fuera y se puede encadenar otro ataque
        public virtual void ComboAttack() {}
    }

}