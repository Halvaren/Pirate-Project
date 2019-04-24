using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefinitiveScript {
    public class SableController : MonoBehaviour
    {
        protected Animator anim;
        //private CharacterController controller;
        public Transform swordTransform;
        protected MeshCollider swordCollider;
        protected SwordCollisionDetector swordScript;

        [SerializeField] AnimationCurve[] attackMovementSpeed;

        protected int comboCount;

        protected bool chaining;
        protected bool nextAttack;

        public float Damage = 10f;

        protected bool attacking;
        protected bool blocking;

        protected CharacterAnimationController m_CharacterAnimationController;
        public CharacterAnimationController CharacterAnimationController
        {
            get {
                if(m_CharacterAnimationController == null) m_CharacterAnimationController = GetComponent<CharacterAnimationController>();
                return m_CharacterAnimationController;
            }
        }

        protected HealthController m_HealthController;
        public HealthController HealthController
        {
            get {
                if(m_HealthController == null) m_HealthController = GetComponent<HealthController>();
                return m_HealthController;
            }
        }

        protected List<SableController> collidedEnemies;

        public LayerMask enemyLayerMask;

        // Start is called before the first frame update
        protected void Awake()
        {
            anim = GetComponent<Animator>();
            //controller = GetComponent<CharacterController>();
            swordCollider = swordTransform.GetComponentInChildren<MeshCollider>();
            swordCollider.enabled = false;

            swordScript = swordTransform.GetComponentInChildren<SwordCollisionDetector>();
            swordScript.SableController = this;
        }

        protected void Start()
        {
            chaining = true;
            nextAttack = false;
            attacking = false;
            comboCount = 0;
             
            collidedEnemies = new List<SableController>();

            swordScript.SetDamage(Damage);
        }

        protected void Update()
        {
            if(Input.GetKeyDown(KeyCode.R))
            {
                CancelAttack();
                CharacterAnimationController.StopAttack();
            }
        }

        protected void CancelAttack()
        {
            chaining = true;
            nextAttack = false;
            attacking = false;
            DisableSwordCollider();
            comboCount = 0;
        }

        public void ComboAttack()
        {
            print(gameObject.name);
            if(chaining && comboCount < 3)
            {
                chaining = false;
                nextAttack = true;

                if(comboCount == 0) CharacterAnimationController.Attack();
                comboCount++;

                attacking = true;
            }
        }

        public virtual void Block(bool input)
        {
            if(!attacking)
            {
                blocking = input;
                CharacterAnimationController.Block(blocking);
            }
        }

        public void StartAttack(int attackId, float time)
        {
            chaining = true;
            nextAttack = false;

            StartCoroutine(Displacement(attackMovementSpeed[attackId - 1], time));  
        }

        protected virtual IEnumerator Displacement(AnimationCurve speedCurve, float time) { yield return null; }

        public void EnableSwordCollider()
        {
            swordCollider.enabled = true;
        }
        
        public void DisableSwordCollider()
        {
            swordCollider.enabled = false;
            collidedEnemies.Clear();
        }

        public bool SwordColliderEnable()
        {
            return swordCollider.enabled;
        }

        public void FinishAttack(int attackId)
        {
            if(nextAttack)
            {
                CharacterAnimationController.Attack();
            }
            else
            {
                comboCount = 0;
                if(attackId < 3 && attacking) CharacterAnimationController.StopAttack();

                attacking = false;
            }
        }

        public bool GetBlocking()
        {
            return blocking;
        }

        public bool GetAttacking()
        {
            return attacking;
        }

        public void HitOnSword(Vector3 hitDirection)
        {
            if(blocking)
            { 
                if(HealthController.ReduceStamina(10f))
                {
                    CharacterAnimationController.Disarm();
                    HealthController.Knockback(5f, hitDirection);
                    blocking = false;
                }
                else
                {
                    CharacterAnimationController.HitOnSword();
                    HealthController.Knockback(5f, hitDirection);
                }
            }
            else if(attacking)
            {
                CharacterAnimationController.Disarm();
                HealthController.Knockback(5f, hitDirection);

                CancelAttack();
            }
        }

        public void HitOnBody(Vector3 hitDirection)
        {
            blocking = false;
            CancelAttack();

            HealthController.Knockback(2.5f, hitDirection);
            if(HealthController.TakeDamage(Damage))
            {
                print("Sa morío");
            }
            else 
            {
                CharacterAnimationController.Hit();
            }
        }

        public void AddCollidedObject(GameObject other)
        {
            if(((1 << other.layer) | enemyLayerMask) == enemyLayerMask) //Lo detectado es un enemigo
            {
                SableController enemy = other.GetComponent<SableController>(); //Un enemigo tiene el componente SableController

                //Se calculan las direcciones de los contrincantes en el momento de la colisión
                Vector3 characterForward = transform.TransformDirection(Vector3.forward);
                Vector3 enemyForward = enemy.transform.TransformDirection(Vector3.forward);

                bool onList = false;
                for(int i = 0; i < collidedEnemies.Count; i++) //Se comprueba si ya ha sido detectado con anterioridad (en principio, habrá sido detectada su espada)
                {
                    if(enemy == collidedEnemies[i])
                    {
                        onList = true;
                        break;
                    }
                }

                if(onList) //Si está en la lista
                {
                    if(!enemy.GetBlocking() && !enemy.GetAttacking()) //Y además no está bloqueando o atacando
                    {
                        enemy.HitOnBody(characterForward); //Debe recibir un golpe
                    }
                    //En cualquier otro caso, cuando se detectó la espada antes que el cuerpo, se habrá golpeado la espada
                }
                else //Si no está en la lista
                {
                    if(enemy.GetBlocking() || (enemy.GetAttacking() && enemy.SwordColliderEnable())) //Si está bloqueando o está atacando
                    {
                        //Se calcula su ángulo entre las direcciones
                        float angle = Vector3.Angle(characterForward, enemyForward);

                        if(angle > 110f) //Si el ángulo es mayor de 120 quiere decir que están bastante encarados el uno al otro, por lo que se puede considerar bloqueado el golpe
                        {
                            enemy.HitOnSword(characterForward);
                            HitOnSword(enemyForward);
                        }
                        else //Está atacando al enemigo por un flanco por el cual no se está protegiendo
                        {
                            enemy.HitOnBody(characterForward);
                        }
                    }
                    else //Si en general no se está protegiendo o atacando y ha atacado en el cuerpo antes de golpear en la espada
                    {
                        enemy.HitOnBody(characterForward);
                    }

                    //Ahora, como no está en la lista, hay que meterlo
                    collidedEnemies.Add(enemy);
                }
            }
            else //El objeto con el que se ha colisionado es una espada
            {
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
                        enemy.HitOnSword(characterForward);
                        HitOnSword(enemyForward);
                    }
                    //En cualquier otro caso, no se debe hacer nada si no se colisiona con el cuerpo

                    //Pero sí se debe meter en la lista, si no lo está
                    collidedEnemies.Add(enemy);
                }
            }
        }
    }

}