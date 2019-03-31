using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shootEffects : MonoBehaviour
{
    private ParticleSystem gunParticles; 
    private  Gun gunScript;
    private LineRenderer gunLine;

    void Awake ()
    {
        //shootableMask = LayerMask.GetMask ("Enemy");
        gunParticles = GetComponent<ParticleSystem> ();
        gunLine = GetComponent <LineRenderer> ();
        //anim = GetComponent<Animator>();
        gunScript = GetComponent<Gun>();
        
        //player = GameObject.FindWithTag("Player");
     
    }
    
    public void gunEffects() 
    {
        gunParticles.Stop ();
        gunParticles.Play();
        gunLine.enabled = true;
        gunLine.SetWidth(0.5f, 0.5f);   
        gunLine.SetPosition (0, gunScript.transform.position);
       /* if (gunScript.enemyHit == true) {
            gunLine.SetPosition (1, gunScript.hit.point);
        }
        else {
            gunLine.SetPosition (1, gunScript.transform.position + gunScript.transform.forward * 8);
        }*/
    }
}
