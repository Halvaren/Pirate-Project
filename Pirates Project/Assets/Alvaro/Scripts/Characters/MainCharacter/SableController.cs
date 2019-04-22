﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefinitiveScript {
    public class SableController : MonoBehaviour
    {
        private Animator anim;
        private CharacterController controller;
        public Transform swordTransform;
        private MeshCollider swordCollider;
        private SwordCollisionDetector swordScript;

        [SerializeField] AnimationCurve[] attackMovementSpeed;

        private int comboCount;

        private bool chaining;
        private bool nextAttack;

        public float Damage = 10f;

        public bool attacking = false;
        public bool blocking = false;

        private CharacterAnimationController m_CharacterAnimationController;
        public CharacterAnimationController CharacterAnimationController
        {
            get {
                if(m_CharacterAnimationController == null) m_CharacterAnimationController = GetComponent<CharacterAnimationController>();
                return m_CharacterAnimationController;
            }
        }

        private List<SableController> collidedEnemies;

        public LayerMask enemyLayerMask;

        // Start is called before the first frame update
        void Awake()
        {
            anim = GetComponent<Animator>();
            controller = GetComponent<CharacterController>();
            swordCollider = swordTransform.GetComponentInChildren<MeshCollider>();
            swordCollider.enabled = false;

            swordScript = swordTransform.GetComponentInChildren<SwordCollisionDetector>();
            swordScript.SableController = this;
        }

        void Start()
        {
            chaining = true;
            nextAttack = false;
            comboCount = 0;
             
            collidedEnemies = new List<SableController>();

            swordScript.SetDamage(Damage);
        }

        void Update()
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 5000000, Color.blue);
        }

        // Update is called once per frame
        public void ComboAttack()
        {
            if(chaining && comboCount < 3)
            {
                chaining = false;
                nextAttack = true;

                if(comboCount == 0) CharacterAnimationController.Attack();
                comboCount++;

                attacking = true;
            }
        }

        public void Block(bool input)
        {
            if(!attacking)
            {
                blocking = input;
            }
        }

        public void StartAttack(int attackId, float time)
        {
            chaining = true;
            nextAttack = false;

            StartCoroutine(Displacement(attackMovementSpeed[attackId - 1], time));  
        }

        IEnumerator Displacement(AnimationCurve speedCurve, float time)
        {
            GameManager.Instance.LocalPlayer.stopMovement = true;

            float elapsedTime = 0.0f;

            while(elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;
                controller.Move(transform.forward * speedCurve.Evaluate(elapsedTime / time) * Time.deltaTime);
                yield return null;
            }

            GameManager.Instance.LocalPlayer.stopMovement = false;
        }

        public void EnableSwordCollider()
        {
            swordCollider.enabled = true;
        }
        
        public void DisableSwordCollider()
        {
            swordCollider.enabled = false;
            collidedEnemies.Clear();
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
                if(attackId < 3) CharacterAnimationController.StopAttack();

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

        public void HitOnSword()
        {

        }

        public void HitOnBody()
        {

        }

        public void AddCollidedObject(GameObject other)
        {
            if(((1 << other.layer) | enemyLayerMask) == enemyLayerMask) //Lo detectado es un enemigo
            {
                SableController enemy = other.GetComponent<SableController>(); //Un enemigo tiene el componente SableController

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
                        enemy.HitOnBody(); //Debe recibir un golpe
                    }
                    //En cualquier otro caso, cuando se detectó la espada antes que el cuerpo, se habrá golpeado la espada
                }
                else //Si no está en la lista
                {
                    if(enemy.GetBlocking()) //Si está bloqueando
                    {
                        print("No está en la lista y está bloqueando");
                        //Se va a comprobar las direcciones de los contrincantes
                        Vector3 characterForward = transform.TransformDirection(Vector3.forward);
                        Vector3 enemyForward = enemy.transform.TransformDirection(Vector3.forward);

                        //Se calcula su ángulo entre ellos
                        float angle = Vector3.Angle(characterForward, enemyForward);

                        print(characterForward + " " + enemyForward + " " + angle);
                        if(angle > 120f) //Si el ángulo es mayor de 120 quiere decir que están bastante encarados el uno al otro, por lo que se puede considerar bloqueado el golpe
                        {
                            enemy.HitOnSword();
                            HitOnSword();
                        }
                        else //Está atacando al enemigo por un flanco por el cual no se está protegiendo
                        {
                            print("Pero es igual porque ataco por detrás");
                            enemy.HitOnBody();
                        }
                    }
                    else //Si en general no se está protegiendo y atacado en el cuerpo antes de golpear en la espada
                    {
                        enemy.HitOnBody();
                    }

                    //Ahora, como no está en la lista, hay que meterlo
                    collidedEnemies.Add(enemy);
                }
            }
            else //El objeto con el que se ha colisionado es una espada
            {
                SableController enemy = other.GetComponent<SwordCollisionDetector>().SableController; //Se puede acceder igualmente a su SableController
                
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
                        enemy.HitOnSword();
                        HitOnSword();
                    }
                    //En cualquier otro caso, no se debe hacer nada si no se colisiona con el cuerpo

                    //Pero sí se debe meter en la lista, si no lo está
                    collidedEnemies.Add(enemy);
                }
            }
        }
    }

}