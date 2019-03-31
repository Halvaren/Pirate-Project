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

        private CharacterAnimationController m_CharacterAnimationController;
        public CharacterAnimationController CharacterAnimationController
        {
            get {
                if(m_CharacterAnimationController == null) m_CharacterAnimationController = GetComponent<CharacterAnimationController>();
                return m_CharacterAnimationController;
            }
        }

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
        public void ComboAttack()
        {
            if(chaining && comboCount < 3)
            {
                chaining = false;
                nextAttack = true;

                if(comboCount == 0) CharacterAnimationController.Attack();
                comboCount++;
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
            if(nextAttack)
            {
                CharacterAnimationController.Attack();
            }
            else
            {
                comboCount = 0;
                if(attackId < 3) CharacterAnimationController.StopAttack();
            }
        }
    }

}