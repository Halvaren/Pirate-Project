using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseCombat : MonoBehaviour
{
    private Animator anim;

    private int comboCount;

    private bool chaining;
    private bool nextAttack;

    public bool attackType; //True para si se puede introducir el combo completo en cualquier momento, false para si se tienen que encadenar bien todos los golpes

    // Start is called before the first frame update
    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        chaining = true;
        nextAttack = false;
        comboCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(attackType)
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
         
    }

    public void DetectNextAttack()
    {
        chaining = true;
        nextAttack = false;
    }

    public void ResetCombo(int attackId)
    {
        if(attackType)
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
