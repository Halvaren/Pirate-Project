﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Probaturas : MonoBehaviour
{
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.X))
        {
            anim.SetTrigger("x");
        }
        if(Input.GetKeyDown(KeyCode.Y))
        {
            anim.SetTrigger("y");
        }
        if(Input.GetKeyDown(KeyCode.Z))
        {
            anim.SetTrigger("z");
        }
        if(Input.GetKeyDown(KeyCode.L))
        {
            anim.SetTrigger("l");   
        }
    }

    public void KnockbackBlock()
    {
        anim.SetTrigger("w");
    }
}