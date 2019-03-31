using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefinitiveScript {
    public class CloseCombat : MonoBehaviour
    {
        private Animator anim;
        public Transform swordTransform;
        private MeshCollider swordCollider;
        private SwordCollisionDetector swordScript;

        [SerializeField] AnimationCurve[] attackMovementSpeed;

        private int comboCount;

        private bool chaining;
        private bool nextAttack;

        public float Damage = 10f;

        private bool moving;

        public bool AttackType; //True para si se puede introducir el combo completo en cualquier momento, false para si se tienen que encadenar bien todos los golpes

        // Start is called before the first frame update
        void Awake()
        {
            anim = GetComponent<Animator>();
            swordCollider = swordTransform.GetComponentInChildren<MeshCollider>();
            swordCollider.enabled = false;

            swordScript = swordTransform.GetComponentInChildren<SwordCollisionDetector>();
        }

        void Start()
        {
            chaining = true;
            nextAttack = false;
            comboCount = 0;

            swordScript.SetDamage(Damage);
        }

        // Update is called once per frame
        void Update()
        {
            if(AttackType)
            {
                if(Input.GetMouseButtonDown(0))
                {
                    comboCount++;
                    if(comboCount == 1) anim.SetTrigger("Attack");
                }  
            }
            else
            {
                if(Input.GetMouseButtonDown(0) && chaining && comboCount < 3)
                {
                    chaining = false;
                    nextAttack = true;

                    if(comboCount == 0) anim.SetTrigger("Attack");
                    comboCount++;
                }
            }

            /* if(moving)
            {
                transform.Translate(transform.InverseTransformDirection(transform.forward) * attackMovementSpeed[comboCount - 1] * Time.deltaTime);
            }*/
            
        }

        public void StartAttack(int attackId)
        {
            chaining = true;
            nextAttack = false;

            float time = anim.GetNextAnimatorClipInfo(0)[0].clip.length;
            StartCoroutine(Displacement(attackMovementSpeed[attackId - 1], time));  
        }

        IEnumerator Displacement(AnimationCurve speedCurve, float time)
        {
            GameManager.Instance.LocalPlayer.stopMovement = true;

            float elapsedTime = 0.0f;

            while(elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;
                transform.Translate(transform.InverseTransformDirection(transform.forward) * speedCurve.Evaluate(elapsedTime / time) * Time.deltaTime);
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
        }

        public void FinishAttack(int attackId)
        {
            if(AttackType)
            {
                if(comboCount == attackId || attackId > 2) {
                    comboCount = 0;
                    if(attackId < 3) anim.SetTrigger("StopAttack");
                }
                else
                {
                    anim.SetTrigger("Attack");
                }
            }
            else
            {
                if(nextAttack)
                {
                    anim.SetTrigger("Attack");
                }
                else
                {
                    comboCount = 0;
                    if(attackId < 3) anim.SetTrigger("StopAttack");
                }
            }
        }
    }

}