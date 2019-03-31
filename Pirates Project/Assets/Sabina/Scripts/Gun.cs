
using UnityEngine;

public class Gun : MonoBehaviour
{
    private float damage = 10f;
    private float range = 10f; 
    private float timeBetweenBullets = 1f;
    private float effectsDisplayTime = 0.1f; 


    //public GameObject player; 
    private ParticleSystem gunParticles; 

    private GameObject impactEffect;
    public Camera cam; 
    private int shootableMask;
    RaycastHit hit;
    //private Animator anim;
    private GameObject player;
    private float timer;
    private LineRenderer gunLine;
    [HideInInspector] public bool shot;
    [HideInInspector] public bool isShooting = false;
    private Ray raymouse;

     void Awake ()
    {
        shootableMask = LayerMask.GetMask ("Enemy");
        gunParticles = GetComponent<ParticleSystem> ();
        gunLine = GetComponent <LineRenderer> ();
        //anim = GetComponent<Animator>();
        
        
        player = GameObject.FindWithTag("Player");
     
    }

    // Update is called once per frame
    void Update()
    {
        timer +=  Time.deltaTime;
        if (Input.GetButtonDown("Fire1") && timer>= timeBetweenBullets){ 
            player.GetComponent<Animator>().SetTrigger("Shot");
            //Shoot();
            
            
        }

        if(timer >= timeBetweenBullets * effectsDisplayTime)
        {           
            DisableEffects ();        
        }
    }
    private void DisableEffects() {
        gunLine.enabled = false;
    }

    public void Shoot() 
    {
        isShooting = true;
        gunParticles.Stop ();
        timer = 0f;
        raymouse = cam.ScreenPointToRay(Input.mousePosition);
        shot = false;
        
        //gunLine.SetWidth(0.5f, 0.5f);   
        gunLine.SetPosition (0, transform.position);
        //Debug.DrawRay(player.transform.position, player.transform.forward*10, Color.green);
        if (Physics.Raycast(player.transform.position, player.transform.forward, out hit, range, shootableMask)) { //layer del enemigo
           
            DebugEnemyBehaviour enemy = hit.transform.gameObject.GetComponent<DebugEnemyBehaviour>();
            if (enemy != null) 
            {
                   enemy.Attacked (damage, transform.forward/3);
            }
            shot = true;
            //gunLine.SetPosition (1, hit.point);
            //shot = true;
           
        }
        /*else {
            gunLine.SetPosition (1, transform.position + transform.forward * 8);
        }*/
        //Debug.Log("dispara");
        
        //Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
        
        //gunLine.enabled = false;
       
        isShooting = false;
    }

    public void shootingEffects() { //activa las particulas, habilita la gunline 
    
        gunParticles.Play();
        gunLine.enabled = true;
        if (shot == true)  gunLine.SetPosition (1, hit.point);
        else gunLine.SetPosition (1, player.transform.position + player.transform.forward * 8);
    }
}